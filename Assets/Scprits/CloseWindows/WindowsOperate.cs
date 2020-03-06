using GCSeries;
using huang.common.recordscreentool;
using liu;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// 程序窗口交互
/// </summary>
public class WindowsOperate : MonoBehaviour
{
    /// <summary>
    /// 程序退出
    /// </summary>
    public void ProgramApplicationQuit()
    {

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    //寻找当前目标窗口的进程
    [DllImport("user32.dll")]
    public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    //根据窗口句柄获取pid
    [DllImport("User32.dll")]
    public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);


    [DllImport("user32.dll", EntryPoint = "GetWindowLongA", SetLastError = true)]
    public static extern long GetWindowLong(IntPtr hwnd, int nIndex);

    public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong)
    {
        if (IntPtr.Size == 4)
        {
            return SetWindowLongPtr32(hWnd, nIndex, dwNewLong);
        }
        return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
    }

    [DllImport("user32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Auto)]
    public static extern IntPtr SetWindowLongPtr32(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", CharSet = CharSet.Auto)]
    public static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern long SetWindowPos(IntPtr hwnd, long hWndInsertAfter, long x, long y, long cx, long cy, long wFlags);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool MoveWindow(IntPtr hwnd, int x, int y, int cx, int cy, bool repaint);

    [DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
    public static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);


    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr SetFocus(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip,
       MonitorEnumDelegate lpfnEnum, IntPtr dwData);

    [DllImport("user32.dll")]
    public static extern bool GetMonitorInfoA(IntPtr monitorHandle, ref MonitorInfo mInfo);


    /// <summary>
    /// Returns the number of Displays using the Win32 functions
    /// </summary>
    /// <returns>collection of Display Info</returns>
    public static DisplayInfoCollection GetDisplays()
    {
        DisplayInfoCollection col = new DisplayInfoCollection();

        EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero,
            delegate (IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData)
            {
                MonitorInfo mi = new MonitorInfo();
                mi.size = (uint)Marshal.SizeOf(mi);
                bool success = GetMonitorInfoA(hMonitor, ref mi);
                if (success)
                {
                    DisplayInfo di = new DisplayInfo();
                    di.ScreenWidth = (mi.rcMonitor.right - mi.rcMonitor.left).ToString();
                    di.ScreenHeight = (mi.rcMonitor.bottom - mi.rcMonitor.top).ToString();
                    di.MonitorArea = mi.rcMonitor;
                    di.WorkArea = mi.rcWork;
                    di.Availability = mi.flags.ToString();
                    col.Add(di);
                }
                return true;
            }, IntPtr.Zero);
        return col;
    }

    public delegate bool MonitorEnumDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData);

    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MonitorInfo
    {
        public uint size;
        public Rect rcMonitor;
        public Rect rcWork;
        public uint flags;
    }

    /// <summary>
    /// The struct that contains the display information
    /// </summary>
    public class DisplayInfo
    {
        public string Availability { get; set; }
        public string ScreenHeight { get; set; }
        public string ScreenWidth { get; set; }
        public Rect MonitorArea { get; set; }
        public Rect WorkArea { get; set; }
    }

    /// <summary>
    /// Collection of display information
    /// </summary>
    public class DisplayInfoCollection : List<DisplayInfo>
    {
    }
}


