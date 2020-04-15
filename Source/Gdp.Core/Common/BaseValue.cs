//2014.09.16, czs, create, 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gdp
{
    /// <summary>
    /// 具有一个类型为 TValue 的 Value 属性。
    /// </summary>
    public class BaseValue<TValue>// : IValue<TValue>, IToTabRow
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseValue() { }
        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="val"></param>
        public BaseValue(TValue val)
        {
            this.Value = val;
        }

        /// <summary>
        /// 双精度Value属性
        /// </summary>
        public virtual TValue Value { get; set; }

        #region override
        /// <summary>
        /// 等
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            BaseValue<TValue> o = obj as BaseValue<TValue>;
            if (o == null) return false;

            return Value.Equals(o.Value);
        }
        /// <summary>
        /// 哈希
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
        /// <summary>
        /// 字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Value.ToString();
        }
        #endregion

        
    }
}
