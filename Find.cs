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
using System.Windows.Forms;


namespace WebSupergoo.PDFSurgeon {
	public partial class Find : Form {
		public Find(MainForm parent) {
			InitializeComponent();
			_parent = parent;
		}

		private MainForm _parent;

		static string _lastSearch = "";
		static bool _lastWholeWord = false;

		private void findNext_Click(object sender, EventArgs e) {
			_parent.Find(findText.Text, wholeWord.Checked);
			_parent.FindDialog = null;
			Close();
		}

		private void Cancel_Click(object sender, EventArgs e) {
			_parent.FindDialog = null;
			Close();
		}

		private void Find_Load(object sender, EventArgs e) {
			findText.Text = _lastSearch;
			wholeWord.Checked = _lastWholeWord;
		}

		private void Find_Shown(object sender, EventArgs e) {
		}

		private void findText_TextChanged(object sender, EventArgs e) {
		}

		private void Find_FormClosing(object sender, FormClosingEventArgs e) {
			_lastSearch = findText.Text;
			_lastWholeWord = wholeWord.Checked;
		}
	}
}