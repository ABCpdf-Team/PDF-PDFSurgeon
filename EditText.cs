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
using System.Windows.Forms;


namespace WebSupergoo.PDFSurgeon {
	public partial class EditText : Form {
		public EditText(MainForm parent, ObjectExtractor obj) {
			InitializeComponent();
			_parent = parent;
			_editor = new StreamEditor(obj);
		}

		private void EditText_Load(object sender, EventArgs e) {
			if (_editor.Stream != null && !_editor.Extractor.IsAscii) {
				if (_editor.Stream.Compressed) {
					const string message = "This object appears to contain compressed data rather than text.\r\n\r\nWould you like to decompress it prior to editing?";
					var action = MessageBox.Show(message, "Warning", MessageBoxButtons.YesNoCancel);
					if (action == DialogResult.Yes) {
						if (!_editor.Stream.Decompress()) {
							MessageBox.Show("Unable to decompress object.", "Warning", MessageBoxButtons.OK);
							return;
						}
					}
					else if (action == DialogResult.Cancel)
						return;
				}
				else {
					const string message = "This object appears to contain data rather than text.\r\n\r\nWould you like to ASCII 85 encode it for editing?";
					var action = MessageBox.Show(message, "Warning", MessageBoxButtons.YesNoCancel);
					if (action == DialogResult.Yes)
						_editor.Stream.CompressAscii85();
					else if (action == DialogResult.Cancel)
						return;
				}
			}
			Size size = Properties.Settings.Default.EditTextSize;
			if (size.Width > 30 && size.Height > 30)
				Size = size;
			Point pos = Properties.Settings.Default.EditTextPosition;
			if (pos.X != 0 && pos.Y != 0) {
				pos = new Point(_parent.Location.X + pos.X, _parent.Location.Y + pos.Y);
				var rect = Screen.GetBounds(pos);
				rect.Inflate(-20, -20);
				if (rect.Contains(pos))
					Location = pos;
			}
			_editor.Load();
			textBox1.Text = _editor.LiveText;
			// Unusual characters may change when put in text box.
			_editor.OriginalText = _editor.LiveText = textBox1.Text;
			bool isLive = _editor.ContentStream != null;
			liveUpdate.Enabled = isLive;
			trackBar1.Enabled = isLive;
			trackBar2.Enabled = isLive;
			formatContent.Enabled = isLive;
			openPdf.Enabled = isLive;

			Text = $"EditText ID: {_editor.Extractor.Object.ID}";
		}
		private void EditText_FormClosing(object sender, FormClosingEventArgs e) {
			Properties.Settings.Default.EditTextSize = Size;
			Properties.Settings.Default.EditTextPosition = new Point(Location.X - _parent.Location.X, Location.Y - _parent.Location.Y);
			Properties.Settings.Default.Save();
			_parent._editTexts.Remove(_editor.Extractor);
		}

		private MainForm _parent;
		private StreamEditor _editor;
		private GraphicsScanner.Fixture _fixture;

		private void textBox1_TextChanged(object sender, EventArgs e) {
			if (liveUpdate.Checked) {
				if (textBox1.Text != _editor.LiveText) {
					_editor.CurrentText = textBox1.Text;
					_parent.RefreshSelectedImage();
					_editor.LiveText = textBox1.Text;
				}
			}
			numericUpDown1.Enabled = false;
		}

		private void ok_Click(object sender, EventArgs e) {
			if ((textBox1.Text != _editor.OriginalText) || (textBox1.Text != _editor.LiveText)) {
				_editor.CurrentText = textBox1.Text;
				_parent.UpdateContent(_editor.Extractor);
				_parent.UpdateImage(_editor.Extractor, true);
				_parent.Dirty = true;
			}
			Close();
		}

		private void cancel_Click(object sender, EventArgs e) {
			if (textBox1.Text != _editor.OriginalText) {
				liveUpdate.Checked = true;
				textBox1.Text = _editor.OriginalText;
			}
			Close();
		}

		private void formatContent_Click(object sender, EventArgs e) {
			try {
				var s = _editor.FormatContentStream();
				if (s == null)
					return;
				textBox1.Text = s;
			}
			catch (Exception ex) {
				MessageBox.Show("Unable to format content stream. " + ex.Message);
			}
		}

		private void openPdf_Click(object sender, EventArgs e) {
			_parent.OpenSnapshot();
		}

		private void liveUpdate_CheckedChanged(object sender, EventArgs e) {
			if (liveUpdate.Checked && _editor.ResourceOwner != null) {
				_parent.SelectByObjectID(_editor.ResourceOwner.ID);
			}
		}

		private void trackBar1_Scroll(object sender, EventArgs e) {
			double lo = (double)trackBar1.Value / trackBar1.Maximum;
			double hi = (double)trackBar2.Value / trackBar2.Maximum;
			StringBuilder sb = _editor.GetPartialContentStream(lo, hi);
			if (sb != null) {
				string s = sb.ToString();
				textBox1.Text = "<< /Length " + s.Length + " >>\r\nstream\r\n" + s + "\r\nendstream\r\n";
				UpdateSearch();
			}
		}

		private void trackBar2_Scroll(object sender, EventArgs e) {
			double lo = (double)trackBar1.Value / trackBar1.Maximum;
			double hi = (double)trackBar2.Value / trackBar2.Maximum;
			StringBuilder sb = _editor.GetPartialContentStream(lo, hi);
			if (sb != null) {
				string s = sb.ToString();
				textBox1.Text = "<< /Length " + s.Length + " >>\r\nstream\r\n" + s + "\r\nendstream\r\n";
				UpdateSearch();
			}
		}

		private void timer1_Tick(object sender, EventArgs e) {
			var fixture = _parent.Fixture;
			if (fixture != _fixture) {
				if (fixture != null) {
					string txt = textBox1.Text;
					string keyword = "stream";
					int start = txt.IndexOf(keyword);
					start = start != -1 ? start + keyword.Length : -1;
					start = (start != -1) && (txt.Length > start) && (txt[start] == '\r') ? start + 1 : start;
					start = (start != -1) && (txt.Length > start) && (txt[start] == '\n') ? start + 1 : start;
					if (start != -1) {
						int offset = start + fixture.Offset;
						int length = Math.Min(textBox1.Text.Length - offset, fixture.Length);
						if (length > 0) {
							textBox1.Select(offset, length);
							textBox1.ScrollToCaret();
							textBox1.Focus();
						}
					}
				}
				_fixture = fixture;
			}
		}

		private void TextBox2_TextChanged(object sender, EventArgs e)
		{
			UpdateSearch();
		}

		private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
		{
			UpdateSearch();
		}

		bool _update = true;

		private void UpdateSearch()
		{
			string search = textBox2.Text;
			string text = textBox1.Text;
			numericUpDown1.Enabled = true;
			bool isEmpty = string.IsNullOrEmpty(search);
			List<int> positions = new List<int>();

			if (!isEmpty)
			{
				int p1 = 0;
				while (true)
				{
					int p2 = text.IndexOf(search, p1, StringComparison.OrdinalIgnoreCase);
					if (p2 == -1)
						break;
					positions.Add(p2);
					p1 = p2 + search.Length;
				}

				isEmpty = positions.Count == 0;
			}

			if (!isEmpty)
			{
				try
				{
					if (_update) {
						_update = false;
						numericUpDown1.Maximum = -1;
						numericUpDown1.Minimum = -positions.Count;
						textBox1.Select(positions[-((int)numericUpDown1.Value) - 1], search.Length);
						textBox1.ScrollToCaret();
						label3.Text = -(numericUpDown1.Value) + "/" + positions.Count.ToString();
					}
				}
				finally
				{
					_update = true;
				}
			}
			else {
				numericUpDown1.Maximum = 0;
				textBox1.Select(0, 0);
				label3.Text = "0/0";
				numericUpDown1.Enabled = false;
			}
		}
	}
}