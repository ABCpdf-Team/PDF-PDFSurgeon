namespace WebSupergoo.PDFSurgeon {
	partial class Find {
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
			this.findNext = new System.Windows.Forms.Button();
			this.Cancel = new System.Windows.Forms.Button();
			this.findText = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.wholeWord = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// findNext
			// 
			this.findNext.Location = new System.Drawing.Point(451, 9);
			this.findNext.Name = "findNext";
			this.findNext.Size = new System.Drawing.Size(74, 27);
			this.findNext.TabIndex = 1;
			this.findNext.Text = "&Find All";
			this.findNext.UseVisualStyleBackColor = true;
			this.findNext.Click += new System.EventHandler(this.findNext_Click);
			// 
			// Cancel
			// 
			this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.Cancel.Location = new System.Drawing.Point(451, 42);
			this.Cancel.Name = "Cancel";
			this.Cancel.Size = new System.Drawing.Size(74, 27);
			this.Cancel.TabIndex = 2;
			this.Cancel.Text = "Cancel";
			this.Cancel.UseVisualStyleBackColor = true;
			this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
			// 
			// findText
			// 
			this.findText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.findText.Location = new System.Drawing.Point(74, 9);
			this.findText.Name = "findText";
			this.findText.Size = new System.Drawing.Size(357, 20);
			this.findText.TabIndex = 0;
			this.findText.TextChanged += new System.EventHandler(this.findText_TextChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(56, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "Find what:";
			// 
			// wholeWord
			// 
			this.wholeWord.AutoSize = true;
			this.wholeWord.Location = new System.Drawing.Point(74, 42);
			this.wholeWord.Name = "wholeWord";
			this.wholeWord.Size = new System.Drawing.Size(135, 17);
			this.wholeWord.TabIndex = 4;
			this.wholeWord.Text = "Match whole word only";
			this.wholeWord.UseVisualStyleBackColor = true;
			// 
			// Find
			// 
			this.AcceptButton = this.findNext;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.Cancel;
			this.ClientSize = new System.Drawing.Size(534, 76);
			this.Controls.Add(this.wholeWord);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.findText);
			this.Controls.Add(this.Cancel);
			this.Controls.Add(this.findNext);
			this.Name = "Find";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Find";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Find_FormClosing);
			this.Load += new System.EventHandler(this.Find_Load);
			this.Shown += new System.EventHandler(this.Find_Shown);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button findNext;
		private System.Windows.Forms.Button Cancel;
		private System.Windows.Forms.TextBox findText;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox wholeWord;
	}
}