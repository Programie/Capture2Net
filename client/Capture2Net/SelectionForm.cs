using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Capture2Net
{
	public partial class SelectionForm : Form
	{
		bool mouseDown;
		int startX;
		int startY;
		public Rectangle cropArea;
		Image sourceImage;

		public bool accepted;
		
		public SelectionForm(Image image)
		{
			this.sourceImage = image;
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
				this.startX = e.X;
				this.startY = e.Y;
				this.mouseDown = true;
			}
		}

		private void SelectionForm_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				this.mouseDown = false;
				this.Invalidate();
			}
		}

		private void SelectionForm_MouseMove(object sender, MouseEventArgs e)
		{
			if (this.mouseDown)
			{
				int x;
				int y;
				int width = Math.Abs(this.startX - e.X);
				int height = Math.Abs(this.startY - e.Y);
				if (this.startX < e.X)
				{
					x = this.startX;
				}
				else
				{
					x = e.X;
				}
				if (this.startY < e.Y)
				{
					y = this.startY;
				}
				else
				{
					y = e.Y;
				}
				this.cropArea = new Rectangle(x, y, width, height);
				this.Invalidate();
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
			var pen = new Pen(Color.Black, 1);
			pen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDotDot;
			e.Graphics.DrawRectangle(pen, this.cropArea);
			e.Graphics.DrawString(this.cropArea.Width + " x " + this.cropArea.Height, new System.Drawing.Font(FontFamily.GenericSerif, 10), new SolidBrush(Color.Blue), 10, 50);
		}
	}
}