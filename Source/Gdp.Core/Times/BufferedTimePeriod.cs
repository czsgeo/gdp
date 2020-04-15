//2014.12.26, czs, edit in namu, 修改为 GpsTimePeriod 使其继承自  Segment<Time>
//2015.04.24, czs, edit in namu, 名称重构为 TimePeriod
//2015.05.11, czs, edit in namu, 名称重构为 BufferedTimePeriod

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;


namespace Gdp
{
    /// <summary>
    /// 具有缓冲的时间段。包含了起始时间，中止时间，以及对应的缓冲时间。
    /// </summary>
    public class BufferedTimePeriod : BufferedSegment<Time, Double>, IComparable<BufferedTimePeriod>
    {
        #region 构造函数
        /// <summary>
        /// 时段构造函数，以系统时间初始化
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public BufferedTimePeriod(DateTime start, DateTime end) : this(new Time(start), new Time(end)) { }
        /// <summary>
        /// 时段构造函数
        /// </summary>
        /// <param name="timePeriod"></param>
        public BufferedTimePeriod(BufferedTimePeriod timePeriod) : this(timePeriod.Start, timePeriod.End) {
            this.StartBuffer = timePeriod.StartBuffer;
            this.EndBuffer = timePeriod.EndBuffer;
        }
        /// <summary>
        /// 时段构造函数
        /// </summary>
        /// <param name="timePeriod"></param>
        public BufferedTimePeriod(TimePeriod timePeriod) : this(timePeriod.Start, timePeriod.End) { }
        /// <summary>
        /// 时段构造函数
        /// </summary>
        /// <param name="start">起始时间</param>
        /// <param name="end">结束时间</param>
        /// <param name="bufferSec">前后缓冲，秒</param>
        public BufferedTimePeriod(Time start, Time end, double bufferSec = 0) : base(start, end, bufferSec) { }
        /// <summary>
        /// 时段构造函数
        /// </summary>
        /// <param name="start">起始时间</param>
        /// <param name="end">结束时间</param>
        /// <param name="timeSpan">前后缓冲</param>
        public BufferedTimePeriod(Time start, Time end, TimeSpan timeSpan) : base(start, end, timeSpan.TotalSeconds) { }
        /// <summary>
        /// 时段构造函数
        /// </summary>
        /// <param name="start">起始时间</param>
        /// <param name="end">结束时间</param>
        /// <param name="startBuffer">起始缓冲，秒</param>
        /// <param name="endBuffer">结束缓冲，秒</param>
        public BufferedTimePeriod(Time start, Time end, double startBuffer, double endBuffer) : base(start, end, startBuffer, endBuffer) { }
        #endregion

        /// <summary>
        /// 缓冲的结束时间
        /// </summary>
        public override Time BufferedEnd { get { return End + EndBuffer; } }
        /// <summary>
        /// 缓冲的起始时间
        /// </summary>
        public override Time BufferedStart { get { return Start - StartBuffer; } }
        /// <summary>
        /// 长度，秒，不含缓冲
        /// </summary>
        public override double Span { get { return (double)(End - Start); } }
        /// <summary>
        /// 含缓冲的长度
        /// </summary>
        public override double BufferedSpan { get { return (double)(BufferedEnd - BufferedStart); } }
        /// <summary>
        /// 中间值
        /// </summary>
        public override Time Median
        {
            get { return Start + Span / 2.0; }
        }
        /// <summary>
        /// 起始时间在前的为小。
        /// 返回值的含义如下：值含义小于零此对象小于 other 参数。零此对象等于 other。大于零此对象大于 other。
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(BufferedTimePeriod other)
        {
            return (int)this.Start.CompareTo(other.Start);
        }

        /// <summary>
        /// 合并这两个时段
        /// </summary>
        /// <param name="other">另一个时段</param>
        /// <returns></returns>
        public BufferedTimePeriod Merge(BufferedTimePeriod other)
        {
            return Merge(this, other);
        }
        /// <summary>
        /// "dd HH:mm:ss"
        /// </summary>
        /// <returns></returns>
        public string ToDayAndTimeString()
        {
            var format = "dd HH:mm:ss";
            return this.Start.DateTime.ToString(format) + "->" + this.End.DateTime.ToString(format);
        }
        /// <summary>
        ///  "HH:mm:ss"
        /// </summary>
        /// <returns></returns>
        public string ToTimeString()
        {
            var format = "HH:mm:ss";
            return this.Start.DateTime.ToString(format) + "->" + this.End.DateTime.ToString(format);
        }
        /// <summary>
        ///  "yyyy-MM-dd HH:mm:ss";
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var format = "yyyy-MM-dd HH:mm:ss";
            return this.Start.DateTime.ToString(format) + "->" + this.End.DateTime.ToString(format);
        }

        public static BufferedTimePeriod operator -(BufferedTimePeriod left, TimeSpan timeSpan)
        {
            return new BufferedTimePeriod(left.Start - timeSpan, left.End - timeSpan, left.StartBuffer, left.EndBuffer);
        }
        public static BufferedTimePeriod operator +(BufferedTimePeriod left, TimeSpan timeSpan)
        {
            return new BufferedTimePeriod(left.Start + timeSpan, left.End + timeSpan, left.StartBuffer, left.EndBuffer);
        }

        #region 静态工具方法



        /// <summary>
        /// 合并两个时间段，不管中间是否具有间隙。
        /// </summary>
        /// <param name="prevObj">第一个时段</param>
        /// <param name="other">第二个时段</param>
        /// <returns></returns>
        public static BufferedTimePeriod Merge(BufferedTimePeriod first, BufferedTimePeriod other)
        {
            Time min = Time.Min(first.Start, other.Start);
            Time max = Time.Max(first.End, other.End);

            BufferedTimePeriod minPeriod = Min(first, other);
            BufferedTimePeriod maxPeriod = Max(first, other);

            return new BufferedTimePeriod(min, max, minPeriod.StartBuffer, maxPeriod.EndBuffer);
        }
        /// <summary>
        /// 返回起始时间最小的区段
        /// </summary>
        /// <param name="prevObj">时段1</param>
        /// <param name="other">时段2</param>
        /// <returns></returns>
        public static BufferedTimePeriod Min(BufferedTimePeriod first, BufferedTimePeriod other)
        {
            if (first.Start < other.Start) return first;
            return other;
        }
        /// <summary>
        /// 返回结束时间最大的区段
        /// </summary>
        /// <param name="prevObj">时段1</param>
        /// <param name="other">时段2</param>
        /// <returns></returns>
        public static BufferedTimePeriod Max(BufferedTimePeriod first, BufferedTimePeriod other)
        {
            if (first.End > other.End) return first;
            return other;
        }

        /// <summary>
        ///  合并时段。如果有相交的时段，则合并。
        /// </summary>
        /// <param name="spans">时段集合</param>
        /// <returns></returns>
        public static List<BufferedTimePeriod> Merge(List<BufferedTimePeriod> spans)
        {
            List<BufferedTimePeriod> outs = new List<BufferedTimePeriod>();

            spans.Sort();
            BufferedTimePeriod last = null;
            foreach (var item in spans)
            {
                if (last == null)
                {
                    last = item;
                    continue;
                }
                if (last.BufferedContains(item.Start))
                {
                    last = BufferedTimePeriod.Merge(last, item);
                }
                else
                {
                    outs.Add(last);
                    last = null;
                }
            }
            if (last != null) outs.Add(last);

            return outs;
        }

        /// <summary>
        /// 长度为0.
        /// </summary>
        public static BufferedTimePeriod Zero { get { return new BufferedTimePeriod(Time.MinValue, Time.MinValue); } }
        /// <summary>
        /// 最大时段
        /// </summary>
        public static BufferedTimePeriod MaxPeriod { get { return new BufferedTimePeriod(Time.MinValue, Time.MaxValue); } }

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="periodString"></param>
        /// <returns></returns>
        public static BufferedTimePeriod Parse(string periodString)
        {
            var splitter = new string[] { "->" };
            var strs = periodString.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
            if (strs.Length == 0) { throw new Exception("时间字符串未能分解" + periodString); }

            var from = DateTime.Parse(strs[0]);
            if (strs.Length == 1)
            {
                return new BufferedTimePeriod(from, from);
            }
            var to = DateTime.Parse(strs[1]);
            return new BufferedTimePeriod(from, to);
        }  
      
        /// <summary>
        /// 对时间序列进行分段
        /// </summary>
        /// <param name="times">时间</param>
        /// <param name="periodSpanSeconds"></param>
        /// <returns></returns>
        public static List<BufferedTimePeriod> GroupToPeriods(IEnumerable<Time> times, double periodSpanSeconds)
        {
            var list = times.Distinct().ToList();
            list.Sort();
            List<BufferedTimePeriod> timePeriods = new List<BufferedTimePeriod>();
            foreach (var item in list)
            {
                timePeriods.Add(new BufferedTimePeriod(item, item, periodSpanSeconds));
            }
            var result = Merge(timePeriods);
            return result;
        }
        #endregion
    }
}