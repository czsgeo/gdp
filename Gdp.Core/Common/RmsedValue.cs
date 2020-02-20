//2014.09.27, czs, create, 具有权值的值

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gdp
{


    /// <summary>
    /// 具有权值的值。
    /// </summary>
    public class RmsedValue<TValue> :  BaseValue<TValue> , IRmsed<TValue>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public RmsedValue() { }
        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="val">数值</param>
        /// <param name="rms">每个分量的均方根</param>
        public RmsedValue(TValue val, TValue rms)  :base( val)
        {
            this.Rms = rms;
        }

        /// <summary>
        /// RMS 属性， 中误差，均方差
        /// </summary>
        public virtual TValue Rms { get; set; }

        /// <summary>
        /// 中误差，均方差
        /// </summary>
        public TValue StdDev { get => Rms; set => Rms = value; }

        #region override
        /// <summary>
        /// 是否相等
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            RmsedValue<TValue> o = obj as  RmsedValue<TValue> ;
            if (o == null) { return false; }

            return o.Value.Equals(this.Value) && o.Rms .Equals(this.Rms);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode() * 3 + Rms.GetHashCode() * 5;
        }

        public override string ToString()
        {
            return Value + "(" + Rms + ")";
        }
        #endregion
    }
}
