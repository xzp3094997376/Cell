using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace xuexue.f3dModel
{
    /// <summary>
    /// 实现了IFileReader接口的通用的读取csv文件方法。
    /// 一些特点如下:
    /// 1. 要求csv文件中以CRLF分割每行
    /// 2. 所有Read方法都会自动提升文件流的位置
    /// </summary>
    public class CSVFile : IFileReader
    {
        /// <summary>
        /// 存储文体的流对象
        /// </summary>
        private FileStream fs;

        /// <summary>
        /// 读取文件的流对象
        /// </summary>
        private StreamReader sr;

        /// <summary>
        /// 文件的标头数据
        /// </summary>
        private List<string> header = new List<string>();

        /// <summary>
        /// 一次读取文件的buffer
        /// </summary>
        private char[] readBuff = new char[128];

        /// <summary>
        /// 拼接字符串
        /// </summary>
        private StringBuilder textSB = new StringBuilder();

        /// <summary>
        /// 打开文件，并创建流对象进行对文件的缓存
        /// </summary>
        /// <param name="filePath"></param>
        public void OpenFile(string filePath)
        {
            try
            {
                //读取文件
                FileInfo fileInfo = new FileInfo(filePath);
                if (!fileInfo.Exists)//判断文件是否存在
                {
                    Console.WriteLine("CSVFile.OpenFile():文件不存在 " + fileInfo.FullName);
                    return;
                }

                fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                sr = new StreamReader(fs, Encoding.UTF8);
            }
            catch (Exception e)
            {
                Close();//异常就关闭
                Console.WriteLine("CSVFile.OpenFile():异常 " + e.Message);
            }
        }

        /// <summary>
        /// 读列表头，也就是第一行
        /// </summary>
        /// <returns></returns>
        public string[] ReadHeaderLine()
        {
            if (sr == null)
            {
                return null;
            }
            //把读取文件的流对象的位置归零，确保读取从开头开始读取
            sr.BaseStream.Position = 0;
            header.Clear();

            while (true)
            {
                //读取指定字符
                int result = sr.ReadBlock(readBuff, 0, readBuff.Length);
                if (result == 0)//如果已经读到文件末尾
                {
                    return null;//读到末尾也没找到换行
                }

                textSB.Append(readBuff, 0, result);

                string curText = textSB.ToString();
                //匹配是否读取到 换行
                Match m = Regex.Match(curText, @"\r\n");
                if (m.Success)
                {
                    //只读第一行然后返回
                    string headerLine = curText.Substring(0, m.Index);
                    textSB.Remove(0, m.Index + m.Length);//从sb中移出已经确定的项

                    string[] titles = headerLine.Replace("\"", "").Replace("\n", "").Replace("\r", "").Split(',');
                    header.AddRange(titles);//自己顺便记录一份
                    return titles;
                }
            }
        }

        /// <summary>
        /// 依次读每行，如果已经读到文件末尾就返回null
        /// </summary>
        /// <returns></returns>
        public string[] ReadLine()
        {
            if (sr == null)
            {
                return null;
            }

            while (true)
            {
                int result = sr.ReadBlock(readBuff, 0, readBuff.Length);
                if (result == 0)//如果已经读到文件末尾
                {
                    return null;//读到末尾也没找到换行
                }

                textSB.Append(readBuff, 0, result);

                string curText = textSB.ToString();
                Match m = Regex.Match(curText, @"\r\n");
                if (m.Success)
                {
                    //只读第一行然后返回
                    string headerLine = curText.Substring(0, m.Index);
                    textSB.Remove(0, m.Index + m.Length);//从sb中移出已经确定的项
                    string[] lines = headerLine.Replace("\"", "").Replace("\n", "").Replace("\r", "").Split(',');

                    return lines;
                }
            }
        }

        /// <summary>
        /// 依次读每行，如果已经读到文件末尾就返回null。
        /// 它的返回值是一个字典，key值为Header，value为这一项的内容。
        /// 如字典包含两个键值对：
        /// {学科,物理}
        /// {一级分类,力学}
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> ReadLine2()
        {
            if (sr == null)
            {
                return null;
            }

            while (true)
            {
                int result = sr.ReadBlock(readBuff, 0, readBuff.Length);

                if (result != 0)//只要有读到了字符，就append
                    textSB.Append(readBuff, 0, result);

                //（修正下面原来的）如果result = 0就结束，会导致textSB保存的数据没有完全取出。
                //if (result == 0)//如果已经读到文件末尾
                //{
                //    return null;//读到末尾也没找到换行
                //}
                if (textSB.Length == 0)
                {
                    return null;//如果已经把下面的东西全执行了还没返回，这里又读完了整个文件，那么只能返回了
                }

                string curText = textSB.ToString();
                Match m = Regex.Match(curText, @"\r\n");
                if (m.Success)
                {
                    //只读第一行然后返回
                    string headerLine = curText.Substring(0, m.Index);
                    textSB.Remove(0, m.Index + m.Length);//从sb中移出已经确定的项
                    string[] lines = headerLine.Replace("\"", "").Replace("\n", "").Replace("\r", "").Split(',');
                    Dictionary<string, string> headerReflexValueDic = new Dictionary<string, string>();
                    for (int i = 0; i < lines.Length; i++)
                    {
                        //UnityEngine.Debug.Log("CSVFile.ReadLine2(): header[0]" + header[i] + "   i = " + i);
                        if(i >= header.Count) { continue; }
                        if (headerReflexValueDic.ContainsKey(header[i]))
                        {
                            UnityEngine.Debug.Log("CSVFile.ReadLine2():字典中存在相同的Key " + header[i].ToString());
                        }
                        else
                        {
                            headerReflexValueDic.Add(header[i], lines[i]);
                        }
                    }
                    return headerReflexValueDic;
                }
            }
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void Close()
        {
            if (fs != null)
            {
                try { fs.Close(); } catch (Exception) { }//尝试关闭文件
                fs = null;
            }
            sr = null;
        }
    }
}