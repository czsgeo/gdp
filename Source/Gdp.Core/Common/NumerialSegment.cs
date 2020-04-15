//2015.01.17, czs, edit, 从Utils中移动到Common中，并让其继承Segment
//2016.10.17, czs, edit in hongqing, 更名为 NumerialSegment

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gdp
{

    /// <summary>
    /// 双精度的数据范围。
    /// </summary>
    public class NumerialSegment :Segment<Double, double>
    {
        public NumerialSegment(double start)
            : base(start, start)
        {

        }
        /// <summary>
        /// 双精度的数据范围,构造函数。
        /// </summary>
        /// <param name="start">起始数值</param>
        /// <param name="end">结束值</param>
        public NumerialSegment(double start, double end): base(start, end)
        { 
        }
        /// <summary>
        /// 检查并扩展，扩展则返回true
        /// </summary>
        /// <param name="val"></param>
        public bool CheckOrExpand(double val)
        {
            bool isAdded = false;
            if (Start > val) { Start = val; isAdded = true; }
            if (End < val) { End = val; isAdded = true; }
            return isAdded;
        }


        /// <summary>
        /// 是否在范围内，含边界。
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public override bool Contains(double val)
        {
            return val <= End && val >= Start;
        }
        /// <summary>
        /// 跨度
        /// </summary>
        public override double Span
        {
            get { return End - Start; }
        }
        /// <summary>
        /// 中间数
        /// </summary>
        public override double Median
        {
            get { return Start + End / 2.0; }
        }
        /// <summary>
        /// 一个副本
        /// </summary>
        /// <returns></returns>
        public NumerialSegment Clone() { return new NumerialSegment(this.Start, this.End); }
    }
}
