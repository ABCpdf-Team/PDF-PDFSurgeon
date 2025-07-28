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
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using SD = System.Drawing;
using SD2 = System.Drawing.Drawing2D;

using System.Drawing.Imaging;
using WebSupergoo.ABCpdf13;
using WebSupergoo.ABCpdf13.Atoms;
using WebSupergoo.ABCpdf13.Objects;

namespace WebSupergoo.PDFSurgeon
{
	/// <summary>
	/// Summary description for MainForm.
	/// </summary>
	public class MainForm : System.Windows.Forms.Form {
		private Doc _doc;
		private Navigator _nav;
		private string _filePath;
		private string _tempPath;
		private List<TemporaryFile> _tempFiles;
		private bool _dirty;
		private ObjectExtractor _selectedExtractor;
		private GraphicsScanner.Graphics _graphics;
		private GraphicsScanner.Fixture _fixture;
		public Dictionary<ObjectExtractor, EditText> _editTexts;

		private Find _findDialog;
		private ListView lst;
		private OpenFileDialog dlgOpenFile;
		private FontDialog dlgSelectFont;
		private SplitContainer split;
		private ColumnHeader cheaderID;
		private ColumnHeader cheaderType;
		private ColumnHeader cheaderName;
		private ColumnHeader cheaderInfo1;
		private ColumnHeader cheaderInfo2;
		private TabControl tabInfo;
		private TabPage pageAtom;
		private TabPage pageContent;
		private RichTextBox txtContent;
		private TabPage pageImage;
		private PictureBox pict;
		private MenuStrip mstrip;
		private ToolStripMenuItem mitemFile;
		private ToolStripMenuItem mitemOpen;
		private ToolStripSeparator mitemSep1;
		private ToolStripMenuItem mitemExit;
		private ImageList imglist;
		private Panel pnlPicture;
		private ContextMenuStrip cmenuIndirectObject;
		private ToolStripMenuItem mitemDecompress;
		private ToolStripMenuItem mItemMakeAscii85;
		private ToolStripMenuItem mItemCompressFlate;
		private ToolStripMenuItem mItemCompressJpeg75;
		private ToolStripMenuItem mItemCompressJpeg50;
		private ToolStripMenuItem mItemCompressJpeg25;
		private ToolStripMenuItem mitemSaveAs;
		private ToolStripMenuItem mitemSave;
		private SaveFileDialog dlgSaveFile;
		private ToolStripMenuItem mItemEdit;
		private ToolStripMenuItem mItemRefresh;
		private ToolStripMenuItem mItemMakeAsciiHex;
		private ToolStripMenuItem editToolStripMenuItem;
		private ToolStripMenuItem findToolStripMenuItem;
		private ToolStripMenuItem selectAllToolStripMenuItem;
		private ToolStripMenuItem viewToolStripMenuItem;
		private ToolStripMenuItem showAllToolStripMenuItem;
		private ToolStripMenuItem selectedToolStripMenuItem;
		private ToolStripMenuItem unselectedToolStripMenuItem;
		private ToolStripMenuItem compactToolStripMenuItem;
		private ToolStripMenuItem saveImageToolStripMenuItem;
		private SaveFileDialog dlgSaveImage;
		private ToolStripMenuItem realizeToolStripMenuItem;
		private ToolStripMenuItem flattenToolStripMenuItem;
		private ToolStripMenuItem mItemFlattenFormXObjects;
		private ToolStripMenuItem mItemExtract;
		private ToolStripSeparator toolStripSeparator1;
		private ToolStripMenuItem overprintToolStripMenuItem;
		private ToolStripMenuItem mitemSaveAll;
		private FolderBrowserDialog dlgSaveFolder;
		private ToolStripMenuItem openInAcrobatToolStripMenuItem;
		private TreeView atomTree;
		private ToolStripMenuItem toolStripBack;
		private ToolStripMenuItem toolStripForward;
		private ToolTip toolTip1;
		private Timer timer1;
		private ToolStripMenuItem exportToolStripMenuItem;
		private SaveFileDialog dlgExportFile;
		private IContainer components;

		public MainForm() {
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			lst.ListViewItemSorter = new ListViewItemComparer();
			Clear();
			string[] args = Environment.GetCommandLineArgs();
			if (args.Length > 1) {
				try {
					Open(args[1]);
				}
				catch (Exception exc) {
					MessageBox.Show(exc.Message);
				}
			}
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing) {
			if (disposing) {
				Clear();
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.dlgOpenFile = new System.Windows.Forms.OpenFileDialog();
			this.dlgSelectFont = new System.Windows.Forms.FontDialog();
			this.split = new System.Windows.Forms.SplitContainer();
			this.lst = new System.Windows.Forms.ListView();
			this.cheaderID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.cheaderType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.cheaderName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.cheaderInfo1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.cheaderInfo2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.cmenuIndirectObject = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mitemDecompress = new System.Windows.Forms.ToolStripMenuItem();
			this.mItemMakeAscii85 = new System.Windows.Forms.ToolStripMenuItem();
			this.mItemMakeAsciiHex = new System.Windows.Forms.ToolStripMenuItem();
			this.mItemCompressFlate = new System.Windows.Forms.ToolStripMenuItem();
			this.mItemCompressJpeg75 = new System.Windows.Forms.ToolStripMenuItem();
			this.mItemCompressJpeg50 = new System.Windows.Forms.ToolStripMenuItem();
			this.mItemCompressJpeg25 = new System.Windows.Forms.ToolStripMenuItem();
			this.flattenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mItemFlattenFormXObjects = new System.Windows.Forms.ToolStripMenuItem();
			this.realizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mItemRefresh = new System.Windows.Forms.ToolStripMenuItem();
			this.mItemEdit = new System.Windows.Forms.ToolStripMenuItem();
			this.mItemExtract = new System.Windows.Forms.ToolStripMenuItem();
			this.tabInfo = new System.Windows.Forms.TabControl();
			this.pageImage = new System.Windows.Forms.TabPage();
			this.pnlPicture = new System.Windows.Forms.Panel();
			this.pict = new System.Windows.Forms.PictureBox();
			this.pageAtom = new System.Windows.Forms.TabPage();
			this.atomTree = new System.Windows.Forms.TreeView();
			this.pageContent = new System.Windows.Forms.TabPage();
			this.txtContent = new System.Windows.Forms.RichTextBox();
			this.imglist = new System.Windows.Forms.ImageList(this.components);
			this.mstrip = new System.Windows.Forms.MenuStrip();
			this.mitemFile = new System.Windows.Forms.ToolStripMenuItem();
			this.mitemOpen = new System.Windows.Forms.ToolStripMenuItem();
			this.mitemSave = new System.Windows.Forms.ToolStripMenuItem();
			this.mitemSaveAs = new System.Windows.Forms.ToolStripMenuItem();
			this.mitemSaveAll = new System.Windows.Forms.ToolStripMenuItem();
			this.mitemSep1 = new System.Windows.Forms.ToolStripSeparator();
			this.mitemExit = new System.Windows.Forms.ToolStripMenuItem();
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.findToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.compactToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.showAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.selectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.unselectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.overprintToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openInAcrobatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripBack = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripForward = new System.Windows.Forms.ToolStripMenuItem();
			this.dlgSaveFile = new System.Windows.Forms.SaveFileDialog();
			this.dlgSaveImage = new System.Windows.Forms.SaveFileDialog();
			this.dlgSaveFolder = new System.Windows.Forms.FolderBrowserDialog();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.dlgExportFile = new System.Windows.Forms.SaveFileDialog();
			((System.ComponentModel.ISupportInitialize)(this.split)).BeginInit();
			this.split.Panel1.SuspendLayout();
			this.split.Panel2.SuspendLayout();
			this.split.SuspendLayout();
			this.cmenuIndirectObject.SuspendLayout();
			this.tabInfo.SuspendLayout();
			this.pageImage.SuspendLayout();
			this.pnlPicture.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pict)).BeginInit();
			this.pageAtom.SuspendLayout();
			this.pageContent.SuspendLayout();
			this.mstrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// dlgOpenFile
			// 
			this.dlgOpenFile.DefaultExt = "pdf";
			this.dlgOpenFile.Filter = "PDF Document (*.pdf)|*.pdf";
			this.dlgOpenFile.Title = "Open a PDF Document";
			// 
			// split
			// 
			this.split.Dock = System.Windows.Forms.DockStyle.Fill;
			this.split.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.split.Location = new System.Drawing.Point(0, 33);
			this.split.Name = "split";
			// 
			// split.Panel1
			// 
			this.split.Panel1.Controls.Add(this.lst);
			// 
			// split.Panel2
			// 
			this.split.Panel2.Controls.Add(this.tabInfo);
			this.split.Size = new System.Drawing.Size(778, 511);
			this.split.SplitterDistance = 455;
			this.split.TabIndex = 3;
			// 
			// lst
			// 
			this.lst.AllowDrop = true;
			this.lst.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.cheaderID,
            this.cheaderType,
            this.cheaderName,
            this.cheaderInfo1,
            this.cheaderInfo2});
			this.lst.ContextMenuStrip = this.cmenuIndirectObject;
			this.lst.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lst.FullRowSelect = true;
			this.lst.HideSelection = false;
			this.lst.Location = new System.Drawing.Point(0, 0);
			this.lst.Name = "lst";
			this.lst.Size = new System.Drawing.Size(455, 511);
			this.lst.TabIndex = 0;
			this.lst.UseCompatibleStateImageBehavior = false;
			this.lst.View = System.Windows.Forms.View.Details;
			this.lst.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lst_ColumnClick);
			this.lst.SelectedIndexChanged += new System.EventHandler(this.lst_SelectedIndexChanged);
			this.lst.DragDrop += new System.Windows.Forms.DragEventHandler(this.lst_DragDrop);
			this.lst.DragOver += new System.Windows.Forms.DragEventHandler(this.lst_DragOver);
			this.lst.DoubleClick += new System.EventHandler(this.lst_DoubleClick);
			// 
			// cheaderID
			// 
			this.cheaderID.Text = "ID";
			// 
			// cheaderType
			// 
			this.cheaderType.Text = "Type";
			this.cheaderType.Width = 86;
			// 
			// cheaderName
			// 
			this.cheaderName.Text = "Details";
			this.cheaderName.Width = 117;
			// 
			// cheaderInfo1
			// 
			this.cheaderInfo1.Text = "Info";
			this.cheaderInfo1.Width = 77;
			// 
			// cheaderInfo2
			// 
			this.cheaderInfo2.Text = "Info";
			this.cheaderInfo2.Width = 300;
			// 
			// cmenuIndirectObject
			// 
			this.cmenuIndirectObject.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.cmenuIndirectObject.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mitemDecompress,
            this.mItemMakeAscii85,
            this.mItemMakeAsciiHex,
            this.mItemCompressFlate,
            this.mItemCompressJpeg75,
            this.mItemCompressJpeg50,
            this.mItemCompressJpeg25,
            this.flattenToolStripMenuItem,
            this.mItemFlattenFormXObjects,
            this.realizeToolStripMenuItem,
            this.saveImageToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.mItemRefresh,
            this.mItemEdit,
            this.mItemExtract});
			this.cmenuIndirectObject.Name = "textBoxContextMenu";
			this.cmenuIndirectObject.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.cmenuIndirectObject.ShowCheckMargin = true;
			this.cmenuIndirectObject.ShowImageMargin = false;
			this.cmenuIndirectObject.Size = new System.Drawing.Size(339, 484);
			this.cmenuIndirectObject.Text = "cmenuIndirectObject";
			this.cmenuIndirectObject.Opening += new System.ComponentModel.CancelEventHandler(this.cmenuTextBox_Opening);
			// 
			// mitemDecompress
			// 
			this.mitemDecompress.Name = "mitemDecompress";
			this.mitemDecompress.Size = new System.Drawing.Size(338, 32);
			this.mitemDecompress.Text = "&Decompress";
			this.mitemDecompress.Click += new System.EventHandler(this.mitemDecompress_Click);
			// 
			// mItemMakeAscii85
			// 
			this.mItemMakeAscii85.Name = "mItemMakeAscii85";
			this.mItemMakeAscii85.Size = new System.Drawing.Size(338, 32);
			this.mItemMakeAscii85.Text = "&Make ASCII85";
			this.mItemMakeAscii85.Click += new System.EventHandler(this.mItemMakeAscii_Click);
			// 
			// mItemMakeAsciiHex
			// 
			this.mItemMakeAsciiHex.Name = "mItemMakeAsciiHex";
			this.mItemMakeAsciiHex.Size = new System.Drawing.Size(338, 32);
			this.mItemMakeAsciiHex.Text = "&Make ASCII Hex";
			this.mItemMakeAsciiHex.Click += new System.EventHandler(this.mItemMakeAsciiHex_Click);
			// 
			// mItemCompressFlate
			// 
			this.mItemCompressFlate.Name = "mItemCompressFlate";
			this.mItemCompressFlate.Size = new System.Drawing.Size(338, 32);
			this.mItemCompressFlate.Text = "Compress Flate";
			this.mItemCompressFlate.Click += new System.EventHandler(this.mItemCompressFlate_Click);
			// 
			// mItemCompressJpeg75
			// 
			this.mItemCompressJpeg75.Name = "mItemCompressJpeg75";
			this.mItemCompressJpeg75.Size = new System.Drawing.Size(338, 32);
			this.mItemCompressJpeg75.Text = "Compress JPEG High Quality";
			this.mItemCompressJpeg75.Click += new System.EventHandler(this.mItemCompressJpeg75_Click);
			// 
			// mItemCompressJpeg50
			// 
			this.mItemCompressJpeg50.Name = "mItemCompressJpeg50";
			this.mItemCompressJpeg50.Size = new System.Drawing.Size(338, 32);
			this.mItemCompressJpeg50.Text = "Compress JPEG Medium Quality";
			this.mItemCompressJpeg50.Click += new System.EventHandler(this.mItemCompressJpeg50_Click);
			// 
			// mItemCompressJpeg25
			// 
			this.mItemCompressJpeg25.Name = "mItemCompressJpeg25";
			this.mItemCompressJpeg25.Size = new System.Drawing.Size(338, 32);
			this.mItemCompressJpeg25.Text = "Compress JPEG Low Quality";
			this.mItemCompressJpeg25.Click += new System.EventHandler(this.mItemCompressJpeg25_Click);
			// 
			// flattenToolStripMenuItem
			// 
			this.flattenToolStripMenuItem.Name = "flattenToolStripMenuItem";
			this.flattenToolStripMenuItem.Size = new System.Drawing.Size(338, 32);
			this.flattenToolStripMenuItem.Text = "Compress Page Contents";
			this.flattenToolStripMenuItem.Click += new System.EventHandler(this.flattenToolStripMenuItem_Click);
			// 
			// mItemFlattenFormXObjects
			// 
			this.mItemFlattenFormXObjects.Name = "mItemFlattenFormXObjects";
			this.mItemFlattenFormXObjects.Size = new System.Drawing.Size(338, 32);
			this.mItemFlattenFormXObjects.Text = "Stamp Form XObjects Into Page";
			this.mItemFlattenFormXObjects.Click += new System.EventHandler(this.mItemFlattenFormXObjects_Click);
			// 
			// realizeToolStripMenuItem
			// 
			this.realizeToolStripMenuItem.Name = "realizeToolStripMenuItem";
			this.realizeToolStripMenuItem.Size = new System.Drawing.Size(338, 32);
			this.realizeToolStripMenuItem.Text = "Realize Image";
			this.realizeToolStripMenuItem.Click += new System.EventHandler(this.realizeToolStripMenuItem_Click);
			// 
			// saveImageToolStripMenuItem
			// 
			this.saveImageToolStripMenuItem.Name = "saveImageToolStripMenuItem";
			this.saveImageToolStripMenuItem.Size = new System.Drawing.Size(338, 32);
			this.saveImageToolStripMenuItem.Text = "Save Image...";
			this.saveImageToolStripMenuItem.Click += new System.EventHandler(this.saveImageToolStripMenuItem_Click);
			// 
			// exportToolStripMenuItem
			// 
			this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
			this.exportToolStripMenuItem.Size = new System.Drawing.Size(338, 32);
			this.exportToolStripMenuItem.Text = "Export Data...";
			this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
			// 
			// mItemRefresh
			// 
			this.mItemRefresh.Name = "mItemRefresh";
			this.mItemRefresh.Size = new System.Drawing.Size(338, 32);
			this.mItemRefresh.Text = "Refresh";
			this.mItemRefresh.Click += new System.EventHandler(this.mItemRefresh_Click);
			// 
			// mItemEdit
			// 
			this.mItemEdit.Name = "mItemEdit";
			this.mItemEdit.Size = new System.Drawing.Size(338, 32);
			this.mItemEdit.Text = "Edit Item...";
			this.mItemEdit.Click += new System.EventHandler(this.mItemEdit_Click);
			// 
			// mItemExtract
			// 
			this.mItemExtract.Name = "mItemExtract";
			this.mItemExtract.Size = new System.Drawing.Size(338, 32);
			this.mItemExtract.Text = "Delete Other Pages...";
			this.mItemExtract.Click += new System.EventHandler(this.mItemExtract_Click);
			// 
			// tabInfo
			// 
			this.tabInfo.Controls.Add(this.pageImage);
			this.tabInfo.Controls.Add(this.pageAtom);
			this.tabInfo.Controls.Add(this.pageContent);
			this.tabInfo.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabInfo.ImageList = this.imglist;
			this.tabInfo.Location = new System.Drawing.Point(0, 0);
			this.tabInfo.Name = "tabInfo";
			this.tabInfo.SelectedIndex = 0;
			this.tabInfo.Size = new System.Drawing.Size(319, 511);
			this.tabInfo.TabIndex = 2;
			this.tabInfo.SelectedIndexChanged += new System.EventHandler(this.tabInfo_SelectedIndexChanged);
			// 
			// pageImage
			// 
			this.pageImage.AutoScroll = true;
			this.pageImage.Controls.Add(this.pnlPicture);
			this.pageImage.ImageIndex = 0;
			this.pageImage.Location = new System.Drawing.Point(4, 29);
			this.pageImage.Name = "pageImage";
			this.pageImage.Size = new System.Drawing.Size(311, 478);
			this.pageImage.TabIndex = 0;
			this.pageImage.Text = "Image";
			this.pageImage.UseVisualStyleBackColor = true;
			// 
			// pnlPicture
			// 
			this.pnlPicture.AutoScroll = true;
			this.pnlPicture.BackColor = System.Drawing.Color.Azure;
			this.pnlPicture.Controls.Add(this.pict);
			this.pnlPicture.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlPicture.Location = new System.Drawing.Point(0, 0);
			this.pnlPicture.Margin = new System.Windows.Forms.Padding(0);
			this.pnlPicture.Name = "pnlPicture";
			this.pnlPicture.Size = new System.Drawing.Size(311, 478);
			this.pnlPicture.TabIndex = 1;
			this.pnlPicture.Click += new System.EventHandler(this.pict_Click);
			// 
			// pict
			// 
			this.pict.Location = new System.Drawing.Point(0, 0);
			this.pict.Name = "pict";
			this.pict.Size = new System.Drawing.Size(311, 493);
			this.pict.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pict.TabIndex = 0;
			this.pict.TabStop = false;
			this.pict.Click += new System.EventHandler(this.pict_Click);
			this.pict.Paint += new System.Windows.Forms.PaintEventHandler(this.pict_Paint);
			this.pict.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.pict_MouseDoubleClick);
			this.pict.MouseLeave += new System.EventHandler(this.pict_MouseLeave);
			this.pict.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pict_MouseMove);
			// 
			// pageAtom
			// 
			this.pageAtom.Controls.Add(this.atomTree);
			this.pageAtom.ImageIndex = 0;
			this.pageAtom.Location = new System.Drawing.Point(4, 29);
			this.pageAtom.Name = "pageAtom";
			this.pageAtom.Size = new System.Drawing.Size(417, 614);
			this.pageAtom.TabIndex = 1;
			this.pageAtom.Text = "Atom";
			this.pageAtom.UseVisualStyleBackColor = true;
			// 
			// atomTree
			// 
			this.atomTree.Dock = System.Windows.Forms.DockStyle.Fill;
			this.atomTree.Location = new System.Drawing.Point(0, 0);
			this.atomTree.Name = "atomTree";
			this.atomTree.ShowNodeToolTips = true;
			this.atomTree.Size = new System.Drawing.Size(417, 614);
			this.atomTree.TabIndex = 0;
			this.atomTree.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.atomTree_NodeMouseDoubleClick);
			// 
			// pageContent
			// 
			this.pageContent.Controls.Add(this.txtContent);
			this.pageContent.ImageIndex = 0;
			this.pageContent.Location = new System.Drawing.Point(4, 29);
			this.pageContent.Name = "pageContent";
			this.pageContent.Size = new System.Drawing.Size(417, 614);
			this.pageContent.TabIndex = 2;
			this.pageContent.Text = "Content";
			this.pageContent.UseVisualStyleBackColor = true;
			// 
			// txtContent
			// 
			this.txtContent.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtContent.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtContent.Location = new System.Drawing.Point(0, 0);
			this.txtContent.Name = "txtContent";
			this.txtContent.ReadOnly = true;
			this.txtContent.Size = new System.Drawing.Size(417, 614);
			this.txtContent.TabIndex = 0;
			this.txtContent.Text = "";
			this.txtContent.WordWrap = false;
			// 
			// imglist
			// 
			this.imglist.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imglist.ImageStream")));
			this.imglist.TransparentColor = System.Drawing.Color.Transparent;
			this.imglist.Images.SetKeyName(0, "cross.ico");
			this.imglist.Images.SetKeyName(1, "tick.ico");
			// 
			// mstrip
			// 
			this.mstrip.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
			this.mstrip.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.mstrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mitemFile,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.toolStripBack,
            this.toolStripForward});
			this.mstrip.Location = new System.Drawing.Point(0, 0);
			this.mstrip.Name = "mstrip";
			this.mstrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.mstrip.Size = new System.Drawing.Size(778, 33);
			this.mstrip.TabIndex = 4;
			this.mstrip.Text = "mstrip";
			// 
			// mitemFile
			// 
			this.mitemFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mitemOpen,
            this.mitemSave,
            this.mitemSaveAs,
            this.mitemSaveAll,
            this.mitemSep1,
            this.mitemExit});
			this.mitemFile.Name = "mitemFile";
			this.mitemFile.Size = new System.Drawing.Size(54, 29);
			this.mitemFile.Text = "&File";
			// 
			// mitemOpen
			// 
			this.mitemOpen.Name = "mitemOpen";
			this.mitemOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
			this.mitemOpen.Size = new System.Drawing.Size(255, 34);
			this.mitemOpen.Text = "&Open...";
			this.mitemOpen.Click += new System.EventHandler(this.mitemOpen_Click);
			// 
			// mitemSave
			// 
			this.mitemSave.Name = "mitemSave";
			this.mitemSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.mitemSave.Size = new System.Drawing.Size(255, 34);
			this.mitemSave.Text = "&Save";
			this.mitemSave.Click += new System.EventHandler(this.mitemSave_Click);
			// 
			// mitemSaveAs
			// 
			this.mitemSaveAs.Name = "mitemSaveAs";
			this.mitemSaveAs.Size = new System.Drawing.Size(255, 34);
			this.mitemSaveAs.Text = "&Save As...";
			this.mitemSaveAs.Click += new System.EventHandler(this.mitemSaveAs_Click);
			// 
			// mitemSaveAll
			// 
			this.mitemSaveAll.Name = "mitemSaveAll";
			this.mitemSaveAll.Size = new System.Drawing.Size(255, 34);
			this.mitemSaveAll.Text = "Export &Revisions...";
			this.mitemSaveAll.Click += new System.EventHandler(this.mitemSaveAll_Click);
			// 
			// mitemSep1
			// 
			this.mitemSep1.Name = "mitemSep1";
			this.mitemSep1.Size = new System.Drawing.Size(252, 6);
			// 
			// mitemExit
			// 
			this.mitemExit.Name = "mitemExit";
			this.mitemExit.Size = new System.Drawing.Size(255, 34);
			this.mitemExit.Text = "E&xit";
			this.mitemExit.Click += new System.EventHandler(this.mitemExit_Click);
			// 
			// editToolStripMenuItem
			// 
			this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.findToolStripMenuItem,
            this.selectAllToolStripMenuItem,
            this.compactToolStripMenuItem});
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			this.editToolStripMenuItem.Size = new System.Drawing.Size(58, 29);
			this.editToolStripMenuItem.Text = "&Edit";
			// 
			// findToolStripMenuItem
			// 
			this.findToolStripMenuItem.Name = "findToolStripMenuItem";
			this.findToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
			this.findToolStripMenuItem.Size = new System.Drawing.Size(248, 34);
			this.findToolStripMenuItem.Text = "&Find All...";
			this.findToolStripMenuItem.Click += new System.EventHandler(this.findToolStripMenuItem_Click);
			// 
			// selectAllToolStripMenuItem
			// 
			this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
			this.selectAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
			this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(248, 34);
			this.selectAllToolStripMenuItem.Text = "Select &All";
			this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
			// 
			// compactToolStripMenuItem
			// 
			this.compactToolStripMenuItem.Name = "compactToolStripMenuItem";
			this.compactToolStripMenuItem.Size = new System.Drawing.Size(248, 34);
			this.compactToolStripMenuItem.Text = "Compact";
			this.compactToolStripMenuItem.Click += new System.EventHandler(this.compactToolStripMenuItem_Click);
			// 
			// viewToolStripMenuItem
			// 
			this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showAllToolStripMenuItem,
            this.selectedToolStripMenuItem,
            this.unselectedToolStripMenuItem,
            this.toolStripSeparator1,
            this.overprintToolStripMenuItem,
            this.openInAcrobatToolStripMenuItem});
			this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
			this.viewToolStripMenuItem.Size = new System.Drawing.Size(65, 29);
			this.viewToolStripMenuItem.Text = "View";
			// 
			// showAllToolStripMenuItem
			// 
			this.showAllToolStripMenuItem.Name = "showAllToolStripMenuItem";
			this.showAllToolStripMenuItem.Size = new System.Drawing.Size(360, 34);
			this.showAllToolStripMenuItem.Text = "All";
			this.showAllToolStripMenuItem.Click += new System.EventHandler(this.allToolStripMenuItem_Click);
			// 
			// selectedToolStripMenuItem
			// 
			this.selectedToolStripMenuItem.Name = "selectedToolStripMenuItem";
			this.selectedToolStripMenuItem.Size = new System.Drawing.Size(360, 34);
			this.selectedToolStripMenuItem.Text = "Selected Only";
			this.selectedToolStripMenuItem.Click += new System.EventHandler(this.selectedToolStripMenuItem_Click);
			// 
			// unselectedToolStripMenuItem
			// 
			this.unselectedToolStripMenuItem.Name = "unselectedToolStripMenuItem";
			this.unselectedToolStripMenuItem.Size = new System.Drawing.Size(360, 34);
			this.unselectedToolStripMenuItem.Text = "Unselected Only";
			this.unselectedToolStripMenuItem.Click += new System.EventHandler(this.unselectedToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(357, 6);
			// 
			// overprintToolStripMenuItem
			// 
			this.overprintToolStripMenuItem.Name = "overprintToolStripMenuItem";
			this.overprintToolStripMenuItem.Size = new System.Drawing.Size(360, 34);
			this.overprintToolStripMenuItem.Text = "Overprint";
			this.overprintToolStripMenuItem.Click += new System.EventHandler(this.overprintToolStripMenuItem_Click);
			// 
			// openInAcrobatToolStripMenuItem
			// 
			this.openInAcrobatToolStripMenuItem.Name = "openInAcrobatToolStripMenuItem";
			this.openInAcrobatToolStripMenuItem.ShortcutKeyDisplayString = "";
			this.openInAcrobatToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.V)));
			this.openInAcrobatToolStripMenuItem.Size = new System.Drawing.Size(360, 34);
			this.openInAcrobatToolStripMenuItem.Text = "View in Acrobat...";
			this.openInAcrobatToolStripMenuItem.Click += new System.EventHandler(this.openInAcrobatToolStripMenuItem_Click);
			// 
			// toolStripBack
			// 
			this.toolStripBack.AutoToolTip = true;
			this.toolStripBack.Name = "toolStripBack";
			this.toolStripBack.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Left)));
			this.toolStripBack.Size = new System.Drawing.Size(44, 29);
			this.toolStripBack.Text = "◀";
			this.toolStripBack.ToolTipText = "Click to go back";
			this.toolStripBack.Click += new System.EventHandler(this.toolStripBack_Click);
			// 
			// toolStripForward
			// 
			this.toolStripForward.AutoToolTip = true;
			this.toolStripForward.Name = "toolStripForward";
			this.toolStripForward.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Right)));
			this.toolStripForward.Size = new System.Drawing.Size(44, 29);
			this.toolStripForward.Text = "▶";
			this.toolStripForward.ToolTipText = "Click to go forwards";
			this.toolStripForward.Click += new System.EventHandler(this.toolStripForward_Click);
			// 
			// dlgSaveFile
			// 
			this.dlgSaveFile.DefaultExt = "pdf";
			this.dlgSaveFile.FileName = "Untitled";
			this.dlgSaveFile.Filter = "PDF Document (*.pdf)|*.pdf";
			this.dlgSaveFile.Title = "Save a PDF Document";
			// 
			// dlgSaveImage
			// 
			this.dlgSaveImage.DefaultExt = "jpg";
			this.dlgSaveImage.FileName = "Untitled";
			this.dlgSaveImage.Filter = "Image Files(*.JPG;*.PNG;*.TIF;*.GIF;*.BMP;)|*.JPG;*.PNG;*.TIF;*.GIF;*.BMP|All fil" +
    "es (*.*)|*.*";
			this.dlgSaveImage.Title = "Save an Image";
			// 
			// dlgSaveFolder
			// 
			this.dlgSaveFolder.Description = "Export the revisions of the saved document in a diffable format.";
			// 
			// timer1
			// 
			this.timer1.Enabled = true;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// dlgExportFile
			// 
			this.dlgExportFile.AddExtension = false;
			this.dlgExportFile.DefaultExt = "txt";
			this.dlgExportFile.FileName = "Untitled";
			this.dlgExportFile.Filter = "Data Files(*.TXT;*.DAT;*.TTF;*.JPG;)|*.TXT;*.DAT;*.TTF;*.JPG;|All files (*.*)|*.*" +
    "";
			this.dlgExportFile.SupportMultiDottedExtensions = true;
			this.dlgExportFile.Title = "Export Data";
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(8, 19);
			this.ClientSize = new System.Drawing.Size(778, 544);
			this.Controls.Add(this.split);
			this.Controls.Add(this.mstrip);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.mstrip;
			this.Name = "MainForm";
			this.Text = "PDF Surgeon";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.split.Panel1.ResumeLayout(false);
			this.split.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.split)).EndInit();
			this.split.ResumeLayout(false);
			this.cmenuIndirectObject.ResumeLayout(false);
			this.tabInfo.ResumeLayout(false);
			this.pageImage.ResumeLayout(false);
			this.pnlPicture.ResumeLayout(false);
			this.pnlPicture.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pict)).EndInit();
			this.pageAtom.ResumeLayout(false);
			this.pageContent.ResumeLayout(false);
			this.mstrip.ResumeLayout(false);
			this.mstrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() {
			Application.EnableVisualStyles();
			Application.Run(new MainForm());
		}

		private void Clear() {
			ResetItemView();
			lst.Items.Clear();
			_graphics = null;
			_fixture = null;
			_editTexts = new Dictionary<ObjectExtractor, EditText>();
			_nav = new Navigator();
		}

		private void ResetItemView() {
			atomTree.Nodes.Clear();
			//txtAtom.Clear();
			txtContent.Clear();
			pict.Visible = false;

			pageImage.ImageIndex = 0;
			pageAtom.ImageIndex = 0;
			pageContent.ImageIndex = 0;
		}

		public void UpdateInfo(HashSet<int> ids) {
			lst.BeginUpdate();
			try {
				foreach (ListViewItem item in lst.Items) {
					ObjectExtractor extractor = item.Tag as ObjectExtractor;
					if ((extractor == null) || (extractor.Object == null))
						continue;
					if (ids == null || ids.Contains(extractor.Object.ID)) {
						var info = extractor.GetProperties();
						for (int i = 0; (i < item.SubItems.Count) && (i < info.Count); i++)
							item.SubItems[i].Text = info[i].Summary;
						extractor.Invalidate();
						UpdateContent(extractor);
					}
				}
			}
			finally {
				lst.EndUpdate();
			}
		}

		public void UpdateListItemInfo(ListViewItem item) {
			if (item == null)
				return;
			ObjectExtractor extractor = item.Tag as ObjectExtractor;
			if ((extractor == null) || (extractor.Object == null))
				return;
			var info = extractor.GetProperties();
			for (int i = 0; (i < item.SubItems.Count) && (i < info.Count); i++)
				item.SubItems[i].Text = info[i].Summary;
			extractor.Invalidate();
			UpdateContent(extractor);
		}

		public void UpdateContent(ObjectExtractor item) {
			// first atom...
			atomTree.SuspendLayout();
			atomTree.Nodes.Clear();
			TreeNode root = atomTree.Nodes.Add($"{item.Object.ID} {item.Object.Gen} R");
			UpdateContent(root, new Font(atomTree.Font, FontStyle.Bold), item.Object.Atom);
			StreamObjectExtractor so = item as StreamObjectExtractor;
			if (item is StreamObjectExtractor) {
				atomTree.Nodes.Add($"stream");
				atomTree.Nodes.Add(((StreamObjectExtractor)item).GetSnippet());
				atomTree.Nodes.Add($"endstream");
			}
			// ... then content
			string content = item.GetContent();
			if (content == null) {
				txtContent.Clear();
				pageContent.ImageIndex = 0;
			}
			else {
				SetTextBoxText(content, txtContent);
				pageContent.ImageIndex = 1;
			}
			// ... then properties
			TreeNode props = atomTree.Nodes.Add($"Calculated Properties");
			var items = item.GetProperties();
			for (int i = 2; i < items.Count; i++)
				props.Nodes.Add(items[i].Detail);
			// ... finish
			atomTree.ExpandAll();
			root.EnsureVisible();
			pageAtom.ImageIndex = 1;
			_nav.Add(item.Object.ID);
			atomTree.ResumeLayout();
		}

		private void UpdateContent(TreeNode root, Font bold, Atom atom) {
			root.Tag = atom;
			if (atom is DictAtom) {
				DictAtom dict = atom as DictAtom;
				root.Text += " <<>>";
				foreach (var pair in dict) {
					TreeNode key = root.Nodes.Add("/" + pair.Key);
					UpdateContent(key, bold, pair.Value);
				}
			}
			else if (atom is ArrayAtom) {
				ArrayAtom array = atom as ArrayAtom;
				bool isRectOrMatrix = false;
				if (array.Count == 4 || array.Count == 6) {
					isRectOrMatrix = true;
					for (int i = 0; i < array.Count; i++) {
						if (array[i] is NumAtom == false) {
							isRectOrMatrix = false;
							break;
						}
					}
				}
				if (isRectOrMatrix) {
					root.Text += " [";
					for (int i = 0; i < array.Count; i++)
						root.Text += " " + array[i].ToString();
					root.Text += " ]";
					if (array.Count == 4) {
						double x1 = ((NumAtom)array[0]).Real;
						double y1 = ((NumAtom)array[1]).Real;
						double x2 = ((NumAtom)array[2]).Real;
						double y2 = ((NumAtom)array[3]).Real;
						XRect rect = XRect.FromSides(x1, y1, x2, y2);
						root.ToolTipText = $"Width {rect.Width} Height {rect.Height}";
					}
				}
				else {
					root.Text += " []";
					for (int i = 0; i < array.Count; i++) {
						TreeNode item = root.Nodes.Add("");
						item.ToolTipText = $"Item {i + 1} of {array.Count}";
						UpdateContent(item, bold, array[i]);
					}
				}
			}
			else {
				root.Text += " " + atom.ToString();
				if (atom is RefAtom)
					root.ToolTipText = $"Jump to {atom}";
			}
		}

		public void UpdateImage(ObjectExtractor extractor, bool reload) {
			Preview preview = extractor.GetPreview(reload);
			if (preview == null)
				pageImage.ImageIndex = 0;
			else
				pageImage.ImageIndex = 1;
		}

		public bool Dirty { get { return _dirty; } set { _dirty = value; } }
		public Find FindDialog { get { return _findDialog; } set { _findDialog = value; } }
		public GraphicsScanner.Fixture Fixture { get { return _fixture; } }

		public void Find(string search, bool wholeWord) {
			if ((_doc == null) || (search == null) || (search == ""))
				return;
			foreach (ListViewItem item in lst.Items) {
				ObjectExtractor extractor = item.Tag as ObjectExtractor;
				string val = extractor != null ? extractor.GetValue(_doc) : "";
				int pos = -1;
				while (true) {
					pos = val.IndexOf(search, pos + 1);
					if (pos < 0)
						break;
					if (!wholeWord)
						break;
					int p1 = pos - 1;
					if ((p1 >= 0) && (!char.IsWhiteSpace(val[p1])) && (!char.IsPunctuation(val[p1])))
						continue; // start of word joined up
					int p2 = pos + search.Length + 1;
					if ((p2 < val.Length) && (!char.IsWhiteSpace(val[p2])) && (!char.IsPunctuation(val[p2])))
						continue; // end of word joined up
					break;
				}
				item.Selected = (pos >= 0);
			}
			lst.Focus();
		}

		public void SelectByObjectID(int id) {
			if (_doc == null)
				return;
			foreach (ListViewItem item in lst.Items) {
				ObjectExtractor extractor = item.Tag as ObjectExtractor;
				item.Selected = (extractor != null) && (extractor.Object != null) && (extractor.Object.ID == id);
			}
		}

		public void OpenSnapshot() {
			if (_tempFiles == null)
				_tempFiles = new List<TemporaryFile>();
			TemporaryFile temp = new TemporaryFile(".pdf");
			// when we save we try to keep the object ids constant
			_doc.SaveOptions.Linearize = false;
			_doc.SaveOptions.Remap = false;
			_doc.Save(temp.Path);
			_tempFiles.Add(temp);
			Process.Start(temp.Path);
		}

		private void Open(string filePath) {
			if (!CloseFile())
				return;
			string oldText = Text;
			Doc doc = null;
			try {
				Text = "PDF Surgeon - "+filePath;
				Update();
				doc = new Doc();
				XReadOptions ro = new XReadOptions();
				ro.Repair = true;
				doc.Read(filePath, ro);
			} catch {
				if(doc!=null)
					doc.Dispose();
				Text = oldText;
				throw;
			}
			Clear();
			_doc = doc;
			_doc.SetInfo(0, "CheckSaveRestore", 0);
			_doc.Rendering.Overprint = overprintToolStripMenuItem.Checked;
			_filePath = filePath;
			_dirty = false;
			lst.BeginUpdate();
			try {
				lst.Items.AddRange(MakeListItems(doc));
			} finally {
				lst.EndUpdate();
			}
		}

		private static ListViewItem[] MakeListItems(Doc doc) {
			ObjectSoup soup = doc.ObjectSoup;
			int count = soup.Count;
			ListViewItem[] items;
			List<ObjectExtractor> extractorList = new List<ObjectExtractor>(count);
			string[] errorList = new string[count];
			ExtractorContext context = new ExtractorContext();
			for (int i = 0; i < count; ++i)
				extractorList.Add(null);
			for (int i = 0; i < count; ++i) {
				IndirectObject obj = soup[i];
				if (obj == null)
					continue;

				try {
					extractorList[i] = ObjectExtractor.FromIndirectObject(obj, context);
				}
				catch (Exception exc) {
					extractorList[i] = new ObjectExtractor(obj, context);
					errorList[i] = exc.Message;
				}
			}
			foreach(int id in context.CMaps) {
				StreamObjectExtractor streamExtractor
					= unchecked((uint)id) >= unchecked((uint)count) ? null :
					extractorList[id] as StreamObjectExtractor;
				if(streamExtractor != null)
					streamExtractor.StreamType = "CMap";
			}
			foreach (KeyValuePair<int, string> pair in context.Xfa) {
				StreamObjectExtractor streamExtractor
					= unchecked((uint)pair.Key) >= unchecked((uint)count) ? null :
					extractorList[pair.Key] as StreamObjectExtractor;
				if (streamExtractor != null) {
					streamExtractor.StreamType = pair.Value == null ? "XML/XFA" :
						string.Format("XML/XFA[{0}]", pair.Value);
				}
			}
			string[] scriptTypeList = { "JavaScript",
				"JavaScript[{0}]",
				"JavaScript[PageID {1}]",
				"JavaScript[{0},PageID {1}]",
				"JavaScript[AnnotationID {2}]",
				"JavaScript[{0},AnnotationID {2}]",
				"JavaScript[PageID {1},AnnotationID {2}]",
				"JavaScript[{0},PageID {1},AnnotationID {2}]"
			};
			foreach (KeyValuePair<int, ScriptType> pair in context.Scripts) {
				StreamObjectExtractor streamExtractor
					= unchecked((uint)pair.Key) >= unchecked((uint)count) ? null :
					extractorList[pair.Key] as StreamObjectExtractor;
				if (streamExtractor != null) {
					int formatIndex = 0;
					if (pair.Value.DocumentName != null)
						formatIndex |= 1;
					if (pair.Value.PageName != null)
						formatIndex |= 2;
					if (pair.Value.AnnotationName != null)
						formatIndex |= 4;
					streamExtractor.StreamType = string.Format(
						scriptTypeList[formatIndex], pair.Value.DocumentName,
						pair.Value.PageName, pair.Value.AnnotationName);
				}
			}
			foreach(KeyValuePair<int, string> pair in context.Type3Glyphs) {
				StreamObjectExtractor streamExtractor
					= unchecked((uint)pair.Key) >= unchecked((uint)count) ? null :
					extractorList[pair.Key] as StreamObjectExtractor;
				if(streamExtractor != null) {
					streamExtractor.StreamType = string.Format(
						"Type 3 Glyph[{0}]", pair.Value);
				}
			}
			foreach (int id in context.PageContents) {
				StreamObjectExtractor streamExtractor
					= unchecked((uint)id) >= unchecked((uint)count) ? null :
					extractorList[id] as StreamObjectExtractor;
				if (streamExtractor != null)
					streamExtractor.ContentPageNumber = context.PageContentIdToPageNumber[id];
			}
			List<ListViewItem> itemList = new List<ListViewItem>(count);
			for (int i = 0; i < count; ++i) {
				ObjectExtractor extractor = extractorList[i];
				if (extractor != null) {
					var props = extractor.GetProperties();
					if (errorList[i] != null)
						props.Insert(Math.Min(props.Count, 2), new Property("Error", errorList[i]));

					List<string> values = new List<string>();
					foreach (var prop in props)
						values.Add(prop.Summary);
					ListViewItem item = new ListViewItem(values.ToArray());
					item.Tag = extractor;
					itemList.Add(item);
				}
			}
			items = itemList.ToArray();
			return items;
		}

		private bool CloseFile() {
			bool ok = true;
			if (_dirty) {
				string text = string.Format("Do you want to save changes to {0} ?",
					Path.GetFileNameWithoutExtension(_filePath));
				switch (MessageBox.Show(text, Application.ProductName, MessageBoxButtons.YesNoCancel)) {
					case DialogResult.Yes :
						_dirty = false;
						Save();
						_doc.Clear();
						break;
					case DialogResult.No :
						break;
					case DialogResult.Cancel :
						ok = false;
						break;
				}
			}
			return ok;
		}

		private void Save() {
			bool swap = File.Exists(_filePath);
			Tuple<string, string> pair = swap ? GetSaveAsName(_filePath) : null;
			string path = swap ? Path.Combine(pair.Item1, pair.Item2): _filePath;
			// when we save we try to keep the object ids constant
			_doc.SaveOptions.Linearize = false;
			_doc.SaveOptions.Remap = false;
			_doc.Save(path);
			_doc.Clear();
			if (swap) {
				try {
					File.Delete(_filePath);
					File.Move(path, _filePath);
				}
				catch (Exception ex) {
					MessageBox.Show($"Saving to {Path.GetFileName(path)}. The reason for this is the following. {ex.Message}");
					_filePath = path;
				}
			}
			_dirty = false;
			Open(_filePath);
			TempFile = null;
		}

		private void SaveAll(string folder) {
			using (TemporaryFile temp = new TemporaryFile(".pdf")) {
				File.Copy(_filePath, temp.Path);
				bool linearized = File.ReadAllText(temp.Path).Contains("/Linearized 1");
				string name = Path.GetFileNameWithoutExtension(_filePath);
				List<long> eofs = null;
				List<int> signedAreas = new List<int>();
				using (Doc doc = new Doc()) {
					doc.Read(temp.Path);
					int n =  doc.ObjectSoup.Revisions;
					if (eofs == null) {
						eofs = new List<long>(doc.ObjectSoup.RevisionEOFs);
						eofs.Reverse();
					}
					if (linearized)
						n--;
					for (int i = 0; i < n; i++) {
						doc.Read(temp.Path);
						XReadOptions ro = new XReadOptions { SkipRevisions = i };
						doc.Read(temp.Path, ro);
						int max = 0;
						foreach (var o in doc.ObjectSoup) {
							if (o == null)
								continue;
							var a = o.Atom; // touch the atom to ensure consistent formatting
							var br = Atom.GetItem(a, "ByteRange") as ArrayAtom;
							if (br != null && br.Count == 4 && Atom.GetItem(a, "Contents") != null)
								max = Math.Max(max, Atom.GetInt(br[2]) + Atom.GetInt(br[3]));
						}
						signedAreas.Add(max);
						doc.SaveOptions.Remap = false;
						doc.SaveOptions.Linearize = false;
						doc.SaveOptions.Incremental = false;
						doc.Save(Path.Combine(folder, $"{name}-rewrite-{n - i}of{n}.pdf"));
					}
				}
				var data = File.ReadAllBytes(temp.Path);
				for (int i = 0; i < eofs.Count; i++) {
					int diff = 0;
					if (signedAreas[i] > 0) {
						diff = signedAreas[i] - (int)eofs[i];
						if (diff < 0 || diff > 10) // something odd
							diff = "%%EOF\r\n".Length;
					}
					var path = Path.Combine(folder, $"{name}-original-{eofs.Count - i}of{eofs.Count}.pdf");
					using (var stream = new FileStream(path, FileMode.Create))
						stream.Write(data, 0, (int)(eofs[i] + diff));
				}
			}
		}

		private string TempFile {
			set {
				try {
					if ((_tempPath != null) && (File.Exists(_tempPath)))
						File.Delete(_tempPath);
				}
				catch {
				}
				_tempPath = value;
			}
			get { return _tempPath; }
		}

		public void SetItemValue(int id, string text) {
			_doc.SetInfo(id, "value", text);
		}

		private ListViewItem GetSelectedItem() {
			ListViewItem item = lst.FocusedItem;
			if(item!=null && item.Selected)
				return item;
			if(lst.SelectedItems.Count>0)
				return lst.SelectedItems[0];

			return null;
		}

		private void SetPreview(Preview preview) {
			if (preview == null) {
				pict.Visible = false;
			}
			else {
				var img = preview.Image;
				var graphics = preview.Graphics;
				if (img != pict.Image)
					pict.Image = img;
				if (graphics != _graphics) {
					_graphics = graphics;
					_fixture = null;
				}
				if (pageImage.ImageIndex != 1)
					pageImage.ImageIndex = 1;
				pict.Visible = true;
			}
		}

		private void SetTextBoxText(string text, RichTextBox textBox) {
			textBox.Clear();
			int i = text.IndexOf('\0');
			if(i<0) {
				textBox.Text = text;
				return;
			}

			textBox.Text = text.Replace('\0', '\ufeff');
			text = textBox.Text;
			textBox.Text = text.Replace('\ufeff', ' ');
			Color backColor = Color.FromKnownColor(KnownColor.Control);
			Color highlight = Color.FromKnownColor(KnownColor.Highlight);
			highlight = Color.FromArgb(highlight.A, (highlight.R+backColor.R)/2,
				(highlight.G+backColor.G)/2, (highlight.B+backColor.B)/2);
			// some character combinations cause mismatches between Select and text positions
			while((i = text.IndexOf('\ufeff', i))>=0) {
				int j = i;
				do {
					++i;
				} while(i<text.Length && text[i]=='\ufeff');
				textBox.Select(j, i-j);
				textBox.SelectionBackColor = highlight;
			}
			textBox.Select(0, 0);
			textBox.SelectionBackColor = new Color();
		}

		private void mitemExit_Click(object sender, EventArgs e) {
			CloseFile();
			Close();
		}

		private void mitemOpen_Click(object sender, EventArgs e) {
			if(dlgOpenFile.ShowDialog()!=DialogResult.OK)
				return;

			try {
				Open(dlgOpenFile.FileName);
			} catch(Exception exc) {
				MessageBox.Show(exc.Message);
			}
		}

		private void cmenuTextBox_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {
		}

		private void cmenuTextBox_Opening(object sender, CancelEventArgs e) {
			IndirectObject extractor = _selectedExtractor != null ? _selectedExtractor.Object : null;
			ContextMenuStrip cmd = (ContextMenuStrip)sender;
			cmd.Items[0].Enabled = extractor is StreamObject;
			cmd.Items[1].Enabled = extractor is StreamObject;
			cmd.Items[2].Enabled = extractor is StreamObject;
			cmd.Items[3].Enabled = extractor is StreamObject;
			cmd.Items[4].Enabled = extractor is PixMap;
			cmd.Items[5].Enabled = extractor is PixMap;
			cmd.Items[6].Enabled = extractor is PixMap;
			cmd.Items[7].Enabled = extractor is Page;
			cmd.Items[8].Enabled = extractor is Page;
			cmd.Items[9].Enabled = extractor is PixMap;
			cmd.Items[10].Enabled = extractor is PixMap;
			cmd.Items[10].Enabled = extractor is StreamObject;
		}

		private void lst_SelectedIndexChanged(object sender, EventArgs e) {
			ListViewItem item = GetSelectedItem();
			ObjectExtractor extractor = item==null? null: item.Tag as ObjectExtractor;
			if(_selectedExtractor==extractor)
				return;

			_selectedExtractor = extractor;
			if(extractor==null) {
				ResetItemView();
				return;
			}

			UpdateContent(extractor);
			UpdateImage(extractor, false);
		}

		private void lst_DoubleClick(object sender, EventArgs e) {
			EditItem();
		}

		private void lst_ColumnClick(object sender, ColumnClickEventArgs e) {
			ListViewItemComparer comparer = lst.ListViewItemSorter as ListViewItemComparer;
			if(comparer!=null){
				if(e.Column==comparer.Column){
					if(comparer.Order==SortOrder.Ascending)
						comparer.Order = SortOrder.Descending;
					else
						comparer.Order = SortOrder.Ascending;
				}else{
					comparer.Column = e.Column;
					comparer.Order = SortOrder.Ascending;
				}
			}
			lst.Sort();
			ListViewItem item = lst.FocusedItem;
			if(item!=null)
				item.EnsureVisible();
		}

		private void lst_DragOver(object sender, DragEventArgs e) {
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Move;
		}

		private void lst_DragDrop(object sender, DragEventArgs e) {
			if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
				string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
				if (files.Length > 0) {
					try {
						Open(files[0]);
					}
					catch (Exception exc) {
						MessageBox.Show(exc.Message);
					}
				}
			}
		}

		private void pict_Click(object sender, EventArgs e) {
			pnlPicture.Focus();
		}

		private void pict_MouseDoubleClick(object sender, MouseEventArgs e) {
			var preview = _selectedExtractor != null ? _selectedExtractor.GetPreview(false) : null;
			if (preview == null)
				return;
			double[] levels = new double[] { 0.02, 0.04, 0.08, 0.16, 0.25, 0.33, 0.5, 0.67, 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0 };
			int pos = 0;
			double bestDelta = double.MaxValue;
			for (int i = 0; i < levels.Length; i++) {
				double delta = Math.Abs(preview.Scale - levels[i]);
				if (delta < bestDelta) {
					bestDelta = delta;
					pos = i;
				}
			}
			bool unzoom = (Control.ModifierKeys & Keys.Alt) == Keys.Alt;
			pos = unzoom ? Math.Max(pos - 1, 0) : Math.Min(pos + 1, levels.Length - 1);
			if (preview.Scale != levels[pos]) {
				preview.Scale = levels[pos];
				preview.Reload();
			}
		}

		private void mitemDecompress_Click(object sender, EventArgs e) {
			ObjectExtractor extractor = _selectedExtractor;
			if (extractor == null) return;
			StreamObject stream = extractor.Object as StreamObject;
			if (stream == null) return;
			stream.Decompress();
			extractor.Invalidate();
			UpdateContent(extractor);
			UpdateListItemInfo(GetSelectedItem());
			Dirty = true;
		}

		private void mItemMakeAscii_Click(object sender, EventArgs e) {
			ObjectExtractor extractor = _selectedExtractor;
			if (extractor == null) return;
			StreamObject stream = extractor.Object as StreamObject;
			if (stream == null) return;
			stream.CompressAscii85();
			extractor.Invalidate();
			UpdateContent(extractor);
			Dirty = true;
		}

		private void mItemMakeAsciiHex_Click(object sender, EventArgs e) {
			ObjectExtractor extractor = _selectedExtractor;
			if (extractor == null) return;
			StreamObject stream = extractor.Object as StreamObject;
			if (stream == null) return;
			stream.CompressAsciiHex();
			extractor.Invalidate();
			UpdateContent(extractor);
			Dirty = true;
		}

		private void mItemCompressFlate_Click(object sender, EventArgs e) {
			ObjectExtractor extractor = _selectedExtractor;
			if (extractor == null) return;
			StreamObject stream = extractor.Object as StreamObject;
			if (stream == null) return;
			stream.CompressFlate();
			extractor.Invalidate();
			UpdateContent(extractor);
			Dirty = true;
		}

		private void mItemCompressJpeg75_Click(object sender, EventArgs e) {
			ObjectExtractor extractor = _selectedExtractor;
			if (extractor == null) return;
			PixMap pixMap = extractor.Object as PixMap;
			if (pixMap == null) return;
			pixMap.Decompress();
			pixMap.CompressJpeg(75);
			extractor.Invalidate();
			UpdateContent(extractor);
			Dirty = true;
		}

		private void mItemCompressJpeg50_Click(object sender, EventArgs e) {
			ObjectExtractor extractor = _selectedExtractor;
			if (extractor == null) return;
			PixMap pixMap = extractor.Object as PixMap;
			if (pixMap == null) return;
			pixMap.Decompress();
			pixMap.CompressJpeg(50);
			extractor.Invalidate();
			UpdateContent(extractor);
			Dirty = true;
		}

		private void mItemCompressJpeg25_Click(object sender, EventArgs e) {
			ObjectExtractor extractor = _selectedExtractor;
			if (extractor == null) return;
			PixMap pixMap = extractor.Object as PixMap;
			if (pixMap == null) return;
			pixMap.Decompress();
			pixMap.CompressJpeg(25);
			extractor.Invalidate();
			UpdateContent(extractor);
			Dirty = true;
		}

		private void flattenToolStripMenuItem_Click(object sender, EventArgs e) {
			ObjectExtractor extractor = _selectedExtractor;
			if (extractor == null) return;
			Page page = extractor.Object as Page;
			if (page == null) return;
			page.Flatten(true);
			extractor.Invalidate();
			UpdateInfo(null);
			UpdateContent(extractor);
			UpdateImage(extractor, true);
			Dirty = true;
		}

		private void mItemFlattenFormXObjects_Click(object sender, EventArgs e) {
			ObjectExtractor extractor = _selectedExtractor;
			if (extractor == null) return;
			Page page = extractor.Object as Page;
			if (page == null) return;
			page.StampFormXObjects(true);
			HashSet<int> ids = new HashSet<int>();
			foreach (var item in page.GetLayers())
				ids.Add(item.ID);
			UpdateInfo(ids);
			UpdateContent(extractor);
			UpdateImage(extractor, true);
			Dirty = true;
		}

		private void realizeToolStripMenuItem_Click(object sender, EventArgs e) {
			ObjectExtractor extractor = _selectedExtractor;
			if (extractor == null) return;
			PixMap pixMap = extractor.Object as PixMap;
			if (pixMap == null) return;
			pixMap.Realize();
			extractor.Invalidate();
			UpdateContent(extractor);
			Dirty = true;
		}

		private void saveImageToolStripMenuItem_Click(object sender, EventArgs e) {
			ObjectExtractor extractor = _selectedExtractor;
			if (extractor == null) return;
			PixMap pixMap = extractor.Object as PixMap;
			if (pixMap == null) return;
			if (dlgSaveImage.ShowDialog() == DialogResult.OK) {
				string filePath = dlgSaveImage.FileName;
				pixMap.Save(filePath);
			}
		}

		private void exportToolStripMenuItem_Click(object sender, EventArgs e) {
			ObjectExtractor extractor = _selectedExtractor;
			if (extractor == null) return;
			StreamObject stm = extractor.Object as StreamObject;
			if (stm == null) return;
			if (dlgExportFile.ShowDialog() == DialogResult.OK) {
				string filePath = dlgExportFile.FileName;
				File.WriteAllBytes(filePath, stm.GetData());
			}
		}

		private void mItemRefresh_Click(object sender, EventArgs e) {
			RefreshSelectedImage();
		}

		public void RefreshSelectedImage() {
			ObjectExtractor extractor = _selectedExtractor;
			if (extractor == null) return;
			extractor.Invalidate();
			UpdateContent(extractor);
			UpdateImage(extractor, true);
		}

		private void mItemEdit_Click(object sender, EventArgs e)
		{
			EditItem();
		}

		private void EditItem()
		{
			ObjectExtractor extractor = _selectedExtractor;
			if ((extractor == null) || (extractor.Object == null)) return;
			EditText theEdit = null;
			_editTexts.TryGetValue(extractor, out theEdit);
			if (theEdit == null)
			{
				theEdit = new EditText(this, extractor);
				_editTexts.Add(extractor, theEdit);
				theEdit.Show();
			}
			else
				theEdit.BringToFront();
		}

		private void mItemExtract_Click(object sender, EventArgs e) {
			ObjectExtractor extractor = _selectedExtractor;
			if (extractor == null) return;
			Page page = extractor.Object as Page;
			if (page == null) return;
			_doc.Page = page.ID;
			_doc.RemapPages(new int[] { _doc.PageNumber });
			UpdateInfo(null);
			UpdateContent(extractor);
			UpdateImage(extractor, true);
			Dirty = true;
		}

		private void mitemSave_Click(object sender, EventArgs e) {
			Save();
		}

		private void mitemSaveAs_Click(object sender, EventArgs e) {
			if (File.Exists(_filePath)) {
				Tuple<string, string> dirAndName = GetSaveAsName(_filePath);
				dlgSaveFile.InitialDirectory = dirAndName.Item1;
				dlgSaveFile.FileName = dirAndName.Item2;
			}
			if (dlgSaveFile.ShowDialog() == DialogResult.OK) {
				_filePath = dlgSaveFile.FileName;
				Save();
			}
		}

		private static Tuple<string, string> GetSaveAsName(string currentFilePath) {
			string directory = Directory.GetParent(currentFilePath).FullName;
			string currentName = Path.GetFileNameWithoutExtension(currentFilePath);
			string newName = "";
			int p = currentName.LastIndexOf(' ');
			if (p >= 0) {
				bool alreadyNumbered = true;
				for (int i = p + 1; i < currentName.Length; i++) {
					if (char.IsNumber(currentName[i]) == false) {
						alreadyNumbered = false;
						break;
					}
				}
				if (alreadyNumbered)
					currentName = currentName.Substring(0, p);
			}
			for (int i = 1; i < 1000; i++) {
				newName = currentName + " " + i.ToString() + Path.GetExtension(currentFilePath);
				if (!File.Exists(Path.Combine(directory, newName)))
					break;
			}
			return new Tuple<string, string>(directory, newName);
		}

		private void mitemSaveAll_Click(object sender, EventArgs e) {
			if (File.Exists(_filePath)) {
				dlgSaveFolder.SelectedPath = Directory.GetParent(_filePath).FullName;
				if (dlgSaveFolder.ShowDialog() == DialogResult.OK) {
					SaveAll(dlgSaveFolder.SelectedPath);
				}
			}
		}

		private void selectAllToolStripMenuItem_Click(object sender, EventArgs e) {
			Control control = ActiveControl;
			while(control!=null) {
				ContainerControl container = control as ContainerControl;
				if(container==null) {
					TextBoxBase txt = control as TextBoxBase;
					if(txt!=null) {
						txt.SelectAll();
						return;
					}
					break;
				}
				control = container.ActiveControl;
			}

			foreach (ListViewItem item in lst.Items)
				item.Selected = true;
		}

		private void findToolStripMenuItem_Click(object sender, EventArgs e) {
			if (_findDialog == null)
				_findDialog = new Find(this);
			_findDialog.Show();
			_findDialog.BringToFront();
		}

		private void compactToolStripMenuItem_Click(object sender, EventArgs e) {
			// remove any redundant objects
			string path = Path.GetTempPath() + Guid.NewGuid().ToString() + ".pdf";
			_doc.SaveOptions.Linearize = false;
			_doc.SaveOptions.Remap = true;
			_doc.Save(path);
			_doc.Clear();
			string oldText = Text;
			string oldPath = _filePath;
			_dirty = false;
			Open(path);
			_dirty = true;
			_filePath = oldPath;
			Text = oldText;
			TempFile = path;
		}

		private void allToolStripMenuItem_Click(object sender, EventArgs e) {
			if (_doc == null)
				return;
			lst.BeginUpdate();
			try {
				lst.Items.Clear();
				lst.Items.AddRange(MakeListItems(_doc));
			}
			finally {
				lst.EndUpdate();
			}
		}

		private void selectedToolStripMenuItem_Click(object sender, EventArgs e) {
			ShowSelected(true);
		}

		private void unselectedToolStripMenuItem_Click(object sender, EventArgs e) {
			ShowSelected(false);
		}

		private void ShowSelected(bool truth) {
			List<ListViewItem> itemList = new List<ListViewItem>();
			foreach (ListViewItem item in lst.Items) {
				if (item.Selected == truth)
					itemList.Add(item);
			}

			lst.BeginUpdate();
			try {
				lst.Items.Clear();
				lst.Items.AddRange(itemList.ToArray());
			}
			finally {
				lst.EndUpdate();
			}
		}

		private void overprintToolStripMenuItem_Click(object sender, EventArgs e) {
			overprintToolStripMenuItem.Checked = !overprintToolStripMenuItem.Checked;
			if (_doc != null) {
				_doc.Rendering.Overprint = overprintToolStripMenuItem.Checked;
				RefreshSelectedImage();
			}
		}

		private void openInAcrobatToolStripMenuItem_Click(object sender, EventArgs e) {
			if (_doc != null)
				OpenSnapshot();
		}

		private void MainForm_Load(object sender, EventArgs e) {
			Size size = Properties.Settings.Default.WindowSize;
			if (size.Width > 30 && size.Height > 30)
				Size = size;
			int distance = Properties.Settings.Default.SplitterDistance;
			if (distance > 10)
				split.SplitterDistance = distance;
			Preview.InitialImage = pict.InitialImage;
			Preview.ErrorImage = pict.ErrorImage;
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
			Properties.Settings.Default.WindowSize = Size;
			Properties.Settings.Default.SplitterDistance = split.SplitterDistance;
			Properties.Settings.Default.Save();
			if (CloseFile()) {
				if (_doc != null) {
					_doc.Dispose();
					_doc = null;
				}
				TempFile = null;
			}
			else
				e.Cancel = true;
		}

		private void atomTree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e) {
			RefAtom rf = e.Node.Tag as RefAtom;
			if (rf == null) return;
			SelectByObjectID(rf.ID);
			if (lst.SelectedItems.Count > 0)
				lst.EnsureVisible(lst.SelectedIndices[0]);
		}

		private void toolStripBack_Click(object sender, EventArgs e) {
			if (_nav.CanNavigate())
				SelectByObjectID(_nav.Back());
		}

		private void toolStripForward_Click(object sender, EventArgs e) {
			if (_nav.CanNavigate())
				SelectByObjectID(_nav.Forward());
		}

		private void pict_MouseMove(object sender, MouseEventArgs e) {
			if (ModifierKeys.HasFlag(Keys.Shift))
				return; // allow focus freeze if shift pressed
			bool refresh = false;
			if (_graphics != null && _graphics.All.Count > 0) {
				Point loc = e.Location;
				var offset = GetImageOffset();
				loc.Offset(-offset.X, -offset.Y);
				GraphicsScanner.Fixture best = null;
				foreach (var part in _graphics.All) {
					if (part.Region.IsVisible(loc)) {
						if (best == null || part.Area < best.Area)
							best = part;
					}
				}
				if (_fixture != best) {
					refresh = true;
					_fixture = best;
					if (_fixture == null) {
						toolTip1.Hide(this);
						toolTip1.Tag = null;
					}
					else if (toolTip1.Tag != _fixture) {
						string txt = $"ID {_fixture.OwnerID} {_fixture.Text}";
						if (!string.IsNullOrEmpty(txt)) {
							Point pp = pict.PointToScreen(new Point());
							Point fp = this.PointToScreen(new Point());
							Point delta = new Point(pp.X - fp.X, pp.Y - fp.Y);
							Point imgloc = GetImageOffset();
							toolTip1.Tag = _fixture;
							if (txt.Length > 500)
								txt = txt.Substring(0, 500) + "...";
							toolTip1.Show(txt, this, loc.X + delta.X + imgloc.X + 32, loc.Y + delta.Y + imgloc.Y + 64);
						}
					}
				}
			}
			if (refresh)
				pict.Invalidate();
		}

		private void pict_MouseLeave(object sender, EventArgs e) {
			_fixture = null;
			toolTip1.Hide(this);
			toolTip1.Tag = null;
			pict.Invalidate();
		}

		private void tabInfo_SelectedIndexChanged(object sender, EventArgs e) {
			_fixture = null;
			toolTip1.Hide(this);
			toolTip1.Tag = null;
			pict.Invalidate();
		}

		private void pict_Paint(object sender, PaintEventArgs e) {
			Graphics g = e.Graphics;
			if (_graphics != null && _graphics.All.Count > 0) {
				var offset = GetImageOffset();
				var pen = new Pen(Color.Blue, 1);
				if ((Control.ModifierKeys & Keys.Control) == Keys.Control && (Control.ModifierKeys & Keys.Alt) == Keys.Alt) {
					var saved = g.Save();
					g.TranslateTransform(offset.X, offset.Y);
					foreach (var fixture in _graphics.All)
						g.DrawPath(pen, fixture.Path);
					g.Restore(saved);
				}
				else if (_fixture != null) {
					var saved = g.Save();
					g.TranslateTransform(offset.X, offset.Y);
					g.DrawPath(pen, _fixture.Path);
					g.Restore(saved);
				}
			}
		}

		private Point GetImageOffset() {
			if (pict.SizeMode != PictureBoxSizeMode.CenterImage)
				return new Point();
			var image = pict.Image;
			Debug.Assert(image != null);
			if (image == null)
				return new Point();
			int dx = (pict.Width - image.Width) / 2;
			int dy = (pict.Height - image.Height) / 2;
			return new Point(dx, dy);
		}

		private void timer1_Tick(object sender, EventArgs e) {
			// We update from a timer because we are loading images on a separate
			// thread and we can't call directly into the UI from that thread.
			ListViewItem item = GetSelectedItem();
			ObjectExtractor obj = item != null ? (ObjectExtractor)item.Tag : null;
			Preview preview = obj != null ? obj.GetPreview(false) : null;
			SetPreview(preview);
		}
	}
}
