//2014.12.25, czs, create in namu, 一段，有两个标量组成。

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gdp
{  
    /// <summary>
    ///  一段，由两个标量组成。具有距离变量 Span。
    /// </summary>
    /// <typeparam name="TValue">标量类型</typeparam>
    /// <typeparam name="TSpan">标量长度类型</typeparam>
    public abstract class Segment<TValue, TSpan> : Segment<TValue> , ISegment<TValue, TSpan> where TValue : IComparable<TValue>
    {
        #region 构造函数
        /// <summary>
        /// 默认构造函数。
        /// </summary>
        public Segment() { }

        /// <summary>
        /// 段的构造函数
        /// </summary>
        /// <param name="start">开始</param>
        /// <param name="end">结束</param>
        public Segment(TValue start, TValue end) : base(start, end) { }
        #endregion

        #region 属性
        /// <summary>
        /// 长度，不含缓冲
        /// </summary>
        public abstract TSpan Span { get; }
        /// <summary>
        /// 中间值
        /// </summary>
        public abstract TValue Median { get; }
 
        #endregion
    }

     /// <summary>
    /// 一段，由两个标量组成。
     /// </summary>
    /// <typeparam name="TValue">标量类型</typeparam>
    public  class Segment<TValue> : ISegment<TValue> where TValue : IComparable<TValue>
    {
        /// <summary>
        /// 默认构造函数。
        /// </summary>
        public Segment() { }

        /// <summary>
        /// 段的构造函数
        /// </summary>
        /// <param name="start">开始</param>
        /// <param name="end">结束</param>
        public Segment(TValue start, TValue end)
        {
            this.Start = start;
            this.End = end;
        }

        #region 属性
        /// <summary>
        /// 开始
        /// </summary>
        public virtual TValue Start { get;  set; }

        /// <summary>
        /// 结束
        /// </summary>
        public virtual TValue End { get; set; }
        /// <summary>
        /// 绑定的对象
        /// </summary>
        public object Tag { get; set; }
        /// <summary>
        /// 绑定的对象与Tag区别在于，此不会打印出来。
        /// </summary>
        public object Object { get; set; }
        /// <summary>
        /// 是否只对起始采用哈希数，利于扩展结尾数。
        /// </summary>
        public bool IsOnlyStartHashCode { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 是否包含指定的数
        /// </summary>
        /// <param name="val">值</param>
        /// <returns></returns>
        public virtual bool Contains(TValue val)
        {
            var first = this.Start.CompareTo(val) <= 0;
            var result =  this.End.CompareTo(val) >= 0;
            return result && first;
        }

        /// <summary>
        /// 完全包含或相等才返回true。
        /// </summary>
        /// <param name="seg">待判断时段</param>
        /// <returns></returns>
        public virtual bool Contains(ISegment<TValue> seg)
        {
            return this.Start.CompareTo(seg.Start) <= 0 && this.End.CompareTo(seg.End) >= 0;
        }

        /// <summary>
        /// 是否相交
        /// </summary>
        /// <param name="segment">另一个段</param> 
        /// <returns></returns>
        public virtual bool IsIntersect(ISegment<TValue> segment)
        {
            return this.Contains(segment.End) || this.Contains(segment.Start)
                || segment.Contains(this.End) || segment.Contains(this.Start);
        }
        /// <summary>
        /// 获取线段相交部分。如果没有，返回null
        /// </summary>
        /// <param name="segment"></param>
        /// <returns></returns>
        public virtual Segment<TValue>  GetIntersect(ISegment<TValue> segment)
        {
            if (IsIntersect(segment))
            {
                TValue start = default(TValue);
                TValue end = default(TValue);

                if (this.Contains(segment.Start)) { start = segment.Start; }
                else if (segment.Contains(Start)) { start = Start; }

                if (this.Contains(segment.End)) { end = segment.End; }
                else if (segment.Contains(End)) { end = End; }

                return new Segment<TValue>(start, end);
            }
            return null;
        }
        #endregion

        #region override
        /// <summary>
        /// 字符串显示
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Start + "→" + End + ", " + Tag; 
        }
        /// <summary>
        /// 是否相交
        /// </summary>
        /// <param name="span">另一个时段</param> 
        /// <returns></returns>
      //  public abstract bool Intersect(Segment<TValue> span);

        /// <summary>
        /// 是否相等
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            Segment<TValue> o = obj as Segment<TValue>;
            if (o == null) return false;

            return this.Start.Equals( o.Start) && this.End .Equals(o.End);
        }
        /// <summary>
        /// 哈希
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var hash = this.Start.GetHashCode() * 13;
            if (IsOnlyStartHashCode) return hash;
            hash += this.End.GetHashCode() * 17; 
            return hash;
        }
        #endregion
    } 
}
