//2015.12.28, czs, create in hongqing, 正则表达式工具

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;



namespace Gdp.Utils
{
    /// <summary>
    /// 正则表达式工具
    /// </summary>
    public static class RegexUtil
    {
        /// <summary>
        ///最后一个小括号匹配项，如果没有则返回null.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetLastBraketContent(string str)
        {
            var contents  = GetBraketContents(str);
            var len  = (contents.Length) ;
            if (len > 0)
            {
                return contents[len - 1];
            }
            return null;
        }

        /// <summary>
        /// 获取最里层括号内的内容，不含括号，返回数组。每个子括号返回一个字符串，与嵌套无关。如：
        /// ((1 1,5 1,5 5,1 5,1 1),(2 2,2 3,3 3,3 2,2 2)),((6 3,9 2,9 4,6 3))
        /// 返回字符串数组（仅数字部分）{{1 1,5 1,5 5,1 5,1 1},{2 2,2 3,3 3,3 2,2 2},{6 3,9 2,9 4,6 3}}
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static string[] GetBraketContents(string str)
        {
            Regex reg = new Regex(@"\([\d,\s]+\)");
            MatchCollection m = reg.Matches(str);

            string[] strs = new string[m.Count];
            for (int i = 0; i < m.Count; i++)
            {
                strs[i] = m[i].Groups[0].Value.Trim().TrimStart('(').TrimEnd(')');
            }
            return strs;
        }

        /// <summary>
        /// 提取双小括号内的内容,含双括号。
        /// ((1 1,5 1,5 5,1 5,1 1),(2 2,2 3,3 3,3 2,2 2)),((6 3,9 2,9 4,6 3))
        /// 返回字符串数组：{{((1 1,5 1,5 5,1 5,1 1),(2 2,2 3,3 3,3 2,2 2))},{((6 3,9 2,9 4,6 3))}}
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string[] GetDoubleBraketContents(string str)
        {
            Regex reg = new Regex(@"\({2}[\d\s,]+(\)\s*,\s*\([\d\s,]+)*\){2}");
            MatchCollection m = reg.Matches(str);

            string[] strs = new string[m.Count];
            for (int i = 0; i < m.Count; i++)
            {
                strs[i] = m[i].Groups[0].Value.Trim();//.TrimStart('(').TrimEnd(')');
            }
            return strs;
        }

    }
}
