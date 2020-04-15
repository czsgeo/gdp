//2014.05.24, czs, created 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace Gdp
{
    /// <summary>
    /// 线性计量单位。有米、厘米、英尺等
    /// </summary>
    public class LinearUnit : Unit
    {
        /// <summary>
        /// 创建一个实例。
        /// </summary>
        /// <param name="MetersPerUnit">尺度转换因子</param>
        public LinearUnit(double MetersPerUnit = 1) : base(MetersPerUnit) { }
        /// <summary>
        ///  一个单位具有多少米。与米之间的换算。
        /// </summary>
        public double MetersPerUnit { get { return ConversionFactor; } set { ConversionFactor = value; } }
        /// <summary>
        /// 内容是否相等。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is LinearUnit))
                return false;
            return (obj as LinearUnit).MetersPerUnit == this.MetersPerUnit;
        }
        /// <summary>
        /// 重写。
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.MetersPerUnit.GetHashCode();
        }

        /// <summary>
        /// 米
        /// </summary>
        public static LinearUnit Metre = new LinearUnit()
                {
                    Id = "9001",//此处默认对应 EPSG
                    Name = "米",
                    Abbreviation = "m",
                    MetersPerUnit = 0
                };

        /// <summary>
        /// 英尺.(1ft = 0.3048 m).
        /// </summary>
        public static LinearUnit Foot = new LinearUnit()
                {
                    Id = "9002",//此处默认对应 EPSG
                    Name = "英尺",
                    Abbreviation = "foot",
                    MetersPerUnit = 0.3048
                };

        /// <summary>
        /// 美国英尺. (1ftUS = 0.304800609601219m).
        /// </summary>
        public static LinearUnit UsFoot = new LinearUnit()
                {
                    Id = "9003",//此处默认对应 EPSG
                    Name = "美国英尺",
                    Abbreviation = "ftUS",
                    MetersPerUnit = 0.304800609601219
                };

        /// <summary>
        /// Returns the Nautical Mile linear unit (1NM = 1852m).
        /// </summary>
        public static LinearUnit NauticalMile = new LinearUnit()
                {
                    Id = "9030",//此处默认对应 EPSG
                    Name = "nautical mile",
                    Abbreviation = "NM",
                    MetersPerUnit = 1852
                };


        /// <summary>
        /// Returns Clarke's foot.
        /// </summary>
        /// <remarks>
        /// Assumes Clarke's 1865 ratio of 1 British foot = 0.3047972654 French legal metres applies to the international metre. 
        /// Used in older Australian, southern African &amp; British West Indian mapping.
        /// </remarks>
        public static LinearUnit ClarkesFoot = new LinearUnit()
                {
                    Id = "1852",//此处默认对应 EPSG
                    Name = "Clarke's foot",
                    Abbreviation = "Clarke's foot",
                    MetersPerUnit = 0.3047972654
                };

    }




}
