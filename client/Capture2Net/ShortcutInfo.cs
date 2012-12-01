using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Capture2Net
{
	public partial class ShortcutInfo : Form
	{
		public string ShortcutScreen
		{
			set
			{
				this.ShortcutScreenField.Text = value;
			}
		}

		public string ShortcutSelection
		{
			set
			{
				this.ShortcutSelectionField.Text = value;
			}
		}

		public string ShortcutWindow
		{
			set
			{
				this.ShortcutWindowField.Text = value;
			}
		}
		
		public ShortcutInfo()
		{
			InitializeComponent();
		}
	}
}