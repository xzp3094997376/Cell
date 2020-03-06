using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rec : MonoBehaviour {

    /// <summary>
    /// ffmpegPath的路径
    /// </summary>
    private string ffmpegPath;

    
    /// <summary>
    /// 录屏保存的路径
    /// </summary>
    [HideInInspector]
    public string recordSavePath;

    /// <summary>
    /// 关闭录屏的路径
    /// </summary>
    private string closeProcessPath;

    RecVideo recVideo;

    private void Start()
    {
        ffmpegPath = Application.streamingAssetsPath + "/ffmpeg.exe";
        closeProcessPath = Application.streamingAssetsPath + "/sendsignal.exe";
        //recordSavePath = @"F:\新建文件夹";
        recVideo = GetComponent<RecVideo>();
    }

    private void Update()
    {
        //if (Input.GetKeyUp(KeyCode.Alpha1))
        //{
        //    Debug.Log("Update.Alpha1");
        //    StartCoroutine("startRec1920");
        //}

        //if (Input.GetKeyUp(KeyCode.Alpha3))
        //{
        //    Debug.Log("Update.Alpha3");
        //    StartCoroutine("startRec960");
        //}

        //if (Input.GetKeyUp(KeyCode.E))
        //{
        //    Debug.Log("结束录屏");
        //    recVideo.ExitRecord(closeProcessPath);
        //}

        //if (Input.GetKeyUp(KeyCode.R))
        //{
        //    Debug.Log("最后结束整个操作");
        //    recVideo.RecEnd(ffmpegPath,closeProcessPath, recordSavePath);
        //}

        //if (Input.GetKeyUp(KeyCode.C))
        //{
        //    Debug.Log("合并视频");
        //    //recVideo.CombineMp4WithoutTxt(ffmpegPath, recordSavePath);
        //}

        //if (Input.GetKeyUp(KeyCode.M))
        //{
        //    Debug.Log("转换分辨率");
        //    //recVideo.ChangeVideoResolution(ffmpegPath, recordSavePath+"\\2.mp4", recordSavePath);
        //}
    }

    /// <summary>
    /// 初始化清楚数据
    /// </summary>
    public void ClearData()
    {
        //recVideo.recIndex = 0;
        recVideo.needDeleteFiles.Clear();
        //recVideo.recFilesDic.Clear();
    }

    /// <summary>
    /// 开启1920x1080窗口的录制
    /// </summary>
    /// <returns></returns>
    public IEnumerator startRec1920()
    {
        yield return new WaitForSeconds(0.02f);
        recVideo.StartRecordScreen(ffmpegPath, recordSavePath, closeProcessPath);
    }

    /// <summary>
    /// 开启960x1080窗口的录制
    /// </summary>
    /// <returns></returns>
    public IEnumerator startRec960()
    {
        yield return new WaitForSeconds(0.02f);
        recVideo.StartRecordHalfScreen(ffmpegPath, recordSavePath, closeProcessPath);
    }

    /// <summary>
    /// 结束录屏
    /// </summary>
    public void RecStop()
    {
        recVideo.RecEnd(ffmpegPath, closeProcessPath, recordSavePath);
    }
}
