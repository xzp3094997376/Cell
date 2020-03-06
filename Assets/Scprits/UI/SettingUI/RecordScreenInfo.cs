using huang.common.screen;
using IniParser;
using IniParser.Model;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;


namespace huang.module.ui.settingui
{
    public class RecordScreenInfo
    {

        public static void SetLocalScreenMode(string config3DPath)
        {
            //初始化INIParser
            var parser = new FileIniDataParser();
            parser.Parser.Configuration.AllowDuplicateKeys = true;
            parser.Parser.Configuration.OverrideDuplicateKeys = true;
            parser.Parser.Configuration.AllowDuplicateSections = true;

            //如果没有ini路径。默认打开3D。
            FileInfo ini = new FileInfo(config3DPath);
            if (!ini.Exists)
            {
                if (!Directory.Exists(ini.Directory.FullName))
                    Directory.CreateDirectory(ini.Directory.FullName);
                var file = File.Create(config3DPath);
                file.Close();
                SectionDataCollection sec = new SectionDataCollection();
                IniData iniData = new IniData(sec);
                sec.AddSection("Screen");
                sec.AddSection("ScreenDimensional");
                sec.AddSection("ScreenMode");
                sec.AddSection("CanUseAR");
                iniData["Screen"].AddKey("IsScreen", "True");
                iniData["ProjectionMode"].AddKey("Mode", "VR-2D");
                //iniData["ScreenDimensional"].AddKey("Dimensional", "2D");
                //iniData["ScreenMode"].AddKey("ScreenMode", "VR");
                iniData["CanUseAR"].AddKey("AR", "True");
                iniData["Display"].AddKey("Name", "");
                iniData["VersionNo"].AddKey("CurVersion", "1.0");


                parser.WriteFile(config3DPath, iniData);      
                
                UISetting.isScreen = true;
                UISetting.isNoneProjection = false;
                UISetting.lastIsNoneProjection = false;
                UISetting.screenDimensional = UISetting.ScreenDimensional.TwoDimensional;
                UISetting.lastScreenDimensional = UISetting.ScreenDimensional.TwoDimensional;
                UISetting.screenmode = UISetting.ScreenMode.VR;
                UISetting.lastScreenmode = UISetting.ScreenMode.VR;

                UISetting.curScreenmode = ScreenManger.DualScreenMode.VR_2D;
                liu.GlobalConfig.Instance.displayName = "";
                liu.GlobalConfig.Instance.versionNO = "1.0";
            }
            else
            {
                IniData iniData = parser.ReadFile(ini.FullName);
                string isOpen = iniData["Screen"]["IsScreen"];
                string projectionMode = iniData["ProjectionMode"]["Mode"];
                //string cDimensional = iniData["ScreenDimensional"]["Dimensional"];
                //string cScreenMode = iniData["ScreenMode"]["ScreenMode"];
                string cAR = iniData["CanUseAR"]["AR"];
                string displayName = iniData["Display"]["Name"];
                string curVersionNo = iniData["VersionNo"]["CurVersion"];

                bool isScreen = isOpen == "True" ? true : false;


                //isScreen = screenNum > 1 ? isScreen : false;

                //var dimensional = cDimensional == "2D" ? UISetting.ScreenDimensional.TwoDimensional : UISetting.ScreenDimensional.ThreeDimensional;
                //var screenMode = cScreenMode == "VR" ? UISetting.ScreenMode.VR : UISetting.ScreenMode.AR;
                var arFunc = cAR == "True" ? true : false;
                switch (projectionMode)
                {
                    case "None":
                        UISetting.isNoneProjection = true;
                        UISetting.screenDimensional = UISetting.ScreenDimensional.None;
                        UISetting.screenmode = UISetting.ScreenMode.None;
                        break;
                    case "VR-2D":
                        UISetting.isNoneProjection = false;
                        UISetting.screenDimensional = UISetting.ScreenDimensional.TwoDimensional;
                        UISetting.screenmode = UISetting.ScreenMode.VR;
                        break;
                    case "VR-3D":
                        UISetting.isNoneProjection = false;
                        UISetting.screenDimensional = UISetting.ScreenDimensional.ThreeDimensional;
                        UISetting.screenmode = UISetting.ScreenMode.VR;
                        break;
                    case "AR":
                        UISetting.isNoneProjection = false;
                        UISetting.screenDimensional = UISetting.ScreenDimensional.TwoDimensional;
                        UISetting.screenmode = UISetting.ScreenMode.AR;
                        break;
                    default:
                        break;
                }

                UISetting.isScreen = isScreen;

                //UISetting.screenmode = UISetting.ScreenMode.VR;
                liu.GlobalConfig.canUseCameraAR = arFunc;
                liu.GlobalConfig.Instance.displayName = displayName;
                liu.GlobalConfig.Instance.versionNO = curVersionNo;

                UISetting.lastIsNoneProjection = UISetting.isNoneProjection;
                UISetting.lastScreenDimensional = UISetting.screenDimensional;
                UISetting.lastScreenmode = UISetting.screenmode;
            }
        }


        public static void SaveScreenModeToLocal(string config3DPath)
        {
            //初始化INIParser
            var parser = new FileIniDataParser();
            parser.Parser.Configuration.AllowDuplicateKeys = true;
            parser.Parser.Configuration.OverrideDuplicateKeys = true;
            parser.Parser.Configuration.AllowDuplicateSections = true;

            //如果没有ini路径。默认打开3D。
            FileInfo ini = new FileInfo(config3DPath);
            if (!ini.Exists)
            {
                Debug.LogWarning("Config3D.ini 路径不存在" + config3DPath);
                return;
            }

            IniData iniData = parser.ReadFile(ini.FullName);
            string isOpen = UISetting.isScreen ? "True" : "False";

            //string cDimensional = UISetting.screenDimensional == UISetting.ScreenDimensional.ThreeDimensional ? "3D" : "2D";
            //string cScreenMode = UISetting.screenmode == UISetting.ScreenMode.AR ? "AR" : "VR";
            string projectionMode;
            if (UISetting.isNoneProjection)
            {
                projectionMode = "None";
            }
            else
            {
                if (UISetting.screenmode == UISetting.ScreenMode.AR)
                {
                    projectionMode = "AR";
                }
                else
                {
                    if (UISetting.screenDimensional == UISetting.ScreenDimensional.TwoDimensional)
                    {
                        projectionMode = "VR-2D";
                    }
                    else
                    {
                        projectionMode = "VR-3D";
                    }
                }
            }

            string cARFunc = liu.GlobalConfig.canUseCameraAR ? "True" : "False";

            iniData["Screen"]["IsScreen"] = isOpen;
            //iniData["ScreenDimensional"]["Dimensional"] = cDimensional;
            //iniData["ScreenMode"]["ScreenMode"] = cScreenMode;

            iniData["ProjectionMode"]["Mode"] = projectionMode;
            iniData["CanUseAR"]["AR"] = cARFunc;
            iniData["Display"]["Name"] = liu.GlobalConfig.Instance.displayName;
            iniData["VersionNo"]["CurVersion"] = liu.GlobalConfig.Instance.versionNO;

            parser.WriteFile(config3DPath, iniData, Encoding.UTF8);

            Debug.Log("Config3D.ini 保存路径：" + config3DPath);
        }
    }
}