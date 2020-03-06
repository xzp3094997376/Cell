using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using UnityEngine;

public static class RecVideoState
{
    /// <summary>
    /// 是否开始执行录屏操作
    /// </summary>
    public static bool excuteChangeVideoAndCombineMP4 = false;
}

public struct RecInfo
{
    /// <summary>
    /// 录制视频的宽度尺寸
    /// </summary>
    public string videoWidthSize;
    /// <summary>
    /// 保存在本地是视频的文件路径，包含文件名
    /// </summary>
    public string videoFileName;
}
public class RecVideo : MonoBehaviour
{
    /// <summary>
    /// 开启录屏这进程
    /// </summary>
    private Process curProcess;

    /// <summary>
    /// 需要删除的视频文件路径（带文件名称及格式）
    /// </summary>
    public List<string> needDeleteFiles = new List<string>();

    /// <summary>
    /// 保存录制视频文件的信息
    /// int - 存储视频的Index顺序
    /// RecInfo - 视频里的信息，宽度和名字
    /// </summary>
    //[HideInInspector]
    //public Dictionary<int, RecInfo> recFilesDic = new Dictionary<int, RecInfo>();

    //[HideInInspector]
    //public int recIndex = 0;

    /// <summary>
    /// 录制1920分辨率的屏
    /// </summary>
    public void StartRecordScreen(string ffmpegPath, string recordSavePath, string closeProcessPath)
    {
        if (curProcess != null)
        {
            if (!curProcess.HasExited)
            {
                ExitRecord(closeProcessPath);
            }
        }

        try
        {
            curProcess = new Process();

            ProcessStartInfo info = new ProcessStartInfo
            {
                FileName = ffmpegPath,
                CreateNoWindow = true,

                WindowStyle = ProcessWindowStyle.Hidden
            };

            string tempName = DateTime.Now.GetHashCode().ToString();
            string fileName = recordSavePath + "\\" + tempName + ".mp4";

            needDeleteFiles.Add(fileName);

            //++recIndex;
            //RecInfo recInfo = new RecInfo
            //{
            //    videoWidthSize = "1920",
            //    videoFileName = fileName
            //};

            //recFilesDic.Add(recIndex, recInfo);

            //string path1 = @"-hwaccel auto -y -f gdigrab -framerate 20 -offset_x 1920 -offset_y 0 -video_size 1920x1080 -i desktop -b:v 1024k -bufsize 1024k -preset ultrafast -pix_fmt yuv420p -an " + @"F:\新建文件夹\1024.mp4";
            string path1 = @"-hwaccel auto -y -f gdigrab -framerate 20 -offset_x 1920 -offset_y 0 -video_size 1920x1080 -i desktop -b:v 4092k -bufsize 4092k -preset ultrafast -pix_fmt yuv420p -an " + fileName;

            info.Arguments = path1;

            curProcess.StartInfo = info;

            curProcess.Start();
            UnityEngine.Debug.Log("StartRecordScreen 打开ffmepg ffmpegPath " + ffmpegPath);
            UnityEngine.Debug.Log("StartRecordScreen 打开ffmepg info.Arguments path1 " + path1);
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.Log("RecordScreen.StartRecordScreen(): +" + "，打开失败，检查路径" + ffmpegPath + "  存储路径 " + recordSavePath + "  异常：" + e.ToString());
        }
    }

    /// <summary>
    /// 录制960分辨率的屏
    /// </summary>
    public void StartRecordHalfScreen(string ffmpegPath, string recordSavePath, string closeProcessPath)
    {
        if (curProcess != null)
        {
            if (!curProcess.HasExited)
            {
                ExitRecord(closeProcessPath);
            }
        }

        try
        {
            curProcess = new Process();

            ProcessStartInfo info = new ProcessStartInfo
            {
                FileName = ffmpegPath,
                CreateNoWindow = true,

                WindowStyle = ProcessWindowStyle.Hidden
            };

            string tempName = DateTime.Now.GetHashCode().ToString();
            string fileName = recordSavePath + "\\" + tempName + ".mp4";

            needDeleteFiles.Add(fileName);

            //++recIndex;
            //RecInfo recInfo = new RecInfo
            //{
            //    videoWidthSize = "960",
            //    videoFileName = fileName
            //};

            //recFilesDic.Add(recIndex, recInfo);

            //string path1 = @"-hwaccel auto -y -f gdigrab -framerate 20 -offset_x 1920 -offset_y 0 -video_size " + recScreenArea + /*1920x1080 */" -i desktop - b:v 1024k -bufsize 1024k -preset ultrafast -pix_fmt yuv420p -an " + storageArea;-vf scale=1920:1080,setdar=16:9 
            string path1 = @"-hwaccel auto -y -f gdigrab -framerate 20 -offset_x 1920 -offset_y 0 -video_size 960x1080 -i desktop -b:v 4092k -bufsize 4092k -preset ultrafast -pix_fmt yuv420p -vf scale=1920*1080 -an " + fileName;

            info.Arguments = path1;

            curProcess.StartInfo = info;

            curProcess.Start();

            UnityEngine.Debug.Log("StartRecordScreen 打开ffmepg ffmpegPath " + ffmpegPath);
            UnityEngine.Debug.Log("StartRecordScreen 打开ffmepg info.Arguments path1 " + path1);
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.Log("RecordScreen.StartRecordScreen(): +" + "，打开失败，检查路径" + ffmpegPath + "  存储路径 " + recordSavePath + "  异常：" + e.ToString());
        }
    }

    /// <summary>
    /// 修改视频的分辨率
    /// </summary>
    /// <param name="ffmpegPath"></param>
    /// <param name="index"></param>
    /// <param name="recordSavePath"></param>
    //public void ChangeVideoResolution(string ffmpegPath, int index, string recordSavePath)
    //{
    //    try
    //    {
    //        curProcess = new Process();

    //        ProcessStartInfo info = new ProcessStartInfo
    //        {
    //            FileName = ffmpegPath,
    //            CreateNoWindow = true,

    //            WindowStyle = ProcessWindowStyle.Hidden
    //        };
    //        //,setdar = 16:9
    //        //string path2 = @" -i " + recordSavePath + "\\" + "2.mp4" + " -vf scale=1920:1080 " /*+ " -y "*/ + recordSavePath + "\\" + "resolution.mp4";
    //        string fileName = recFilesDic[index].videoFileName;

    //        needDeleteFiles.Add(fileName);

    //        string tempName = DateTime.Now.GetHashCode().ToString();
    //        string fileName2 = recordSavePath + "\\" + tempName + ".mp4";

    //        needDeleteFiles.Add(fileName2);

    //        recFilesDic[index] = new RecInfo
    //        {
    //            videoWidthSize = "1920",
    //            videoFileName = fileName2
    //        };

    //        string path2 = @" -i " + fileName + " -vf scale=1920:1080 " + " -y " + fileName2;

    //        info.Arguments = path2;
    //        curProcess.StartInfo = info;

    //        UnityEngine.Debug.Log("RecVideo.ChangeVideoResolution(): 打开ffmepg path2 " + path2);
    //        curProcess.Start();

    //        curProcess.WaitForExit();//阻塞等待进程结束
    //        curProcess.Close();
    //        curProcess.Dispose();

    //        UnityEngine.Debug.Log("结束合并！！");
    //    }
    //    catch (System.Exception e)
    //    {
    //        //UnityEngine.Debug.Log("RecVideo.ChangeVideoResolution(): +" + "，打开失败，检查路径" + ffmpegPath + "  文件路径 " + fileName + "  异常：" + e.ToString());
    //    }
    //}


    /// <summary>
    /// 合并合成视频
    /// </summary>
    /// <param name="ffmpegPath">ffmpeg文件的位置</param>
    /// <param name="StrOutMp4Path">输出文件的路径</param>
    void CombineMp4WithoutTxt(string ffmpegPath, string StrOutMp4Path)
    {
        Process p = new Process();//建立外部调用线程
        p.StartInfo.FileName = ffmpegPath;//要调用外部程序的绝对路径

        string tempName = DateTime.Now.GetHashCode().ToString();
        string commond = "";

        //for (int i = 0; i < recFilesDic.Count; i++)
        //{
        //    commond += " -i " + recFilesDic[i + 1].videoFileName;
        //}
        string filePath = StrOutMp4Path + "\\out.txt";
        File.WriteAllText(filePath, string.Empty);
        FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate);
        StreamWriter sw = new StreamWriter(fs);
        
        //开始写入
        for (int i = 0; i < needDeleteFiles.Count; i++)
        {
            sw.Write("file '" + needDeleteFiles[i] + "'");
            sw.WriteLine();
            //commond += " -i " + needDeleteFiles[i];
        }

        //清空缓冲区
        sw.Flush();
        //关闭流
        sw.Close();
        fs.Close();

        //string StrArg = "-i " + videoPaths[0] + " -i " + videoPaths[1] + " -filter_complex concat=n=2 " + " -y " + StrOutMp4Path + "\\" + tempName +".mp4";
        string StrArg = "-f concat -safe 0 -i " + filePath + " -c copy " + StrOutMp4Path + "\\" + tempName + ".mp4";
        //string StrArg = commond + " -filter_complex concat=n=" + needDeleteFiles.Count + " " + " -y " + StrOutMp4Path + "\\" + tempName + ".mp4";
        p.StartInfo.Arguments = StrArg;

        UnityEngine.Debug.Log("p.StartInfo.Arguments   " + StrArg);

        p.StartInfo.UseShellExecute = false;//不使用操作系统外壳程序启动线程(一定为FALSE,详细的请看MSDN)
        p.StartInfo.RedirectStandardError = true;//把外部程序错误输出写到StandardError流中(这个一定要注意,FFMPEG的所有输出信息,都为错误输出流,用StandardOutput是捕获不到任何消息的...这是我耗费了2个多月得出来的经验...mencoder就是用standardOutput来捕获的)
        p.StartInfo.CreateNoWindow = true;//不创建进程窗口
        /*p.ErrorDataReceived += new DataReceivedEventHandler(Output);*///外部程序(这里是FFMPEG)输出流时候产生的事件,这里是把流的处理过程转移到下面的方法中,详细请查阅MSDN
        p.Start();//启动线程
        p.BeginErrorReadLine();//开始异步读取
        p.WaitForExit();//阻塞等待进程结束

        p.Close();//关闭进程
        p.Dispose();//释放资源
        RecVideoState.excuteChangeVideoAndCombineMP4 = false;
        //合并视频完成后对多余的视频进行删除
        RemoveVideosFile();
    }

    /// <summary>
    /// 移除多余的视频文件
    /// </summary>
    void RemoveVideosFile()
    {
        for (int i = 0; i < needDeleteFiles.Count; i++)
        {
            File.Delete(needDeleteFiles[i]);
        }
        curProcess.Close();
        curProcess.Dispose();
        curProcess = null;
    }

    /// <summary>
    /// 结束录制
    /// </summary>
    /// <param name="closeProcessPath"></param>
    public void RecEnd(string ffmpegPath, string closeProcessPath, string recordSavePath)
    {
        //先停止ffmpeg.exe的运行
        ExitRecord(closeProcessPath);
        _ffmpegFilePath = ffmpegPath;
        _recordPath = recordSavePath;
        Thread thread = new Thread(AAA);
        thread.Start();
        //AAA();
        //然后判断录制保存的视频是否大于1，大于1的时候才需要视频的合并操作
        //if (recFilesDic.Count > 1)
        //{
        //    //把960的分辨率的视频修改为1920的分辨率

        //    for (int i = 0; i < recFilesDic.Count; i++)
        //    {
        //        if (recFilesDic[i + 1].videoWidthSize == "960")
        //        {
        //            ChangeVideoResolution(ffmpegPath, i + 1, recordSavePath);
        //        }

        //        UnityEngine.Debug.Log("for index " + i);
        //    }

        //    //转换完之后，对视频进行合并
        //    CombineMp4WithoutTxt(ffmpegPath, recordSavePath);
        //}

    }
    string _ffmpegFilePath, _recordPath;
    void AAA()
    {
        if (needDeleteFiles.Count > 1)
        {
            RecVideoState.excuteChangeVideoAndCombineMP4 = true;
            //把960的分辨率的视频修改为1920的分辨率

            //for (int i = 0; i < recFilesDic.Count; i++)
            //{
            //    if (recFilesDic[i + 1].videoWidthSize == "960")
            //    {
            //        ChangeVideoResolution(a, i + 1, b);
            //    }

            //    UnityEngine.Debug.Log("for index " + i);
            //}

            //转换完之后，对视频进行合并
            CombineMp4WithoutTxt(_ffmpegFilePath, _recordPath);
        }
        //CombineMp4WithoutTxt(a, b);
    }

    /// <summary>
    /// 停止录屏
    /// </summary>
    public void ExitRecord(string closeProcessPath)
    {
        if (curProcess != null)
        {
            Process process = new Process();

            ProcessStartInfo info = new ProcessStartInfo
            {
                FileName = closeProcessPath,
                CreateNoWindow = true,

                WindowStyle = ProcessWindowStyle.Hidden,

                Arguments = curProcess.Id.ToString()
            };

            process.StartInfo = info;

            process.Start();
            process.WaitForExit();

            UnityEngine.Debug.Log("RecordScreen.ExitRecord(): 启动关闭ffmpeg ");
        }
    }
}
