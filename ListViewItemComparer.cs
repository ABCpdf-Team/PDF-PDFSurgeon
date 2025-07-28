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
using System.Collections;
using System.Windows.Forms;
using System.Globalization;


namespace WebSupergoo.PDFSurgeon
{
	/// <summary>
	/// Summary description for ListViewItemComparer.
	/// </summary>
	public class ListViewItemComparer: IComparer
	{
		private int _column;
		private SortOrder _order;
		private IComparer _stringComparer;

		public ListViewItemComparer(){
			_column = 0;
			_order = SortOrder.None;
			_stringComparer = new CaseInsensitiveComparer(
				CultureInfo.InvariantCulture);
		}

		public int Column{
			get{ return _column; }
			set{ _column = value; }
		}

		public SortOrder Order{
			get{ return _order; }
			set{ _order = value; }
		}

		public int Compare(object x, object y){
			if(_order==SortOrder.None)
				return 0;

			ListViewItem itemX = (ListViewItem)x;
			ListViewItem itemY = (ListViewItem)y;

			int v = 0;
			if(_column==0 && TryCompareID(ref v, itemX, itemY))
				return v;

			return CompareStringColumn(itemX, itemY);
		}

		private bool TryCompareID(ref int outV, ListViewItem x, ListViewItem y){
			if(_order==SortOrder.None){
				outV = 0;
				return true;
			}

			ObjectExtractor xExtractor = x.Tag as ObjectExtractor;
			if(xExtractor==null)
				return false;

			ObjectExtractor yExtractor = y.Tag as ObjectExtractor;
			if(yExtractor==null)
				return false;

			outV = Comparer.DefaultInvariant.Compare(
				xExtractor.Object.ID, yExtractor.Object.ID);
			if(_order==SortOrder.Descending)
				outV = -outV;
			return true;
		}

		private int CompareStringColumn(ListViewItem x, ListViewItem y){
			if(_order==SortOrder.None)
				return 0;

			ListViewItem.ListViewSubItem sitemX = _column>=x.SubItems.Count?
				null: x.SubItems[_column];
			ListViewItem.ListViewSubItem sitemY = _column>=y.SubItems.Count?
				null: y.SubItems[_column];

			if(sitemX==sitemY)
				return 0;

			string xText = sitemX==null? "": sitemX.Text;
			string yText = sitemY==null? "": sitemY.Text;

			return xText=="" && yText==""? 0: xText==""? 1: yText==""? -1:
				_stringComparer.Compare(sitemX.Text, sitemY.Text);
		}
	}
}
