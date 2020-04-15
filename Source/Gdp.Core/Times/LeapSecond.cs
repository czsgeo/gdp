//2017.08.05, czs, create in hongiqng, LeapSecond 闰秒/跳秒类的设计
 
using System.Collections.Generic;
using System.Text;
using System;
using System.Linq; 

namespace Gdp
{ 
    /// <summary>
    /// 闰秒/跳秒类的设计
    /// </summary>
    public class LeapSecond
    {

        public Dictionary<DateTime, int> Data;
        static public LeapSecond Instance = new LeapSecond();
        private LeapSecond()
        {
            this.Data = new Dictionary<DateTime,int>(); 
           this.Data.Add(DateTime.Parse("2017-01-01 00:00:00"), 27);
           this.Data.Add(DateTime.Parse("2015-07-01 00:00:00"), 26);
           this.Data.Add(DateTime.Parse("2012-07-01 00:00:00"), 25);
           this.Data.Add(DateTime.Parse("2009-01-01 00:00:00"), 24);
           this.Data.Add(DateTime.Parse("2006-01-01 00:00:00"), 23);
           this.Data.Add(DateTime.Parse("1999-01-01 00:00:00"), 22);
           this.Data.Add(DateTime.Parse("1997-07-01 00:00:00"), 21);
           this.Data.Add(DateTime.Parse("1996-01-01 00:00:00"), 20);
           this.Data.Add(DateTime.Parse("1994-07-01 00:00:00"), 19);
           this.Data.Add(DateTime.Parse("1993-07-01 00:00:00"), 18);
           this.Data.Add(DateTime.Parse("1992-07-01 00:00:00"), 17);
           this.Data.Add(DateTime.Parse("1991-01-01 00:00:00"), 16);
           this.Data.Add(DateTime.Parse("1990-01-01 00:00:00"), 15);
           this.Data.Add(DateTime.Parse("1988-01-01 00:00:00"), 14);
           this.Data.Add(DateTime.Parse("1985-07-01 00:00:00"), 13);
           this.Data.Add(DateTime.Parse("1983-07-01 00:00:00"), 12);
           this.Data.Add(DateTime.Parse("1982-07-01 00:00:00"), 11);
           this.Data.Add(DateTime.Parse("1981-07-01 00:00:00"), 10);
           this.Data.Add(DateTime.Parse("1980-01-01 00:00:00"), 9);
           this.Data.Add(DateTime.Parse("1979-01-01 00:00:00"), 8);
           this.Data.Add(DateTime.Parse("1978-01-01 00:00:00"), 7);
           this.Data.Add(DateTime.Parse("1977-01-01 00:00:00"), 6);
           this.Data.Add(DateTime.Parse("1976-01-01 00:00:00"), 5);
           this.Data.Add(DateTime.Parse("1975-01-01 00:00:00"), 4);
           this.Data.Add(DateTime.Parse("1974-01-01 00:00:00"), 3);
           this.Data.Add(DateTime.Parse("1973-01-01 00:00:00"), 2);
           this.Data.Add(DateTime.Parse("1972-07-01 00:00:00"), 1);
        }
        /// <summary>
        /// 起始跳秒时刻
        /// </summary>
        public static DateTime MinLeapTime = DateTime.Parse("1972-07-01 00:00:00");
        /// <summary>
        /// 当前最大跳秒时刻
        /// </summary>
        public static DateTime MaxLeapTime = Instance.Data.Keys.Max();
        /// <summary>
        /// GPST起始时间的跳秒数量。
        /// </summary>
        public static int OfStartOfGpsT = Instance.GetLeapSecond(Time.StartOfGpsT.DateTime);
        /// <summary>
        /// 北斗起始时间的跳秒数量
        /// </summary>
        public static int OfStartOfBdT = Instance.GetLeapSecond(Time.StartOfBdT.DateTime);
        /// <summary>
        /// GPS起始时间和BDS起始时间之间的跳秒数量。
        /// </summary>
        public static int BeTweenBdTAndGpsT = OfStartOfBdT - OfStartOfGpsT;

        /// <summary>
        /// 获取两个时刻内是否有闰秒
        /// </summary>
        /// <param name="startUtc"></param>
        /// <param name="endUtc"></param>
        /// <returns></returns>
        public int GetLeapSecondBetween(DateTime startUtc, DateTime endUtc)
        {
            var start = GetLeapSecond(startUtc);
            var end = GetLeapSecond(endUtc);       
            return end - start;
        }
        /// <summary>
        /// 自GPST起始时间以来到指定时刻发生的跳秒。
        /// </summary>
        /// <param name="timeUtc"></param>
        /// <returns></returns>
        public int GetLeapSecondFromGpsT(DateTime timeUtc)
        {
            if (timeUtc < Time.StartOfGpsT.DateTime) { throw new ArgumentException("时间不可小于 " + Time.StartOfGpsT); }
            return GetLeapSecond(timeUtc) - OfStartOfGpsT;
        }
        /// <summary>
        /// 获取UTC跳秒
        /// </summary>
        /// <param name="timeUtc"></param>
        /// <returns></returns>
        public int GetLeapSecond(DateTime timeUtc)
        {
            if (timeUtc < MinLeapTime) { return 0; }
            if (timeUtc >= MaxLeapTime) { return this.Data[MaxLeapTime]; }

            //在中间
            var dates = this.Data.Keys.OrderByDescending(m=>m);
            foreach (var date in dates)
            {
                if (timeUtc >= date) { return this.Data[date]; }                
            }
            throw new Exception("不可能出现的错误出现了！");
        }

    }
}
