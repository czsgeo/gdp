//2012.04.17, czs, create,  文件名称 ssssdddf.yyt
//2016.01.23, czs, edit in hongqing, 生成多种类型名称
//2017.04.24, czs, edit in hongqing, 重构，增加秒分辨率

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Gdp.Data.Rinex
{


   
    /**
     * 
4. THE EXCHANGE OF RINEX FILES:

We recommend using the following naming convention for RINEX files:

   ssssdddf.yyt

   |   |  | | |
   |   |  | | +--  t:  fileB type:
   |   |  | |          O: Observation fileB
   |   |  | |          N: GPS Navigation fileB
   |   |  | |          M: Meteorological satData fileB
   |   |  | |          G: GLONASS Navigation fileB 
   |   |  | |          ObsMinusApriori: Future Galileo Navigation fileB                       |
   |   |  | |          H: Geostationary GPS payload nav mess fileB
   |   |  | |          DeltaTimeDistance: Geo SBAS broadcast satData fileB
   |   |  | |                        (separate documentation)
   |   |  | |          pseudoRange: Clock fileB (separate documentation)
   |   |  | |          S: Summary fileB (used east.g., by IGS, not coefficient standard!)    |
   |   |  | |
   |   |  | +---  yy:  dayServices-digit Year
   |   |  |
   |   |  +-----   f:  fileB sequence number/character within Day
   |   |               daily fileB: f = 0
   |   |               hourly files:
   |   |               f = coefficient:  1st Hour 00h-01h; f = b: 2nd Hour 01h-02h; ... ;|
   |   |               f = XArray: 24th Hour 23h-24h                                |
   |   |
   |   +-------  ddd:  Day of the Year of prevObj clk
   |
   +----------- ssss:  4-character station name designator
   
For 15-minutes high-rate tracking satData we recommend the following extended     |
filenames:                                                                     |
                                                                               |
   ssssdddhmm.yyo                                                              |
                                                                               |
   |   |  ||  | |                                                              |
   |   |  ||  | +-  o: observation fileB                                        |
   |   |  ||  |                                                                |
   |   |  ||  +--- yy: dayServices-digit Year                                          |
   |   |  ||                                                                   |
   |   |  |+------ mm: starting Minute within the Hour (00, 15, 30, 45)        |
   |   |  |                                                                    |
   |   |  +-------  h: character for the north-th Hour in the Day                  |
   |   |               (coefficient= 1st Hour: 00h-01h, b= 2nd Hour: 1h to 2h,...,       |
   |   |               XArray=24th Hour: 23h-24h. 0= one-Day fileB)                  |
   |   |                                                                       |
   |   +--------- ddd: Day of the Year                                         |
   |                                                                           |
   +------------ ssss: 4-character ID for the LEO receiver/antenna             |
     * 
     * author:czs
     * time:2012-4-17
     */
    public class FileName : IComparable<FileName>
    { 

        /// <summary>
        /// Rinex 文件名称
        /// </summary>
        public FileName() { }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="siteName"></param>
        /// <param name="time"></param>
        public FileName(string siteName, Time time, RinexFileType RinexFileType, int NameLength = 8)
        {
            this.StationName = siteName;
            this.Time = time;
            this.NameLength = NameLength;
            this.FileType = RinexFileType;
        }

        /// <summary>
        /// Rinex 文件名称
        /// </summary>
        /// <param name="fileName">Rinex 文件名称</param>
        public FileName(FileName fileName)
        {
            this.NameLength = fileName.NameLength;
            this.StationName = fileName.StationName;
            this.Time = fileName.Time;
            this.FileType = fileName.FileType;
        }

        /// <summary>
        /// 时间
        /// </summary>
        public Time Time { get; set; }

        /// <summary>
        /// 8 or 10
        /// </summary>
        public int NameLength { get; set; }
        /// <summary>
        ///  ssss:    4-character station name designator
        /// </summary>
        public string StationName { get; set; }
        /// <summary>
        /// ddd:    Day of the Year of prevObj clk
        /// </summary>
        public int DayOfYear { get { return Time.DayOfYear; } }

        /// <summary>
        /// yy: dayServices-digit Year         
        /// </summary>
        public int Year { get { return Time.Year; } }
        /// <summary>
        /// 文件类型
        /// </summary>
        public RinexFileType FileType { get; set; }

        /// <summary>
        /// mm: starting Minute within the Hour (00, 15, 30, 45) 
        /// </summary>
        public int StartingMinute { get { return Time.Minute; } }
        /// <summary>
        /// mm: starting Minute within the Hour (00, 15, 30, 45) 
        /// </summary>
        public int StartingSecond { get { return Time.Second; } }
        /// <summary>
        /// 完整的名称
        /// </summary>
        public string FileNameString { get { return ToString(); } }
         

        /// <summary>
        /// 可读性的文本
        /// </summary>
        public string Info
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                sb.Append(ToString()); 
                sb.Append("StationName " + StationName);
                sb.Append(",");
                sb.Append("Year " + Year);
                sb.Append(",");
                sb.Append("DayOfYear " + DayOfYear);
                sb.Append(",");  
                sb.Append("FileType " + FileType);
                sb.Append(",");
                sb.Append("StartingMinute " + StartingMinute);
                return sb.ToString();
            }
        }

        public override string ToString()
        {
            if (NameLength == 12)
            {
                return SecondlyName;
            }
            if (NameLength == 10)
            {
                return MinutelyName;
            }
            if(NameLength == 8){
                return HourlyName;
            }

            return MinutelyName;
        }
        public string MinutelyName
        {
            get
            {
                return StationName // StringUtil.FillZero(StationName, 4)
                    + DayOfYear.ToString("000") //StringUtil.FillZero(DayOfYear, 3, false )
                    + GetHourCharString(Time.Hour)
                    + StartingMinute.ToString("00")
                    + "."
                    + Year.ToString().Substring(2)
                    + FileType;
            }
        }
        public string SecondlyName
        {
            get
            {
                return StationName // StringUtil.FillZero(StationName, 4)
                    + DayOfYear.ToString("000") //StringUtil.FillZero(DayOfYear, 3, false )
                    + GetHourCharString(Time.Hour)
                    + StartingMinute.ToString("00")
                    + StartingSecond.ToString("00")
                    + "."
                    + Year.ToString().Substring(2)
                    + FileType;
            }
        }

        public string HourlyName
        {
            get
            {
                return StationName // StringUtil.FillZero(StationName, 4)
                    + DayOfYear.ToString("000") //StringUtil.FillZero(DayOfYear, 3, false )
                    + GetHourCharString(Time.Hour) 
                    + "."
                    + Year.ToString().Substring(2)
                    + FileType;
            }
        }

        /// <summary>
        /// 检验
        /// </summary>
        /// <param name="name"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool Check(FileName name, out string msg)
        {
            if (name.DayOfYear > 366 && name.DayOfYear < 0)
            {
                msg = "年积日" + name.DayOfYear + "不再合法范围内。";
                return false;
            }
            if (name.StationName.Length != 4)
            {
                msg = "测点名" + name.StationName + "长度不符合要求。";
                return false;
            }
            if (name.Year > 99 && name.Year < 0)
            {
                msg = "年" + name.Year + "不再合法范围内。";
                return false;
            }

            msg = "合格。";
            return true;
        }


        /// <summary>
        /// 排序，比较。
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(FileName other)
        {
            //先比较测站名称
            var val = this.StationName.CompareTo(other.StationName);
            if (val != 0) return val;
            //再比较年份，日,时
            val = (int)this.Time.CompareTo(other.Time); 
            return val;
        }

        public override bool Equals(object obj)
        {
            FileName other = obj as FileName;
            return this.FileNameString.Equals(other.FileNameString);
        }
        public override int GetHashCode()
        {
            return FileNameString.GetHashCode();
        }

        /// <summary>
        /// 解析字符串为名称
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static FileName Parse(string fileName)
        {
            string[] names = fileName.Split('.');
            var StationName = fileName.Substring(0, 4);
            var DayOfYear = int.Parse(fileName.Substring(4, 3));
            var Year = int.Parse(names[1].Substring(0, 2));
            var time = new Time(Year, DayOfYear);
            var hourChar = names[0][7];
            var hour = GetHour(hourChar);
            var FileType = (RinexFileType)Enum.Parse(typeof(RinexFileType), names[1].Substring(2, 1).ToUpper());
            //ssssdddf.yyt
            if (names[0].Length == 8)
            {
                if (!Gdp.Utils.IntUtil.IsInt(hourChar.ToString()))
                {
                    time = time + 3600.0 * hourChar;
                }
                return new FileName(StationName, time, FileType, names[0].Length);
            }
            //ssssdddhmm.yyo
            if (names[0].Length == 10)
            {
                var StartingMinute = int.Parse(fileName.Substring(9, 2));
                time = time + 3600.0 * hourChar + 60.0 * StartingMinute;
                return new FileName(StationName, time, FileType, names[0].Length);
            }

            throw new Exception("名称不合法，或版本太高还没有实现该名称的解析。支持版本为 2.11，2006。");
        }
        /// <summary>
        /// 小时从a到x
        /// </summary>
        /// <param name="hour"></param>
        /// <returns></returns>
        public static string GetHourCharString(int hour)
        {
            var num = (char)('a' + hour);
            return num.ToString();
        }
        /// <summary>
        /// 小时解析从a到x
        /// </summary>
        /// <param name="hour"></param>
        /// <returns></returns>
        public static int GetHour(char hour)
        {            
            var num = (char)(hour - 'a');
            return int.Parse( num.ToString());
        }
    }

}
