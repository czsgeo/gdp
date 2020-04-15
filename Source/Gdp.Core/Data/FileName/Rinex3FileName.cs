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
    /// RINEX 3 的文件名称
    /// </summary>
    public class Rinex3FileName
    {
        /// <summary>
        /// 解析名称
        /// </summary>
        /// <param name="fileName"></param>
        public Rinex3FileName(string fileName)
        {
            if(fileName.Length < 38)
            {
                throw new Exception("Rinex 3 至少 38字符。");
            }
            this.Name = fileName.Substring(0, 9);
            this.StationName = fileName.Substring(0, 6);
            this.ContryCode = fileName.Substring(6, 3);
            this.DataSource = fileName.Substring(10, 1);
            this.StartTime = fileName.Substring(12, 11);
            this.Interval = fileName.Substring(24, 3);
            this.ContentType = fileName.Substring(28, 3);
            this.FileFormat = fileName.Substring(32, 3);
            this.DataSource = fileName.Substring(35, 3); 
        }



        /// <summary>
        /// 默认构造函数，初始化了时间，间隔和文件类型
        /// </summary>
        public Rinex3FileName()
        {
            this.Time = Time.UtcNow;
            ContryCode = "000";
            DataSource = "S";
            Period = "01D";
            Interval = "01S";
            ContentType = "MO";
            FileFormat = "rnx";
            Compression = "crx";
            IsCompression = false;
        }
        #region 属性 
        /// <summary>
        /// 时间
        /// </summary>
        public Time Time { get; set; }
        /// <summary>
        /// 测站名称
        /// </summary>
        public string StationName { get; set; }


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
        /// 起始时间 YYYYDDDHHMM
        /// </summary>
        public string StartTime { get; set; }
        /// <summary>
        /// 起始时间 YYYYDDDHHMM
        /// </summary>
        public string Name { get; set; }
        #endregion
        /// <summary>
        /// 构建
        /// </summary> 
        /// <returns></returns>
        public override string ToString()
        { 
            string spliter = "_";
            if (String.IsNullOrWhiteSpace(Name))
            {
                Name = Gdp.Utils.StringUtil.FillZero(this.StationName, 6) + ContryCode;
            }
                
            if (String.IsNullOrWhiteSpace(StartTime))
            {
                StartTime = Time.DateTime.ToString("yyyy") + Time.DayOfYear.ToString("000") + Time.Hour.ToString("00") + Time.Minute.ToString("00");
            }
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

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Rinex3FileName Parse(string str)
        {
            return new Rinex3FileName(str);
        }

    }
}
