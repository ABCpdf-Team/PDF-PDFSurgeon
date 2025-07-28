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
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using SD2 = System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

using WebSupergoo.ABCpdf13;
using WebSupergoo.ABCpdf13.Objects;
using WebSupergoo.ABCpdf13.Atoms;
using WebSupergoo.ABCpdf13.Operations;

namespace WebSupergoo.PDFSurgeon {
	public sealed class ExtractorContext {
		private static readonly string[] _aActionEntryList = {
			"O", "C", "K", "F", "V", "C",
			"E", "X", "D", "U", "Fo", "Bl", "PO", "PC", "PV", "PI",
			"WC", "WS", "DS", "WP", "DP"
		};
		private static readonly string[] _aActionSearchList;

		static ExtractorContext() {
			_aActionSearchList = new string[_aActionEntryList.Length];
			for (int i = 0; i < _aActionEntryList.Length; ++i)
				_aActionSearchList[i] = ',' + _aActionEntryList[i] + ',';
		}

		private HashSet<int> _pageContentList;
		private Dictionary<int, int> _pageContentIdToPageId;
		private Dictionary<int, int> _pageContentIdToPageNumber;
		private List<int> _cmapList;
		private SortedDictionary<int, string> _xfaDict;
		private SortedDictionary<int, ScriptType> _scriptDict;
		private SortedDictionary<int, string> _type3GlyphDict;

		public ExtractorContext() {
			_pageContentList = new HashSet<int>();
			_pageContentIdToPageId = new Dictionary<int, int>();
			_pageContentIdToPageNumber = new Dictionary<int, int>();
			_cmapList = new List<int>();
			_xfaDict = new SortedDictionary<int, string>();
			_scriptDict = new SortedDictionary<int, ScriptType>();
			_type3GlyphDict = new SortedDictionary<int, string>();
		}

		public HashSet<int> PageContents { get { return _pageContentList; } }
		public Dictionary<int, int> PageContentIdToPageId { get { return _pageContentIdToPageId; } }
		public Dictionary<int, int> PageContentIdToPageNumber { get { return _pageContentIdToPageNumber; } }
		public List<int> CMaps { get { return _cmapList; } }
		public SortedDictionary<int, string> Xfa { get { return _xfaDict; } }
		public SortedDictionary<int, ScriptType> Scripts { get { return _scriptDict; } }
		public SortedDictionary<int, string> Type3Glyphs { get { return _type3GlyphDict; } }

		public void AddXfa(int id, string name) {
			string oldName;
			if (_xfaDict.TryGetValue(id, out oldName)) {
				if (oldName == name)
					return;
				if (name != null) {
					if (oldName == "")
						return;
					name = "";
				}
			}
			_xfaDict[id] = name;
		}

		public void AddScript(int id, ScriptType type) {
			ScriptType oldType;
			if (_scriptDict.TryGetValue(id, out oldType))
				oldType.Add(type);
			else
				_scriptDict[id] = type;
		}

		public void AddAction(Doc doc, int id, ScriptType type) {
			int jsId = doc.GetInfoInt(id, "/A*/JS:Ref");
			if (jsId != 0 && doc.GetInfo(id, "/A*/S*:Name") == "JavaScript")
				AddScript(jsId, type);
		}

		public void AddAdditionalAction(Doc doc, int id, ScriptType type) {
			string keys = doc.GetInfo(id, "/AA*:Keys");
			if (keys == "")
				return;
			keys = ',' + keys + ',';
			for (int i = 0; i < _aActionSearchList.Length; ++i) {
				if (keys.IndexOf(_aActionSearchList[i]) >= 0) {
					string path = "/AA*/" + _aActionEntryList[i];
					int jsId = doc.GetInfoInt(id, path + "*/JS:Ref");
					if (jsId != 0 && doc.GetInfo(id, path + "*/S*:Name") == "JavaScript")
						AddScript(jsId, type);
				}
			}
		}

		public void AddType3Glyph(Doc doc, int id) {
			string s = doc.GetInfo(id, "/CharProcs*:Keys");
			if (s != "") {
				string[] keys = s.Split(new char[] { ',' },
					StringSplitOptions.RemoveEmptyEntries);
				foreach (string name in keys) {
					int glyphId = doc.GetInfoInt(id, "/CharProcs*/" + name + ":Ref");
					if (glyphId != 0) {
						string oldName;
						if (_type3GlyphDict.TryGetValue(glyphId, out oldName)) {
							if (oldName == name || oldName == "")
								continue;
							_type3GlyphDict[glyphId] = "";
						}
						else
							_type3GlyphDict[glyphId] = name;
					}
				}
			}
		}
	}


	public class Preview {
		public static Image InitialImage { get; set; }
		public static Image ErrorImage { get; set; }

		public Page Page;
		public IndirectObject Target;
		public double Scale;
		public Image Image;
		public GraphicsScanner.Graphics Graphics;

		private Preview() { }

		public Preview(Page page, IndirectObject target) {
			Page = page;
			Image = InitialImage;
			Target = target;
			var mediaBox = page.MediaBox;
			double dimension = mediaBox != null ? Math.Max(mediaBox.Width, mediaBox.Height) : 0;
			Scale = dimension < 850 ? 1.0 : 850 / dimension;
		}

		public Preview(Bitmap bm) {
			Image = bm != null ? bm : ErrorImage;
			Scale = 1.0;
		}

		public void Reload() {
			if (Page == null)
				return;
			Image = InitialImage;
			Graphics = null;
			Doc doc = Page.Doc;
			doc.Page = Page.ID;
			doc.Rect.SetRect(doc.MediaBox);
			double top = doc.Rect.Top;
			double dpi = 72.0 * Scale;
			doc.Rendering.DotsPerInch = dpi;
			// We could run parts of this in a separate thread. However we need
			// to be careful here as the process of changing the value of the content stream
			// might interfere with an ongoing render operation with catastrophic results.
			try {
				using (var operation = new RenderOperation(doc)) {
					Image = operation.GetBitmap();
					var scanner = new GraphicsScanner(Target);
					var results = scanner.GetResults();
					results.Transform((float)top, (float)dpi);
					Graphics = results;
				}
			}
			catch {
				Image = ErrorImage;
			}
		}
	}

	public sealed class ScriptType {
		public string DocumentName;
		public string PageName;
		public string AnnotationName;

		public static ScriptType NewDocument(string name) {
			ScriptType v = new ScriptType();
			v.DocumentName = name;
			return v;
		}
		public static ScriptType NewPage(string name) {
			ScriptType v = new ScriptType();
			v.PageName = name;
			return v;
		}
		public static ScriptType NewAnnotation(string name) {
			ScriptType v = new ScriptType();
			v.AnnotationName = name;
			return v;
		}

		public void Add(ScriptType v) {
			if (DocumentName != v.DocumentName)
				DocumentName = DocumentName == null ? v.DocumentName : "";
			if (PageName != v.PageName)
				PageName = PageName == null ? v.PageName : "";
			if (AnnotationName != v.AnnotationName)
				AnnotationName = AnnotationName == null ? v.AnnotationName : "";
		}
	}

	public class Property {
		public Property(string key, string value) { K = key; V = value; }
		public string K { get; set; }
		public string V { get; set; }
		public string Summary { get { int n = 0; return K != "ID" && int.TryParse(V, out n) ? $"{K} {V}" : $"{V}"; } }
		public string Detail { get { return V.Contains(" ") ? $"{K}: \"{V}\"" : $"{K}: {V}"; } }
	}

	public class ObjectExtractor {
		public static ObjectExtractor FromIndirectObject(IndirectObject obj, ExtractorContext context) {
			if (obj == null)
				throw new ArgumentNullException("obj", "IndirectObject obj cannot be null.");

			if (obj is StreamObject)
				return StreamObjectExtractor.FromStreamObject((StreamObject)obj, context);
			if (obj is Annotation)
				return new AnnotationExtractor((Annotation)obj, context);
			if (obj is Page)
				return new PageExtractor((Page)obj, context);
			if (obj is Pages)
				return new PagesExtractor((Pages)obj, context);
			if (obj is FontObject)
				return new FontObjectExtractor((FontObject)obj, context);
			if (obj is Field)
				return FieldExtractor.FromField((Field)obj, context);
			if (obj is GraphicsState)
				return new GraphicsStateExtractor((GraphicsState)obj, context);
			if (obj is Bookmark)
				return BookmarkExtractor.FromBookmark((Bookmark)obj, context);
			if (obj is ColorSpace)
				return new ColorSpaceExtractor((ColorSpace)obj, context);
			if (obj is Catalog)
				return new CatalogExtractor((Catalog)obj, context);

			return new ObjectExtractor(obj, context);
		}

		private IndirectObject _obj;
		private ExtractorContext _context;

		public ObjectExtractor(IndirectObject obj, ExtractorContext context) {
			_obj = obj;
			_context = context;
		}

		public Doc Doc { get { return _obj.Doc; } }
		public IndirectObject Object { get { return _obj; } }
		public ExtractorContext Context { get { return _context; } }

		public virtual bool IsAscii { get { return true; } }

		public virtual void Invalidate() { }

		public virtual string GetValue(Doc doc) {
			if ((doc == null) || (_obj == null))
				return "";
			return doc.GetInfo(_obj.ID, "value").Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n");
		}

		public virtual void SetValue(Doc doc, string value) {
			if ((doc == null) || (_obj == null))
				return;
			int id = _obj.ID;
			doc.SetInfo(id, "value", value);
			_obj = doc.ObjectSoup[id];
			Invalidate();
		}

		public List<Property> GetProperties() {
			var props = new List<Property>();
			try {
				GetProperties(props);
			}
			catch (Exception ex) {
				props.Add(new Property("Error", $"{ex.Message}"));
			}
			return props;
		}

		public virtual void GetProperties(List<Property> props) {
			props.Add(new Property("ID", $"{_obj.ID}"));
			props.Add(new Property("Name", $"{_obj.GetType().Name}"));
		}

		public virtual Preview GetPreview(bool reload) { return null; }
		public virtual string GetAtom() { return Object.Atom.ToString(); }
		public virtual string GetContent() { return Object.Atom.ToString(); }

		protected static string FormatList(string[] list) {
			if (list == null || list.Length <= 0)
				return "";

			StringBuilder builder = new StringBuilder();
			builder.Append(list[0]);
			for (int i = 1; i < list.Length; ++i)
				builder.Append(", ").Append(list[i]);

			return builder.ToString();
		}
	}

	public class CatalogExtractor : ObjectExtractor {
		public CatalogExtractor(Catalog obj, ExtractorContext context) : base(obj, context) {
			Doc doc = obj.Doc;
			int id = obj.ID;
			int treeId = doc.GetInfoInt(id, "/Names*/JavaScript:Ref");
			if (treeId != 0)
				AddScriptInTree(doc, context, treeId);

			int count = doc.GetInfoInt(id, "/AcroForm*/XFA*:Count");
			if (count > 0) {
				for (int i = 0; i < count; i += 2) {
					context.AddXfa(doc.GetInfoInt(id,
						"/AcroForm*/XFA*[" + (i + 1).ToString() + "]:Ref"),
						doc.GetInfo(id, "/AcroForm*/XFA*[" + i.ToString() + "]*:Text"));
				}
			}
			else {
				int xfaId = doc.GetInfoInt(id, "/AcroForm*/XFA:Ref");
				if (xfaId != 0)
					context.AddXfa(xfaId, null);
			}
		}
		private static void AddScriptInTree(Doc doc, ExtractorContext context, int nodeId) {
			int count = doc.GetInfoInt(nodeId, "/Kids*:Count");
			if (count > 0) {
				for (int i = 0; i < count; ++i) {
					int childNodeId = doc.GetInfoInt(nodeId, "/Kids*[" + i.ToString() + "]:Ref");
					if (childNodeId != 0)
						AddScriptInTree(doc, context, childNodeId);    // recur on childNodeID
				}
				return;
			}

			count = doc.GetInfoInt(nodeId, "/Names*:Count");
			for (int i = 0; i < count; i += 2) {
				string path = "/Names*[" + (i + 1).ToString();
				int jsId = doc.GetInfoInt(nodeId, path + "]*/JS:Ref");
				if (jsId != 0 && doc.GetInfo(nodeId, path + "]*/S*:Name") == "JavaScript") {
					context.AddScript(jsId, ScriptType.NewDocument(
						doc.GetInfo(nodeId, "/Names*[" + i.ToString() + "]*:Text")));
				}
			}
		}

		public new Catalog Object { get { return (Catalog)base.Object; } }

		public override void GetProperties(List<Property> props) {
			base.GetProperties(props);
			Catalog obj = Object;
			props.Add(new Property("Revisions", $"{obj.Soup.Revisions}"));
		}
	}

	public class ColorSpaceExtractor : ObjectExtractor {
		public ColorSpaceExtractor(ColorSpace obj, ExtractorContext context) : base(obj, context) { }

		public new ColorSpace Object { get { return (ColorSpace)base.Object; } }

		public override void GetProperties(List<Property> props) {
			base.GetProperties(props);
			ColorSpace obj = Object;
			props.Add(new Property("Name", $"{obj.Name}"));
			props.Add(new Property("ColorSpace", $"{ obj.ColorSpaceType}"));
			props.Add(new Property("Components", $"{ obj.Components}"));
		}
	}

	public class BookmarkExtractor : ObjectExtractor {
		public static BookmarkExtractor FromBookmark(Bookmark obj, ExtractorContext context) {
			if (obj is Outline)
				return new OutlineExtractor((Outline)obj, context);

			return new BookmarkExtractor(obj, context);
		}

		public BookmarkExtractor(Bookmark obj, ExtractorContext context) : base(obj, context) { }

		public new Bookmark Object { get { return (Bookmark)base.Object; } }

		public override void GetProperties(List<Property> props) {
			base.GetProperties(props);
			Bookmark obj = Object;
			props.Add(new Property("Title", $"{obj.Title}"));
		}
	}

	public class OutlineExtractor : BookmarkExtractor {
		public OutlineExtractor(Outline obj, ExtractorContext context) : base(obj, context) { }

		public new Outline Object { get { return (Outline)base.Object; } }
	}

	public class GraphicsStateExtractor : ObjectExtractor {
		public GraphicsStateExtractor(GraphicsState obj, ExtractorContext context) : base(obj, context) { }

		public new GraphicsState Object { get { return (GraphicsState)base.Object; } }
	}

	public class FieldExtractor : ObjectExtractor {
		public static FieldExtractor FromField(Field obj, ExtractorContext context) {
			if (obj is Signature)
				return new SignatureExtractor((Signature)obj, context);

			return new FieldExtractor(obj, context);
		}

		public bool IsCombinedWithWidget = false;
		public FieldExtractor(Field obj, ExtractorContext context) : base(obj, context) { }

		public new Field Object { get { return (Field)base.Object; } }

		public override void GetProperties(List<Property> props) {
			if (!IsCombinedWithWidget)
				base.GetProperties(props);
			Field obj = Object;
			props.Add(new Property("Name", $"{obj.Name}"));
			props.Add(new Property("Value", $"{obj.Value}"));
			props.Add(new Property("Type", $"{obj.FieldType}"));
			if (!string.IsNullOrWhiteSpace(obj.Format))
				props.Add(new Property("Format", $"{obj.Format}"));
		}
	}

	public class SignatureExtractor : FieldExtractor {
		public SignatureExtractor(Signature obj, ExtractorContext context) : base(obj, context) { Object.Validate(); }

		public new Signature Object { get { return (Signature)base.Object; } }

		public override void GetProperties(List<Property> props) {
			base.GetProperties(props);
			Signature obj = Object;
			props.Add(new Property("Revision", $"{obj.SigningRevision} of {obj.Soup.Revisions}"));
			props.Add(new Property("IsModified", $"{obj.IsModified}"));
			props.Add(new Property("IsTimeValid", $"{obj.IsTimeValid}"));
			props.Add(new Property("IsSecure", $"{obj.IsSecure}"));
			props.Add(new Property("IsTrusted", $"{obj.IsTrusted}"));
			if (obj.Signer != null)
				props.Add(new Property("Signer", $"{obj.Signer}"));
			if (obj.Reason != null)
				props.Add(new Property("Reason", $"{obj.Reason}"));
			if (obj.Location != null)
				props.Add(new Property("Location", $"{obj.Location}"));
		}
	}

	public class FontObjectExtractor : ObjectExtractor {
		public FontObjectExtractor(FontObject obj, ExtractorContext context)
			: base(obj, context) {
			Doc doc = obj.Doc;
			int id = obj.ID;
			int cmapId = doc.GetInfoInt(id, "/ToUnicode:Ref");
			if (cmapId != 0)
				context.CMaps.Add(cmapId);
			context.AddType3Glyph(doc, id);
		}

		public new FontObject Object { get { return (FontObject)base.Object; } }
	}

	public class AnnotationExtractor : ObjectExtractor {
		public FieldExtractor _fieldExtractor = null;
		public FormXObjectExtractor _formxExtractor = null;

		public AnnotationExtractor(Annotation obj, ExtractorContext context) : base(obj, context) {
			Doc doc = obj.Doc;
			int id = obj.ID;
			ScriptType scryptType = ScriptType.NewAnnotation(id.ToString());
			context.AddAction(doc, id, scryptType);
			context.AddAdditionalAction(doc, id, scryptType);
			var field = obj.Doc.Form[obj.FullName];
			if (field != null) {
				_fieldExtractor = FieldExtractor.FromField(field, context);
				_fieldExtractor.IsCombinedWithWidget = true;
			}
		}

		public new Annotation Object { get { return (Annotation)base.Object; } }

		public override void GetProperties(List<Property> props) {
			base.GetProperties(props);
			Annotation obj = Object;
			props.Add(new Property("SubType", $"{obj.SubType}"));
			if (_fieldExtractor != null)
				_fieldExtractor.GetProperties(props);

		}

		public override Preview GetPreview(bool reload) {
			Annotation obj = Object;
			if (_formxExtractor == null) {
				var ap = obj.NormalAppearance;
				if (ap == null) {
					obj.UpdateAppearance();
					ap = obj.NormalAppearance;
				}
				if (ap != null)
					_formxExtractor = new FormXObjectExtractor(ap, Context);
			}
			if (_formxExtractor != null)
				return _formxExtractor.GetPreview(reload);
			return null;
		}
	}

	public class PageExtractor : ObjectExtractor {
		private string _atom;
		private int _pageNum;
		private Preview _image;

		public PageExtractor(Page obj, ExtractorContext context) : base(obj, context) {
			_atom = obj.Atom.ToString();
			Doc doc = obj.Doc;
			int id = obj.ID;
			doc.Page = id;
			if (doc.Page == id)
				_pageNum = doc.PageNumber;
			context.AddAdditionalAction(doc, id, ScriptType.NewPage(id.ToString()));
			foreach (StreamObject so in obj.GetLayers()) {
				context.PageContents.Add(so.ID);
				context.PageContentIdToPageId[so.ID] = id;
				context.PageContentIdToPageNumber[so.ID] = _pageNum;
			}
		}

		public new Page Object { get { return (Page)base.Object; } }

		public override void GetProperties(List<Property> props) {
			base.GetProperties(props);
			Page obj = Object;
			if (_pageNum != 0)
				props.Add(new Property("Page", $"{_pageNum}"));
		}

		public override Preview GetPreview(bool reload) {
			if (_image == null || reload) {
				_image = new Preview(Object, Object);
				_image.Reload();
			}
			return _image;
		}

		public override string GetAtom() { return _atom; }
	}

	public class PagesExtractor : ObjectExtractor {
		public PagesExtractor(Pages obj, ExtractorContext context) : base(obj, context) { }

		public new Pages Object { get { return (Pages)base.Object; } }

		public override void GetProperties(List<Property> props) {
			base.GetProperties(props);
			Pages obj = Object;
			props.Add(new Property("Count", $"{obj.Count}"));
		}
	}

	public class StreamObjectExtractor : ObjectExtractor {
		public static StreamObjectExtractor FromStreamObject(StreamObject obj, ExtractorContext context) {
			if (obj is PixMap)
				return new PixMapExtractor((PixMap)obj, context);
			if (obj is Layer)
				return LayerExtractor.FromLayer((Layer)obj, context);
			if (obj is IccProfile)
				return new IccProfileExtractor((IccProfile)obj, context);
			if (obj is FormXObject)
				return new FormXObjectExtractor((FormXObject)obj, context);

			return new StreamObjectExtractor(obj, context);
		}

		private string _atom;

		public StreamObjectExtractor(StreamObject obj, ExtractorContext context) : base(obj, context) { }

		public new StreamObject Object { get { return (StreamObject)base.Object; } }

		public override bool IsAscii {
			get {
				StreamObject so = Object;
				if (so == null) return true;
				byte[] data = so.GetData();
				for (int i = 0; i < data.Length; i++) {
					if ((data[i] >= 32) && (data[i] < 128))
						continue;
					if ((data[i] != '\r') && (data[i] != '\n') && (data[i] != '\t'))
						return false;
				}
				return base.IsAscii;
			}
		}

		public override string GetValue(Doc doc) {
			if (!ContentPageNumber.HasValue)
				return base.GetValue(doc);
			StreamObject so = Object;
			if ((doc == null) || (so == null))
				return "";
			return doc.GetInfo(so.ID, "value");
		}

		public string StreamType { get; set; }
		public int? ContentPageNumber { get; set; }

		public override void Invalidate() {
			_atom = null;
			StreamType = null;
			base.Invalidate();
		}

		public override void GetProperties(List<Property> props) {
			base.GetProperties(props);
			UpdateStreamType();
			StreamObject obj = Object;
			props.Add(new Property("StreamType", $"{StreamType}"));
			props.Add(new Property("Compression", $"{obj.Compression}"));
			props.Add(new Property("Length", $"{obj.Length}"));
		}

		private void UpdateStreamType() {
			if (StreamType != null)
				return;

			if (ContentPageNumber != null) {
				StreamType = $"Page {ContentPageNumber} Content";
				return;
			}

			StreamObject obj = Object;
			Doc doc = obj.Doc;
			int id = obj.ID;
			string type = doc.GetInfo(id, "/Type*:Name");
			if (type == "Metadata")
				StreamType = "Metadata";
			else if (doc.GetInfo(id, "/PatternType") != "")
				StreamType = "Pattern";
			else if (doc.GetInfoInt(id, "/FunctionType*:Num") == 4)
				StreamType = "Function/Type-4";
			else
				StreamType = "";
		}

		private bool ContainsText() {
			StreamObject obj = Object;
			CompressionType[] comps = obj.Compressions;
			CompressionType comp = comps.Length > 0 ? comps[0] : CompressionType.None;
			return (comp == CompressionType.AsciiHex) || (comp == CompressionType.Ascii85);
		}

		public override string GetAtom() {
			if (_atom == null)
				_atom = base.GetAtom();
			return _atom;
		}

		public override string GetContent() {
			UpdateStreamType();
			if (StreamType != "") {
				if (_atom == null)
					_atom = base.GetAtom();
				StreamObject obj = Object;
				int len = obj.CopyDecompressedData();
				byte[] data = new byte[len];
				obj.CopyDecompressedData(data);
				return Encoding.ASCII.GetString(data);
			}
			if (ContainsText()) {
				return Object.GetText();
			}
			return base.GetContent();
		}

		public string GetSnippet() {
			const int max = 250;
			string txt = Object.GetText();
			if (txt.Length > max)
				txt = txt.Substring(0, max);
			txt = Regex.Escape(txt);
			txt = txt.Replace("\u0000", @"\0");
			if (txt.Length > max)
				txt = txt.Substring(0, max - 3) + "...";
			return txt;
		}
	}

	public class IccProfileExtractor : StreamObjectExtractor {
		public IccProfileExtractor(IccProfile obj, ExtractorContext context) : base(obj, context) { }

		public new IccProfile Object { get { return (IccProfile)base.Object; } }
	}

	public class PixMapExtractor : StreamObjectExtractor {
		private Preview _image;
		public PixMapExtractor(PixMap obj, ExtractorContext context) : base(obj, context) { }

		public new PixMap Object { get { return (PixMap)base.Object; } }

		public override void GetProperties(List<Property> props) {
			base.GetProperties(props);
			PixMap obj = Object;
			int n = obj.Components;
			int bpc = obj.BitsPerComponent;
			props.Add(new Property("PixMap", $"{obj.Width}\u00d7{obj.Height} {obj.ColorSpaceType} {n} col {bpc} bit"));
		}

		public override Preview GetPreview(bool reload) {
			if (_image == null || reload)
				if (Object.Width > 0 && Object.Height > 0)
					_image = new Preview(Object.GetBitmap());
			return _image;
		}
	}

	public class LayerExtractor : StreamObjectExtractor {
		public static LayerExtractor FromLayer(Layer obj, ExtractorContext context) {
			return new LayerExtractor(obj, context);
		}

		public LayerExtractor(Layer obj, ExtractorContext context) : base(obj, context) { }

		public new Layer Object { get { return (Layer)base.Object; } }

		public override void GetProperties(List<Property> props) {
			base.GetProperties(props);
			Layer obj = Object;
			props.Add(new Property("Rect", $"{obj.Rect}"));
		}
	}

	public class FormXObjectExtractor : StreamObjectExtractor {
		private Preview _image;

		public FormXObjectExtractor(FormXObject obj, ExtractorContext context) : base(obj, context) { }

		public new FormXObject Object { get { return (FormXObject)base.Object; } }

		public override Preview GetPreview(bool reload) {
			if (_image == null || reload) {
				var pair = MakePage();
				_image = new Preview(pair.Item1, pair.Item2);
				_image.Reload();
			}
			return _image;
		}

		private Tuple<Page, FormXObject> MakePage() {
			FormXObject me = Object;
			ObjectSoupSubset subset = new ObjectSoupSubset(me.Soup);
			subset.AddFamily(me);
			Doc doc = new Doc();
			doc.MediaBox.String = me.BBox.String;
			doc.Rect.String = doc.MediaBox.String;
			doc.Page = doc.AddPage();
			subset.CopyTo(doc.ObjectSoup);
			foreach (var io in doc.ObjectSoup)
				if (io != null)
					io.Transcode();
			int id = subset.RemapIDs[me.ID];
			FormXObject form = doc.ObjectSoup[id] as FormXObject;
			form.Matrix = null;
			doc.AddXObject(form);
			Page page = (Page)doc.ObjectSoup[doc.Page];
			return new Tuple<Page, FormXObject>(page, form);
		}
	}

	internal sealed class TemporaryFile : IDisposable {
		private string mPath;

		private TemporaryFile() { }

		public TemporaryFile(string ext) {
			mPath = GetTempFilePath(ext);
		}

		~TemporaryFile() {
			Dispose(false);
		}

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		// sealed class cannot introduce virtual method
		// warned if sealed class introduces protected member
		private /*virtual*/ void Dispose(bool disposing) {
			try {
				DeleteFile();
			}
			catch {
			}
		}

		public string Path { get { return mPath; } }

		public void DeleteFile() {
			if (mPath != null) {
				if (File.Exists(mPath))
					File.Delete(mPath);
				mPath = null;
			}
		}

		private static string GetTempFilePath(string ext) {
			if (string.IsNullOrWhiteSpace(ext))
				ext = ".dat";
			else if (!ext.StartsWith("."))
				ext = "." + ext;
			return System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString() + ext);
		}
	}

	internal class Navigator {
		private List<int> _ids = new List<int>();
		private int _pos = -1;

		public void Add(int id) {
			if (_pos >= 0 && _pos < _ids.Count && _ids[_pos] == id)
				return;
			while (_pos >= 0 && _pos < _ids.Count - 1)
				_ids.RemoveAt(_ids.Count - 1);
			_ids.Add(id);
			_pos = _ids.Count - 1;
		}

		public bool CanNavigate() {
			return _ids.Count > 0;
		}

		public int Forward() {
			_pos = Math.Min(_pos + 1, _ids.Count - 1);
			return _ids[_pos];
		}

		public int Back() {
			_pos = Math.Max(_pos - 1, 0);
			return _ids[_pos];
		}
	}
}
