using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Capture2Net
{
	public partial class SelectionForm : Form
	{
		MouseDownType mouseDownType;
		Point start;
		public Rectangle cropArea;
		Rectangle oldCropArea;
		Image sourceImage;

		public bool accepted;

		Pen pen;
		SolidBrush dimmingBrush;
		StringFormat sizeInfoStringFormat;

		enum MouseDownType
		{
			None,
			Draw,
			Move
		}
		
		public SelectionForm(Image image)
		{
			this.sourceImage = image;

			// TODO: Allow user to change colors and pen width
			this.dimmingBrush = new SolidBrush(Color.FromArgb(50, 0, 0, 0));
			this.pen = new Pen(Brushes.White, 1);

			this.sizeInfoStringFormat = new StringFormat();
			this.sizeInfoStringFormat.Alignment = StringAlignment.Center;
			this.sizeInfoStringFormat.LineAlignment = StringAlignment.Center;
			
			InitializeComponent();
		}

		private void SelectionForm_Load(object sender, EventArgs e)
		{
			this.Left = NativeMethods.GetSystemMetrics(NativeMethods.SM_XVIRTUALSCREEN);
			this.Top = NativeMethods.GetSystemMetrics(NativeMethods.SM_YVIRTUALSCREEN);
			this.Width = NativeMethods.GetSystemMetrics(NativeMethods.SM_CXVIRTUALSCREEN);
			this.Height = NativeMethods.GetSystemMetrics(NativeMethods.SM_CYVIRTUALSCREEN);

			this.Invalidate();
		}

		/// <summary>
		/// Helper class containing API functions
		/// </summary>
		private class NativeMethods
		{
			public const int SM_XVIRTUALSCREEN = 76;
			public const int SM_YVIRTUALSCREEN = 77;
			public const int SM_CXVIRTUALSCREEN = 78;
			public const int SM_CYVIRTUALSCREEN = 79;

			[DllImport("user32.dll")]
			public static extern int GetSystemMetrics(int nIndex);
		}

		private void SelectionForm_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				if (this.cropArea.Contains(e.X, e.Y))
				{
					this.mouseDownType = MouseDownType.Move;
				}
				else
				{
					this.mouseDownType = MouseDownType.Draw;
				}
				
				this.start.X = e.X;
				this.start.Y = e.Y;
			}
		}

		private void SelectionForm_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				this.mouseDownType = MouseDownType.None;
				this.oldCropArea = this.cropArea;
				this.Invalidate();
			}
		}

		private void SelectionForm_MouseMove(object sender, MouseEventArgs e)
		{
			if (this.mouseDownType == MouseDownType.Draw)
			{
				int x;
				int y;
				int width = Math.Abs(this.start.X - e.X);
				int height = Math.Abs(this.start.Y - e.Y);
				if (this.start.X < e.X)
				{
					x = this.start.X;
				}
				else
				{
					x = e.X;
				}
				if (this.start.Y < e.Y)
				{
					y = this.start.Y;
				}
				else
				{
					y = e.Y;
				}
				this.cropArea = new Rectangle(x, y, width, height);
				this.Invalidate();
			}
			else if (this.mouseDownType == MouseDownType.Move)
			{
				this.cropArea.Location = new Point(e.X - (this.start.X - this.oldCropArea.Left), e.Y - (this.start.Y - this.oldCropArea.Top));
				this.Invalidate();
			}
			else if (this.mouseDownType == MouseDownType.None)
			{
				if (this.cropArea.Contains(e.X, e.Y))
				{
					this.Cursor = Cursors.SizeAll;
				}
				else
				{
					this.Cursor = Cursors.Cross;
				}
			}
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (keyData == Keys.Escape)
			{
				this.accepted = false;
				this.Close();
				return true;
			}
			
			if (keyData == Keys.Enter)
			{
				if (this.cropArea.Width == 0 || this.cropArea.Height == 0)
				{
					if (MessageBox.Show("Can not capture a screenshot with a width or height of zero pixels!", "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) != System.Windows.Forms.DialogResult.Retry)
					{
						this.accepted = false;
						this.Close();
						return true;
					}
				}
				else
				{
					this.accepted = true;
					this.Close();
					return true;
				}
			}
			
			return base.ProcessCmdKey(ref msg, keyData);
		}

		private void SelectionForm_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.DrawImage(this.sourceImage, 0, 0);
			Region outsideRegion = new Region(e.ClipRectangle);
			outsideRegion.Exclude(this.cropArea);
			e.Graphics.FillRegion(this.dimmingBrush, outsideRegion);
			e.Graphics.DrawRectangle(this.pen, this.cropArea);
			e.Graphics.DrawString(this.cropArea.Width + " x " + this.cropArea.Height, new System.Drawing.Font(FontFamily.GenericSansSerif, 10), new SolidBrush(Color.Blue), this.cropArea, this.sizeInfoStringFormat);
		}
	}
}