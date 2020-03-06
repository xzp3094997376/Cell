using System;
using System.Collections.Generic;

namespace xuexue.f3dModel
{
    /// <summary>
    /// 读文件方法接口
    /// </summary>
    public interface IFileReader
    {
        /// <summary>
        /// 打开一个文件
        /// </summary>
        /// <param name="filePath"></param>
        void OpenFile(string filePath);

        /// <summary>
        /// 读列表头，也就是第一行
        /// </summary>
        /// <returns></returns>
        string[] ReadHeaderLine();

        /// <summary>
        /// 依次读每行，如果已经读到文件末尾就返回null
        /// </summary>
        /// <returns></returns>
        string[] ReadLine();

        /// <summary>
        /// 依次读每行，如果已经读到文件末尾就返回null。
        /// 它的返回值是一个字典，key值为Header，value为这一项的内容。
        /// 如字典包含两个键值对：
        /// {学科,物理}
        /// {一级分类,力学}
        /// </summary>
        /// <returns></returns>
        Dictionary<string, string> ReadLine2();

        /// <summary>
        /// 关闭
        /// </summary>
        void Close();
    }
}