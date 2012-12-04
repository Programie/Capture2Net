using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Capture2Net
{
	public class Shortcuts : IDisposable
	{
		CloudConfig cloudConfigInstance;
		Screenshot screenshotInstance;
		
		Hotkey shortcutScreen;
		Hotkey shortcutSelection;
		Hotkey shortcutWindow;
		
		List<string> registeredShortcutScreen;
		List<string> registeredShortcutSelection;
		List<string> registeredShortcutWindow;
		
		public List<string> RegisteredShortcutScreen
		{
			get
			{
				return this.registeredShortcutScreen;
			}
		}

		public List<string> RegisteredShortcutSelection
		{
			get
			{
				return this.registeredShortcutSelection;
			}
		}

		public List<string> RegisteredShortcutWindow
		{
			get
			{
				return this.registeredShortcutWindow;
			}
		}

		public Shortcuts(CloudConfig cloudConfigInstance)
		{
			this.cloudConfigInstance = cloudConfigInstance;
		}

		~Shortcuts()
		{
			this.Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this.screenshotInstance != null)
			{
				this.screenshotInstance.Dispose();
			}
			this.Unregister();
		}

		public void Register()
		{
			this.Unregister();
			
			this.registeredShortcutScreen = new List<string>();
			this.registeredShortcutSelection = new List<string>();
			this.registeredShortcutWindow = new List<string>();

			try
			{
				var jsonObject = this.cloudConfigInstance.JsonData["screenshots"]["screen"]["shortcut"];
				var key = (Keys)System.Convert.ToInt32(jsonObject["key"].ToString());
				int keyModifiers = (int)Hotkey.KeyModifiers.None;
				if ((bool)jsonObject["control"])
				{
					keyModifiers += (int)Hotkey.KeyModifiers.Control;
					this.registeredShortcutScreen.Add("Ctrl");
				}
				if ((bool)jsonObject["alt"])
				{
					keyModifiers += (int)Hotkey.KeyModifiers.Alt;
					this.registeredShortcutScreen.Add("Alt");
				}
				if ((bool)jsonObject["shift"])
				{
					keyModifiers += (int)Hotkey.KeyModifiers.Shift;
					this.registeredShortcutScreen.Add("Shift");
				}
				if ((bool)jsonObject["windows"])
				{
					keyModifiers += (int)Hotkey.KeyModifiers.Windows;
					this.registeredShortcutScreen.Add("Windows");
				}
				this.registeredShortcutScreen.Add(((Keys)key).ToString());
				this.shortcutScreen = new Hotkey(100, key, (Hotkey.KeyModifiers)keyModifiers, new System.EventHandler(this.ShortcutScreen));
			}
			catch (ApplicationException)
			{
				MessageBox.Show("Unable to register hotkey for 'Screen'!");
			}
			try
			{
				var jsonObject = this.cloudConfigInstance.JsonData["screenshots"]["selection"]["shortcut"];
				var key = (Keys)System.Convert.ToInt32(jsonObject["key"].ToString());
				int keyModifiers = (int)Hotkey.KeyModifiers.None;
				if ((bool)jsonObject["control"])
				{
					keyModifiers += (int)Hotkey.KeyModifiers.Control;
					this.registeredShortcutSelection.Add("Ctrl");
				}
				if ((bool)jsonObject["alt"])
				{
					keyModifiers += (int)Hotkey.KeyModifiers.Alt;
					this.registeredShortcutSelection.Add("Alt");
				}
				if ((bool)jsonObject["shift"])
				{
					keyModifiers += (int)Hotkey.KeyModifiers.Shift;
					this.registeredShortcutSelection.Add("Shift");
				}
				if ((bool)jsonObject["windows"])
				{
					keyModifiers += (int)Hotkey.KeyModifiers.Windows;
					this.registeredShortcutSelection.Add("Windows");
				}
				this.registeredShortcutSelection.Add(((Keys)key).ToString());
				this.shortcutSelection = new Hotkey(101, key, (Hotkey.KeyModifiers)keyModifiers, new System.EventHandler(this.ShortcutSelection));
			}
			catch (ApplicationException)
			{
				MessageBox.Show("Unable to register hotkey for 'Selection'!");
			}
			try
			{
				var jsonObject = this.cloudConfigInstance.JsonData["screenshots"]["window"]["shortcut"];
				var key = (Keys)System.Convert.ToInt32(jsonObject["key"].ToString());
				int keyModifiers = (int)Hotkey.KeyModifiers.None;
				if ((bool)jsonObject["control"])
				{
					keyModifiers += (int)Hotkey.KeyModifiers.Control;
					this.registeredShortcutWindow.Add("Ctrl");
				}
				if ((bool)jsonObject["alt"])
				{
					keyModifiers += (int)Hotkey.KeyModifiers.Alt;
					this.registeredShortcutWindow.Add("Alt");
				}
				if ((bool)jsonObject["shift"])
				{
					keyModifiers += (int)Hotkey.KeyModifiers.Shift;
					this.registeredShortcutWindow.Add("Shift");
				}
				if ((bool)jsonObject["windows"])
				{
					keyModifiers += (int)Hotkey.KeyModifiers.Windows;
					this.registeredShortcutWindow.Add("Windows");
				}
				this.registeredShortcutWindow.Add(((Keys)key).ToString());
				this.shortcutWindow = new Hotkey(102, key, (Hotkey.KeyModifiers)keyModifiers, new System.EventHandler(this.ShortcutWindow));
			}
			catch (ApplicationException)
			{
				MessageBox.Show("Unable to register hotkey for 'Window'!");
			}
		}

		public void Unregister()
		{
			if (this.shortcutScreen != null)
			{
				this.shortcutScreen.Dispose();
				this.shortcutScreen = null;
			}
			if (this.shortcutSelection != null)
			{
				this.shortcutSelection.Dispose();
				this.shortcutSelection = null;
			}
			if (this.shortcutWindow != null)
			{
				this.shortcutWindow.Dispose();
				this.shortcutWindow = null;
			}
		}

		private void ShortcutScreen(object sender, EventArgs e)
		{
			if (this.screenshotInstance != null)
			{
				this.screenshotInstance.Dispose();
			}
			this.screenshotInstance = new Screenshot(this.cloudConfigInstance);
			this.screenshotInstance.Screen();
		}

		private void ShortcutSelection(object sender, EventArgs e)
		{
			if (this.screenshotInstance != null)
			{
				this.screenshotInstance.Dispose();
			}
			this.screenshotInstance = new Screenshot(this.cloudConfigInstance);
			this.screenshotInstance.Selection();
		}

		private void ShortcutWindow(object sender, EventArgs e)
		{
			if (this.screenshotInstance != null)
			{
				this.screenshotInstance.Dispose();
			}
			this.screenshotInstance = new Screenshot(this.cloudConfigInstance);
			this.screenshotInstance.Window();
		}
	}
}