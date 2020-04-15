//2016.01.24, czs, create in hongqing, 文件名称命名器
//2018.06.05, czs, edit in hmx, RINEX 3 的格式

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text; 



namespace Gdp.Data.Rinex
{

    /// <summary>
    /// 序列文件。连续的文件。
    /// </summary>
    public class RinexFileNameBuilder// : IBuilder<String, Time>, IBuilder<String>
    {
        /// <summary>
        /// 默认构造函数，初始化了时间，间隔和文件类型
        /// </summary>
        public RinexFileNameBuilder(double Version = 3.02)
        {
            this.Time = Time.UtcNow;
            this.Version = Version;
            this.TimeResolution = TimeUnit.Hour;
            this.RinexFileType = Rinex.RinexFileType.O;
            ContryCode = "000";
            DataSource = "R";
            Period = "01D";
            Interval = "01S";
            ContentType = "MO";
            FileFormat = "rnx";
            Compression = "crx";
            IsCompression = false;
        }
        #region 属性
        /// <summary>
        /// 版本
        /// </summary>
        public double Version { get; set; }
        /// <summary>
        /// 时间分辨率
        /// </summary>
        public TimeUnit TimeResolution { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public Time Time { get; set; }
        /// <summary>
        /// 测站名称
        /// </summary>
        public string StationName { get; set; }
        /// <summary>
        /// 文件类型
        /// </summary>
        public RinexFileType RinexFileType { get; set; }

        /// <summary>
        /// 设置版本
        /// </summary>
        /// <param name="Version"></param>
        /// <returns></returns>
        public RinexFileNameBuilder SetVersion(double Version)
        {
            this.Version = Version;
            return this;
        }
        /// <summary>
        /// 设置分辨率
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public RinexFileNameBuilder SetTimeResolution(TimeUnit name)
        {
            this.TimeResolution = name;
            return this;
        }
        /// <summary>
        /// 设置时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public RinexFileNameBuilder SetTime(Time time)
        {
            this.Time = time;
            return this;
        }
        /// <summary>
        /// 设置测站名称
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public RinexFileNameBuilder SetSationName(string name)
        {
            this.StationName = name;
            return this;
        }
        /// <summary>
        /// 设置文件类型
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public RinexFileNameBuilder SetRinexFileType(RinexFileType name)
        {
            this.RinexFileType = name;
            return this;
        }
        #endregion
        #region 3.0 Only

        /// <summary>
        /// 1 Character – defining the data source
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// 3 Characters - defining the intended (nominal)file period
        /// string Period = "01D";
        /// </summary>
        public string Interval { get; set; }
        /// <summary>
        /// 3 Characters - defining the intended   (nominal) file period
        /// </summary>
        public string Period { get; set; }
        /// <summary>
        /// USA CAN etc
        /// </summary>
        public string ContryCode { get; set; }
        /// <summary>
        /// 1 Character – defining the data source
        /// </summary>
        public string DataSource { get; set; }
        /// <summary>
        /// 3 Characters –   defining the file format
        /// </summary>
        public string FileFormat { get; set; }
        /// <summary>
        /// Char. - defining the compression method
        /// </summary>
        public string Compression { get; set; }
        /// <summary>
        /// 是否为压缩文件
        /// </summary>
        public bool IsCompression { get; set; }

        /// <summary>
        /// 设置 
        /// </summary>
        /// <param name="Interval"></param>
        /// <returns></returns>
        public RinexFileNameBuilder SetInterval(string Interval)
        {
            this.Interval = Interval;
            return this;
        }
        /// <summary>
        /// 设置 
        /// </summary>
        /// <param name="ContentType"></param>
        /// <returns></returns>
        public RinexFileNameBuilder SetContentType(string ContentType)
        {
            this.ContentType = ContentType;
            return this;
        }
        /// <summary>
        /// 设置 
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public RinexFileNameBuilder SetPeriod(string Period)
        {
            this.Period = Period;
            return this;
        }
        /// <summary>
        /// 设置 
        /// </summary>
        /// <param name="ContryCode"></param>
        /// <returns></returns>
        public RinexFileNameBuilder SetContryCode(string ContryCode)
        {
            this.ContryCode = ContryCode;
            return this;
        }
        /// <summary>
        /// 设置 
        /// </summary>
        /// <param name="DataSource"></param>
        /// <returns></returns>
        public RinexFileNameBuilder SetDataSource(string DataSource)
        {
            this.DataSource = DataSource;
            return this;
        }
        /// <summary>
        /// 设置 
        /// </summary>
        /// <param name="FileFormat"></param>
        /// <returns></returns>
        public RinexFileNameBuilder SetFileFormat(string FileFormat)
        {
            this.FileFormat = FileFormat;
            return this;
        }
        /// <summary>
        /// 设置 
        /// </summary>
        /// <param name="Compression"></param>
        /// <returns></returns>
        public RinexFileNameBuilder SetCompression(string Compression)
        {
            this.Compression = Compression;
            return this;
        }
        /// <summary>
        /// 设置 
        /// </summary>
        /// <param name="IsCompression"></param>
        /// <returns></returns>
        public RinexFileNameBuilder SetIsCompression(bool IsCompression)
        {
            this.IsCompression = IsCompression;
            return this;
        }
        #endregion


        public string Build(RinexObsFile file)
        {
            var header = file.Header;
            file.UpdateAndGetHeaderTimePeriodWithContentTime();
            file.CheckOrUpdateAndGetHeaderIntervalWithContent();
            file.Header.TrySetAndGetContryCodeWithFileName();

            return Build(header);
        }

        public string Build(RinexObsFileHeader header)
        {
            this.StationName = header.MarkerName;

            this.TimeResolution = TimeUnit.Hour;
            this.RinexFileType = Rinex.RinexFileType.O;
            ContryCode = header.CountryCode;
            DataSource = "R";
            Period = GetPeriodCode(header.TimePeriod.TimeSpan);
            Interval = header.Interval.ToString("00") + "S";
            ContentType = header.SatTypeMarker + "O";
            FileFormat = "rnx";
            Compression = "crx";
            IsCompression = false;

            return Build(header.StartTime);
        }

        public string GetPeriodCode(TimeSpan span)
        { 
            if (span.TotalSeconds < 60)
            {
                return  Math.Round(span.TotalSeconds).ToString("00") + "S";
            }
            if (span.TotalMinutes < 60)
            {
                return Math.Round(span.TotalMinutes).ToString("00") + "M";
            }
            if (span.TotalHours < 24)
            {
                return Math.Round(span.TotalHours).ToString("00") + "H";
            }
            return Math.Round(span.TotalDays).ToString("00") + "D";
        }


        /// <summary>
        /// 构建
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public string Build(Time time)
        {
            this.Time = time;
            if (Version < 3)
            {
                int nameLen = 8;
                if (TimeResolution == TimeUnit.Minute)
                    nameLen = 10;
                if (TimeResolution == TimeUnit.Second)
                    nameLen = 12;
                FileName name = new FileName(StationName, time, RinexFileType, nameLen);
                return name.ToString();
            }
            else
            {
                string spliter = "_";
                //9 Characters – defining the site, station and country code
                string Name = Gdp.Utils.StringUtil.FillZero(this.StationName, 6) + ContryCode;
                //1 Character – defining the data source
                //  string dataSource = "S";
                //11 Characters - defining YYYYDDDHHMM
                string StartTime = time.DateTime.ToString("yyyy") + time.DayOfYear.ToString("000") + time.Hour.ToString("00") + time.Minute.ToString("00");
                //3 Characters - defining the intended (nominal)file period
                // string Period = "01D";
                //3 Characters – defining the                observation frequency
                // string Interval = "01S";

                // 2 Characters –defining the content type
                //  string ContentType = "MO";
                //3 Characters –   defining the file format
                //   string FileFormat ="rnx";
                //Char. - defining the compression method
                //  string Compression = "gz";
                StringBuilder sb = new StringBuilder();
                sb.Append(Name);
                sb.Append(spliter);
                sb.Append(DataSource);
                sb.Append(spliter);
                sb.Append(StartTime);
                sb.Append(spliter);
                sb.Append(Period);
                sb.Append(spliter);
                sb.Append(Interval);
                sb.Append(spliter);
                sb.Append(ContentType);
                sb.Append(".");
                sb.Append(FileFormat);

                if (IsCompression)
                {
                    sb.Append(".");
                    sb.Append(Compression);
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// 构建
        /// </summary>
        /// <returns></returns>
        public string Build()
        {
            return Build(Time);
        }
    }

}