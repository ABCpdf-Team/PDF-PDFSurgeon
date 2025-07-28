// ===========================================================================
//	©2013-2024 WebSupergoo. All rights reserved.
//
//	This source code is for use exclusively with the ABCpdf product with
//	which it is distributed, under the terms of the license for that
//	product. Details can be found at
//
//		http://www.websupergoo.com/
//
//	This copyright notice must not be deleted and must be reproduced alongside
//	any sections of code extracted from this module.
// ===========================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Diagnostics;
using System.Security.Cryptography;
using SD = System.Drawing;
using SD2 = System.Drawing.Drawing2D;

using WebSupergoo.ABCpdf13;
using WebSupergoo.ABCpdf13.Atoms;
using WebSupergoo.ABCpdf13.Objects;
using WebSupergoo.ABCpdf13.Operations;


namespace WebSupergoo.PDFSurgeon {
	public class StreamEditor {
		private byte[] _liveData;
		private static readonly HashSet<string> _pathMake = new HashSet<string>(new string[] { "m", "l", "c", "v", "y", "h", "re", "W", "W*" });
		private static readonly HashSet<string> _pathPaint = new HashSet<string>(new string[] { "S", "s", "f", "F", "f*", "B", "B*", "b", "b*", "n" });

		private StreamEditor() { }

		public StreamEditor(ObjectExtractor obj) {
			Extractor = obj;
		}

		public ObjectExtractor Extractor { get; private set; }
		public StreamObject Stream { get { return Extractor.Object as StreamObject; } }
		public StreamObject ContentStream { get; private set; }
		public IndirectObject ResourceOwner { get; private set; }
		public string OriginalText { get; set; }
		public string LiveText { get; set; }
		public string CurrentText { get { return Extractor.GetValue(Extractor.Doc); } set { Extractor.SetValue(Extractor.Doc, value); } }

		public void Load() {
			StreamObject so = Stream;
			ContentStream = (so != null) && (!so.Compressed) && ((so is FormXObject) || (Extractor.Context.PageContents.Contains(so.ID))) ? so : null;
			ResourceOwner = ContentStream as FormXObject;
			if (ResourceOwner == null) {
				int id = 0;
				if (Extractor.Context.PageContentIdToPageId.TryGetValue(Extractor.Object.ID, out id))
					ResourceOwner = Extractor.Doc.ObjectSoup[id];
			}
			OriginalText = LiveText = CurrentText;
		}

		public StringBuilder GetPartialContentStream(double lo, double hi) {
			// get low and high values
			if (lo > hi) {
				double t = lo;
				lo = hi;
				hi = t;
			}
			// get stream data
			if ((_liveData == null) && (ContentStream != null))
				_liveData = ContentStream.GetData();
			if (_liveData == null)
				return null;
			// analyse content stream
			ArrayAtom array = ArrayAtom.FromContentStream(_liveData);
			IList<Tuple<string, int>> items = OpAtom.Find(array);
			Stack<int> saveRestoreStack = new Stack<int>();
			Stack<int> markedContentStack = new Stack<int>();
			List<Tuple<int, int>> pairs = new List<Tuple<int, int>>();
			int beginText = -1, beginPath = -1;
			for (int i = 0; i < items.Count; i++) {
				var pair = items[i];
				string op = ((OpAtom)array[pair.Item2]).Text;
				if (op == "q")
					saveRestoreStack.Push(i);
				else if (op == "Q")
					pairs.Add(new Tuple<int, int>(saveRestoreStack.Pop(), i));
				if (op == "BMC" || op == "BDC")
					markedContentStack.Push(i);
				else if (op == "EMC") {
					// Marked content start and end can be interwoven with other content start and end
					// so we only put this sequence in as a pair if there is nothing inside it.
					if (i == markedContentStack.Pop() + 1)
						pairs.Add(new Tuple<int, int>(i - 1, i));
				}
				else if (op == "BT")
					beginText = i;
				else if (op == "ET") {
					if (beginText >= 0)
						pairs.Add(new Tuple<int, int>(beginText, i));
					beginText = -1;
				}
				else if (_pathMake.Contains(op)) {
					if (beginPath < 0)
						beginPath = i;
				}
				else if (_pathPaint.Contains(op)) {
					if (beginPath >= 0)
						pairs.Add(new Tuple<int, int>(beginPath, i));
					beginPath = -1;
				}
			}
			//Debug.Assert(saveRestoreStack.Count == 0);
			//Debug.Assert(markedContentStack.Count == 0);
			// build set of sections which will be discarded
			int loIndex = (int)(items.Count * lo);
			int hiIndex = (int)(items.Count * hi);
			Dictionary<int, int> discardPairs = new Dictionary<int, int>();
			foreach (var pair in pairs) {
				if (pair.Item2 < loIndex)
					discardPairs[pair.Item1] = pair.Item2;
				else if (pair.Item1 > hiIndex)
					discardPairs[pair.Item1] = pair.Item2;
			}
			// build contents
			int index = 0;
			StringBuilder sb = new StringBuilder();
			bool writeCommand = true;
			for (int i = 0; i < items.Count; i++) {
				int endIndex = 0;
				if (discardPairs.TryGetValue(i, out endIndex)) {
					writeCommand = false;
					i = endIndex;
				}
				var pair = items[i];
				if (writeCommand)
					AppendCommand(sb, array, index, pair.Item2, false);
				writeCommand = true;
				index = pair.Item2 + 1;
			}
			return sb;
		}

		public string FormatContentStream() {
			if (ContentStream == null)
				return null;
			return FormatContentStream(ContentStream);
		}

		private static string FormatContentStream(StreamObject stream) {
			bool composite = false;
			ArrayAtom array = ArrayAtom.FromContentStream(stream.GetData());
			int indent = 0;
			HashSet<string> indentPlus = new HashSet<string>(new string[] { "q", "BT" });
			HashSet<string> indentMinus = new HashSet<string>(new string[] { "Q", "ET" });
			IList<Tuple<string, int>> items = OpAtom.Find(array);
			int index = 0;
			StringBuilder code = new StringBuilder();
			foreach (var pair in items) {
				string op = ((OpAtom)array[pair.Item2]).Text;
				// add indent to code
				if (indentMinus.Contains(op))
					indent--;
				for (int i = 0; i < indent; i++)
					code.Append(" ");
				AppendCommand(code, array, index, pair.Item2, composite);
				if (indentPlus.Contains(op))
					indent++;
				index = pair.Item2 + 1;
			}
			// write out any atoms that are left over
			for (int i = index; i < array.Count; i++) {
				code.Append(" ");
				code.Append(array[i].ToString());
			}
			// update dict atom at start
			DictAtom dict = (DictAtom)stream.Atom.Clone();
			Atom.SetItem(dict, "Length", new NumAtom(code.Length));
			StringBuilder result = new StringBuilder();
			result.AppendLine(dict.ToString());
			result.AppendLine("stream");
			result.AppendLine(code.ToString());
			result.AppendLine("endstream");
			return result.ToString();
		}

		private static void AppendCommand(StringBuilder sb, ArrayAtom array, int startIndex, int endIndex, bool multibyte) {
			for (int i = startIndex; i <= endIndex; i++) {
				if (i != startIndex)
					sb.Append(" ");
				Atom item = array[i];
				// We write arrays out individually so that we can override default cr lf behavior.
				// We write strings out with checking so that we can use a hex encoding if appropriate.
				// NB With ToString format overrides we can eliminate this function.
				ArrayAtom itemArray = item as ArrayAtom;
				StringAtom itemString = item as StringAtom;
				if (itemArray != null) {
					int n = itemArray.Count;
					sb.Append("[");
					for (int j = 0; j < n; j++) {
						sb.Append(itemArray[j].ToString());
						if (j != n - 1)
							sb.Append(" ");
					}
					sb.Append("]");
				}
				else if (itemString != null) {
					var data = itemString.Data;
					int hexCount = 0;
					for (int j = 0; j < data.Length; j++) {
						if (data[j] < 32 || data[j] > 127)
							hexCount++;
					}
					if (hexCount > data.Length / 3) {
						sb.Append('<');
						foreach (var b in data)
							sb.Append(b.ToString("X2"));
						sb.Append('>');
					}
					else {
						sb.Append(item.ToString());
					}
				}
				else {
					sb.Append(item.ToString());
				}
			}
			sb.AppendLine();
		}
	}

	public class GraphicsScanner : ContentStreamScanner {
		[DebuggerDisplay("Bounds = {Bounds} Text = \"{Text}\"")]
		public class Fixture {
			public Fixture(SD2.GraphicsPath path, int offset, int length, byte[] data) { PdfPath = path; Offset = offset; Length = length; Data = data; }
			public SD2.GraphicsPath PdfPath { get; set; }
			public SD2.GraphicsPath Path { get; set; }
			public SD.Region Region { get; set; }
			public double Area { get; set; }
			public int Offset { get; set; }
			public int Length { get; set; }
			public byte[] Data { get; set; }
			public int OwnerID { get; set; }
			public string Text {
				get {
					int length = Math.Min(Data.Length - Offset, Length);
					if (Offset < 0 || length <= 0)
						return "";
					return ASCIIEncoding.ASCII.GetString(Data, Offset, length).TrimEnd();
				}
			}
			public string Context {
				get {
					return ASCIIEncoding.ASCII.GetString(Data, 0, Data.Length);
				}
			}
			public RectangleF Bounds {
				get {
					return Path != null ? Path.GetBounds() : new RectangleF();
				}
			}
		}
		public class Graphics {
			public List<Fixture> All { get; set; } = new List<Fixture>();
			public Dictionary<int, List<Fixture>> Container { get; set; } = new Dictionary<int, List<Fixture>>();
			public void Transform(float top, float dpi) {
				var xform = new SD2.Matrix();
				xform.Scale(1, -1, SD2.MatrixOrder.Append);
				xform.Translate(0, (float)top, SD2.MatrixOrder.Append);
				float scale = dpi / 72.0f;
				xform.Scale(scale, scale, SD2.MatrixOrder.Append);
				foreach (var part in All) {
					part.Path = (SD2.GraphicsPath)part.PdfPath.Clone();
					part.Path.Transform(xform);
					try {
						for (int i = 0; i < 10; i++) {
							part.Area = 0;
							part.Region = new Region(part.Path);
							foreach (var rect in part.Region.GetRegionScans(new SD2.Matrix()))
								part.Area += Math.Abs(rect.Width * rect.Height);
							if (part.Area > 1)
								break;
							// Widen throws out of memory if path is a point
							part.Path.Widen(new Pen(Color.Black, 1));
						}
					}
					catch {
					}
				}
			}
		}

		private static readonly HashSet<string> _ops;

		static GraphicsScanner() {
			List<string> state = new List<string>();
			state.AddRange(ContentStreamScanner.OperatorCategories.PathPainting);
			state.AddRange(ContentStreamScanner.OperatorCategories.PathConstruction);
			state.AddRange(ContentStreamScanner.OperatorCategories.ClippingPaths);
			state.AddRange(ContentStreamScanner.OperatorCategories.SpecialGraphicsState);
			state.AddRange(ContentStreamScanner.OperatorCategories.TextObjects);
			state.AddRange(ContentStreamScanner.OperatorCategories.TextState);
			state.AddRange(ContentStreamScanner.OperatorCategories.TextPositioning);
			state.AddRange(ContentStreamScanner.OperatorCategories.TextShowing);
			state.AddRange(ContentStreamScanner.OperatorCategories.XObjects);
			state.Add("Meta:InlineImage");
			_ops = new HashSet<string>(state);
		}

		private IndirectObject _owner = null;
		private byte[] _data = null;
		private SD.PointF _point = new PointF();
		private SD2.GraphicsPath _path = null;
		private Graphics _results = null;
		private int _start = 0, _pos = 0;

		public GraphicsScanner(IndirectObject owner) : base() {
			if (owner == null)
				throw new NullReferenceException(nameof(owner));
			_owner = owner;
			Operators = _ops;
			DoWidths = true;
			DoShowText = true;
			DoUnicode = false;
		}

		public Graphics GetResults() {
			Process();
			return _results;
		}

		public void Process() {
			var contentIDs = new List<int>();
			var contentEnds = new List<int>();
			if (_owner is FormXObject) {
				FormXObject xo = (FormXObject)_owner;
				_data = new byte[xo.CopyDecompressedData()];
				xo.CopyDecompressedData(_data);
				contentIDs.Add(xo.ID);
				contentEnds.Add(_data.Length);
			}
			else if (_owner is Page) {
				Page page = (Page)_owner;
				int offset = 0;
				foreach (var so in page.GetLayers()) {
					int length = so.CopyDecompressedData();
					contentIDs.Add(so.ID);
					contentEnds.Add(offset + length);
					offset += length + 1;
				}
				_data = page.GetContentData();
			}
			else
				throw new Exception("Object is not a content stream type.");
			_point = new PointF();
			_path = null;
			_results = new Graphics();
			_results.All = new List<Fixture>();
			// get data
			IList<int> offsets = null;
			ArrayAtom array = ArrayAtom.FromContentStream(_data, out offsets);
			offsets.Add(_data.Length);
			Debug.Assert(offsets.Count == array.Count + 1);
			// process
			_start = _pos = 0;
			Process(_owner, array);
			// convert ArrayAtom based offsets to byte based offsets
			foreach (var part in _results.All) {
				Debug.Assert(part.Length >= 0);
				if (part.Offset + part.Length < offsets.Count) {
					int p1 = offsets[part.Offset];
					int p2 = offsets[part.Offset + part.Length];
					Debug.Assert(p2 - p1 >= 0);
					part.Offset = p1;
					part.Length = p2 - p1;
				}
				else {
					part.Offset = 0;
					part.Length = 0;
				}
			}
			// identify original containers
			if (true) {
				List<Fixture> list = null;
				int item = -1, last = -1, id = -1;
				foreach (var part in _results.All) {
					while (part.Offset >= last) {
						item++;
						last = contentEnds[item];
						list = new List<Fixture>();
						id = contentIDs[item];
						_results.Container[id] = list;
					}
					part.OwnerID = id;
					list.Add(part);
				}
			}
			// add in annotations
			if (_owner is Page) {
				Page page = (Page)_owner;
				foreach (var annot in page.GetAnnotations()) {
					var path = MakeRectPath(annot.Rect);
					var desc = $"Annotation ID {annot.ID}";
					var data = ASCIIEncoding.ASCII.GetBytes(desc);
					var fixture = new Fixture(path, 0, data.Length, data);
					_results.All.Add(fixture);
					var list = new List<Fixture>();
					list.Add(fixture);
					_results.Container[annot.ID] = list;
				}
			}
		}

		public override void ProcessItem(IndirectObject owner, ArrayAtom contents, string op, int pos) {
			_pos = pos;
			try {
				base.ProcessItem(owner, contents, op, pos);
			}
			catch (Exception ex) {
#if FALSE
				Debug.Assert(false, "Unexpected exception. Probable corrupt stream.");
#endif
			}
			switch (op) {
				// path construction
				case "m":
					if (true) {
						if (_path == null) {
							_start = pos - 2;
							_path = new SD2.GraphicsPath();
						}
						_path.StartFigure();
						_point = GetPointF(contents, pos, 0, 1);
					}
					break;
				case "l":
					if (true) {
						if (_path == null) {
							_start = pos - 2;
							_path = new SD2.GraphicsPath();
						}
						PointF point = GetPointF(contents, pos, 0, 1);
						_path.AddLine(_point, point);
						_point = point;
					}
					break;
				case "c":
					if (true) {
						if (_path == null) {
							_start = pos - 6;
							_path = new SD2.GraphicsPath();
						}
						PointF control1 = GetPointF(contents, pos, 0, 3);
						PointF control2 = GetPointF(contents, pos, 1, 3);
						PointF point = GetPointF(contents, pos, 2, 3);
						_path.AddBezier(_point, control1, control2, point);
						_point = point;
					}
					break;
				case "v":
					if (true) {
						if (_path == null) {
							_start = pos - 4;
							_path = new SD2.GraphicsPath();
						}
						PointF control = GetPointF(contents, pos, 0, 2);
						PointF point = GetPointF(contents, pos, 1, 2);
						_path.AddBezier(_point, _point, control, point);
						_point = point;
					}
					break;
				case "y":
					if (true) {
						if (_path == null) {
							_start = pos - 4;
							_path = new SD2.GraphicsPath();
						}
						PointF control = GetPointF(contents, pos, 0, 2);
						PointF point = GetPointF(contents, pos, 1, 2);
						_path.AddBezier(_point, control, point, point);
						_point = point;
					}
					break;
				case "h":
					if (true) {
						if (_path == null) {
							_start = pos;
							_path = new SD2.GraphicsPath();
						}
						_path.CloseFigure();
					}
					break;
				case "re":
					if (true) {
						if (_path == null) {
							_start = pos - 4;
							_path = new SD2.GraphicsPath();
						}
						_path.StartFigure();
						_point = GetPointF(contents, pos, 0, 2);
						PointF size = GetPointF(contents, pos, 1, 2);
						RectangleF rectF = new RectangleF(_point.X, _point.Y, size.X, size.Y);
						_path.AddLine(rectF.X, rectF.Y, rectF.X + rectF.Width, rectF.Y);
						_path.AddLine(rectF.X + rectF.Width, rectF.Y, rectF.X + rectF.Width, rectF.Y + rectF.Height);
						_path.AddLine(rectF.X + rectF.Width, rectF.Y + rectF.Height, rectF.X, rectF.Y + rectF.Height);
						_path.CloseFigure();
					}
					break;
				// path drawing
				case "S":
				case "s":
				case "f":
				case "F":
				case "f*":
				case "B":
				case "B*":
				case "b":
				case "b*":
				case "n":
					if (_path != null) {
						_path.Transform(State.CTM.Matrix);
						_results.All.Add(new Fixture(_path, _start, pos - _start + 1, _data));
						_start = pos;
						_path = null;
						_point = new PointF();
					}
					else {
#if FALSE
						var snip = new ArrayAtom();
						for (int i = pos - 20; i < pos + 20 && i >= 0; i++)
							snip.Add(contents[i].Clone());
						string snippet = snip.ToString();
						Debug.Assert(false, "Degenerate path detected.");
#endif
					}
					break;
				// xobject
				case "Do":
					if (true) {
						var pair = Resources.GetResource(_owner, ResourceType.XObject, GetName(contents, pos));
						FormXObject formX = pair != null ? pair.Object as FormXObject : null;
						XRect rect = formX != null ? formX.BBox : XRect.FromSides(0, 0, 1, 1);
						RectangleF rectF = new RectangleF((float)rect.Left, (float)rect.Top, (float)rect.Width, (float)rect.Height);
						var path = MakeRectPath(rect);
						if (formX != null && formX.Matrix != null)
							path.Transform(formX.Matrix.Matrix);
						path.Transform(State.CTM.Matrix);
						_results.All.Add(new Fixture(path, pos - 1, 2, _data));
					}
					break;
				// other
				case "Meta:InlineImage":
					if (true) {
						XRect rect = XRect.FromSides(0, 0, 1, 1);
						var path = new SD2.GraphicsPath();
						path.AddLine((float)rect.Left, (float)rect.Bottom, (float)rect.Right, (float)rect.Bottom);
						path.AddLine((float)rect.Right, (float)rect.Bottom, (float)rect.Right, (float)rect.Top);
						path.AddLine((float)rect.Right, (float)rect.Top, (float)rect.Left, (float)rect.Top);
						path.CloseFigure();
						path.Transform(State.CTM.Matrix);
						_results.All.Add(new Fixture(path, pos, 1, _data));
					}
					break;
			}
		}

		public override void ShowText(List<int> codes, List<int> widths, List<double> advances, double advanceTotal, StringBuilder text, List<string> strings, bool vertical) {
			advanceTotal /= State.TextFontSize;
			var rect = !vertical ? XRect.FromSides(0, 0, advanceTotal, 1) : XRect.FromSides(0, 0, 1, advanceTotal);
			var trm = Text.GetTextRenderingMatrix(State); // includes font size and CTM
			var corners = trm.TransformPoints(rect.GetCorners());
			var points = new PointF[corners.Length];
			for (int i = 0; i < corners.Length; i++)
				points[i] = new PointF((float)corners[i].X, (float)corners[i].Y);
			var path = new SD2.GraphicsPath();
			path.AddLines(points);
			path.CloseFigure();
			_results.All.Add(new Fixture(path, _pos - 1, 2, _data));
			base.ShowText(codes, widths, advances, advanceTotal, text, strings, vertical);
		}

		private static string GetName(ArrayAtom array, int pos) {
			return array[pos - 1, (Enum)null];
		}

		private static PointF GetPointF(ArrayAtom array, int pos, int index, int count) {
			pos = pos - (count * 2) + (index * 2);
			return new PointF((float)array[pos, (double)0], (float)array[pos + 1, (double)0]);
		}

		private SD2.GraphicsPath MakeRectPath(XRect rect) {
			var path = new SD2.GraphicsPath();
			path.AddLine((float)rect.Left, (float)rect.Bottom, (float)rect.Right, (float)rect.Bottom);
			path.AddLine((float)rect.Right, (float)rect.Bottom, (float)rect.Right, (float)rect.Top);
			path.AddLine((float)rect.Right, (float)rect.Top, (float)rect.Left, (float)rect.Top);
			path.CloseFigure();
			return path;
		}
	}
}

