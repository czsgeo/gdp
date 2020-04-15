//2014.12.26, czs, edit in namu, 修改为 GpsTimePeriod 使其继承自  Segment<Time>
//2015.04.24, czs, edit in namu, 名称重构为 TimePeriod

using System;
using System.Collections.Generic;
using System.Linq;

namespace Gdp
{
    /// <summary>
    /// 一个时间段。包含了起始时间和中止时间。
    /// </summary>
    public class TimePeriod : Segment<Time, Double>, IComparable<TimePeriod>
    {
        #region 构造函数
        /// <summary>
        /// 时段构造函数，以系统时间初始化
        /// </summary>
        /// <param name="start"></param>
        /// <param name="span"></param>
        public TimePeriod(DateTime start, TimeSpan span) : this(start, start + span) { }
        /// <summary>
        /// 时段构造函数，以系统时间初始化
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public TimePeriod(DateTime start, DateTime end) : base(new Time(start), new Time(end)) { }
        /// <summary>
        /// 时段构造函数，以时间初始化
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public TimePeriod(Time start, Time end) : base((start), (end)) { }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="commonPeriod"></param>
        public TimePeriod(Segment<Time> commonPeriod) : this(commonPeriod.Start, commonPeriod.End)
        {

        }
        #endregion
        /// <summary>
        /// 起始时间
        /// </summary>
        public DateTime StartDateTime { get { return Start.DateTime; } }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndDateTime { get { return End.DateTime; } }
        /// <summary>
        /// 时段
        /// </summary>
        public TimeSpan TimeSpan { get { return TimeSpan.FromSeconds(Span); } }
        /// <summary>
        /// 长度，不含缓冲.
        /// </summary>
        public override double Span { get { return (double)(End - Start); } }

        /// <summary>
        /// 扩展时段
        /// </summary>
        /// <param name="timePeriod"></param>
        /// <returns></returns>
        public TimePeriod Exppand(TimePeriod timePeriod)
        {
            var start = this.Start < timePeriod.Start ? this.Start : timePeriod.Start;
            var end = this.End > timePeriod.End ? this.End : timePeriod.End;
            return new TimePeriod(start, end);
        }
        /// <summary>
        /// 扩展时段
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public TimePeriod Exppand(Time time)
        {
            var start = this.Start < time ? this.Start : time;
            var end = this.End > time ? this.End : time;
            return new TimePeriod(start, end);
        }
        /// <summary>
        /// 扩展时段
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public TimePeriod ExppandSelf(Time time)
        {
            var start = this.Start < time ? this.Start : time;
            var end = this.End > time ? this.End : time;
            this.Start = start;
            this.End = end;
            return this;
        }
        /// <summary>
        /// 扩展时段
        /// </summary>
        /// <param name="timePeriod"></param>
        /// <returns></returns>
        public TimePeriod ExppandSelf(TimePeriod timePeriod)
        {
            var start = this.Start < timePeriod.Start ? this.Start : timePeriod.Start;
            var end = this.End > timePeriod.End ? this.End : timePeriod.End;
            this.Start = start;
            this.End = end;
            return this;
        }
        /// <summary>
        /// 切割成等长的时段。
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<TimePeriod> Split(int count)
        {
            var list = new List<TimePeriod>();
            var span = this.Span / count;

            for (int i = 0; i < count; i++)
            {
                var start = this.Start + TimeSpan.FromSeconds(i * span);
                var end = start + TimeSpan.FromSeconds(span);
                TimePeriod TimePeriod = new TimePeriod(start, end);
                list.Add(TimePeriod);
            }
            return list;
        }
        /// <summary>
        /// 中间数
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
        public int CompareTo(TimePeriod other)
        {
            return (int)this.Start.CompareTo(other.Start);
        }

        /// <summary>
        /// 长度为0.
        /// </summary>
        public static TimePeriod Zero { get { return new TimePeriod(Time.MaxValue, Time.MinValue); } }
        /// <summary>
        /// 最大时段
        /// </summary>
        public static TimePeriod MaxPeriod { get { return new TimePeriod(Time.MinValue, Time.MaxValue); } }

        /// <summary>
        /// 最大时间
        /// </summary>
        /// <param name="timePeriods"></param>
        /// <returns></returns>
        public static Time MaxTime(List<TimePeriod> timePeriods)
        {
            Time time = Time.MinValue;
            foreach (var item in timePeriods)
            {
                if (time < item.End) { time = item.End; }
            }
            return time;
        }

        /// <summary>
        /// 最大时间
        /// </summary>
        /// <param name="timePeriods"></param>
        /// <returns></returns>
        public static Time MinTime(List<TimePeriod> timePeriods)
        {
            Time time = Time.MaxValue;
            foreach (var item in timePeriods)
            {
                if (time > item.Start) { time = item.Start; }
            }
            return time;
        }
        /// <summary>
        /// 分隔符
        /// </summary>
        public const String Spliter = ">";


        /// <summary>
        /// 字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var min = Time.Min(Start, End);
            var max = Time.Max(Start, End);

            string str = min.ToString() + Spliter + max.ToString();

            // base.ToString() + "(" + TimeSpan.FromSeconds(Span).ToString() + ")";
            //if(Tag != null)//不再继续显示，如果需要，请单独输出//2018.11.27, czs, hmx, 
            //{
            //    str += ", " + Tag;
            //}
            return str;
        }
        /// <summary>
        /// 字符串
        /// </summary>
        /// <param name="isWithDate"></param>
        /// <param name="isWithTime"></param>
        /// <param name="isWithSeconds"></param>
        /// <param name="isLaterWithDate"></param>
        /// <param name="secondFormat">00.0000</param>
        /// <returns></returns>
        public string ToString(bool isWithDate, bool isWithTime, bool isWithSeconds = true, bool isLaterWithDate = false, string secondFormat = "00")
        {
            var min = Time.Min(Start, End);
            var max = Time.Max(Start, End);

            string str = min.ToString(isWithDate, isWithTime, isWithSeconds, secondFormat) + ">" + max.ToString(isLaterWithDate, isWithTime, isWithSeconds, secondFormat);
            return str;
        }
        /// <summary>
        /// 默认的路径
        /// </summary>
        /// <returns></returns>
        public string ToDefualtPathString()
        {
            return ToPathString(true, true, false, false, true, true);
        }
        /// <summary>
        /// 路径字符串。
        /// </summary>
        /// <param name="isWithDate">是否具有日期</param>
        /// <param name="isWithTime">是否具有时间</param>
        /// <param name="isWithSecond">是否具有秒数</param>
        /// <param name="isRoundToTenMinute">是否 四舍五入到整十分钟 -</param>
        /// <param name="isEndWithDate">结束时间是否包含日期，否则只有时间， 如 2019.01.06.01.46-03.18</param>
        /// <param name="isEndWithHourMinuteSpan">是否以小跨度结束， 如 2019.01.06.01.46_1h30m</param>
        /// <returns></returns>
        public string ToPathString(
            bool isWithDate = true, 
            bool isWithTime = true,
            bool isWithSecond = true, 
            bool isEndWithDate= true,
            bool isRoundToTenMinute=false,
            bool isEndWithHourMinuteSpan =false)
        {
            var start = Start;
            var end = End;
            if (isRoundToTenMinute)
            {
                start = Start.TrimToTenMinute();
                end = End.TrimToTenMinute();
            }
            String path = start.ToPathString(isWithDate, isWithTime, isWithSecond, "00");
            if (isEndWithHourMinuteSpan)
            {
                var span = Span;
                var hours =(int)(span / 3600);
                var minuteFloat = (span % 3600) / 60;
                var minute = (int)Math.Round((minuteFloat / 10.0)) * 10; 
                if (minute >= 60)
                {
                    hours += 1;
                    minute -= 60;
                }
                path += "_" + hours + "h" + minute.ToString("00") + "m";
            }
            else
            {
                path+= "-" + end.ToPathString(isEndWithDate, isWithTime, isWithSecond, "00");
            }
            return path;
        }

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static TimePeriod Parse(string content)
        {
            var strs = content.Split(new char[] { '>', ',', '→' }, StringSplitOptions.RemoveEmptyEntries);
            var start = Time.Parse(strs[0]);
            var end = Time.Parse(strs[1]);
            string tag = null;
            if (strs.Length > 2) { tag = strs[2]; }
            return new TimePeriod(start, end) { Tag = tag };
        }
        #region  按照时段分组

        /// <summary>
        /// 按照时段分组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="periods"></param>
        /// <param name="MinPhaseSpanMinutes"></param>
        /// <returns></returns>
        public static Dictionary<TimePeriod, List<T>> GroupToPeriods<T>(Dictionary<T, TimePeriod> periods, double MinPhaseSpanMinutes)
        {
            var timePeriods = TimePeriod.GroupToPeriods(periods.Values, MinPhaseSpanMinutes * 60);

            var result = new Dictionary<TimePeriod, List<T>>();

            foreach (var grouped in timePeriods)
            {
                var commonPeriod = grouped.Key;
                var vals = grouped.Value;
                var list = new List<T>();
                result[commonPeriod] = list;

                foreach (var kv in periods)
                {
                    if (vals.Contains(kv.Value))
                    {
                        list.Add(kv.Key);
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// 对时间段序列进行分段,key为公共时段部分
        /// </summary>
        /// <param name="times"></param>
        /// <param name="periodSpanSeconds">允许的最短公共时段</param>
        /// <returns></returns>
        public static Dictionary<TimePeriod, List<TimePeriod>> GroupToPeriods(IEnumerable<TimePeriod> times, double periodSpanSeconds)
        {
            var list = times.Distinct().ToList();
            list.Sort();//时间靠前的在前面


            var groups = new Dictionary<TimePeriod, List<TimePeriod>>();
            int length = list.Count;
            TimePeriod lastOk = null;
            List<TimePeriod> group = new List<TimePeriod>();
            for (int i = 0; i < length; i++)
            {
                TimePeriod current = list[i];
                group.Add(current);
                lastOk = current;
                for (int j = i + 1; j < length; j++)
                {
                    var next = list[j];
                    var intersect = current.GetIntersect(next);

                    if (intersect == null || Math.Abs(intersect.End - intersect.Start) < periodSpanSeconds)//没有，或小于则不要了
                    {
                        groups[lastOk] = group;
                        group = new List<TimePeriod>();

                        i = j - 1;//从下一个开始
                        break;
                    }

                    current = new TimePeriod(intersect);
                    group.Add(next);
                    lastOk = current;
                }
            }

            groups[lastOk] = group;

            return groups;
        }
        /// <summary>
        /// 最大相交,如果没，则返回 null。
        /// </summary>
        /// <param name="TimePeriods"></param>
        /// <param name="timePeriod"></param>
        /// <returns></returns>
        public static TimePeriod  GetMaxCommon(List<TimePeriod> TimePeriods, TimePeriod timePeriod)
        { 
            var result = new List<TimePeriod>();
            TimePeriod maxCommonKey = null;
            TimePeriod maxInter = null;
            foreach (var item in TimePeriods)
            {
                var inter = item.GetIntersect(timePeriod);
                if (inter == null) { continue; }

                if(maxInter == null)
                {
                    maxCommonKey = item;
                    maxInter = inter;
                }
                else if( maxInter.TimeSpan < inter.TimeSpan)
                { 
                    maxCommonKey = item;
                    maxInter = inter;
                } 
            }
            return maxCommonKey;
        }
        /// <summary>
        /// 获取所有指定个时段排列组合
        /// </summary>
        /// <param name="timePeriods"></param>
        /// <param name="differPeriodCount">可选2 或 3</param>
        /// <returns></returns>
        static public List<List<TimePeriod>> GetDifferPeriods(List<TimePeriod> timePeriods, int differPeriodCount)
        {
            var result = new List<List<TimePeriod>>();
            int length = timePeriods.Count;
            for (int i = 0; i < length; i++)
            {
                var p1 = timePeriods[i];
                for (int j = 0; j < i; j++)
                {
                    var p2 = timePeriods[j];
                    if (differPeriodCount <= 2)
                    {
                        var times = new List<TimePeriod>();
                        times.Add(p1);
                        times.Add(p2);

                        result.Add(times);
                    }
                    else//3个
                    {
                        for (int k = 0; k < j; k++)
                        {
                            var p3 = timePeriods[k];
                            var times = new List<TimePeriod>();
                            times.Add(p1);
                            times.Add(p2);
                            times.Add(p3);

                            result.Add(times);
                        }
                    }
                }
            }
            return result;
        } 

        /// <summary>
        /// 相交部分。
        /// </summary>
        /// <param name="segment"></param>
        /// <returns></returns>
        public new TimePeriod GetIntersect(ISegment<Time> segment)
        {
            var sec = base.GetIntersect(segment);
            if (sec == null) { return null; }
            return new TimePeriod(sec);

        }
        #endregion

    }
}