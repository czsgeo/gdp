//2014.11.16, czs,  create in numu, 数值格式化

using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Runtime;
using System.Data;
using System.Globalization;
using Gdp.Utils;

namespace Gdp
{ 
     
    // var msg18 = string.Format(new MyHelloFormatProvider(), 
    //    "{0:UPP}  {1:LOW}", new Rectangle() { Name = "MyRectangle", Width = 14.3, Height = 10 },
    //    new Square() { Name = "MySquare", Side = 24.2 });

    /// <summary>
    /// 自定义处理数组的格式类
    /// </summary>
    public class EnumerableFormatter : ICustomFormatter
    {
        /// <summary>
        /// 构造函数
        /// </summary> 
        public EnumerableFormatter()
        {
            NumeralFormatProvider = new NumeralFormatProvider();
        }
        /// <summary>
        /// 数字格式化提供者
        /// </summary>
        IFormatProvider NumeralFormatProvider { get; set; }

        /// <summary>
        /// 数组的格式过程
        /// </summary>
        /// <param name="format">格式化参数,如 ",5.3" 表示分隔符为逗号，字符宽5，小数为3位</param>
        /// <param name="arg">待格式化的值</param>
        /// <param name="formatProvider">格式提供者</param>
        /// <returns></returns>
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            SplitFormatSymbol sym = new SplitFormatSymbol(format);

            string spliter = sym.Spliter.ToString();

            if (arg is IEnumerable<Double> || arg is IEnumerable<float>)
            {
                IEnumerable<Double> list = arg as IEnumerable<Double>;
                StringBuilder sb = new StringBuilder();
                int i = 0;

                foreach (var item in list)
                {
                    if (i != 0) sb.Append(spliter);
                    string str = String.Format(NumeralFormatProvider, "{0:" + sym.NumeralFormat + "}", item);
                    sb.Append(str);  

                    i++;                 
                } 
                return sb.ToString();
            }
            else if (arg is IEnumerable)
            {
                bool isDic = false;
                if (arg is IDictionary) isDic = true;

                IEnumerable array = arg as IEnumerable;
                StringBuilder sb = new StringBuilder();
                int i = 0;
                foreach (var item in array)
                {
                    if (i != 0) sb.Append(spliter);

                    if (isDic)
                    {
                        KeyValuePair<Object, Object> kv = (KeyValuePair<Object, Object>)item;
                        sb.Append(kv.Key.ToString() + ":" + kv.Value.ToString());
                    }
                    else
                    {
                        sb.Append(Gdp.Utils.StringUtil.FillSpaceLeft(item.ToString(), sym.StringWidth));
                    } 
                    i++;
                } 

                return sb.ToString();
            }
            return arg + "";
        }
    }

    /// <summary>
    /// 自定义常用格式字符解析
    /// 第一个字符为分隔符，第二个到“.”为字符串最小宽度，之后为有效小数位。
    /// </summary>
    public class SplitFormatSymbol
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="format">如 ",5.3" 表示分隔符为逗号，字符宽5，小数为3位</param>
        public SplitFormatSymbol(string format = null)
        {
            FractionCount = 4;
            StringWidth = 1;
            Spliter = ',';

            if (format != null)
            {
                Parse(format);
            }
        }

        /// <summary>
        /// 解析格式。如 ",5.3" 表示分隔符为逗号，字符宽5，小数为3位。
        /// </summary>
        /// <param name="format"></param>
        public void Parse(string format)
        {
            if (format != null)
            {
               // format = format.Trim(); //会把\t给去掉
                if (format.Length > 0) { this.Spliter = format[0]; }
                if (format.Length > 1)
                { 
                    format = format.Substring(1);//去掉第一个spliter

                    string [] strs = format.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    if(strs.Length > 0){
                         this.StringWidth = int.Parse(strs[0]);
                    }

                    if(strs.Length > 1){
                         this.FractionCount = int.Parse(strs[1]);
                    } 
                }
            }
        }

        /// <summary>
        /// 小数位数
        /// </summary>
        public int FractionCount { get; set; }

        /// <summary>
        /// 字符串宽度
        /// </summary>
        public int StringWidth { get; set; }

        /// <summary>
        /// 元素之间隔分隔符
        /// </summary>
        public char Spliter { get; set; }
        /// <summary>
        /// 数字格式
        /// </summary>
        public string NumeralFormat
        {
            get
            {
                return StringWidth + "." + FractionCount;
            }
        }
        /// <summary>
        /// 字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Spliter + StringWidth + "." + FractionCount;
        }
    }

    /// <summary>
    /// 专用于数字格式化。
    /// </summary>
    public class NumeralFormatSymbol
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="format"></param>
        public NumeralFormatSymbol(string format = null)
        {
            FractionCount = 4;
            StringWidth = 8;

            if (format != null)
            {
                Parse(format);
            }
        }
        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="format"></param>
        public void Parse(string format)
        {
            if (format != null)
            {
                format = format.Trim();
                if (format.Length > 1)
                {
                    int divide = format.IndexOf('.');
                    if (divide != -1)
                    {
                        this.StringWidth = int.Parse(format.Substring(0, divide));
                        this.FractionCount = int.Parse(format.Substring(divide + 1));
                    }
                    else
                    {
                        this.StringWidth = int.Parse(format);
                    }
                }
            }
        }

        /// <summary>
        /// 小数位数
        /// </summary>
        public int FractionCount { get; set; }

        /// <summary>
        /// 字符串宽度
        /// </summary>
        public int StringWidth { get; set; }

        public override string ToString()
        {
            return  StringWidth + "." + FractionCount;
        }
    }


}