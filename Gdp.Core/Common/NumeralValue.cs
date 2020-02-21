//2014.09.16, czs, create, 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gdp
{     

    /// <summary>
    /// 具有一个双精度Value属性。
    /// </summary>
    public  class NumeralValue : BaseValue<Double>// , INumeralValue
    {
        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="val"></param>
        public NumeralValue(double val= 0):base(val)
        { 

        }

        /// <summary>
        /// 是否值为 0 
        /// </summary>
        public bool IsZero { get { return this.Equals(Zero); } }


        /// <summary>
        /// 值为0。
        /// </summary>
        public static NumeralValue Zero { get { return new NumeralValue(); } }
    }
     
}
