using huang.common.recordscreentool;
using huang.common.screen;
using huang.module.ui.settingui;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace liu
{

    /// <summary>
    /// 录屏控制
    /// </summary>
    public class RecordOperate : MonoBehaviour
    {
        /// <summary>
        /// 桌面的路径
        /// </summary>
        string deskPath;

        /// <summary>
        /// 记录录屏开始时间的UI
        /// </summary>
        RecEvent RecEvent;

        Rec rec;

        private void Start()
        {
            deskPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            RecEvent = FindObjectOfType<RecEvent>();
            rec = FindObjectOfType<Rec>();
        }

        private void OnApplicationQuit()
        {
            StopRec();
        }

        ScreenControlObj screenControlObj
        {
            get { return FindObjectOfType<ScreenControlObj>(); }
        }

        public bool isKeyCore_LeftAltPress = false;
        public bool isKeyCore_SPress = false;
        [HideInInspector]
        public bool startFlag = false;
        void Update()
        {

        }

        /// <summary>
        /// 开始录屏
        /// </summary>
        public bool StartRec()
        {
            if (!RecVideoState.excuteChangeVideoAndCombineMP4)
            {
                string ffmpegPath = Application.streamingAssetsPath + "/ffmpeg.exe";
                string closeprocessPath = Application.streamingAssetsPath + "/sendsignal.exe";
                if (File.Exists(ffmpegPath) && File.Exists(closeprocessPath))
                {
                    //GetScreenMode.GetProjectionMode();    //获取投屏状态
                    int screenCount = GetScreenMode.GetSreenNum();  //获取屏幕个数
                    if (screenControlObj != null)
                    {
                        if (screenControlObj.curMode != ScreenManger.DualScreenMode.None && screenCount > 1)//开了投屏就开始录，否则不录
                        {
                            //RecUI.SetActive(true);
                            RecEvent.ShowRecUI(true);
                            //deskPath = UISetting.RecordPath;
                            rec.recordSavePath = UISetting.RecordPath;
                            if (!startFlag) //说明是重新打开录屏，所以需要清楚数据
                            {
                                rec.ClearData();
                            }

                            if (UISetting.screenDimensional != UISetting.ScreenDimensional.ThreeDimensional)
                            {
                                StartCoroutine(rec.startRec1920());
                            }
                            else
                            {
                                StartCoroutine(rec.startRec960());
                            }

                            //StartCoroutine(DelayStartRec(ffmpegPath, deskPath, closeprocessPath, 0.5f));
                            //RecordScreenTool.Instance.StartRecordScreen(ffmpegPath, deskPath, closeprocessPath);
                            //screenControlObj.OpenRecWin();
                            startFlag = true;

                            Debug.Log("startFlag = true");
                            return true;
                        }
                        else
                        {
                            screenControlObj.curMode = ScreenManger.DualScreenMode.None;
                            OperateWarnning.Instance.ShowWarnningPanel("只有开启投屏才能录屏！");
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    Debug.LogError("RecordScreenTool.Update()    程序ffmpeg.exe不在  " + ffmpegPath + "  路径下   或者 closeprocess.exe程序不在 + " + closeprocessPath + "   路径下");
                    return false;
                }
            }
            else
            {
                OperateWarnning.Instance.ShowWarnningPanel("录制的视频正在输出中，请稍后再开启录屏操作。谢谢！");
                return false;
            }
        }

        IEnumerator DelayStartRec(string ffmpegPath, string deskPath, string closeprocessPath, float time)
        {
            yield return new WaitForSeconds(time);
            RecordScreenTool.Instance.StartRecordScreen(ffmpegPath, deskPath, closeprocessPath);
        }
        public void StopRec()
        {
            if (startFlag)
            {
                //RecordScreenTool.Instance.StopRecoreScreen();
                rec.RecStop();
                RecEvent.ShowRecUI(false);
                startFlag = false;
            }
        }
    }
}
