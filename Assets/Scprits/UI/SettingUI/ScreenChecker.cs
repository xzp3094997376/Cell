using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using MyFrameWork;
using static F3D.Screen.ScreenHelper;

/// <summary>
/// 设备检测
/// 
/// 如有新设备，添加到StremingAssets目录下Monitors.ini文件
/// 如下用逗号分隔
/// [Display]
/// GCMonitors = HRE069B,HRE099,HRE100
/// ProjMonitors = SCT2346,DT232
/// </summary>
public class ScreenChecker : Singleton<ScreenChecker>{

    public struct ErrResult
    {
        /// <summary>
        /// 只有一块屏
        /// </summary>
        public const int OnlyOneScreen = 1001;
        /// <summary>
        /// 结果正确
        /// </summary>
        public const int Succ = 0;
        /// <summary>
        /// 非公司一体机
        /// </summary>
        public const int NotOurGC = 1002;

        /// <summary>
        /// 非公司大屏
        /// </summary>
        public const int NotOurProjScreen = 1003;
        public int errCode;
        public string reason;

        public override string ToString()
        {
            return string.Format("errCode: " + errCode + " ,reason: " + reason);
        }
    }

    private string monitorPath = "";
    /// <summary>
    /// 一体机设备名
    /// </summary>
    public const string GCDeviceName = "HRE069B";
    /// <summary>
    /// 大屏设备名
    /// </summary>
    public const string ProjDeviceName = "SCT2346";

    string[] GCDevices;
    string[] ProjDevices;

    public struct DeviceInfo
    {
        public string OSVersion;
        public string GraphicsDeviceID;
        public string GraphicsDeviceName;
        public string GraphicsDeviceType;
        public string GraphicsDeviceVendor;
        public string GraphicsDeviceVendorID;
        public string GraphicsDeviceVersion;
    }

    private DeviceInfo deviceInfo;

	public ScreenChecker()
    {
        //Common.AppLog.AddMsg(Common.LogLevel.INFO, "ScreenChecker OS: " + deviceInfo.OSVersion);
        GCDevices = new string[]{ GCDeviceName};
        ProjDevices = new string[] { ProjDeviceName };
        monitorPath = Application.streamingAssetsPath+"/Monitors.ini";

        deviceInfo = new DeviceInfo();
        deviceInfo.OSVersion = SystemInfo.operatingSystem;
        deviceInfo.GraphicsDeviceID = SystemInfo.graphicsDeviceID.ToString();
        deviceInfo.GraphicsDeviceName = SystemInfo.graphicsDeviceName;
        deviceInfo.GraphicsDeviceType = SystemInfo.graphicsDeviceType.ToString("g");
        deviceInfo.GraphicsDeviceVendor = SystemInfo.graphicsDeviceVendor;
        deviceInfo.GraphicsDeviceVendorID = SystemInfo.graphicsDeviceVendorID.ToString();
        deviceInfo.GraphicsDeviceVersion = SystemInfo.graphicsDeviceVersion;

        if (!File.Exists(monitorPath))
        {
            ConfigINI ci = new ConfigINI(monitorPath);
            string gcDevices = string.Join(",", GCDeviceName);
            ci.SetData("Display", "GCMonitors", gcDevices);
            string projDevices = string.Join(",", ProjDeviceName);
            ci.SetData("Display", "ProjMonitors", projDevices);
            ci.Save();
        }
        else
        {
            ConfigINI ci = new ConfigINI(monitorPath);
            GCDevices = ci.ReadData("Display", "GCMonitors").Split(',');
            ProjDevices = ci.ReadData("Display", "ProjMonitors").Split(',');
        }
    }

    /// <summary>
    /// 是否是公司型号大屏
    /// </summary>
    /// <returns>接公司型号大屏返回true，如果只有一个屏或者不是接指定型号设备屏返回false</returns>
    public ErrResult IsProjDeviceOurCompany()
    {
        var displays = F3D.Screen.ScreenHelper.GetAllDisplayDevice();
        if(displays.Count < 2)
        {
            return new ErrResult { errCode = ErrResult.OnlyOneScreen, reason = "未接2块屏，不能投屏" };
        }

        List<DisplayDevice> findDisplay = displays.FindAll(d=> d.m_IsPrimary == false);
        foreach(string dev in ProjDevices)
        {
            for(int i = 0; i < findDisplay.Count; i++)
            {
                bool result = findDisplay[i].m_Monitors.FindIndex(d => d.m_Name == dev) > -1;
                if (result)
                {
                    return new ErrResult { errCode = ErrResult.Succ, reason = "" };
                }
            }
        }
        return new ErrResult { errCode = ErrResult.NotOurProjScreen, reason = "不是公司型号大屏，不能投屏" };
    }

    /// <summary>
    /// 是否是公司型号一体机
    /// </summary>
    /// <returns></returns>
    public ErrResult IsGCDeviceOurCompany()
    {
        var displays = F3D.Screen.ScreenHelper.GetAllDisplayDevice();

        DisplayDevice findDisplay = displays.Find(d => d.m_IsPrimary == true);
        foreach (string dev in GCDevices)
        {
            bool result = findDisplay.m_Monitors.FindIndex(d => d.m_Name == dev) > -1;
            if (result)
            {
                return new ErrResult { errCode = ErrResult.Succ, reason = "" };
            }
        }
        return new ErrResult { errCode = ErrResult.NotOurGC, reason = "不是公司型号一体机" };
    }

    public string OSInfo
    {
        get
        {
            return SystemInfo.operatingSystem;
        }
    }

    //public bool IsWin10
    //{
    //    get {
    //        Common.AppLog.AddMsg(Common.LogLevel.INFO, "ScreenChecker OS: "+ deviceInfo.OSVersion);
    //        bool isWin10 = deviceInfo.OSVersion.StartsWith("Windows 10");
    //        bool is64bit = deviceInfo.OSVersion.EndsWith("64bit");
    //        /*deviceInfo.OSVersion == "Windows 10  (10.0.0) 64bit"*/;
    //        return isWin10 && is64bit;
    //    }
    //}

    public bool IsWin7
    {
        get
        {
            return deviceInfo.OSVersion.Contains("Windows 7");
        }
    }

    public string GraphicsInfo
    {
        get
        {
            return string.Format("SystemInfo.graphicsDeviceID:{0},SystemInfo.graphicsDeviceName:{1},SystemInfo.graphicsDeviceType:{2},SystemInfo.graphicsDeviceVendor:{3},SystemInfo.graphicsDeviceVendorID:{4},SystemInfo.graphicsDeviceVersion:{5}", 
                SystemInfo.graphicsDeviceID, 
                SystemInfo.graphicsDeviceName, 
                SystemInfo.graphicsDeviceType.ToString("g"), 
                SystemInfo.graphicsDeviceVendor, 
                SystemInfo.graphicsDeviceVendorID, 
                SystemInfo.graphicsDeviceVersion);
        }
    }
}
