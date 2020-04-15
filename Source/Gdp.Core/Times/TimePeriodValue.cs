//2017.03.09, czs, create in hongqing, 分时段数值

using System;
using System.Collections.Generic;
using System.Text;


namespace Gdp
{
    /// <summary>
    /// 时间序列对象维护。
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class TimeSquentialValue<TItem, TValue> : BaseList<TimePeriodValue<TValue>>
        where TItem : TimePeriodValue<TValue>
    {
        public TimeSquentialValue()
        {

        }

        /// <summary>
        /// 获取数值
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public TimePeriodValue<TValue> Get(Time time)
        {
            foreach (var item in this)
            {
                if (item.TimePeriod.Contains(time))
                {
                    return item;
                }
            }
            return null;
        }


        /// <summary>
        /// 是否包含指定时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool Contains(DateTime time)
        {
            return Contains(new Time(time));
        }
            
            /// <summary>
        /// 是否包含指定时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool Contains(Time time)
        {
            foreach (var item in this)
            {
                if (item.TimePeriod.Contains(time))
                {
                    return true;
                }
            }
            return false; 
        }


    }




    /// <summary>
    /// 分时段数值
    /// </summary>
    public class TimePeriodValue<TValue> : IComparable<TimePeriodValue<TValue>>
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        /// <param name="perid"></param>
        /// <param name="val"></param>
        public TimePeriodValue(TimePeriod perid, TValue val)
        {
            this.Value = val;
            this.TimePeriod = perid;
        }
        /// <summary>
        /// 时段
        /// </summary>
        public TimePeriod TimePeriod { get; set; }
        /// <summary>
        /// 数值
        /// </summary>
        public TValue Value { get; set; }

        public override string ToString()
        {
            return TimePeriod + " " + Value;
        }

        /// <summary>
        /// 是否相等
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var o = obj as TimePeriodValue<TValue>;
            if (o == null) return false;

            return this.TimePeriod.Equals(o.TimePeriod) && this.Value.Equals(o.Value);
        }
        /// <summary>
        /// 哈希
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var hash = this.TimePeriod.GetHashCode() * 13;
            hash += this.Value.GetHashCode() * 17;
            return hash;
        }

        public int CompareTo(TimePeriodValue<TValue> other)
        {
            return TimePeriod.CompareTo(other.TimePeriod);
        }
    }
}