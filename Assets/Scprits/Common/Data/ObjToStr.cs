using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xuexue.f3dModel
{
    /// <summary>
    /// 一个自定义的对象ToString()的扩展方法
    /// </summary>
    public static class ObjToStr
    {
        public static string ToStr(this string[] strArr)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            for (int i = 0; i < strArr.Length; i++)
            {
                sb.Append($"{strArr[i]},");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append("}");
            return sb.ToString();
        }

        public static string ToStr(this Dictionary<string, string> dict)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            foreach (var item in dict)
            {
                sb.Append($"{item},");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append("}");
            return sb.ToString();
        }
    }
}
