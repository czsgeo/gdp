//2014.05.22, Cui Yang, edit.新增了一些方法
//2014.06.23， czs， add， add  SplitByBlank
//2018.06.04, lly, edit in zz, 发现计算分辨率出现了问题。 

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Linq;

namespace Gdp.Utils
{
    /// <summary>
    /// 字符串实用工具类。
    /// </summary>
    public static class StringUtil
    {
        /// <summary>
        /// 判断是否所有字符都是ASIIC
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        static public bool IsAllAsiicChars(string text)
        { 
            for (int i = 0; i < text.Length; i++)
            {
                if ((int)text[i] > 127)
                    return false; 
            }
            return true;
        }

    /// <summary>
    /// 追加一行
    /// </summary>
    /// <param name="sb"></param>
    /// <param name="ParamNames"></param>
    /// <param name="prefix"></param>
    public static void AppendListLine(StringBuilder sb, List<string> ParamNames, string prefix)
        {
            if (ParamNames != null)
            {
                sb.Append(prefix);
                int i = 0;
                foreach (var item in ParamNames)
                {
                    if (i != 0) { sb.Append(","); }
                    sb.Append(item);
                    i++;
                }
                sb.AppendLine();
            }
        }

        /// <summary>
        /// 提取名称.如果没有，返回null
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="prefix"></param>
        /// <param name="splitter"></param>
        /// <returns></returns>
        public static List<string> ExtractNames(string[] lines, string prefix, string[] splitter = null)
        {
            List<string> paramNames = null;
            foreach (var line in lines)
            {
                if (line.Contains(prefix))
                {
                    var content = line.Replace(prefix, "");

                    if (splitter == null)
                    {
                        splitter = new string[] { ",", ";", "\t", " " };
                    }
                    string[] strs = content.Split(splitter, StringSplitOptions.RemoveEmptyEntries);

                    paramNames = new List<string>(strs);
                    break;
                }
            }

            return paramNames;
        }

        #region 唯一标识,不同的精度不同的键。
        /// <summary>
        /// 获取一个唯一的识别键。不同的精度不同的键。
        /// </summary>
        /// <param name="val">值</param>
        /// <param name="resolution">分辨率，默认以米为单位, 100表示100米</param>
        /// <returns></returns>
        public static long GetUniqueKeyOfLong(double val, double resolution = 1e-3)
        {
            if (resolution == 0) { return ((long)val); }

            var lon = (long)(val / resolution);

            return lon;
        }
        /// <summary>
        /// 获取一个唯一的识别键。不同的精度不同的键。
        /// </summary>
        /// <param name="val">值</param>
        /// <param name="resolution">分辨率，默认以米为单位, 100表示100米</param>
        /// <returns></returns>
        public static string GetUniqueKey(double val, double resolution = 1e-3)
        {
            return GetUniqueKeyOfLong(val, resolution) + "";
        }
        /// <summary>
        /// 获取一个唯一的识别键。不同的精度不同的键。
        /// </summary>
        /// <param name="vals">值</param>
        /// <param name="resolution">分辨率，默认以米为单位, 100表示100米</param>
        /// <returns></returns>
        public static string GetUniqueKey(IEnumerable<double> vals, double resolution = 1e-3)
        {
            var sb = new StringBuilder();
            int i = 0;
            foreach (var item in vals)
            {
                if (i != 0) { sb.Append(","); }
                sb.Append(GetUniqueKeyOfLong(item, resolution));
                i++;
            }

            return sb.ToString();
        }

        #endregion

        /// <summary>
        /// 可读的文本，简洁，不长
        /// </summary>
        /// <param name="val"></param>
        /// <param name="len"></param>
        /// <param name="digital"></param>
        /// <returns></returns>
        public static string ToReadableText(double val, int len = 9, int digital = 5)
        {
            int numCount = ((int)val).ToString().Length + digital;
            var item = val.ToString("G");
            if (item.Length > len)
            {
                item = val.ToString("G"+ numCount);
            }
            return Gdp.Utils.StringUtil.FillSpaceLeft(item, len);
        }
        /// <summary>
        /// 转换成换行符分隔的一个字符串
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static string ToLineString(List<string> lines)
        {
            StringBuilder result = new StringBuilder();
            foreach (var item in lines)
            {
                result.AppendLine(item);
            }
            return result.ToString();
        }

        /// <summary>
        /// 提取其中的数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int GetNumber(string str)
        {
            string num = "";
            foreach (var item in str)
            {
                if (Char.IsNumber(item))
                {
                    num += item.ToString();
                } 
            }
            return int.Parse(num);
        }
        /// <summary>
        /// 解析字符串
        /// </summary>
        /// <param name="content"></param>
        /// <param name="spliters"></param>
        /// <returns></returns>
        public static double [] ParseDoubles(string content, char [] spliters)
        {
            var items = content.Split(spliters, StringSplitOptions.RemoveEmptyEntries);
            int length = items.Length;
            double[] result = new double[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = Gdp.Utils.StringUtil.ParseDouble(items[i]);
            }
            return result;
        }
         
        /// <summary>
        /// 可读的长度
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetReadableDistance(double length)
        {
            if (Math.Abs(length) < 1e-3) { return length.ToString("G5") + " m"; }
            if (Math.Abs(length) < 1) { return length.ToString("G5") + " m"; }
            if (Math.Abs(length) < 1e3) { return length.ToString("0.00000") + " m"; }
            if (Math.Abs(length) < 1e6) { return length.ToString("##0, 000.00000") + " m"; }
            if (Math.Abs(length) < 1e9) { return length.ToString("##0, 000, 000.00000") + " m"; }

            return length.ToString("000, 000, 000.00000") + " m";
        }

        /// <summary>
        /// 截取部分字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static string GetLimitedString(string str, int maxLength = 200, bool appendSuspensionPoints = true)
        {
            if (str.Length <= maxLength) return str;

            var s = str.Substring(0, maxLength);
            if (appendSuspensionPoints) s += "...";
            return s;
        }

        /// <summary>
        /// 统计字符串起始有多少个起始字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="starter"></param>
        /// <returns></returns>
        public static int StartCount(String str, string starter)
        {
            int count = 0;

            while (str.StartsWith(starter))
            {
                str = str.Substring(starter.Length);
                count++;
            }
            return count;
        }

        /// <summary>
        /// 检核命名是否合法,只能是数字，字母或下划线，不能包含特殊字符，如*，#，|等
        /// </summary>
        /// <param name="name">命名</param>
        /// <returns></returns>
        public static bool IsLegalName(string name)
        {
            foreach (var item in name)
            {
                if (
                      (item >= 65 && item <= 90) //A-Z
                    || (item >= 97 && item <= 122)//a-z
                    || item == 45  //- 减号
                    || item == 95 //_  下划线
                    ) { continue; }
                else { return false; }
            }
            return true;
        }

        /// <summary>
        /// 标准化参数名称，使其满足参数命名规则
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetStandardName(string name)
        {
            var sName = name.Replace("-", "_").Replace(" ", "");
            if (!Char.IsLetter(sName[0]) && sName[0] != '_') sName += "_";
            return sName;
        }
        /// <summary>
        /// 返回数组字符串
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="objs">数组</param>
        /// <param name="splitter">分隔符</param>
        /// <returns></returns>
        public static string GetArrayString<T>(T[] objs, string splitter = ",")
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach (var item in objs)
            {
                sb.Append(item);

                i++;

                if (i != objs.Length) sb.Append(splitter);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 通过空白进行分割。
        /// </summary>
        /// <param name="line">待分割字符串</param>
        /// <returns></returns>
        public static string[] SplitByBlank(string line)
        {
            return line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        }
        /// <summary>
        /// 通过制表符进行分割。
        /// </summary>
        /// <param name="line">待分割字符串</param>
        /// <returns></returns>
        public static string[] SplitByTab(string line)
        {
            return line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
        }
        /// <summary>
        /// 裁剪头尾的空格，制表符。
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string TrimBlank(string str)
        {
            return str.Trim(new char[] { '\t', ' ' });
        }

        /// <summary>
        /// 将字符串按照指定的分段长度全部拆解。
        /// 如果最后一个字符串长度不足，则不解析。
        /// </summary>
        /// <param name="line"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static List<string> Split(string line, int len)
        {
            int count = (int)(line.Length / len);
            return Split(line, len, count);
        }
        /// <summary>
        /// 按照指定分隔符切割
        /// </summary>
        /// <param name="line"></param>
        /// <param name="c"></param>
        /// <param name="isRremoveEmpties"></param>
        /// <returns></returns>
        public static string[] Split(string line, char c = '\t', bool isRremoveEmpties = true)
        {
            StringSplitOptions option = StringSplitOptions.None;
            if (isRremoveEmpties) { option = System.StringSplitOptions.RemoveEmptyEntries; }
            return line.Split(new char[] { c }, option);
        }
        /// <summary>
        /// 按照指定分隔符切割
        /// </summary>
        /// <param name="line"></param>
        /// <param name="c"></param>
        /// <param name="isRremoveEmpties"></param>
        /// <returns></returns>
        public static string[] Split(string line, char [] c, bool isRremoveEmpties = true)
        {
            StringSplitOptions option = StringSplitOptions.None;
            if (isRremoveEmpties) { option = System.StringSplitOptions.RemoveEmptyEntries; }
            return line.Split( c, option);
        }
        /// <summary>
        /// 拆解字符串。
        /// </summary>
        /// <param name="line"></param>
        /// <param name="len"></param>
        /// <param name="count">指定数量，防止字符串无关部分被解析</param>
        /// <returns></returns>
        public static List<string> Split(string line, int len, int count)
        {
            List<string> list = new List<string>();
            for (int i = 0; i < count; i++)
            {
                int leng = len;
                //最后一行，防止超界限
                if (i + 1 == count && line.Length < (i + 1) * len)
                    leng = line.Length - i * len;

                list.Add(StringUtil.FillSpace(line.Substring(i * len, leng), len));
            }
            return list;
        }
        /// <summary>
        /// 是否匹配
        /// </summary>
        /// <param name="str"></param>
        /// <param name="keyword"></param>
        /// <param name="remainMathedOrNot">是留下匹配上的还是没有匹配上的。</param>
        /// <param name="isFuzzyMathching"></param>
        /// <returns></returns>
        public static bool IsMatch(string str, string keyword, bool remainMathedOrNot, bool isFuzzyMathching)
        {
            bool match = false;
            if (isFuzzyMathching)
                match = (remainMathedOrNot && (str.Contains(keyword))) || (!remainMathedOrNot && (!str.Contains(keyword)));
            else
                match = (remainMathedOrNot && (str == keyword)) || (!remainMathedOrNot && (str != keyword));
            return match;
        }
        /// <summary>
        /// 是否匹配
        /// </summary>
        /// <param name="str"></param>
        /// <param name="keyword"></param>
        /// <param name="isFuzzyMathching"></param>
        /// <returns></returns>
        public static bool IsMatch(string str, string keyword, bool isFuzzyMathching)
        {
            bool match = false;
            if (isFuzzyMathching)
                match = (str.Contains(keyword));
            else
                match = (str == keyword);
            return match;
        }
        /// <summary>
        /// 是否是浮点数或数字。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsDecimalOrNumber(Object obj)
        {
            return IsDecimal(obj) || IsNumber(obj);
        }

        /// <summary>
        /// 是否是整型数字。
        /// depreciated.不推荐此法，执行效率比下一个函数慢5倍。
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static bool IsNumber(string str)
        {
            return Regex.IsMatch(str, @"^([0-9]+)$");
        }
        /// <summary>
        /// 是否为数字,整数。逐字判断。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNumber(object obj)
        {
            if(obj == null) { return false; }

            if (obj is int || obj is Int16 || obj is Int64)
            {
                return true;
            }

            string str = obj.ToString().Trim();
            str = str.TrimStart(new char[] { '-', '+' }); 
            foreach (var item in str)
            {
                if (!char.IsNumber(item)) return false;
            }
            return true;
        }

        /// <summary>
        /// 必须有小数点才是浮点数。是否为数字，包括小数，普通数字表达，不包含科学计数法。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsDecimal(Object obj)
        {
            if (obj == null) { return false; }

            if(obj is Double || obj is float || obj is Decimal)
            {
                return true;
            }
            var str = obj.ToString().Trim();
            if(str == Double.NaN.ToString())
            {
                return true;
            }

            double val = 0;
            if(double.TryParse(str, out val))
            {
                return true;
            } 

            if (!str.Contains(".")) { return false; }
            str = str.TrimStart(new char[] { '-', '+'}); 

            foreach (var item in str)
            {
                if (!char.IsNumber(item) 
                    && !item.Equals('.')) return false;
            }
            return true;
        }
        /// <summary>
        /// 只提取数字部分,支持小数和科学表达式。如 er123ert，最后为123.
        /// 小数点只提取第一个。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        static public double GetDecimal(object obj)
        {
            string str = obj.ToString().Trim();
            //str = str.TrimStart(new char[] { '-', '+' });
            StringBuilder sb = new StringBuilder();
            int i = 0;
            bool alreadyhasDot = false;
            bool ScientificExpression = false; 
            char lastChar;
            foreach (var item in str)
            {
                i++;
                lastChar = item;
                //正负号
                if (item == '-' && sb.Length == 0)
                {
                    sb.Append(item);
                }
                //小数点
                if (!alreadyhasDot && item == '.')
                {
                    sb.Append(item);
                    alreadyhasDot = true;
                }
                //科学表达式
                if (!ScientificExpression && sb.Length > 0 && item == 'E')
                {
                    sb.Append(item);
                    ScientificExpression = true; 
                }
                //科学表达式后的符号
                if (lastChar == 'E' && item == '-')
                {
                    sb.Append(item);
                }
                //数字
                if (char.IsNumber(item))
                {
                    sb.Append(item);
                }
            }
            var str2 = str.ToString();
            if (string.IsNullOrWhiteSpace(str2)) { return 0; }

            return Double.Parse(sb.ToString());

        }
        /// <summary>
        /// 获取针对数组格式化的字符串。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        static public string GetFormatedString<T>(IEnumerable<T> array, string spliter = ",")
        {
            return String.Format(new EnumerableFormatProvider(), "{0:" + spliter + "}", array);
        }

        /// <summary>
        /// 格式化双精度
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="format">格式，只有数字管用</param>
        /// <param name="width">宽度</param>
        /// <returns></returns>
        public static string GetForamtedString(Object obj, int width = 13, string format = "E13.6")
        { 
            if (ObjectUtil.IsFloat(obj))
            {
                var item = (double)obj;
                string formatedItem = null;
                if (StringUtil.IsNumber(Math.Abs(item)) && item < 1e7 && item > -1e7) { formatedItem = StringUtil.FillSpaceLeft(item.ToString(), width); }
                else { formatedItem = DoubleUtil.ScientificFomate(item, format); }
                return formatedItem;
            }

            return StringUtil.FillSpaceLeft(obj.ToString(), width);
        }

        /// <summary>
        /// 格式化浮点数
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static string GetFormatedDouble(double x, int len = 19)
        {
            if ((x > 1E-7 && x < 1E9) || (x > -1E9 && x < 1E-7)) return StringUtil.FillSpaceLeft(x.ToString("0.0000##"), len);
            return DoubleUtil.ScientificFomate(x, "E" + len + ".12", false);
        }
        public static string GetFormatedDoubleForTab(double x, int len = 19)
        {
            if (x == 0) return StringUtil.FillSpaceLeft(0, len);
            //不含 0 周围比较小的数字。
            if ((x > 1E-4 && x < 1E9) || (x > -1E9 && x < -1E-4))
            {
                if (IsNumber(x))
                {
                    return StringUtil.FillSpaceLeft(x, len);
                }
                return StringUtil.FillSpaceLeft(x.ToString("0.0000"), len);

            }
            return DoubleUtil.ScientificFomate(x, "E" + len + ".10", false);
        }
        /// <summary>
        /// 获取字符串中，非ASC字符的数量。
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int GetNonAscCount(string str)
        {
            int count = 0;
            foreach (var item in str)
            {
                if (item < 0 || item > 127) count++;
            }
            return count;
        }
        /// <summary>
        /// 字符串长度,非ASC字符认为是2个字符位置，如中文。
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static public int GetAscLength(string str)
        {
            int lenTotal = 0;
            int n = str.Length;
            string strWord = "";
            int asc;
            for (int i = 0; i < n; i++)
            {
                strWord = str.Substring(i, 1);
                asc = Convert.ToChar(strWord);
                if (asc < 0 || asc > 127) lenTotal += 2;
                else lenTotal += 1;
            }
            return lenTotal;
        }

        /// <summary>
        /// 以 0 填充到指定长度。
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="len"></param>
        /// <param name="appendToRight"></param>
        /// <returns></returns>
        public static string FillZero(object obj, int len, bool appendToRight = true)
        {
            return Fill(obj.ToString(), len, '0', appendToRight);
        }
        /// <summary>
        /// 0 填满 左边
        /// </summary>
        /// <param name="line"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string FillZeroLeft(string line, int len) { return Fill(line, len, '0', false); }
        /// <summary>
        /// 0 填满左边
        /// </summary>
        /// <param name="line"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string FillZeroLeft(int line, int len) { return Fill(line.ToString(), len, '0', false); }
        /// <summary>
        /// 0 填满左边
        /// </summary>
        /// <param name="line"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string FillZeroLeft(double line, int len) { return Fill(line.ToString(), len, '0', false); }
        /// <summary>
        /// 以 空白 填充到指定长度。
        /// </summary>
        /// <param name="line"></param>
        /// <param name="len"></param>
        /// <param name="appendToRight"></param>
        /// <returns></returns>
        public static string FillSpace(string line, int len, bool appendToRight = true)
        {
            if (line == null) line = "";
            return Fill(line, len, ' ', appendToRight);
        }
        /// <summary>
        /// 以 空白 从左填充到指定长度。
        /// </summary>
        /// <param name="line"></param>
        /// <param name="len"></param>
        /// <param name="forceToLen">强制到指定长度</param>
        /// <returns></returns>
        public static string FillSpaceLeft(string line, int len, bool forceToLen =true)
        {
            if (forceToLen && line!= null && line.Length > len)
            {
                line = SubString(line, 0, len);
            }
            return Fill(line, len, ' ', false);
        }
        /// <summary>
        /// 以 空白 从右填充到指定长度。
        /// </summary>
        /// <param name="line"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string FillSpaceRight(string line, int len)
        {
            return Fill(line, len, ' ', true);
        }

        /// <summary>
        /// 填满左边
        /// </summary>
        /// <param name="line"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string FillSpaceLeft(object line, int len)
        {
            return Fill(line.ToString(), len, ' ', false);
        }
        /// <summary>
        ///  填充到指定长度。
        /// </summary>
        /// <param name="line"></param>
        /// <param name="len"></param>
        /// <param name="cha"></param>
        /// <param name="appendToRight"></param>
        /// <param name="isCutSurplusChar"></param>
        /// <param name="isCutRight"></param>
        /// <returns></returns>
        public static string Fill(string line, int len, char cha = ' ', bool appendToRight = true, bool isCutSurplusChar = false, bool isCutRight = true)
        {
            if (line == null) { line = ""; }
            if (line.Length > len && isCutSurplusChar)
            {
                return GetFixedLength(line, len, isCutRight);
            }

            var newLine = line;
            if (appendToRight)
            {
               newLine = line.PadRight(len, cha);
            }
            else
            {
                newLine =  line.PadLeft(len, cha);
            }
            return newLine;
        }


        /// <summary>
        /// 字符串转换为Base64字符串
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static string ToBase64(string str)
        {
            System.Text.Encoding asciiEncoding = System.Text.Encoding.ASCII;
            byte[] byteArray = asciiEncoding.GetBytes(str);
            return Convert.ToBase64String(byteArray, 0, byteArray.Length);
        }
        // Keep searching for aString at the start of s
        // until num == 0 or aString is not found at the start of s      
        public static string TrimStart(ref string s, string aString, int num = 42949671)
        {
            try
            {
                if (aString == "")
                { return s; }
                while ((num > 0) && (s.IndexOf(aString, 0) == 0) && (s.Length > 0))
                {
                    s = s.Remove(0, aString.Length);
                    --num;
                }
                return s;
            }
            catch (Exception e)
            {
                throw new Exception("Exception thrown:" + e.ToString());
            }
        }

        public static string TrimStart(ref string s, char aString, int num = 42949671)
        {
            return TrimStart(ref s, aString.ToString(), num);
        }

        /// <summary>
        /// 查找第一个单词
        /// </summary>
        /// <param name="s">待查找的字符串，且剔除过空格符</param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static string firstWord(ref string s, char delimiter = ' ')
        {
            try
            {
                //return s if there are no delimiters
                int length = s.Length;
                int pos = -1;//第一个是字符delimiter处的位置

                for (int i = 0; i < length; i++)
                {
                    if (s[i] != delimiter)
                    {
                        pos = i;
                        break;
                    }
                }
                if (pos == -1)//如果位置是第一个，则直接返回
                { return s; }
                int endPos = s.IndexOf(delimiter, pos);
                if (endPos == -1)
                { return s; }
                else
                {
                    return s.Substring(pos, endPos - pos);
                }

            }
            catch (Exception e)
            {
                throw new Exception("Exception thrown: " + e.ToString());
            }

        }
        /// <summary>
        /// 去除第一个词。分隔符要注意，第一个匹配上则返回。
        /// </summary>
        /// <param name="s">字符串</param>
        /// <param name="delimiters">分隔符</param>
        /// <returns></returns>
        public static string TrimFirstWord(string s, char[] delimiters)
        {
            string str = s.Trim();
            List<int> list = new List<int>();
            foreach (var item in delimiters)
            {
                int index = 0;
                index = str.IndexOf(item);

                if (index != -1 && index != 0)
                {
                    list.Add(index);
                }
            }
            list.Sort();
            int i = list[0];
            string result = str.Substring(i + 1);
            return result;
        }

        /// <summary>
        /// 去除第一个词,并返回。
        /// </summary>
        /// <param name="s"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static string TrimFirstWord(ref string s, char delimiter = ' ')
        {
            try
            {
                s = s.Trim();
                if (!s.Contains(delimiter.ToString())) { return s; }

                int index = s.IndexOf(delimiter);
                var word = s.Substring(0, index);
                s = s.Substring(index + 1);
                return word;

                //s = TrimStart(ref s, delimiter);

                //string toReturn = firstWord(ref s, delimiter);//

                //s = TrimStart(ref s, toReturn);

                //s = TrimStart(ref s, delimiter);

                //return toReturn;
            }
            catch (Exception e)
            { throw new Exception("Exception thrown:" + e.ToString()); }

        }
        /// <summary>
        /// 解析整数
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        internal static int ParseInt(object str, int defaultVal = Int32.MaxValue)
        {
            Int32 val = defaultVal;
            Int32.TryParse(str + "", out val);
            return val;
        }
        /// <summary>
        /// 解析字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        public static double ParseDouble(object str, double defaultVal = Double.NaN)
        {
            double val = defaultVal;
            Double.TryParse(str + "", out val);
            return val;
        }
        /// <summary>
        /// 在字符串中截取部分，并转换为Double类型。
        /// </summary>
        /// <param name="line"></param>
        /// <param name="startIndex"></param>
        /// <param name="maxLen"></param>
        /// <returns></returns>
        public static double ParseDouble(string line, int startIndex, int maxLen = Int16.MaxValue)
        {

            int len = line.Length;
            if (len <= startIndex) return 0;
            string str = null;
            if (len >= startIndex + maxLen) str = line.Substring(startIndex, maxLen);
            else str = line.Substring(startIndex);

            if (String.IsNullOrWhiteSpace(str)) return 0;

            return Double.Parse(str);
        }
        /// <summary>
        /// 解析Int类型，如果不成功则返回 defaulValue 。
        /// </summary>
        /// <param name="intString"></param>
        /// <param name="defaulValue"></param>
        /// <returns></returns>
        public static int ParseInt(string intString, int defaulValue = 0)
        {
            int  o ;
            if(!int.TryParse(intString, out o))
            {
                o = defaulValue;
            }
            return o;
        }
        /// <summary>
        /// 尝试截取指定的子字符串，若字符串不足，则返回能读到的部分。
        /// 如果字符串长度比起始位置短，则返回空字符串 String.Empty。
        /// </summary>
        /// <param name="line">待截取的字符串</param>
        /// <param name="startIndex">起始编号</param>
        /// <param name="len">截取长度</param>
        /// <returns></returns>
        public static string SubString(string line, int startIndex, int len =Int16.MaxValue)
        {
            if (String.IsNullOrWhiteSpace(line)) { return String.Empty; }

            if (line.Length > len + startIndex) return line.Substring(startIndex, len);

            if (line.Length <= startIndex) return string.Empty;

            return line.Substring(startIndex);
        }
        /// <summary>
        /// 获取指定长度的字符串，其余添加空格或截取
        /// </summary>
        /// <param name="str">原字符串</param>
        /// <param name="count">长度</param>
        /// <param name="isAppendToRight">是否追加</param>
        /// <param name="isCutRight">是否从右边截取，否则左边</param>
        /// <returns></returns>
        public static string GetFixedLength(string str, int count, bool isCutRight = true, bool isAppendToRight = true)
        {
            if(str == null) { str = ""; }
            int len = str.Length;
            if (len == count) { return str; }
            if (len > count)
            {
                if (isCutRight)
                {
                    return str.Substring(0, count);                 
                }
                else
                { 
                    return str.Substring(len - count, count);
                }               
            }
            return StringUtil.FillSpace(str, count, isAppendToRight);
        }
        /// <summary>
        /// 通用序列化。对于列表，只遍历3次。
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="loopCount">最大循环次数，避免数组发生死循环，不用设置</param>
        /// <param name="isShowBracket">对于数组，是否用括号括起来</param>
        /// <param name="spliter"></param>
        /// <returns></returns>
        public static string ToString(object obj, bool isShowBracket = false, string spliter = ",", int loopCount = 0)
        {
            loopCount++;

            if (obj == null) return "";

            if (obj is string) return obj.ToString();

            if (obj is System.Collections.IDictionary) return DictionaryToString((System.Collections.IDictionary)obj, isShowBracket, spliter, loopCount);

            if (obj is System.Collections.IEnumerable) return EnumerableToString((System.Collections.IEnumerable)obj, isShowBracket, spliter, loopCount);

            return obj.ToString();
        }
        /// <summary>
        /// 列表打印
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="loopCount"></param>
        /// <param name="isShowBracket">是否用括号括起来</param>
        /// <param name="spliter"></param>
        /// <returns></returns>
        public static string DictionaryToString(System.Collections.IDictionary dic, bool isShowBracket = false, string spliter = ",", int loopCount = 0)
        {
            loopCount++;
            StringBuilder sb = new StringBuilder();
            int i = 0;
            if (isShowBracket) sb.Append("[");
            if (loopCount <= 3)
                foreach (var v in dic.Keys)
                {
                    if (i != 0) sb.Append(spliter);
                    sb.Append(ToString(v, isShowBracket, spliter, loopCount));
                    sb.Append(":");
                    sb.Append(ToString(dic[v], isShowBracket, spliter, loopCount));
                    i++;
                }
            if (isShowBracket) sb.Append("]");
            return sb.ToString();
        }


        /// <summary>
        /// 列表打印
        /// </summary>
        /// <param name="List"></param>
        /// <param name="loopCount"></param>
        /// <param name="spliter"></param>
        /// <param name="isShowBracket">是否用括号包起来</param>
        /// <returns></returns>
        public static string EnumerableToString(System.Collections.IEnumerable List, bool isShowBracket = false, string spliter =",", int loopCount = 0)
        {
            loopCount++;
            StringBuilder sb = new StringBuilder();
            int i = 0;
           if(isShowBracket)  sb.Append("[");
            if (loopCount <= 3)
                foreach (var v in List)
                {
                    if(v is char || v is byte ) return List.ToString();

                    if (i != 0) sb.Append(spliter);
                    sb.Append(ToString(v, isShowBracket, spliter, loopCount));
                    i++;
                }
            if (isShowBracket) sb.Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// 为字符串添加小括号
        /// </summary>
        /// <returns></returns>
        public static string Bracket(string str)
        {
            if (!str.StartsWith("(")) { str = "(" + str; }
            if (!str.EndsWith(")")) { str = str + ")"; }
            return str;
        }

        /// <summary>
        /// 移除字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="tobeRemoved"></param>
        /// <returns></returns>
        public static string RemoveString(string str, string[] tobeRemoved)
        {
            foreach (var item in tobeRemoved)
            {
                str = str.Replace(item, "");
            }
            return str;
        }
        /// <summary>
        /// 最后一个字符
        /// </summary>
        /// <param name="ObsPath"></param>
        /// <returns></returns>
        public static string GetLastChar(string ObsPath, int count = 1)
        {
            return ObsPath.Substring(ObsPath.Length - count); 
        }

        /// <summary>
        /// 对枚举，按照指定分隔符组合成字符串。
        /// </summary>
        /// <param name="list"></param>
        /// <param name="spliter"></param>
        /// <returns></returns>
        public static string ToString(IEnumerable list, string spliter)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach (var item in list)
            {
                if (i != 0) { sb.Append(spliter); }
                sb.Append(item);
               
                i++;
            }
            return sb.ToString();
        }
        /// <summary>
        /// 是否包含
        /// </summary>
        /// <param name="ParamNames"></param>
        /// <param name="key"></param>
        /// <param name="isFuzzyMatching"></param>
        /// <returns></returns>
        public static bool Contanis(List<string> ParamNames, string key, bool isFuzzyMatching = false)
        {
            if (isFuzzyMatching)
            {
                var find = ParamNames.FirstOrDefault(m => m.Contains(key));
                return find != null;
            }
            return ParamNames.Contains(key);
        }
    }
}
