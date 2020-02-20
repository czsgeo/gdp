//2016.11.25, czs, create in hongqing, 是否启用对象

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace Gdp
{
    /// <summary>
    /// 是否启用对象
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class EnableValue<TValue>// : IEnabled
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public EnableValue() { }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="val"></param>
        /// <param name="enabled"></param>
        public EnableValue(TValue val, bool enabled) { this.Value = val; this.Enabled = enabled; }
        /// <summary>
        /// 标记
        /// </summary>
        public bool Enabled { get; set; }
        /// <summary>
        /// 对象，值
        /// </summary>
        public TValue Value { get; set; }
    }
    /// <summary>
    /// 可启用的字符串
    /// </summary>
    public class EnableString : EnableValue<String>
    { 
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public EnableString() { }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="val"></param>
        /// <param name="enable"></param>
        public EnableString(String val,bool enable = false) { this.Value = val; this.Enabled = enable; } 

    }

    /// <summary>
    /// 可启用的整数
    /// </summary>
    public class EnableInteger : EnableValue<int>
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public EnableInteger() { } 
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="val"></param>
        /// <param name="enabled"></param>
        public EnableInteger(int val, bool enabled = false) : base(val, enabled) { }
    }
    /// <summary>
    /// 可启用的浮点数
    /// </summary>
    public class EnableFloat : EnableValue<double>
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public EnableFloat() { }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="val"></param>
        /// <param name="enabled"></param>
        public EnableFloat(double val, bool enabled = false) :base(val,enabled) {  }
    }
    /// <summary>
    /// 时段
    /// </summary>
    public class EnabledTimePeriod : EnableValue<TimePeriod>
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public EnabledTimePeriod() { }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="val"></param>
        public EnabledTimePeriod(TimePeriod val) { this.Value = val; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="val"></param>
        public EnabledTimePeriod(DateTime from, DateTime to) { this.Value = new TimePeriod(from, to); }
    }

    /// <summary>
    /// 可启用的浮点数范围
    /// </summary>
    public class EnableFloatSpan : EnableValue<NumerialSegment>
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public EnableFloatSpan() { }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="val"></param>
        public EnableFloatSpan(NumerialSegment val) { this.Value = val; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="val"></param>
        public EnableFloatSpan(double from, double to) { this.Value = new NumerialSegment(from, to); }
    }
    /// <summary>
    /// 可启用的布尔值
    /// </summary>
    public class EnableBool : EnableValue<bool>
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public EnableBool() { }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="val"></param>
        public EnableBool(bool val) { this.Value = val; }
    }

    /// <summary>
    /// 可启用的时间
    /// </summary>
    public class DateTimeBool : EnableValue<DateTime>
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public DateTimeBool() { }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="val"></param>
        public DateTimeBool(DateTime val) { this.Value = val; }
    }
}
