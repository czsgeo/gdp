//2014.05.31, czs, created 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace Gdp
{
    /// <summary>
    /// 时间计量单位。有秒、小时、天、年等
    /// </summary>
    public class TimeUnit : Unit
    {
        
        /// <summary>
        /// 创建一个实例。
        /// </summary>
        /// <param name="LinearUnit">尺度转换因子</param>
        public TimeUnit(double LinearUnit = 1) : base(LinearUnit) { }
        /// <summary>
        /// 以秒为基准。
        /// 一个单位具有多少弧度。
        /// </summary>
        public double SecondPerUnit { get { return ConversionFactor; } set { ConversionFactor = value; } } 

        /// <summary>
        /// 内容是否相等。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is TimeUnit))
                return false;
            return (obj as TimeUnit).SecondPerUnit == this.SecondPerUnit;
        }
        /// <summary>
        /// 重写。
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.SecondPerUnit.GetHashCode();
        }

        #region 常用
        /// <summary>
        /// 秒
        /// </summary>
        public static TimeUnit Second
        {
            get
            {
                return new TimeUnit()
                {
                  //  Id = "9001",//此处默认对应 EPSG
                    Name = "秒",
                    Abbreviation = "s",
                    SecondPerUnit = 0
                };
            }
        }
        /// <summary>
        /// 毫秒
        /// </summary>
        public static TimeUnit MiliSecond
        {
            get
            {
                return new TimeUnit()
                {
                 //   Id = "9002",//此处默认对应 EPSG
                    Name = "毫秒",
                    Abbreviation = "ms",
                    SecondPerUnit = 0.001
                };
            }
        }
        /// <summary>
        /// 小时
        /// </summary>
        public static TimeUnit Hour
        {
            get
            {
                return new TimeUnit()
                {
                 //   Id = "9003",//此处默认对应 EPSG
                    Name = "小时",
                    Abbreviation = "h",
                    SecondPerUnit = 3600.0
                };
            }
        }
        /// <summary>
        /// 分
        /// </summary>
        public static TimeUnit Minute
        {
            get
            {
                return new TimeUnit()
                {
              //      Id = "9030",//此处默认对应 EPSG
                    Name = "分",
                    Abbreviation = "m",
                    SecondPerUnit = 60.0
                };
            } 
        }


        /// <summary>
        /// 天
        /// </summary>
        public static TimeUnit Day
        {
            get
            {
                return new TimeUnit()
                {
                    // Id = "1852",//此处默认对应 EPSG
                    Name = "天",
                    Abbreviation = "d",
                    SecondPerUnit = 86400.0
                };
            }
        }

        /// <summary>
        /// 星期
        /// </summary>
        public static TimeUnit Week
        {
            get
            {
                return new TimeUnit()
                {
                    // Id = "1852",//此处默认对应 EPSG
                    Name = "星期",
                    Abbreviation = "w",
                    SecondPerUnit = 604800.0
                };
            }
        }
        #endregion
    }
}
