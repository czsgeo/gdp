//2014.11.19, czs,  create in numu, 数值格式化

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime;
using System.Data;
using System.Globalization;

namespace Gdp
{
    /// <summary>
    /// 数字格式化。适用于 GNSS 数据。
    /// 直到地球同步卫星的数字都显示全，后面保留到亚毫米级别。
    /// 如果太小或太大就采用科学计数法。
    /// </summary>
    public class NumeralFormatter : ICustomFormatter
    {
        static double minusMax = -1e-3;
        static double minusMin = -1e9;
        static double min = 1e-3;
        static double max = 1e9;

        /// <summary>
        /// 是否适合直接采用ToString方法。
        /// </summary>
        /// <param name="val">待处理数据</param>
        /// <returns></returns>
        bool IsSuitToString(double val)
        {
            if ((val > minusMin && val < minusMax)
                || (val > min && val < max))
                return true;
            return false;
        }
        /// <summary>
        /// 是否是整数。
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        bool IsInt(double val)
        {
            return (val - (int)val) == 0;
        }
        /// <summary>
        /// 默认的有效位数
        /// </summary>
        const int ValidCount = 4;
        /// <summary>
        /// 格式化
        /// </summary>
        /// <param name="format">有效位数</param>
        /// <param name="arg"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            NumeralFormatSymbol sym = new NumeralFormatSymbol(format); 
            int validCount = sym.FractionCount;
           // int.TryParse(format, out validCount);

            if (arg is double || arg is float)
            {
                double val = (double)arg;
                //可以正常记录
                if (IsSuitToString(val))
                {
                    if (ValidCount != validCount)
                    {
                        ZeroPad pad = new ZeroPad(validCount);
                        return  Gdp.Utils.StringUtil.FillSpaceLeft( val.ToString(pad.PadString), sym.StringWidth);
                    }
                    return Gdp.Utils.StringUtil.FillSpaceLeft(val.ToString("0.00000"), sym.StringWidth);
                }
                //整数则直接输出
                if (IsInt(val)) return Gdp.Utils.StringUtil.FillSpaceLeft(val.ToString(), sym.StringWidth);

                //采用科学计数法
                return Gdp.Utils.StringUtil.FillSpaceLeft(val.ToString("G" + (validCount-1)), sym.StringWidth);//0.12345E9
                //return Geo.Utils.StringUtil.FillSpaceLeft(currentVal.ToString("E" + validCount), sym.StringWidth);//0.12345E9
               // return currentVal.ToString("E4");
            }
            return arg.ToString();
        }
    }

    /// <summary>
    /// 0 填充的字符串。用于格式化数字输出。
    /// </summary>
    public class ZeroPad
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="count"></param>
        public ZeroPad(int count)
        {
           PadString =  GetPadString(count);
        }
        /// <summary>
        /// 填充
        /// </summary>
        public string PadString { get; set; }
        /// <summary>
        /// 获取填充
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public string GetPadString(int count)
        { 
            PadString = "0.00000";
            if (count > 0)
            {
                PadString = "0.";
                PadString =  PadString.PadRight(count + 2, '0');
            }
            return PadString;
        }
    }
}
