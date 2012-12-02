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
			return this.Capture(NativeMethods.GetDesktopWindow(), NativeMethods.GetSystemMetrics(NativeMethods.SM_XVIRTUALSCREEN), NativeMethods.GetSystemMetrics(NativeMethods.SM_YVIRTUALSCREEN), NativeMethods.GetSystemMetrics(NativeMethods.SM_CXVIRTUALSCREEN), NativeMethods.GetSystemMetrics(NativeMethods.SM_CYVIRTUALSCREEN));
		}

		/// <summary>
		/// Creates an Image object containing a screen shot of a specific window
		/// </summary>
		/// <param name="handle">
		/// The handle to the window. (In windows forms, this is obtained by the Handle property)
		/// </param>
		public Image Capture(IntPtr handle)
		{
			NativeMethods.RECT windowRect = new NativeMethods.RECT();
			NativeMethods.GetWindowRect(handle, ref windowRect);
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
			return this.Capture(NativeMethods.GetDesktopWindow(), left, top, width, height);
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
			var dcSource = NativeMethods.GetWindowDC(handle);
			var dcDestination = NativeMethods.CreateCompatibleDC(dcSource);
			
			// Copy source to destination
			var bitmap = NativeMethods.CreateCompatibleBitmap(dcSource, width, height);
			var bitmapObject = NativeMethods.SelectObject(dcDestination, bitmap);
			NativeMethods.BitBlt(dcDestination, 0, 0, width, height, dcSource, left, top, NativeMethods.SRCCOPY);
			NativeMethods.SelectObject(dcDestination, bitmapObject);

			// Cleanup
			NativeMethods.DeleteDC(dcDestination);
			NativeMethods.ReleaseDC(handle, dcSource);

			// Create image
			var image = Image.FromHbitmap(bitmap);
			NativeMethods.DeleteObject(bitmap);

			return image;
		}

		public IntPtr GetDesktopWindow()
		{
			return NativeMethods.GetDesktopWindow();
		}

		/// <summary>
		/// Helper class containing native API functions
		/// </summary>
		private class NativeMethods
		{
			public const int SM_XVIRTUALSCREEN = 76;
			public const int SM_YVIRTUALSCREEN = 77;
			public const int SM_CXVIRTUALSCREEN = 78;
			public const int SM_CYVIRTUALSCREEN = 79;
			public const int SRCCOPY = 0x00CC0020;// BitBlt dwRop parameter

			[StructLayout(LayoutKind.Sequential)]
			public struct RECT
			{
				public int left;
				public int top;
				public int right;
				public int bottom;
			}

			// GDI32
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

			// User32
			[DllImport("user32.dll")]
			public static extern IntPtr GetDesktopWindow();
			[DllImport("user32.dll")]
			public static extern IntPtr GetWindowDC(IntPtr hWnd);
			[DllImport("user32.dll")]
			public static extern int ReleaseDC(IntPtr hWnd,IntPtr hDC);
			[DllImport("user32.dll")]
			public static extern int GetWindowRect(IntPtr hWnd, ref RECT rect);
			[DllImport("user32.dll")]
			public static extern int GetSystemMetrics(int nIndex);
		}
	}
}