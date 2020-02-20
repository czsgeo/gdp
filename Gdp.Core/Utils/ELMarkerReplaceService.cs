//2015.05.10, czs, create in namu, 路径服务
//2019.01.06, czs, edit in hmx, 增加时间分辨率，修复IGS超快星历访问

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace Gdp
{
    //2017.08.18, czs, create in hongqing,  EL 关键字
    /// <summary>
    ///  EL 关键字
    /// </summary>
    public class ELMarker
    {
        #region EL 关键字

        public const string Year = "{Year}";
        public const string SubYear = "{SubYear}";
        public const string Month = "{Month}";
        public const string Day = "{Day}";
        public const string Week = "{Week}";
        public const string BdsWeek = "{BdsWeek}";
        public const string WeekOfYear = "{WeekOfYear}";
        public const string DayOfYear = "{DayOfYear}";
        public const string DayOfWeek = "{DayOfWeek}";
        public const string Hour = "{Hour}";
        public const string Minute = "{Minute}";
        public const string Second = "{Second}";
        public const string SourceName = "{SourceName}";
        public const string SiteName = "{SiteName}";
        public const string ProductType = "{ProductType}";
        public const string FileDirectory = "{FileDirectory}";
        public const string UrlDirectory = "{UrlDirectory}";
        public const string FileName = "{FileName}";

        #endregion
    }

    /// <summary>
    /// EL 标签替换器
    /// </summary>
    public class ELMarkerReplaceService : AbstractService<List<string>, List<string>>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dictionary"></param>
        public ELMarkerReplaceService(Dictionary<string, string> dictionary, int HourInterval = 6)
        { 
            this.Dictionary = dictionary;
            this.HourInterval = HourInterval;
        }
        /// <summary>
        /// 字典
        /// </summary>
        public Dictionary<string, string> Dictionary { get; set; }
        /// <summary>
        /// 小时间隔
        /// </summary>
        public int HourInterval { get; set; }
        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="Models"></param>
        /// <returns></returns>
        public override List<string> Get(List<string> Models)
        {
            List<string> results = new List<string>();
            foreach (var model in Models)
            {
                string mo = Get(model);

                results.Add(mo);
            }
            return (results);

        }

        public string Get(string model)
        {
            string mo = model;
            foreach (var kv in Dictionary)
            {
                mo = mo.Replace(kv.Key, kv.Value);
            }
            return mo;
        }

        /// <summary>
        /// 对时间解析替换
        /// </summary>
        /// <param name="urlModels"></param>
        /// <param name="timeFrom"></param>
        /// <param name="timeTo"></param>
        /// <param name="intervalSpan"></param>
        /// <returns></returns>
        public static List<string> BuildWithTime(List<string> urlModels, DateTime timeFrom, DateTime timeTo, TimeSpan intervalSpan)
        {
            List<string> total = new List<string>();

            for (DateTime time = timeFrom; time <= timeTo; time += intervalSpan)
            {
                total.AddRange(BuildWithTime(urlModels, new Time(time)));
            }
            return total;
        }
        /// <summary>
        /// 对时间解析替换
        /// </summary>
        /// <param name="urlModels"></param>
        /// <param name="timeFrom"></param>
        /// <param name="timeTo"></param>
        /// <param name="intervalSpan"></param>
        /// <returns></returns>
        public static List<string> BuildWithTime(string urlModels, DateTime timeFrom, DateTime timeTo, TimeSpan intervalSpan)
        {
            return BuildWithTime(new List<string>() { urlModels }, timeFrom, timeTo, intervalSpan);
        }
        /// <summary>
        /// 将key替换成keyValues
        /// </summary>
        /// <param name="urlModels"></param>
        /// <param name="eLKey"></param>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        public static List<string> BuildWithKeys(List<string> urlModels, string eLKey, string [] keyValues)
        {
            if(keyValues.Length ==0) { return urlModels; }

            List<string> pathes = new List<string>();
            foreach (var item in keyValues)
            {
                foreach (var model in urlModels)
                {
                    string path = model.Replace(eLKey, item);
                    pathes.Add(path);
                }

            }
            return pathes; 
        }

        /// <summary>
        /// 对时间解析替换
        /// </summary>
        /// <param name="urlModels"></param>
        /// <param name="ifrom"></param>
        /// <returns></returns>
        public static List<string> BuildWithTime(List<string> urlModels, Time ifrom)
        {
            List<string> subPathes = new List<string>();
            foreach (var model in urlModels)
            {
                string path = BuildWithTime(model, ifrom);
                subPathes.Add(path);
            }
            return subPathes;
        }
        /// <summary>
        /// 替换一个
        /// </summary>
        /// <param name="time"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string BuildWithTime( string model, Time time, int HourInterval = 6, int minutInterval = 1, int secondInterval = 1)
        {
            var dic = GetTimeKeyWordDictionary(time, HourInterval, minutInterval, secondInterval);

            var replacer = new ELMarkerReplaceService(dic,   HourInterval);
             return  replacer.Get(model); 
        }
        /// <summary>
        /// Key 为时间 EL
        /// </summary>
        /// <param name="ifrom"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetTimeKeyWordDictionary(Time ifrom, int HourInterval = 6, int minutInterval = 1, int secondInterval = 1)
        {
            Dictionary<string, string> Dictionary = new Dictionary<string, string>();
            Dictionary.Add(ELMarker.Year, ifrom.Year.ToString("0000"));
            Dictionary.Add(ELMarker.SubYear, ifrom.Year.ToString("0000").Substring(2));
            Dictionary.Add(ELMarker.Month, ifrom.Month.ToString("00"));
            Dictionary.Add(ELMarker.Day, ifrom.Day.ToString("00"));
            Dictionary.Add("{Week}", ifrom.GpsWeek.ToString("0000"));
            Dictionary.Add("{BdsWeek}", ifrom.BdsWeek.ToString("0000"));
            Dictionary.Add("{WeekOfYear}", ifrom.WeekOfYear.ToString("00"));
            Dictionary.Add("{DayOfWeek}", ((int)ifrom.DayOfWeek) + "");
            Dictionary.Add("{DayOfYear}", ifrom.DayOfYear.ToString("000"));

            var hour = (ifrom.Hour / HourInterval) * HourInterval;
            Dictionary.Add("{Hour}", hour.ToString("00"));

            var minute = (ifrom.Minute / minutInterval) * minutInterval;
            Dictionary.Add("{Minute}", minute.ToString("00"));

            var second = (ifrom.Second / secondInterval) * secondInterval;
            Dictionary.Add("{Second}", second.ToString("00"));

            return Dictionary;
        }

    }
}