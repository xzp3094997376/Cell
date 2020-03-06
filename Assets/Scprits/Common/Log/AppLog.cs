using huang.common.paths;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;


namespace Common
{
    public enum LogLevel
    {
        DEBUG,
        INFO,
        WARN,
        ERROR,
        FATAL,
    }
    public class AppLog
    {
        class LogData
        {
            public LogLevel m_level = LogLevel.INFO;
            public string m_message = string.Empty;
            public string m_fileName = "Log";
            public LogData(LogLevel level, string message, string fileName)
            {
                m_level = level;
                m_message = message;
                m_fileName = fileName;
            }

        }
        class LogDataList : List<LogData>
        {
            public void Write(string path, DateTime time)
            {
                try
                {
                    using (FileStream stream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                    {
                        foreach (LogData data in this)
                        {
                            string text = time.ToString() + "  " + $"[{data.m_level.ToString()}]  " + data.m_message + System.Environment.NewLine;
                            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(text);
                            stream.Write(buffer, 0, buffer.Length);
                            stream.Flush();
                        }
                    }
                }
                catch
                {
                }
            }
        }
        internal const string LogDirectory = "C:\\F3DFuture\\AppLog\\";

        private static AppLog gAppLog = null;
        public static AppLog Instance { get { if (gAppLog == null) gAppLog = new AppLog(); return gAppLog; } }
        public static LogLevel Level = LogLevel.INFO;
        public static bool IsHourSplit = true;
        private bool m_isClosed = false;
        public static int MaxCacheCount = 1000;                                                             //日志最大缓存数量
        private LogDataList outCacheList = new LogDataList();
        private Dictionary<string, LogDataList> outCacheDic = new Dictionary<string, LogDataList>();        //文件名称和日志列表键值对
        private static string ProductName = "NoProductName";

        private AppLog()
        {
            string productName = UnityEngine.Application.productName;
            if (!string.IsNullOrEmpty(productName))
                ProductName = productName;

            Clear();
            Task.Factory.StartNew(Upate);
            UnityEngine.Application.quitting += Application_quitting;
        }

        private void Application_quitting()
        {
            m_isClosed = true;
        }

        private void Upate()
        {
            while (!m_isClosed)
            {
                //载入数据
                outCacheDic.Clear();
                lock (outCacheList)
                {
                    foreach (LogData data in outCacheList)
                    {
                        if (outCacheDic.ContainsKey(data.m_fileName))
                            outCacheDic[data.m_fileName].Add(data);
                        else
                        {
                            LogDataList list = new LogDataList();
                            list.Add(data);
                            outCacheDic.Add(data.m_fileName, list);
                        }
                    }
                    outCacheList.Clear();
                }
                if (outCacheDic.Count > 0)
                {
                    DateTime time = DateTime.Now;
                    foreach (var item in outCacheDic)
                    {
                        string fileName = item.Key;
                        LogDataList list = item.Value;
                        string path = GetFilePath(time, fileName);
                        list.Write(path, time);
                    }
                }
                System.Threading.Thread.Sleep(100);
            }
        }

        private static string GetFilePath(DateTime time, string fileName)
        {
            string dir = LogDirectory + ProductName + $"\\{time.Year}-{time.Month}-{time.Day}";

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if (IsHourSplit)
                return dir + $"\\{fileName}_" + time.Hour + ".txt";
            else
                return dir + $"\\{fileName}.txt";
        }

        private void Add(LogData data)
        {
            if (m_isClosed) return;
            lock (outCacheList)
            {
                if (outCacheList.Count >= MaxCacheCount)
                    return;
                outCacheList.Add(data);
            }
        }
        /// <summary>
        /// 输出日志 位置在C:\F3DFuture\AppLog\ + productName
        /// 尽量使用AddFormat 填写targetSite
        /// </summary>
        /// <param name="level">级别(默认不输出Debug 需要设置Level全局变量)</param>
        /// <param name="message">异常</param>
        /// <param name="fileName">日志文件名（不能带后缀）</param>
        public static void AddMsg(LogLevel level, string message, string fileName = "Log")
        {
            AddFormat(level, "", message, fileName);
        }
        /// <summary>
        /// 输出日志 位置在C:\F3DFuture\AppLog\ + productName
        /// 尽量使用AddFormat 填写targetSite
        /// </summary>
        /// <param name="level">级别(默认不输出Debug 需要设置Level全局变量)</param>
        /// <param name="exp">异常</param>
        /// <param name="fileName">日志文件名（不能带后缀）</param>
        public static void AddMsg(LogLevel level, Exception exp, string fileName = "Log")
        {
            AddFormat(level, "", exp, fileName);
        }
        /// <summary>
        /// 输出日志 位置在C:\F3DFuture\AppLog\ + productName
        /// </summary>
        /// <param name="level">级别(默认不输出Debug 需要设置Level全局变量)</param>
        /// <param name="targetSite">函数类+名称</param>
        /// <param name="exp">异常</param>
        /// <param name="fileName">日志文件名（不能带后缀）</param>
        public static void AddFormat(LogLevel level, string targetSite, Exception exp, string fileName = "Log")
        {
            AddFormat(level, targetSite, exp.Message, fileName);
        }
        /// <summary>
        /// 输出日志 位置在C:\F3DFuture\AppLog\ + productName
        /// </summary>
        /// <param name="level">级别(默认不输出Debug 需要设置Level全局变量)</param>
        /// <param name="targetSite">函数类+名称</param>
        /// <param name="message">详细信息</param>
        /// <param name="fileName">日志文件名（不能带后缀）</param>
        public static void AddFormat(LogLevel level, string targetSite, string message, string fileName = "Log")
        {
#if UNITY_EDITOR
            AppLog.Level = LogLevel.DEBUG;
#endif
            if (level < Level) return;
            if (!string.IsNullOrEmpty(targetSite))
                targetSite = "TargetSite:" + targetSite + "  ";
            Instance.Add(new LogData(level, targetSite + message, fileName));
#if UNITY_EDITOR
            if (level >= LogLevel.ERROR)
                UnityEngine.Debug.LogError($"[{level.ToString()}]  " + targetSite + message);
            else if(level == LogLevel.WARN)
                UnityEngine.Debug.LogWarning($"[{level.ToString()}]  " + targetSite + message);
            else
                UnityEngine.Debug.Log($"[{level.ToString()}]  " + targetSite + message);
#endif
        }

        private static void Clear(int beforeDay = 7)
        {
            string productName = UnityEngine.Application.productName;
            if (string.IsNullOrEmpty(productName))
                productName = "NoProductName";

            string path = LogDirectory + productName;
            try
            {
                if (!System.IO.Directory.Exists(path)) return;
                DateTime now = System.DateTime.Now;
                List<string> deletes = new List<string>();
                DirectoryInfo dinfo = new DirectoryInfo(path);
                foreach (DirectoryInfo temp in dinfo.GetDirectories())
                {
                    try
                    {
                        string name = temp.Name;
                        string[] arr = name.Split('-');
                        if (arr.Length != 3) continue;
                        DateTime dt = new DateTime(int.Parse(arr[0]), int.Parse(arr[1]), int.Parse(arr[2]));
                        TimeSpan ts = now - dt;
                        if (ts.Days > beforeDay)
                            deletes.Add(temp.FullName);
                    }
                    catch
                    {
                        deletes.Add(temp.FullName);
                    }
                }
                foreach (string temp in deletes)
                    System.IO.Directory.Delete(temp, true);
            }
            catch
            {

            }
        }
    }
}
