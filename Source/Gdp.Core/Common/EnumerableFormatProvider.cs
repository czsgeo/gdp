//2014.11.16, czs,  create in numu, 数值格式化

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
    /// 数组的格式类提供者。
    /// </summary>
    public class EnumerableFormatProvider : IFormatProvider 
    {
          /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ValidFractionCount">小数点后的有效位数</param>
        public EnumerableFormatProvider(int ValidFractionCount = 4)
        {
            this.ValidFractionCount = ValidFractionCount; 
         }

        /// <summary>
        ///  获取格式器。IFormatProvider
        /// </summary>
        /// <param name="formatType"></param>
        /// <returns></returns>
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
                return new EnumerableFormatter();
            else if (formatType == typeof(DateTime))
                return DateTimeFormatInfo.CurrentInfo; 
            //else  if(formatType == typeof(double))
                return NumberFormatInfo.CurrentInfo; 
        }

        public int ValidFractionCount { get; set; }
    } 
}
