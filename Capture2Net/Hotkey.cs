using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Capture2Net
{
	class Hotkey : Form
	{
		[DllImport("user32.dll")]
		private static extern bool RegisterHotKey(IntPtr hWnd, int id, KeyModifiers fsModifiers, Keys vk);
		[DllImport("user32.dll")]
		private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

		public enum KeyModifiers
		{
			None = 0,
			Alt = 1,
			Control = 2,
			Shift = 4,
			Windows = 8
		}

		private const int WM_HOTKEY = 0x0312;
		private int id;

		private event EventHandler HotKeyPressed;

		public Hotkey(int id, Keys key, KeyModifiers modifier, EventHandler hotKeyPressed)
		{
			this.id = id;
			HotKeyPressed = hotKeyPressed;
			RegisterHotKey(key, modifier);
		}

		~Hotkey()
		{
			UnregisterHotKey(this.Handle, this.id);
		}

		public void UnregisterShortcut()
		{
			UnregisterHotKey(this.Handle, this.id);
		}

		private void RegisterHotKey(Keys key, KeyModifiers modifier)
		{
			if (key == Keys.None)
			{
				return;
			}

			if (!RegisterHotKey(this.Handle, this.id, modifier, key))
			{
				throw new ApplicationException("Hotkey already in use");
			}
		}

		protected override void WndProc(ref Message message)
		{
			switch (message.Msg)
			{
				case WM_HOTKEY:
					HotKeyPressed(this, new EventArgs());
					break;
			}
			base.WndProc(ref message);
		}
	}
}