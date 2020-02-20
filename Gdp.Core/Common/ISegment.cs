//2015.05.11, czs, create in namu, 顶层分段接口
//2016.08.03, czs, edit in fujian yongan, 增加对象绑定。
 
using System;

namespace Gdp
{
    /// <summary>
    /// 顶层分段接口，数值与分段单位或对象不同。
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TSpan"></typeparam>
    public interface ISegment<TValue, TSpan> : ISegment<TValue> where TValue : IComparable<TValue>
    { 
        /// <summary>
        /// 长度，不含缓冲
        /// </summary>
        TSpan Span { get; }
    }

    /// <summary>
    /// 顶层分段接口
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public interface ISegment<TValue> where TValue : IComparable<TValue>
    {
        /// <summary>
        /// 结束值
        /// </summary>
        TValue End { get;  }

        /// <summary>
        /// 起始值
        /// </summary>
        TValue Start { get;  }

        /// <summary>
        /// 完全包含或相等才返回true。
        /// </summary>
        /// <param name="val">是否包含值</param>
        /// <returns></returns>
        bool Contains(TValue val);

        /// <summary>
        /// 完全包含或相等才返回true。
        /// </summary>
        /// <param name="timePeriod">待判断时段</param>
        /// <returns></returns>
        bool Contains(ISegment<TValue> timePeriod);

        /// <summary>
        /// 是否相交
        /// </summary>
        /// <param name="segment">另一个段</param> 
        /// <returns></returns>
        bool IsIntersect(ISegment<TValue> segment);
        /// <summary>
        /// 绑定对象。
        /// </summary>
        object Tag { get; set; }

    }
}
