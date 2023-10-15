using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class DesktopScreenshot {
	/// <summary>HBITMAP</summary>
	private IntPtr bitmap = IntPtr.Zero;

	/// <summary>byte*</summary>
	private IntPtr bitmapBits = IntPtr.Zero;

	private int bitmapWidth = -1;
	private int bitmapHeight = -1;

	public Texture2D texture;
	public int width { get { return texture.width; } }
	public int height { get { return texture.height; } }
	public void Resize(int width, int height) { texture.Reinitialize(width, height); }

	/// <summary>
	/// Creates a screenshoter with a new texture.
	/// </summary>
	/// <param name="width">Width of texture, in pixels</param>
	/// <param name="height">Height of texture, in pixels</param>
	public DesktopScreenshot(int width, int height) {
		texture = new Texture2D(width, height, TextureFormat.BGRA32, false);
	}

	/// <summary>
	/// Creates a screenshoter for an existing BGRA32 texture.
	/// </summary>
	/// <param name="texture">A texture with format=TextureFormat.BGRA32</param>
	public DesktopScreenshot(Texture2D texture) {
		this.texture = texture;
	}

	/// <summary>
	/// Frees the underlying WinAPI bitmap if needed.
	/// Don't forget to call this when you're done using the thing!
	/// Calling this more than once has no effect.
	/// </summary>
	public void Destroy() {
		if (bitmap != IntPtr.Zero) {
			DeleteObject(bitmap);
			bitmap = IntPtr.Zero;
		}
		bitmapBits = IntPtr.Zero;
		bitmapWidth = -1;
		bitmapHeight = -1;
	}

	private void Prepare(int width, int height, IntPtr dc) {
		if (bitmapWidth == width && bitmapHeight == height) return;
		if (bitmap != IntPtr.Zero) DeleteObject(bitmap);
		bitmapWidth = width;
		bitmapHeight = height;
		BITMAPINFO bmi = new BITMAPINFO();
		bmi.bmiHeader = new BITMAPINFO.BITMAPINFOHEADER();
		bmi.bmiHeader.Init();
		bmi.bmiHeader.biPlanes = 1;
		bmi.bmiHeader.biBitCount = 32;
		bmi.bmiHeader.biWidth = width;
		bmi.bmiHeader.biHeight = height;
		bitmap = CreateDIBSection(dc, ref bmi, 0, out bitmapBits, IntPtr.Zero, 0);
	}

	/// <param name="x">Left coordinate of region to capture</param>
	/// <param name="y">Top coordinate of region to capture</param>
	public void Capture(int x, int y) {
		var width = texture.width;
		var height = texture.height;
		var hScreen = GetDC(IntPtr.Zero);
		var hMain = CreateCompatibleDC(hScreen);
		//
		Prepare(width, height, hMain);
		var hSwap = SelectObject(hMain, bitmap);
		MemSet(bitmapBits, 0, width * height * 4);
		BitBlt(hMain, 0, 0, width, height, hScreen, x, y, TernaryRasterOperations.SRCCOPY);
		SelectObject(hMain, hSwap);
		texture.LoadRawTextureData(bitmapBits, width * height * 4);
		//
		texture.Apply();
		DeleteDC(hMain);
		ReleaseDC(IntPtr.Zero, hScreen);
	}

	/// <summary>
	/// Prepares the native bitmap and clears pixels in the linked texture.
	/// </summary>
	public void Clear() {
		var width = texture.width;
		var height = texture.height;
		var hScreen = GetDC(IntPtr.Zero);
		var hMain = CreateCompatibleDC(hScreen);
		Prepare(width, height, hMain);
		//
		MemSet(bitmapBits, 0, width * height * 4);
		texture.LoadRawTextureData(bitmapBits, width * height * 4);
		//
		texture.Apply();
		DeleteDC(hMain);
		ReleaseDC(IntPtr.Zero, hScreen);
	}

	/// <summary>
	/// Captures a region of a screen and returns a new BGRA32 texture for it.<br/>
	/// A convenience function - consider instantiating DesktopScreenshot if you want to capture a region repeatedly.
	/// </summary>
	/// <param name="rect">A region to capture, measuring in pixels from primary display's top-left corner</param>
	/// <returns>new Texture2D with format=BGRA32</returns>
	public static Texture2D Capture(RectInt rect) {
		var cap = new DesktopScreenshot(rect.width, rect.height);
		cap.Capture(rect.x, rect.y);
		var tex = cap.texture;
		cap.Destroy();
		return tex;
	}

	/// <returns>A RectInt covering bounds of the primary display</returns>
	public static RectInt GetDisplayBounds() {
		return new RectInt(0, 0,
			GetSystemMetrics(SystemMetric.SM_CXSCREEN),
			GetSystemMetrics(SystemMetric.SM_CYSCREEN)
		);
	}

	/// <returns>A RectInt covering bounds of desktop (encompassing all displays)</returns>
	public static RectInt GetDesktopBounds() {
		return new RectInt(
			GetSystemMetrics(SystemMetric.SM_XVIRTUALSCREEN),
			GetSystemMetrics(SystemMetric.SM_YVIRTUALSCREEN),
			GetSystemMetrics(SystemMetric.SM_CXVIRTUALSCREEN),
			GetSystemMetrics(SystemMetric.SM_CYVIRTUALSCREEN)
		);
	}

	public class DisplayInfo {
		public RectInt screenArea;
		public RectInt workArea;
		public string deviceName;
		public bool isPrimary;
		override public string ToString() {
			return string.Format("{0} {1} primary={2}", deviceName, screenArea, isPrimary);
		}
	}

	// "IL2CPP does not support marshaling delegates that point to instance methods to native code"
	private static List<DisplayInfo> GetDisplayInfos_list;
	[AOT.MonoPInvokeCallback(typeof(EnumDisplayMonitorsFunc))] // <- needs to be commented out in pre-5.3
	private static bool GetDisplayInfos_iter(IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData) {
		MonitorInfoEx inf = new MonitorInfoEx();
		inf.Init();
		if (GetMonitorInfo(hMonitor, ref inf)) {
			DisplayInfo di = new DisplayInfo();
			di.screenArea = inf.Monitor.ToRectInt();
			di.workArea = inf.WorkArea.ToRectInt();
			di.deviceName = inf.DeviceName;
			di.isPrimary = (inf.Flags & 1) != 0;
			GetDisplayInfos_list.Add(di);
		}
		return true;
	}

	/// <summary>
	/// Queries information about all available displays.
	/// </summary>
	/// <returns>An array of classes with per-display information</returns>
	public static DisplayInfo[] GetDisplayInfos() {
		GetDisplayInfos_list = new List<DisplayInfo>();
		EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, GetDisplayInfos_iter, IntPtr.Zero);
		return GetDisplayInfos_list.ToArray();
	}

	#region P/Invoke

	enum BitmapCompressionMode : uint {
		BI_RGB = 0,
		BI_RLE8 = 1,
		BI_RLE4 = 2,
		BI_BITFIELDS = 3,
		BI_JPEG = 4,
		BI_PNG = 5
	}

	[StructLayout(LayoutKind.Sequential)]
	struct BITMAPINFO {
		[StructLayout(LayoutKind.Sequential)]
		public struct BITMAPINFOHEADER {
			public uint biSize;
			public int biWidth;
			public int biHeight;
			public ushort biPlanes;
			public ushort biBitCount;
			public BitmapCompressionMode biCompression;
			public uint biSizeImage;
			public int biXPelsPerMeter;
			public int biYPelsPerMeter;
			public uint biClrUsed;
			public uint biClrImportant;

			public void Init() {
				biSize = (uint)Marshal.SizeOf(this);
			}
		}
		public BITMAPINFOHEADER bmiHeader;
	}

	[DllImport("gdi32.dll")]
	static extern IntPtr CreateDIBSection(IntPtr hdc, [In] ref BITMAPINFO pbmi,
   uint pila, out IntPtr ppvBits, IntPtr hSection, uint dwOffset);

	[DllImport("user32.dll")]
	static extern IntPtr GetDC(IntPtr hWnd);

	[DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC", SetLastError = true)]
	static extern IntPtr CreateCompatibleDC([In] IntPtr hdc);

	[DllImport("gdi32.dll", EntryPoint = "SelectObject")]
	public static extern IntPtr SelectObject([In] IntPtr hdc, [In] IntPtr hgdiobj);

	public enum TernaryRasterOperations : uint { SRCCOPY = 0x00CC0020 }

	[DllImport("gdi32.dll", EntryPoint = "BitBlt", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	static extern bool BitBlt([In] IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, [In] IntPtr hdcSrc, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);

	[DllImport("user32.dll")]
	static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

	[DllImport("gdi32.dll", EntryPoint = "DeleteDC")]
	public static extern bool DeleteDC([In] IntPtr hdc);

	[DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool DeleteObject([In] IntPtr hObject);

	[DllImport("msvcrt.dll", SetLastError = false)]
	static extern IntPtr memcpy(IntPtr dest, IntPtr src, int count);

	[DllImport("msvcrt.dll", EntryPoint = "memset", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
	public static extern IntPtr MemSet(IntPtr dest, int c, int byteCount);

	enum SystemMetric {
		SM_CXSCREEN = 0,
		SM_CYSCREEN = 1,
		SM_XVIRTUALSCREEN = 76,
		SM_YVIRTUALSCREEN = 77,
		SM_CXVIRTUALSCREEN = 78,
		SM_CYVIRTUALSCREEN = 79,
	}

	[DllImport("user32.dll")]
	static extern int GetSystemMetrics(SystemMetric smIndex);

	delegate bool EnumDisplayMonitorsFunc(IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData);
	[DllImport("user32.dll")]
	static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, EnumDisplayMonitorsFunc lpfnEnum, IntPtr dwData);

	[StructLayout(LayoutKind.Sequential)]
	struct RECT {
		public int left, top, right, bottom;
		public RectInt ToRectInt() {
			return new RectInt(left, top, right - left, bottom - top);
		}
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	struct MonitorInfoEx {
		public int Size;
		public RECT Monitor;
		public RECT WorkArea;
		public uint Flags;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string DeviceName;
		public void Init() {
			Size = 40 + 2 * 32;
			DeviceName = string.Empty;
		}
	}

	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	static extern bool GetMonitorInfo(IntPtr hMonitor, ref MonitorInfoEx lpmi);

	#endregion
}
