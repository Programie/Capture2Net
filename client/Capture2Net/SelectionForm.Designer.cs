﻿namespace Capture2Net
{
	partial class SelectionForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.SuspendLayout();
            // 
            // SelectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 300);
            this.Cursor = System.Windows.Forms.Cursors.Cross;
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectionForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Capture2Net Rectangle Selection";
            this.Load += new System.EventHandler(this.SelectionForm_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.SelectionForm_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SelectionForm_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SelectionForm_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.SelectionForm_MouseUp);
            this.ResumeLayout(false);

		}

		#endregion

	}
}