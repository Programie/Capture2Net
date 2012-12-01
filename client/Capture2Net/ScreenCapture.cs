using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Capture2Net
{
	/// <summary>
	/// Provides functions to capture the entire screen or a particular window
	/// </summary>
	public class ScreenCapture
	{
		/// <summary>
		/// Creates an Image object containing a screen shot of a specific window
		/// </summary>
		/// <param name="handle">
		/// The handle to the window. (In windows forms, this is obtained by the Handle property)
		/// Use null to capture the whole screen
		/// </param>
		public Image Capture()
		{
			return this.Capture(User32.GetDesktopWindow(), User32.GetSystemMetrics(User32.SM_XVIRTUALSCREEN), User32.GetSystemMetrics(User32.SM_YVIRTUALSCREEN), User32.GetSystemMetrics(User32.SM_CXVIRTUALSCREEN), User32.GetSystemMetrics(User32.SM_CYVIRTUALSCREEN));
		}

		/// <summary>
		/// Creates an Image object containing a screen shot of a specific window
		/// </summary>
		/// <param name="handle">
		/// The handle to the window. (In windows forms, this is obtained by the Handle property)
		/// </param>
		public Image Capture(IntPtr handle)
		{
			User32.RECT windowRect = new User32.RECT();
			User32.GetWindowRect(handle, ref windowRect);
			return this.Capture(handle, 0, 0, windowRect.right - windowRect.left, windowRect.bottom - windowRect.top);
		}

		/// <summary>
		/// Creates an Image object containing a screen shot of the specified area of the screen
		/// </summary>
		/// <param name="left">
		/// The left position
		/// </param>
		/// <param name="top">
		/// The top position
		/// </param>
		/// <param name="width">
		/// The width
		/// </param>
		/// <param name="height">
		/// The height
		/// </param>
		public Image Capture(int left, int top, int width, int height)
		{
			return this.Capture(User32.GetDesktopWindow(), left, top, width, height);
		}

		/// <summary>
		/// Creates an Image object containing a screen shot of the specified area of the specified window
		/// </summary>
		/// <param name="handle">
		/// The handle to the window. (In windows forms, this is obtained by the Handle property)
		/// </param>
		/// <param name="left">
		/// The left position
		/// </param>
		/// <param name="top">
		/// The top position
		/// </param>
		/// <param name="width">
		/// The width
		/// </param>
		/// <param name="height">
		/// The height
		/// </param>
		public Image Capture(IntPtr handle, int left, int top, int width, int height)
		{
			// Get source and destination DC
			var dcSource = User32.GetWindowDC(handle);
			var dcDestination = GDI32.CreateCompatibleDC(dcSource);
			
			// Copy source to destination
			var bitmap = GDI32.CreateCompatibleBitmap(dcSource, width, height);
			var bitmapObject = GDI32.SelectObject(dcDestination, bitmap);
			GDI32.BitBlt(dcDestination, 0, 0, width, height, dcSource, left, top, GDI32.SRCCOPY);
			GDI32.SelectObject(dcDestination, bitmapObject);

			// Cleanup
			GDI32.DeleteDC(dcDestination);
			User32.ReleaseDC(handle, dcSource);

			// Create image
			var image = Image.FromHbitmap(bitmap);
			GDI32.DeleteObject(bitmap);

			return image;
		}

		public IntPtr GetDesktopWindow()
		{
			return User32.GetDesktopWindow();
		}

		/// <summary>
		/// Helper class containing Gdi32 API functions
		/// </summary>
		private class GDI32
		{
			public const int SRCCOPY = 0x00CC0020;// BitBlt dwRop parameter

			[DllImport("gdi32.dll")]
			public static extern bool BitBlt(IntPtr hObject,int nXDest,int nYDest, int nWidth,int nHeight,IntPtr hObjectSource, int nXSrc,int nYSrc,int dwRop);
			[DllImport("gdi32.dll")]
			public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC,int nWidth, int nHeight);
			[DllImport("gdi32.dll")]
			public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
			[DllImport("gdi32.dll")]
			public static extern bool DeleteDC(IntPtr hDC);
			[DllImport("gdi32.dll")]
			public static extern bool DeleteObject(IntPtr hObject);
			[DllImport("gdi32.dll")]
			public static extern IntPtr SelectObject(IntPtr hDC,IntPtr hObject);
		}

		/// <summary>
		/// Helper class containing User32 API functions
		/// </summary>
		private class User32
		{
			public const int SM_XVIRTUALSCREEN = 76;
			public const int SM_YVIRTUALSCREEN = 77;
			public const int SM_CXVIRTUALSCREEN = 78;
			public const int SM_CYVIRTUALSCREEN = 79;
			
			[StructLayout(LayoutKind.Sequential)]
			public struct RECT
			{
				public int left;
				public int top;
				public int right;
				public int bottom;
			}

			[DllImport("user32.dll")]
			public static extern IntPtr GetDesktopWindow();
			[DllImport("user32.dll")]
			public static extern IntPtr GetWindowDC(IntPtr hWnd);
			[DllImport("user32.dll")]
			public static extern IntPtr ReleaseDC(IntPtr hWnd,IntPtr hDC);
			[DllImport("user32.dll")]
			public static extern IntPtr GetWindowRect(IntPtr hWnd,ref RECT rect);
			[DllImport("user32.dll")]
			public static extern int GetSystemMetrics(int nIndex);
		}
	}
}