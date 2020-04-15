//2014.05.24, czs, created 
//2014.06.08, czs, edit 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace Gdp
{
    /// <summary>
    /// 计量单位。如米、弧度、度等。
    /// </summary>
    public class Unit : IdentifiedObject//, IUnit
    { 
        /// <summary>
        /// 创建一个实例。
        /// </summary>
        /// <param name="ConversionFactor">尺度转换因子</param>
        public Unit(double ConversionFactor = 1) { this.ConversionFactor = ConversionFactor; }

        /// <summary>
        /// 与标准尺寸的转换。
        /// 对于特定的转换，如时间转换，采用的名称是 SecondPerUnit，其值是一样的。
        /// </summary>
        public double ConversionFactor { get; set; }
         

        /// <summary>
        /// 默认以分号隔开的字符串。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Id + "_" + Name + "_" + ConversionFactor;
        }


        /// <summary>
        /// Checks whether the values of this instance is equal to the values of another instance.
        /// Only parameters used for coordinate system are used for comparison.
        /// Name, abbreviation, authority, alias and remarks are ignored in the comparison.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Unit))
                return false;
            return (obj as Unit).ConversionFactor == this.ConversionFactor;
        }

        /// <summary>
        /// 哈稀数
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
     


}
