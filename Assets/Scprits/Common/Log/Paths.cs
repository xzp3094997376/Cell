using System;
using System.Reflection;
using System.Text;
namespace huang.common.paths
{
    public class Paths
    {
        /// <summary>
        /// 模块名(程序名)
        /// </summary>
        public static string ModuleName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

        /// <summary>
        /// 模块的完整路径。
        /// </summary>
        public static string ModulePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;

        /// <summary>
        /// 当前工作目录的完全限定路径
        /// </summary>
        public static string EnvCurrentDirectory = System.Environment.CurrentDirectory;

        /// <summary>
        /// 应用程序的当前工作目录
        /// </summary>
        public static string DirCurrentDirectory = System.IO.Directory.GetCurrentDirectory();

        /// <summary>
        /// 获取程序的基目录
        /// </summary>
        public static string CurDomainBaseDirectory = System.AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// 应用程序的目录的名称
        /// </summary>
        public static string ApplicationBase = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

        /// <summary>
        /// 获取启动了应用程序的可执行文件的路径，不包括可执行文件的名称
        /// </summary>
        //public static string StartupPath = System.Windows.Forms.Application.StartupPath;  //winform
        public static string StartupPath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);

        /// <summary>
        /// 获取启动了应用程序的可执行文件的路径及文件名
        /// </summary>
        //public static string ExecutablePath = System.Windows.Forms.Application.ExecutablePath; //winform
        public static string ExecutablePath = System.Reflection.Assembly.GetExecutingAssembly().Location;

        /// <summary>
        /// 应用程序名有扩展名
        /// </summary>
        public static string ExeName = System.IO.Path.GetFileName(ModulePath);

        /// <summary>
        /// 应用程序名无扩展名
        /// </summary>
        public static string ExeNameNoExt = System.IO.Path.GetFileNameWithoutExtension(ModulePath);

        /// <summary>
        /// 一次性输出所有字段文本
        /// </summary>
        /// <returns></returns>
        public static new string ToString()
        {
            Type type = typeof(Paths);
            StringBuilder sb = new StringBuilder();
            sb.Append("Paths:当前所有字段文本" + Environment.NewLine);
            foreach (FieldInfo info in type.GetFields())
            {
                sb.Append("Field:" + info.Name + " Value:" + info.GetValue(null) + Environment.NewLine);
            }
            return sb.ToString();
        }
    }
}