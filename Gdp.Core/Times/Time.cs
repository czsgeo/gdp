//2019.08.21, czs, create in hongqing, 轻量级高精度时间类

using System;
using System.Collections.Generic;
using System.Text;

namespace Gdp
{
    /// <summary>
    /// 轻量级高精度时间类
    /// </summary>
    public class Time:IComparable<Time>
    {
        public static Time MinValue = new Time(1980, 1, 1);// new Time(DateTime.MinValue);// new DateTime(1, 1, 1);
        public static Time MaxValue = new Time(3600, 1, 1);// new Time(DateTime.MaxValue);// new DateTime(9999, 1, 1);

        internal static Time Default=>new Time(new DateTime(),0);

        public Time()
        { }

        public Time(int year, int dayOfYear)
        {
            this.DateTime = new DateTime(year, 1, 1, 1,0,0,0) + TimeSpan.FromDays(dayOfYear);
        }

        public Time(int year, int month, int day, int hour=0, int minute=0, double seconds=0)
        {
            this.DateTime = new DateTime(year, month, day, hour, minute, (int)seconds, (int)(seconds%1 *1000));
            this.FractionOfMilliSecond = (seconds * 1000) % 1;
        }        
            
        public Time(DateTime dateTime, double fractionOfMSec =0)
        {
            this.DateTime = dateTime;
            this.FractionOfMilliSecond = fractionOfMSec;
        }

        /// <summary>
        /// 毫秒以下的时间，单位毫秒（1milli=0.001s）
        /// </summary>
        public double FractionOfMilliSecond { get; set; }
        /// <summary>
        /// 精度为毫秒（0.001s）的时间
        /// </summary>
        public DateTime DateTime { get; set; }
        public int DayOfYear => DateTime.DayOfYear;
        public DayOfWeek DayOfWeek => DateTime.DayOfWeek;
        public static Time UtcNow => new Time(DateTime.UtcNow);

        public int GpsWeek => (int)((this.DateTime - StartOfGpsT.DateTime).TotalDays / 7);
        public int BdsWeek => (int)((this.DateTime - StartOfBdT.DateTime).TotalDays / 7);
        public int WeekOfYear => (int)(this.DayOfYear / 7);
        public int Year => DateTime.Year;
        public int Month => DateTime.Month;
        public int Day => DateTime.Day;
        public int Hour => DateTime.Hour;
        public int Minute => DateTime.Minute;
        public int Second => DateTime.Second;
        public double Seconds => DateTime.Second + (DateTime.Millisecond + FractionOfMilliSecond) * 0.001;
        public double SecondsOfDay=>DateTime.TimeOfDay.TotalSeconds + FractionOfMilliSecond * 0.001;
        public int SubYear => Year % 100;

        public double SecondsOfWeek => ((int)DateTime.DayOfWeek * 24 * 3600) + DateTime.TimeOfDay.TotalSeconds + FractionOfMilliSecond * 0.001;

        public static Time StartOfGpsT = new Time(new DateTime(1980, 1, 6));
        public static Time StartOfBdT = new Time(new DateTime(2006, 1, 1));

        public static Time operator +(Time d, TimeSpan t)
        {
            return new Time(d.DateTime + t, d.FractionOfMilliSecond);
        }
        public static Time operator -(Time d, TimeSpan t)
        {
            return new Time(d.DateTime - t, d.FractionOfMilliSecond);
        }
        public static Time operator +(Time d, double second)
        {
            return new Time(d.DateTime + TimeSpan.FromSeconds(second), d.FractionOfMilliSecond);
        }
        public static Time operator -(Time d, double second)
        {
            return new Time(d.DateTime - TimeSpan.FromSeconds(second), d.FractionOfMilliSecond);
        }
        public static double operator -(Time left, Time right)
        {
            return ( left.DateTime -right.DateTime).TotalSeconds + ( left.FractionOfMilliSecond - right.FractionOfMilliSecond) * 0.001 ;
        }
        public static bool operator >(Time left, Time right)
        {
            return (left - right) > 0;
        }
        public static bool operator <(Time left, Time right)
        {
            return (left - right) < 0;
        }
        public static bool operator >=(Time left, Time right)
        {
            return (left - right) >= 0;
        }
        public static bool operator <=(Time left, Time right)
        {
            return (left - right) <= 0;
        }


        public static Time Parse(string key)
        {
            return TimeIoUtil.Parse(key);
            var datetime = DateTime.Parse(key);
            return new Time(datetime, 1);
        }

        public static Time Parse<TKey>(TKey key)
        {
            return Parse(key.ToString());
        }
        public static Time Parse(string [] key)
        {
            return TimeIoUtil.Parse(key);
        }

        public static Time Min(Time start, Time end)
        {
            return start < end ? start : end;
        }
        public static Time Max(Time start, Time end)
        {
            return start > end ? start : end;
        }

        public int CompareTo(Time other)
        {
            int val = this.DateTime.CompareTo(other.DateTime);
            if(val == 0)
            {
                return FractionOfMilliSecond.CompareTo(other.FractionOfMilliSecond);
            }
            return val;
        }

        internal Time TrimToTenMinute()
        {
            return new Time(DateTime.Year, DateTime.Month, DateTime.Day, DateTime.Hour, (DateTime.Minute % 10) * 10, 0);
        }

        /// <summary>
        /// 利于作为文件名称输出的字符串。
        /// </summary>
        /// <returns></returns>
        public string ToPathString(bool isWithDate = true, bool isWithTime = true, bool isWithSecond = true, string secondFormat = "00.000")
        {
            var dateSpliter = "-";
            var str = "";
            if (isWithDate)
            {
                str += Year + dateSpliter +
                  Month.ToString("00") + dateSpliter +
                  Day.ToString("00") + "_";
            }
            var timeSpliter = ".";
            if (isWithTime)
            {
                str += Hour.ToString("00") + timeSpliter +
                      Minute.ToString("00");
                if (isWithSecond)
                {
                    str += timeSpliter + Seconds.ToString(secondFormat);
                }
            }
            return str;
        }
        /// <summary>
        /// 2002-05-23 12:00:00
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(10);
            sb.Append(Year)
                .Append("-")
                .Append(Month.ToString("00"))
                .Append("-")
                .Append(Day.ToString("00"))
                .Append(" ")
                .Append(Hour.ToString("00"))
                .Append(":")
                .Append(Minute.ToString("00"))
                .Append(":")
                .Append(Second.ToString("00"));
            return sb.ToString(); 
        }

        /// <summary>
        /// 2002-05-23 12:00:00
        /// </summary>
        /// <returns></returns>
        public string ToString(bool isWithDate, bool isWithTime, bool isWithSeconds = true, string secondFormat = "00.00")
        {
            StringBuilder sb = new StringBuilder();
            if (isWithDate)
            {
                sb.Append(Year)
                    .Append("-")
                    .Append(Month.ToString("00"))
                    .Append("-")
                    .Append(Day.ToString("00"))
                    .Append(" ");
            }
            if (isWithTime)
            {
                sb.Append(Hour.ToString("00"))
                .Append(":")
                .Append(Minute.ToString("00"));
                if (isWithSeconds)
                {
                    sb.Append(":")
                      .Append(Second.ToString(secondFormat));
                }
            }
            return sb.ToString();
        }

        public void RoundFractionSecondToInt()
        {
            this.FractionOfMilliSecond = 0;
            if(DateTime.TimeOfDay.TotalSeconds % 1 != 0)
            {
                this.DateTime = new DateTime(Year, Month, Day, Hour, Minute, Second);
            }
        }
    }
}
