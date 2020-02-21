//2014.12.25, czs, create in namu, 一段，有两个标量组成。

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gdp
{
    /// <summary>
    /// 具有前后缓冲的一段，由两个标量组成。
    /// </summary>
    public abstract class BufferedSegment<TValue, TSpan> : Segment<TValue, TSpan>//,  IBufferedSegment<TValue,TSpan> 
        where TValue : IComparable<TValue>
    {
        #region 构造函数
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public BufferedSegment() { }
        /// <summary>
        /// 段的构造函数
        /// </summary>
        /// <param name="start">开始</param>
        /// <param name="end">结束</param>
        /// <param name="buffer">缓冲</param>
        public BufferedSegment(TValue start, TValue end, TSpan buffer) : base(start, end) { this.StartBuffer = buffer;this.EndBuffer = buffer;  }
        /// <summary>
        /// 段的构造函数
        /// </summary>
        /// <param name="start">开始</param>
        /// <param name="end">结束</param>
        /// <param name="startBuffer">前端的缓冲</param>
        /// <param name="endBuffer">后端的缓冲</param>
        public BufferedSegment(TValue start, TValue end, TSpan startBuffer, TSpan endBuffer) : base(start, end) {this.StartBuffer = startBuffer;  this.EndBuffer = endBuffer; }
        #endregion

        #region 属性
        /// <summary>
        /// 含缓冲的长度
        /// </summary>
        public abstract TSpan BufferedSpan { get; }

        /// <summary>
        ///  缓冲，如 即可推估的时间。单位：秒。
        ///  一般设为2倍间距，导航文件为2*2=4小时，Sp3为30分钟。
        /// </summary>
        public TSpan EndBuffer { get; set; }
        /// <summary>
        ///  缓冲，如 即可推估的时间。单位：秒。
        ///  一般设为2倍间距，导航文件为2*2=4小时，Sp3为30分钟。
        /// </summary>
        public TSpan StartBuffer { get; set; }

        /// <summary>
        /// 缓冲后的开始
        /// </summary>
        public abstract TValue BufferedStart { get;  }

        /// <summary>
        /// 缓冲后的结束
        /// </summary>
        public abstract TValue BufferedEnd { get;  }
        #endregion

        #region 方法 
        /// <summary>
        /// 两边缓冲设置为相同
        /// </summary>
        /// <param name="buffer"></param>
        public void SetSameBuffer(TSpan buffer) { this.StartBuffer = buffer; this.EndBuffer = buffer; }
        /// <summary>
        /// 完全包含或相等才返回true，包含缓冲部分。
        /// </summary>
        /// <param name="val">待判断时段</param>
        /// <returns></returns>
        public bool BufferedContains( TValue  val)
        {
            return this.BufferedStart.CompareTo(val) <= 0 && this.BufferedEnd.CompareTo(val) >= 0;
        }

        /// <summary>
        /// 完全包含或相等才返回true，包含缓冲部分。
        /// </summary>
        /// <param name="segment">待判断时段</param>
        /// <returns></returns>
        public bool BufferedContains(ISegment<TValue, TSpan> segment)
        {
            return this.BufferedStart.CompareTo(segment.Start) <= 0 && this.BufferedEnd.CompareTo(segment.End) >= 0;
        }
        /// <summary>
        /// 完全包含或相等才返回true，包含缓冲部分。
        /// </summary>
        /// <param name="segment">待判断时段</param>
        /// <returns></returns>
        public bool BufferedContains(BufferedSegment<TValue, TSpan> segment)
        {
            return this.BufferedStart.CompareTo(segment.BufferedStart) <= 0 && this.BufferedEnd.CompareTo(segment.BufferedEnd) >= 0;
        }
        /// <summary> 
        /// 是否相交，包含缓冲部分。
        /// </summary>
        /// <param name="segment">另一个段</param> 
        /// <returns></returns>
        public bool BufferedIntersect(ISegment<TValue, TSpan> segment)
        {
            return this.BufferedContains(segment.End) || this.BufferedContains(segment.Start);
        }
        /// <summary> 
        /// 是否相交，包含缓冲部分。
        /// </summary>
        /// <param name="segment">另一个段</param> 
        /// <returns></returns>
        public bool BufferedIntersect(BufferedSegment<TValue, TSpan> segment)
        {
            return this.BufferedContains(segment.BufferedStart) || this.BufferedContains(segment.BufferedEnd);
        } 
        #endregion

        public override bool Equals(object obj)
        {
            var o = obj as BufferedSegment<TValue, TSpan>;
            if( o == null) return false;
            return base.Equals(obj) && this.StartBuffer .Equals( o.StartBuffer) && this.EndBuffer.Equals(o.EndBuffer) ;
        }
         
    }      
}
