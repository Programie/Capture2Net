namespace Capture2Net
{
	partial class PendingUploads
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
			this.List = new System.Windows.Forms.ListView();
			this.listColumn_Date = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.listColumn_Status = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.SuspendLayout();
			// 
			// List
			// 
			this.List.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.listColumn_Date,
            this.listColumn_Status});
			this.List.Dock = System.Windows.Forms.DockStyle.Fill;
			this.List.Location = new System.Drawing.Point(0, 0);
			this.List.Name = "List";
			this.List.Size = new System.Drawing.Size(584, 262);
			this.List.TabIndex = 0;
			this.List.UseCompatibleStateImageBehavior = false;
			this.List.View = System.Windows.Forms.View.Details;
			// 
			// listColumn_Date
			// 
			this.listColumn_Date.Text = "Date";
			this.listColumn_Date.Width = 200;
			// 
			// listColumn_Status
			// 
			this.listColumn_Status.Text = "Status";
			this.listColumn_Status.Width = 200;
			// 
			// PendingUploads
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(584, 262);
			this.Controls.Add(this.List);
			this.Name = "PendingUploads";
			this.Text = "Pending Uploads";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListView List;
		private System.Windows.Forms.ColumnHeader listColumn_Date;
		private System.Windows.Forms.ColumnHeader listColumn_Status;
	}
}