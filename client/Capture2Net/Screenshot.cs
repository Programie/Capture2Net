using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Media;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Capture2Net
{
	public class Screenshot : IDisposable
	{
		ScreenCapture screenCaptureInstance;
		CloudConfig cloudConfigInstance;
		SelectionForm selectionForm;
		Image image;
		IntPtr activeWindow;

		enum ScreenshotType
		{
			Screen,
			Selection,
			Window
		}

		internal class NativeMethods
		{
			[DllImport("user32.dll")]
			public static extern IntPtr GetForegroundWindow();
			[DllImport("user32.dll", CharSet = CharSet.Unicode)]
			public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
			[DllImport("user32.dll")]
			public static extern int GetWindowTextLength(IntPtr hWnd);
		}

		public Screenshot(CloudConfig cloudConfigInstance)
		{
			this.screenCaptureInstance = new ScreenCapture();
			this.cloudConfigInstance = cloudConfigInstance;
			this.activeWindow = NativeMethods.GetForegroundWindow();
		}

		~Screenshot()
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
			if (this.selectionForm != null)
			{
				this.selectionForm.Dispose();
			}
		}

		// Screenshot of whole screen
		public void Screen()
		{
			this.image = this.screenCaptureInstance.Capture();
			this.StartUpload(ScreenshotType.Screen);
		}

		// Allow to select the area
		public void Selection()
		{
			this.image = this.screenCaptureInstance.Capture();
			this.selectionForm = new SelectionForm(image);
			this.selectionForm.Show();
			this.selectionForm.FormClosed += this.SelectionFormClosed;
		}

		// Screenshot of current active window
		public void Window()
		{
			this.image = this.screenCaptureInstance.Capture(this.activeWindow);
			this.StartUpload(ScreenshotType.Window);
		}

		private void SelectionFormClosed(object sender, FormClosedEventArgs e)
		{
			if (this.selectionForm.accepted)
			{
				var bitmap = new Bitmap(this.image);
				this.image.Dispose();
				this.image = bitmap.Clone(this.selectionForm.cropArea, bitmap.PixelFormat);
				this.StartUpload(ScreenshotType.Selection);
			}
		}

		/// <summary>
		/// Save the screenshot to a temporary file and starts the upload (Executes this application using parameter "/upload temp-file-name")
		/// </summary>
		/// <param name="type">The type of the screenshot from ScreenshotType structure</param>
		/// <returns>The path of the temporary file in which the screenshot and info data block was written to</returns>
		private void StartUpload(ScreenshotType type)
		{
			new SoundPlayer(Properties.Resources.CameraSound).PlaySync();
			var tempFile = System.IO.Path.GetTempFileName();
			var jsonData = this.cloudConfigInstance.JsonData["screenshots"][type.ToString().ToLower()];
			var fileExtension = jsonData["imageFormat"].ToString().ToLower();
			ImageFormat imageFormat;
			switch (fileExtension)
			{
				case "bmp":
					imageFormat = ImageFormat.Bmp;
					break;
				case "gif":
					imageFormat = ImageFormat.Gif;
					break;
				case "png":
					imageFormat = ImageFormat.Png;
					break;
				case "jpg":
					imageFormat = ImageFormat.Jpeg;
					break;
				case "tif":
					imageFormat = ImageFormat.Tiff;
					break;
				default:
					fileExtension = "jpg";
					imageFormat = ImageFormat.Jpeg;
					break;
			}

			// Create new file stream for the image and following info data
			var imageStream = new FileStream(tempFile, FileMode.Create);
			
			// Save image to file stream
			this.image.Save(imageStream, imageFormat);

			// Write info data block to file stream
			List<string> infoDataList = new List<string>();

			var activeWindowTitle = new StringBuilder(NativeMethods.GetWindowTextLength(this.activeWindow) + 1);// Length of window title + 1 for the null character
			NativeMethods.GetWindowText(this.activeWindow, activeWindowTitle, activeWindowTitle.Capacity);

			infoDataList.Add("\tINFODATA\t");// Code to identify the start of the info data block (Must contain characters which are never used in the info data)
			infoDataList.Add("screenshotType=" + type.ToString().ToLower());// Screenshot type
			infoDataList.Add("fileExtension=" + fileExtension);// Image type
			infoDataList.Add("activeWindow=" + activeWindowTitle);// Title of the current active window
			infoDataList.Add("userName=" + System.Security.Principal.WindowsIdentity.GetCurrent().Name);// Username of current logged in user (Windows user)
			infoDataList.Add("hostName=" + Dns.GetHostName());// Hostname of this computer

			var infoData = ASCIIEncoding.ASCII.GetBytes(string.Join("\n", infoDataList.ToArray()));
			imageStream.Write(infoData, 0, infoData.Length);
			
			// Close the file stream
			imageStream.Close();

			Process.Start(Environment.GetCommandLineArgs()[0], "/upload \"" + tempFile + "\"");
		}
	}
}