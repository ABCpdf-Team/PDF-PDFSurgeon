namespace WebSupergoo.PDFSurgeon {
	partial class EditText {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
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
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.ok = new System.Windows.Forms.Button();
			this.cancel = new System.Windows.Forms.Button();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.formatContent = new System.Windows.Forms.Button();
			this.liveUpdate = new System.Windows.Forms.CheckBox();
			this.trackBar2 = new System.Windows.Forms.TrackBar();
			this.trackBar1 = new System.Windows.Forms.TrackBar();
			this.label1 = new System.Windows.Forms.Label();
			this.openPdf = new System.Windows.Forms.Button();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this.trackBar2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
			this.SuspendLayout();
			// 
			// textBox1
			// 
			this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox1.HideSelection = false;
			this.textBox1.Location = new System.Drawing.Point(11, 29);
			this.textBox1.MaxLength = 100000000;
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBox1.Size = new System.Drawing.Size(561, 278);
			this.textBox1.TabIndex = 0;
			this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
			// 
			// ok
			// 
			this.ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.ok.Location = new System.Drawing.Point(498, 429);
			this.ok.Name = "ok";
			this.ok.Size = new System.Drawing.Size(74, 27);
			this.ok.TabIndex = 1;
			this.ok.Text = "OK";
			this.ok.UseVisualStyleBackColor = true;
			this.ok.Click += new System.EventHandler(this.ok_Click);
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(418, 429);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(74, 27);
			this.cancel.TabIndex = 2;
			this.cancel.Text = "Cancel";
			this.cancel.UseVisualStyleBackColor = true;
			this.cancel.Click += new System.EventHandler(this.cancel_Click);
			// 
			// menuStrip1
			// 
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 1, 0, 1);
			this.menuStrip1.Size = new System.Drawing.Size(584, 24);
			this.menuStrip1.TabIndex = 3;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// formatContent
			// 
			this.formatContent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.formatContent.Location = new System.Drawing.Point(333, 365);
			this.formatContent.Margin = new System.Windows.Forms.Padding(2);
			this.formatContent.Name = "formatContent";
			this.formatContent.Size = new System.Drawing.Size(115, 19);
			this.formatContent.TabIndex = 12;
			this.formatContent.Text = "Format Content";
			this.formatContent.UseVisualStyleBackColor = true;
			this.formatContent.Click += new System.EventHandler(this.formatContent_Click);
			// 
			// liveUpdate
			// 
			this.liveUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.liveUpdate.AutoSize = true;
			this.liveUpdate.Location = new System.Drawing.Point(331, 343);
			this.liveUpdate.Margin = new System.Windows.Forms.Padding(2);
			this.liveUpdate.Name = "liveUpdate";
			this.liveUpdate.Size = new System.Drawing.Size(231, 17);
			this.liveUpdate.TabIndex = 11;
			this.liveUpdate.Text = "Update Page Image as Content is Changed";
			this.liveUpdate.UseVisualStyleBackColor = true;
			this.liveUpdate.CheckedChanged += new System.EventHandler(this.liveUpdate_CheckedChanged);
			// 
			// trackBar2
			// 
			this.trackBar2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.trackBar2.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.trackBar2.Enabled = false;
			this.trackBar2.Location = new System.Drawing.Point(11, 365);
			this.trackBar2.Margin = new System.Windows.Forms.Padding(2);
			this.trackBar2.Maximum = 100;
			this.trackBar2.Name = "trackBar2";
			this.trackBar2.Size = new System.Drawing.Size(240, 45);
			this.trackBar2.TabIndex = 9;
			this.trackBar2.TickStyle = System.Windows.Forms.TickStyle.None;
			this.trackBar2.Value = 100;
			this.trackBar2.Scroll += new System.EventHandler(this.trackBar2_Scroll);
			// 
			// trackBar1
			// 
			this.trackBar1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.trackBar1.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.trackBar1.Enabled = false;
			this.trackBar1.Location = new System.Drawing.Point(11, 344);
			this.trackBar1.Margin = new System.Windows.Forms.Padding(2);
			this.trackBar1.Maximum = 100;
			this.trackBar1.Name = "trackBar1";
			this.trackBar1.Size = new System.Drawing.Size(240, 45);
			this.trackBar1.TabIndex = 8;
			this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.None;
			this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(8, 322);
			this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(344, 13);
			this.label1.TabIndex = 13;
			this.label1.Text = "Use the sliders to strip content off the start or end of the content stream.";
			// 
			// openPdf
			// 
			this.openPdf.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.openPdf.Location = new System.Drawing.Point(452, 365);
			this.openPdf.Margin = new System.Windows.Forms.Padding(2);
			this.openPdf.Name = "openPdf";
			this.openPdf.Size = new System.Drawing.Size(115, 19);
			this.openPdf.TabIndex = 14;
			this.openPdf.Text = "Open PDF";
			this.openPdf.UseVisualStyleBackColor = true;
			this.openPdf.Click += new System.EventHandler(this.openPdf_Click);
			// 
			// timer1
			// 
			this.timer1.Enabled = true;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// textBox2
			// 
			this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox2.Location = new System.Drawing.Point(55, 3);
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new System.Drawing.Size(393, 20);
			this.textBox2.TabIndex = 15;
			this.textBox2.TextChanged += new System.EventHandler(this.TextBox2_TextChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(8, 6);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(41, 13);
			this.label2.TabIndex = 16;
			this.label2.Text = "Search";
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(478, 6);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(0, 13);
			this.label3.TabIndex = 17;
			// 
			// numericUpDown1
			// 
			this.numericUpDown1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.numericUpDown1.InterceptArrowKeys = false;
			this.numericUpDown1.Location = new System.Drawing.Point(555, 3);
			this.numericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericUpDown1.Name = "numericUpDown1";
			this.numericUpDown1.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.numericUpDown1.Size = new System.Drawing.Size(17, 20);
			this.numericUpDown1.TabIndex = 19;
			this.numericUpDown1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericUpDown1.ValueChanged += new System.EventHandler(this.NumericUpDown1_ValueChanged);
			// 
			// EditText
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(584, 468);
			this.Controls.Add(this.numericUpDown1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textBox2);
			this.Controls.Add(this.openPdf);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.formatContent);
			this.Controls.Add(this.trackBar2);
			this.Controls.Add(this.liveUpdate);
			this.Controls.Add(this.trackBar1);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.ok);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "EditText";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "EditText";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EditText_FormClosing);
			this.Load += new System.EventHandler(this.EditText_Load);
			((System.ComponentModel.ISupportInitialize)(this.trackBar2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Button ok;
		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.Button formatContent;
		private System.Windows.Forms.CheckBox liveUpdate;
		private System.Windows.Forms.TrackBar trackBar2;
		private System.Windows.Forms.TrackBar trackBar1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button openPdf;
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.NumericUpDown numericUpDown1;
	}
}