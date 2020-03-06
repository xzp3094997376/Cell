using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.Text;
/// <summary>
/// f3d日志记录
/// </summary>
public class F3DDebug
{
    /// <summary>
    /// 获取F3D当前文件名、执行函数、执行代码行数和列数
    /// </summary>
    /// <param name="str"></param>
    public static void Log(string str, StackTrace st)
    {
        //StackTrace st = new StackTrace(new StackFrame(true));
        StringBuilder sb = new StringBuilder();
        sb.Append(string.Format(" Stack trace for current level: {0}", st.ToString()));
        StackFrame sf = st.GetFrame(0);
        sb.Append(string.Format("\n 【File: 】：{0}", sf.GetFileName()));
        sb.Append(string.Format("\n 【 Method: 】：{0}", sf.GetMethod().Name));
        sb.Append(string.Format("\n 【Line Number: 】：{0}", sf.GetFileLineNumber()));
        sb.Append(string.Format("\n 【Column Number: 】：{0}", sf.GetFileColumnNumber()));
        sb.Append(string.Format(" \n {0}", str));
        //UnityEngine.Debug.Log(sb.ToString());
        Common.AppLog.AddMsg(Common.LogLevel.DEBUG,sb.ToString());
    }

}

