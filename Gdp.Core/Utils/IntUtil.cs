//2014.10.30, czs, create, 整数工具

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


namespace Gdp.Utils
{
    /// <summary>
    /// 提供静态数学方法，是系统 Math 的补充。
    /// </summary>
    public static class IntUtil
    {
        /// <summary>
        /// 尝试解析，如失败返回 0
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int TryParse(object str)
        {
            if (str == null) return 0;
            string s = str.ToString().Trim();

            int num = 0;
            int.TryParse(s, out num);
            return num;
        }
        /// <summary>
        /// 解析数字 四 －>　4,只返回解析的第一个数字。
        /// </summary>
        /// <param name="ChineseCharacter"></param>
        /// <returns></returns>
        public static  int GetSingleNumberFromChineseCharacter(string ChineseCharacter)
        {
            if (ChineseCharacter.Contains("零")) return 0;
            if (ChineseCharacter.Contains("一")) return 1;
            if (ChineseCharacter.Contains("二")) return 2;
            if (ChineseCharacter.Contains("三")) return 3;
            if (ChineseCharacter.Contains("四")) return 4;
            if (ChineseCharacter.Contains("五")) return 5;
            if (ChineseCharacter.Contains("六")) return 6;
            if (ChineseCharacter.Contains("七")) return 7;
            if (ChineseCharacter.Contains("八")) return 8;
            if (ChineseCharacter.Contains("九")) return 9;

            return 0;
        }
        /// <summary>
        /// 是否为整数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsInt(string str)
        {
            str = str.Trim();
            foreach (var item in str)
            {
                if(!Char.IsNumber(item)){return false;}                
            }
            return true;

            return Regex.IsMatch(str, @"^([0-9]+)$");
        }


        /// <summary>
        ///  返回阶乘
        /// </summary>
        /// <param name="step">含</param>
        /// <returns></returns>
        public static int GetStepMult(int step)
        {
            int result = 1;
            for (int i = 1; i <= step; i++)
            {
                result *= i;
            }
            return result;
        }
        /// <summary>
        /// 加，阶
        /// </summary>
        /// <param name="step">含</param>
        /// <returns></returns>
        public static int GetStepPlus(int step)
        {
            int result = 0;
            for (int i = 1; i <= step; i++)
            {
                result += i;
            }
            return result;
        }
        /// <summary>
        /// 解析整数，错误返回 最大值。
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int ParseInt(string str)
        {
            int result = int.MaxValue;
            if(int.TryParse(str, out result))
            {
                return result;
            }
            return int.MaxValue;
        }

        /// <summary>
        /// 提取解析字符串中的数字。忽略非数字。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static int TryGetInt(string name)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var item in name)
            {
                if (IsInt(item.ToString())) { sb.Append(item); }
            }

            return Int32.Parse( sb.ToString());
        }
        /// <summary>
        /// 获取正数或0: 若正数则直接返回，否则返回 0.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        internal static int GetPositiveOrZero(int index)
        {
            if (index < 0) return 0;
            return index;
        }
    }
}
