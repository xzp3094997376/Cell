using huang.common.screen;
using liu;
using SimpleFileBrowser;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GCSeries;
using MyFrameWork;
using System.IO;

namespace huang.module.ui.settingui
{
    public class UISetting : MonoBehaviour
    {
        /// <summary>
        /// 投屏维数
        /// </summary>
        public enum ScreenDimensional
        {
            /// <summary>
            /// 2D
            /// </summary>
            TwoDimensional,
            /// <summary>
            /// 3D
            /// </summary>
            ThreeDimensional,
            None
        }

        public enum ScreenMode
        {
            /// <summary>
            /// VR投屏
            /// </summary>
            VR,
            /// <summary>
            /// AR投屏
            /// </summary>
            AR,
            None
        }

        //根据配置，是否允许投屏
        public static bool isScreen = false;
        //记录上一次根据屏幕控制是否投屏
        public static bool isNoneProjection = true;
        public static bool lastIsNoneProjection = true;

        public static ScreenDimensional screenDimensional = ScreenDimensional.None;
        public static ScreenDimensional lastScreenDimensional = ScreenDimensional.None;
        public static ScreenMode screenmode = ScreenMode.None;
        public static ScreenMode lastScreenmode = ScreenMode.None;
        public static ScreenManger.DualScreenMode curScreenmode { get; set; } = ScreenManger.DualScreenMode.None;

        /// <summary>
        /// 存储路径
        /// </summary>
        public static string RecordPath
        {
            get; private set;
        }

        GameObject nonARCameraWarnning;
        Text nonARCameraWarnningText;
        private bool canUseVR3D;

        /// <summary>
        /// 屏幕数量
        /// </summary>
        int projectionState = -1;

        /// <summary>
        /// 版本号
        /// </summary>
        Text versionText;

        private void OnEnable()
        {
            SetRecordVidoPath();
            //screenNum = F3Device.DeviceManager.Instance.AllMonitors.Count;
        }



        /// <summary>
        /// 记录当前屏幕数量
        /// </summary>
        int screenNum;
        public static IntPtr mainWinPtr;
        void Awake()
        {
            mainWinPtr = FARDll.FindWindow(null, Application.productName);
            //ScreenManger.Instance.SetScreenState();
            screenNum = F3Device.DeviceManager.Instance.AllMonitors.Count;
            Common.AppLog.AddMsg(Common.LogLevel.DEBUG,"UISetting.Awake(): screenNum  当前屏幕数量 " + screenNum);

            //projectionState = liu.GetScreenMode.GetProjectionMode();
            projectionState = Projection.GetScreenMode.GetProjectionMode();
            //Common.AppLog.AddMsg(Common.LogLevel.DEBUG,"UISetting.Awake(): projectionState= " + projectionState);

            if (screenNum > 1 && projectionState != 2)
            {
                //Common.AppLog.AddMsg(Common.LogLevel.DEBUG,"UISetting.Awake(): 开启扩展模式 " + projectionState);

                Common.ScreenHelper.SetProjection(Common.ScreenHelper.SDC_TOPOLOGY_EXTEND);
            }

            uint id = FrameTimerHeap.AddTimer(1000, 2000, OnUpdateJudge);
            //FrameTimerHeap.DelTimer(id);  //删除定时器
            var openFildPath = transform.Find("FieldPath");

            AttributeGetValue();
            SetScreenToggleListener();
            SetOtherListener();

            versionText = transform.Find("Version/PathText").GetComponent<Text>();

            Invoke("DelayCall", 0.2f);

            nonARCameraWarnning = aRToggle.transform.Find("NonARCameraWarnning").gameObject;
            nonARCameraWarnningText = nonARCameraWarnning.GetComponent<Text>();
            nonARCameraWarnning.SetActive(false);
            UGUIEventListener.Get(aRToggle.gameObject).onClick += ClickARToggle;
        }

        void OnUpdateJudge()
        {
            JudgeHasExternalCamera();

            JudgeMoreThanTwoScreen();
        }

        /// <summary>
        /// 获取投屏模式
        /// </summary>
        /// <returns></returns>
        public static ScreenManger.DualScreenMode GetScreenMode()
        {
            if (isNoneProjection == true)
            {
                return ScreenManger.DualScreenMode.None;
            }
            if (screenDimensional == ScreenDimensional.ThreeDimensional)
            {
                if (screenmode == ScreenMode.AR)
                {
                    return ScreenManger.DualScreenMode.AR;
                }
                else if (screenmode == ScreenMode.VR)
                {
                    return ScreenManger.DualScreenMode.VR;
                }
            }
            else if (screenDimensional == ScreenDimensional.TwoDimensional)
            {
                if (screenmode == ScreenMode.AR)
                {
                    return ScreenManger.DualScreenMode.AR_2D;
                }
                else if (screenmode == ScreenMode.VR)
                {
                    return ScreenManger.DualScreenMode.VR_2D;
                }
            }
            return ScreenManger.DualScreenMode.None;
        }

        RecordOperate recordOperate;

        /// <summary>
        /// 根据切换的状态，切换录屏的录制屏幕宽度
        /// </summary>
        /// <param name="startMode">1 = 960  2 = 1920 </param>
        void RecSwitch()
        {
            if (recordOperate == null) recordOperate = FindObjectOfType<RecordOperate>();
            if (recordOperate.startFlag)
            {
                recordOperate.StartRec();
            }
        }

        /// <summary>
        /// 设置投屏toggle事件
        /// </summary>
        void SetScreenToggleListener()
        {
            noneScreenToggle.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<bool>((isOn) =>
            {
                if (isOn)
                {
                    isNoneProjection = true;
                    nonScreenText.color = Color.yellow;
                    SetScreenMode();
                }
                else
                {
                    nonScreenText.color = Color.white;
                }
            }));

            twoDVRToggle.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<bool>((isOn) =>
            {
                if (isOn)
                {
                    if (isScreen)
                    {
                        isNoneProjection = false;
                        screenDimensional = ScreenDimensional.TwoDimensional;
                        screenmode = ScreenMode.VR;
                        TwoDVRText.color = Color.yellow;
                        SetScreenMode();
                    }
                    else
                    {
                        noneScreenToggle.isOn = true;
                    }
                }
                else
                {
                    TwoDVRText.color = Color.white;
                }

            }));

            threeDVRToggle.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<bool>((isOn) =>
            {
                if (isOn)
                {
                    if (isScreen)
                    {
                        isNoneProjection = false;
                        screenDimensional = ScreenDimensional.ThreeDimensional;
                        screenmode = ScreenMode.VR;
                        ThreeDVRText.color = Color.yellow;
                        SetScreenMode();
                    }
                    else
                    {
                        noneScreenToggle.isOn = true;
                    }
                }
                else
                {
                    ThreeDVRText.color = Color.white;
                }
            }));

            aRToggle.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<bool>((isOn) =>
            {
                if (isOn)
                {
                    if (isScreen)
                    {
                        isNoneProjection = false;
                        screenDimensional = ScreenDimensional.TwoDimensional;
                        screenmode = ScreenMode.AR;
                        ARText.color = Color.yellow;
                        SetScreenMode();
                    }
                    else
                    {
                        noneScreenToggle.isOn = true;
                    }
                }
                else
                {
                    ARText.color = Color.white;
                }
            }));
        }

        void SetScreenMode()
        {
            var curMode = GetScreenMode();
            ScreenManger.Instance.SetScreenMode(curMode,RecSwitch);
        }




        /// <summary>
        /// 设置Setting面板的按钮状态
        /// 这是缺屏幕的
        /// </summary>
        /// <param name="isSupportScreen"></param>
        void EnableScreenMode(bool isSupportScreen)
        {
            AttributeGetValue();
            //如果投屏模式中投屏高亮，则2D/3D模式和投屏模式可以交互
            if (isSupportScreen)
            {
                twoDVRToggle.interactable = threeDVRToggle.interactable = aRToggle.interactable = true;

                ARText.color = TwoDVRText.color = ThreeDVRText.color = Color.white;
                TwoDVRBackground.color = ThreeDVRBackground.color = ARBackground.color = Color.white;
                ARCheckmark.color = TwoDVRCheckmark.color = ThreeDVRCheckmark.color = Color.yellow;

                if (isScreen)
                {
                    if (lastIsNoneProjection)
                    {
                        noneScreenToggle.isOn = true;
                    }
                    else
                    {
                        if (screenmode == ScreenMode.AR)
                        {
                            aRToggle.isOn = true;
                        }
                        else
                        {
                            if (screenDimensional == ScreenDimensional.ThreeDimensional)
                            {
                                threeDVRToggle.isOn = true;
                            }
                            else
                            {
                                twoDVRToggle.isOn = true;
                            }
                        }
                    }
                }
                else
                {
                    noneScreenToggle.isOn = true;
                }

            }
            else //2D/3D模式和投屏模式不支持交互
            {
                twoDVRToggle.interactable = threeDVRToggle.interactable = aRToggle.interactable = false;

                TwoDVRBackground.color = ThreeDVRBackground.color = ARBackground.color =
                ThreeDVRCheckmark.color = TwoDVRCheckmark.color = ARCheckmark.color = new Color32(140, 140, 140, 255);

                TwoDVRText.color = ThreeDVRText.color = ARText.color = new Color32(140, 140, 140, 255);

                noneScreenToggle.isOn = true;

            }
        }


        Image TwoDVRBackground, TwoDVRCheckmark, ThreeDVRBackground, ThreeDVRCheckmark,
           ARBackground, ARCheckmark;
        Text TwoDVRText, ThreeDVRText, ARText;

        Toggle noneScreenToggle;
        Toggle twoDVRToggle;
        Toggle threeDVRToggle;
        Toggle aRToggle;

        Text nonScreenText;
        //给每个属性赋值
        void AttributeGetValue()
        {
            var projectionType = transform.Find("ProjectionType");

            noneScreenToggle = projectionType.Find("NoneScreenToggle").GetComponent<Toggle>();
            nonScreenText = noneScreenToggle.transform.Find("Label").GetComponent<Text>();


            aRToggle = projectionType.Find("ARToggle").GetComponent<Toggle>();

            twoDVRToggle = projectionType.Find("TwoDToggle").GetComponent<Toggle>();
            threeDVRToggle = projectionType.Find("ThreeDToggle").GetComponent<Toggle>();
            //-----------------------Background-----------------------
            if (null == TwoDVRBackground)
            {
                TwoDVRBackground = twoDVRToggle.transform.Find("Background").GetComponent<Image>();
            }

            if (null == ThreeDVRBackground)
            {
                ThreeDVRBackground = threeDVRToggle.transform.Find("Background").GetComponent<Image>();
            }

            if (null == ARBackground)
            {
                ARBackground = aRToggle.transform.Find("Background").GetComponent<Image>();
            }
            //-----------------------Label-----------------------
            if (null == TwoDVRText)
            {
                TwoDVRText = twoDVRToggle.transform.Find("Label").GetComponent<Text>();
            }

            if (null == ThreeDVRText)
            {
                ThreeDVRText = threeDVRToggle.transform.Find("Label").GetComponent<Text>();
            }

            if (null == ARText)
            {
                ARText = aRToggle.transform.Find("Label").GetComponent<Text>();
            }
            //-----------------------Checkmark-------------------------------------
            if (null == TwoDVRCheckmark)
            {
                TwoDVRCheckmark = TwoDVRBackground.transform.Find("Checkmark").GetComponent<Image>();
            }

            if (null == ThreeDVRCheckmark)
            {
                ThreeDVRCheckmark = ThreeDVRBackground.transform.Find("Checkmark").GetComponent<Image>();
            }

            if (null == ARCheckmark)
            {
                ARCheckmark = ARBackground.transform.Find("Checkmark").GetComponent<Image>();
            }
        }

        /// <summary>
        /// 设置应用更新、确定、取消的按钮事件
        /// </summary>
        void SetOtherListener()
        {
            var updateBtn = transform.Find("Version/UpdateBtn").GetComponent<Button>();
            var okBtn = transform.Find("OKBtn").GetComponent<Button>();
            var cancelBtn = transform.Find("CancelBtn").GetComponent<Button>();
            var openFieldBtn = transform.Find("RecordPath/OpenFieldBtn").GetComponent<Button>();

            updateBtn.onClick.AddListener(delegate
            {

            });

            okBtn.onClick.AddListener(delegate
            {

                lastIsNoneProjection = isNoneProjection;
                lastScreenDimensional = screenDimensional;
                lastScreenmode = screenmode;
                var curMode = GetScreenMode();

                curScreenmode = curMode;

                ScreenManger.Instance.SetScreenMode(curMode);


                CloseSettingUI();
            });

            cancelBtn.onClick.AddListener(delegate
            {
                isNoneProjection = lastIsNoneProjection;
                screenDimensional = lastScreenDimensional;
                screenmode = lastScreenmode;

                SetScreenMode();
                SetAllToggleState();
                CloseSettingUI();
            });

            openFieldBtn.onClick.AddListener(delegate
            {
                if (recordOperate == null) recordOperate = FindObjectOfType<RecordOperate>();
                if (!recordOperate.startFlag)
                {
                    SetSavePath();
                }
                else
                {
                    OperateWarnning.Instance.ShowWarnningPanel("正在执行录屏中，请等录屏结束后再去修改录屏保存的位置。谢谢！");
                }
            });
        }


        void SetAllToggleState()
        {
            if (isScreen && canProjection)
            {
                if (!isNoneProjection)
                {
                    if (screenDimensional == ScreenDimensional.TwoDimensional)
                    {
                        if (screenmode == ScreenMode.VR)
                        {
                            twoDVRToggle.isOn = true;
                        }
                        else
                        {
                            aRToggle.isOn = true;
                        }
                    }
                    else
                    {
                        if (screenmode == ScreenMode.VR)
                            threeDVRToggle.isOn = true;
                    }
                }
                else
                {
                    noneScreenToggle.isOn = true;
                }
            }
            else
            {
                noneScreenToggle.isOn = true;
            }
        }

        /// <summary>
        /// 关闭投屏
        /// 设置状态为灰
        /// </summary>
        public void CloseProjection()
        {
            noneScreenToggle.isOn = true;
            //因为屏幕数量小于二关闭的投屏
            EnableScreenMode(false);
        }


        void DelayCall()
        {
            thisPanelShowState(false);
            //先获取投屏状态
            RecordScreenInfo.SetLocalScreenMode(GlobalConfig.config3DPath);
            //设置toggle的颜色
            //EnableScreenMode(isScreen);
            //开启投屏
            //SetScreenMode();
            
            if(isScreen && screenNum > 1)
            {
                //在有外接屏幕情况下，默认识别到我司3D大屏自动投屏3D - VR投屏
                IntPtr ptr = FARDll.FindWindow(null, "ClientWinCpp");
                var device = F3Device.DeviceManager.Instance.FindHidDevice(ptr);
                if(device!=null && device is F3Device.IGraph3Device)
                {
                    canUseVR3D = true;
                    StartCoroutine(ShortcutKeySetVR3D());
                }
                else
                {
                    canUseVR3D = false;
                    StartCoroutine(ShortcutKeySetVR2D());
                }
            }

            versionText.text = GlobalConfig.Instance.versionNO;
            FindObjectOfType<Direct3DWin>().TargetDisplayName = GlobalConfig.Instance.displayName;
        }

        public void thisPanelShowState(bool state)
        {
            if (state)
            {
                transform.localScale = Vector3.one;
                OnEnable();
            }
            else
                transform.localScale = Vector3.zero;
        }

        private void ClickARToggle(GameObject go)
        {
            if (!hasCameraFlag || !GlobalConfig.canUseCameraAR)
            {
                nonARCameraWarnning.SetActive(true);
                if (!GlobalConfig.canUseCameraAR)
                    nonARCameraWarnningText.text = "AR功能不适用";
                else
                    nonARCameraWarnningText.text = "未检测到摄像头";

                nonARCameraWarnning.transform.DOMove(nonARCameraWarnning.transform.position, 3f).OnComplete
                    (() =>
                    {
                        nonARCameraWarnning.SetActive(false);
                    });
            }
        }


        void Update()
        {
            //换用定时器作Update检测,其他地方可以不用重复定义Update
//#if !UNITY_EDITOR
            //TimerManager.Instance.Update();
//#endif
        }

        int lastScreenCount = -1;
        bool canProjection = true;
        /// <summary>
        /// 根据屏幕的数量，实时更新投屏的功能
        /// </summary>
        void JudgeMoreThanTwoScreen()
        {
            //Common.AppLog.AddMsg(Common.LogLevel.DEBUG,"UISetting.JudgeMoreThanTwoScreen(): 判断屏幕的数量");
            if (Input.GetKeyDown(KeyCode.Alpha1) && Input.GetKeyDown(KeyCode.Alpha3) && Input.GetKeyDown(KeyCode.Alpha4))
            {
                OperateWarnning.Instance.ShowWarnningPanel("添加了监测增加或者减少屏的操作!");
            }
            F3Device.DeviceManager.Instance.Refresh();
            screenNum = F3Device.DeviceManager.Instance.AllMonitors.Count;
            //screenNum = Projection.GetScreenMode.GetSreenNum();

            //一开始有两屏幕，后面段了一个屏幕
            //一开始有一个屏幕，后面加了一个屏幕
            if (screenNum < 2)
            {
                if (lastScreenCount != 1)
                {
                    lastScreenCount = 1;
                    CloseProjection();
                    canProjection = false;
                }
            }
            else
            {
                //因为初始化为-1
                if (lastScreenCount <= 1)
                {
                    canProjection = true;
                    lastScreenCount = 2;
                    EnableScreenMode(canProjection && isScreen);
                }
            }
        }

        public static void Switch2DSelf()
        {
            var device = F3Device.DeviceManager.Instance.FindHidDevice(mainWinPtr);
            if(device != null && device is F3Device.IGraph3Device)
            {
                Monitor23DMode.instance?.SetCamera23DState(false);
                (device as F3Device.IGraph3Device).Switch_2D();
            }
        }

        public static void Switch3DSelf()
        {
            //获取Unity编辑器窗口句柄
            //mainWinPtr = F3Device.API.GetProcessWnd();
            var device = F3Device.DeviceManager.Instance.FindHidDevice(mainWinPtr);
            if (device != null && device is F3Device.IGraph3Device)
            {
                Monitor23DMode.instance?.SetCamera23DState(true);
                (device as F3Device.IGraph3Device).Switch_LR_3D();
            }
        }

        bool hasCameraFlag = true;

        /// <summary>
        /// 判断是否有外接相机
        /// 如果存在，则允许AR投屏
        /// 没有则AR投屏disable
        /// </summary>
        public void JudgeHasExternalCamera()
        {
            bool hasExternalCamera = false;
            bool canUseARFunc = liu.GlobalConfig.canUseCameraAR;
            var devices = WebCamTexture.devices;
            //UnityEngine.Debug.Log("devices.Length " + devices.Length);
            if (devices.Length > 0)
            {
                string _deviceName = "";
                foreach (var item in devices)
                {
                    if (item.name.EndsWith("C920"))
                    {
                        _deviceName = item.name;
                        hasExternalCamera = true;
                        break;
                    }
                }
            }


            if (!hasExternalCamera || !canUseARFunc)//没有外接相机
            {
                //ScreenManger.Instance.SetWebCameraIsCanntUse();
                if (isScreen) //如果投屏，则需要判断现在是否点选了AR投屏
                {
                    if (screenmode == ScreenMode.AR) //把VRToggle设为true，aRToggle.enable 设为false，字体和图片颜色都设置为灰色
                    {
                        //不允许开启AR投屏，转换成开启2D投屏
                        twoDVRToggle.isOn = true;
                    }
                    aRToggle.interactable = false;
                    ARBackground.color = ARCheckmark.color = ARText.color = new Color32(140, 140, 140, 255);
                }
            }
            else
            {
                if (isScreen)
                {
                    if (hasExternalCamera == hasCameraFlag)
                        return;

                    aRToggle.interactable = true;
                    ARBackground.color = ARCheckmark.color = ARText.color = Color.white;
                }
            }
            hasCameraFlag = hasExternalCamera == canUseARFunc ? canUseARFunc : false;

            if (isScreen)
            {
                if (!canUseVR3D)
                {
                    threeDVRToggle.interactable = false;
                    ThreeDVRBackground.color = ThreeDVRCheckmark.color = ThreeDVRText.color = new Color32(140, 140, 140, 255);
                }
            }
        }


        private void SetRecordVidoPath()
        {
            var pathText = transform.Find("RecordPath/PathText").GetComponent<Text>();
            RecordPath = PlayerPrefs.GetString("RecordPath", RecordPath); ;
            if (string.IsNullOrEmpty(RecordPath))
            {
                RecordPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                PlayerPrefs.SetString("RecordPath", RecordPath);
            }
            pathText.text = RecordPath;
        }


        GameObject fileBrowserObj;
        /// <summary>
        /// 设置文件存储路径
        /// </summary>
        void SetSavePath()
        {
            //UIEntity.ShowUI("SimpleFileBrowser", (e, ui) =>
            // {
            //     ui.gameObject.SetActive(false);
            //     FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");
            //     FileBrowser.AddQuickLink("Users", "C:\\Users", null);
            //     StartCoroutine(ShowLoadDialogCoroutine());
            //     return ui;
            // });
            if (null == fileBrowserObj)
            {
                fileBrowserObj = Instantiate(Resources.Load<GameObject>("SimpleFileBrowser/SimpleFileBrowser"));
                fileBrowserObj.transform.parent = transform.parent;
                fileBrowserObj.transform.localPosition = Vector3.zero;
                fileBrowserObj.transform.localScale = Vector3.one;
                //fileBrowserScript = fileBrowserObj.GetComponent<FileBrowser>();
            }
            fileBrowserObj.SetActive(false);
            FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");
            FileBrowser.AddQuickLink("Users", "C:\\Users", null);
            StartCoroutine(ShowLoadDialogCoroutine());
        }

        IEnumerator ShowLoadDialogCoroutine()
        {
            yield return FileBrowser.WaitForSaveDialog(true, "C:\\Users", "录屏保存", "保存");
            var pathText = transform.Find("RecordPath/PathText").GetComponent<Text>();
            Common.AppLog.AddMsg(Common.LogLevel.DEBUG,"UISetting.ShowLoadDialogCoroutine()" + FileBrowser.Success + " " + FileBrowser.Result);
            if (FileBrowser.Success)
            {
                pathText.text = FileBrowser.Result;
                RecordPath = FileBrowser.Result;
                PlayerPrefs.SetString("RecordPath", RecordPath);
            }
        }

        /// <summary>
        /// 保存配置到本地
        /// </summary>
        public static void SaveMode()
        {
#if UNITY_EDITOR
            string config3DPath = "./Config3D.ini";

#else
            string config3DPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config/Config3D.ini").Replace("\\", "/");
#endif
        }


        void ShowSettingUI()
        {
            thisPanelShowState(true);
        }

        void CloseSettingUI()
        {
            thisPanelShowState(false);
        }

        private void OnApplicationQuit()
        {
            if (projectionState == 1)
            {
                Common.ScreenHelper.SetProjection(Common.ScreenHelper.SDC_TOPOLOGY_CLONE);
            }
        }

        //增加快捷方式

        float time = 0.66f;
        /// <summary>
        /// 快捷键切换VR2D
        /// </summary>
        public IEnumerator ShortcutKeySetVR2D()
        {
            Common.AppLog.AddMsg(Common.LogLevel.DEBUG,"UISetting.ShortcutKeySetVR2D(): VR2D快捷键");
            Projection2DVR();
            yield return new WaitForSeconds(time);
            SyncProjectionState();
        }

        /// <summary>
        /// 快捷键切换VR3D
        /// </summary>
        public IEnumerator ShortcutKeySetVR3D()
        {
            if (canUseVR3D)
            {
                Common.AppLog.AddMsg(Common.LogLevel.DEBUG, "UISetting.ShortcutKeySetVR3D(): VR3D快捷键");
                Projection3DVR();
                yield return new WaitForSeconds(time);
                SyncProjectionState();
            }
        }

        /// <summary>
        /// 快捷键切换AR2D
        /// </summary>
        public IEnumerator ShortcutKeySetAR()
        {
            Common.AppLog.AddMsg(Common.LogLevel.DEBUG,"UISetting.ShortcutKeySetAR2D(): AR快捷键");
            yield return new WaitForSeconds(time);
            ProjectionAR();
            SyncProjectionState();
        }

        /// <summary>
        /// 快捷键关闭投屏
        /// </summary>
        public IEnumerator ShortcutKeySetNone()
        {
            Common.AppLog.AddMsg(Common.LogLevel.DEBUG,"UISetting.ShortcutKeySetAR3D(): 关闭投屏快捷键");
            yield return new WaitForSeconds(time);
            ProjectionNone();
            SyncProjectionState();
        }

        /// <summary>
        /// 2D投屏模式
        /// </summary>
        void Projection2DVR()
        {
            if (!twoDVRToggle.isOn)
            {
                twoDVRToggle.isOn = true;
            }
        }

        /// <summary>
        /// 3D投屏模式
        /// </summary>
        void Projection3DVR()
        {
            if (!threeDVRToggle.isOn)
            {
                threeDVRToggle.isOn = true;
            }
        }

        /// <summary>
        /// VR投屏模式
        /// </summary>
        void ProjectionNone()
        {
            if (!noneScreenToggle.isOn)
            {
                noneScreenToggle.isOn = true;
            }
        }

        /// <summary>
        /// AR投屏模式
        /// </summary>
        void ProjectionAR()
        {
            if (!aRToggle.isOn)
            {
                aRToggle.isOn = true;
            }
        }

        /// <summary>
        /// 同步投屏的状态
        /// </summary>
        void SyncProjectionState()
        {
            lastIsNoneProjection = isNoneProjection;
            lastScreenDimensional = screenDimensional;
            lastScreenmode = screenmode;
        }

#region 画质设置
        public static void LevelLow()
        {
            QualitySettings.SetQualityLevel(QualitySets.Low);
            Common.AppLog.AddMsg(Common.LogLevel.INFO,"UISetting.LevelLow(): low");
        }
        public static void LevelMiddle()
        {
            QualitySettings.SetQualityLevel(QualitySets.Middle);
            Common.AppLog.AddMsg(Common.LogLevel.INFO,"UISetting.LevelMiddle(): Middle");
        }
        public static void LevelHigh()
        {
            QualitySettings.SetQualityLevel(QualitySets.High);
            Common.AppLog.AddMsg(Common.LogLevel.INFO,"UISetting.LevelMiddle(): High");
        }
#endregion
    }

    /// <summary>
    /// 画质等级定义
    /// </summary>
    public struct QualitySets
    {
        public static int Low
        {
            get { return 0; }
        }


        public static int Middle
        {
            get { return 3; }
        }


        public static int High
        {
            get { return 5; }
        }
    }
}