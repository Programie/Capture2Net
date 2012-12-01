namespace Capture2Net
{
	partial class ShortcutInfo
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShortcutInfo));
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.ShortcutScreenField = new System.Windows.Forms.Label();
			this.ShortcutSelectionField = new System.Windows.Forms.Label();
			this.ShortcutWindowField = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Padding = new System.Windows.Forms.Padding(5);
			this.label1.Size = new System.Drawing.Size(54, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "Screen:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 32);
			this.label2.Name = "label2";
			this.label2.Padding = new System.Windows.Forms.Padding(5);
			this.label2.Size = new System.Drawing.Size(64, 23);
			this.label2.TabIndex = 1;
			this.label2.Text = "Selection:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 55);
			this.label3.Name = "label3";
			this.label3.Padding = new System.Windows.Forms.Padding(5);
			this.label3.Size = new System.Drawing.Size(59, 23);
			this.label3.TabIndex = 2;
			this.label3.Text = "Window:";
			// 
			// ShortcutScreenField
			// 
			this.ShortcutScreenField.AutoSize = true;
			this.ShortcutScreenField.Location = new System.Drawing.Point(82, 9);
			this.ShortcutScreenField.Name = "ShortcutScreenField";
			this.ShortcutScreenField.Padding = new System.Windows.Forms.Padding(5);
			this.ShortcutScreenField.Size = new System.Drawing.Size(10, 23);
			this.ShortcutScreenField.TabIndex = 3;
			// 
			// ShortcutSelectionField
			// 
			this.ShortcutSelectionField.AutoSize = true;
			this.ShortcutSelectionField.Location = new System.Drawing.Point(82, 32);
			this.ShortcutSelectionField.Name = "ShortcutSelectionField";
			this.ShortcutSelectionField.Padding = new System.Windows.Forms.Padding(5);
			this.ShortcutSelectionField.Size = new System.Drawing.Size(10, 23);
			this.ShortcutSelectionField.TabIndex = 4;
			// 
			// ShortcutWindowField
			// 
			this.ShortcutWindowField.AutoSize = true;
			this.ShortcutWindowField.Location = new System.Drawing.Point(82, 55);
			this.ShortcutWindowField.Name = "ShortcutWindowField";
			this.ShortcutWindowField.Padding = new System.Windows.Forms.Padding(5);
			this.ShortcutWindowField.Size = new System.Drawing.Size(10, 23);
			this.ShortcutWindowField.TabIndex = 5;
			// 
			// ShortcutInfo
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.ClientSize = new System.Drawing.Size(234, 86);
			this.Controls.Add(this.ShortcutWindowField);
			this.Controls.Add(this.ShortcutSelectionField);
			this.Controls.Add(this.ShortcutScreenField);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ShortcutInfo";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Shortcut Information";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label ShortcutScreenField;
		private System.Windows.Forms.Label ShortcutSelectionField;
		private System.Windows.Forms.Label ShortcutWindowField;
	}
}