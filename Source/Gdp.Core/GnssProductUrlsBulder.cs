//2020.01.25, czs, create in hongqing, 实时星历获取器

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
 
using Gdp.IO; 

namespace Gdp
{ 


    public class GnssProductProvider
    {
        Log log = new Log(typeof(GnssProductProvider)); 
        GnssProductUrlsBulder GnssProductUrlsBulder { get; set; }
        static GnssProductProvider instance = new GnssProductProvider();
        static public GnssProductProvider Instance => instance;
        private GnssProductProvider()
        {
            GnssProductUrlsBulder = new GnssProductUrlsBulder(); 
        }
        public Dictionary<SatelliteType, List<string>> GetUltraSp3FilePathes(Time time)
        {

            var urls = GnssProductUrlsBulder.GetProductPath(time);

            Dictionary<SatelliteType, List<string>> result = new Dictionary<SatelliteType, List<string>>();
            foreach (var item in urls)
            {
                List<string> uri = null;
                if (item.Value.PathesExits.Count != 0)//若有，就不用下载了。
                {
                    uri = item.Value.PathesExits;
                }
                else
                {
                    uri = item.Value.ToBeDownloads;
                }
                log.Info("星历已有 " + item.Value.PathesExits.Count + ", 待下载： " + item.Value.ToBeDownloads.Count);
                //var localPathes = InputFileManager.GetLocalFilePathes(uri.ToArray(), "*.sp3");
                //result[item.Key] = localPathes;
            }

            return result;
        }



    }

    public class ProductPath
    {
        public List<string> ToBeDownloads { get; set; }
        public List<string> PathesExits { get; set; }
    }



    /// <summary>
    /// 实时星历获取器
    /// </summary>
    public class GnssProductUrlsBulder
    {

        public Dictionary<SatelliteType, List<string>> UrlTemplates { get; set; }
        public Dictionary<SatelliteType, List<string>> LocalDirectories { get; set; }
        public GnssProductUrlsBulder()
        { 
            this.UrlTemplates = new Dictionary<SatelliteType, List<string>>();
            this.UrlTemplates[SatelliteType.G] = new List<string>()
            {
                 "ftp://cddis.gsfc.nasa.gov/pub/gps/products/{GpsWeek}/igu{GpsWeek}{DayOfWeek}_00.sp3.Z",
                 "ftp://cddis.gsfc.nasa.gov/pub/gps/products/{GpsWeek}/igu{GpsWeek}{DayOfWeek}_06.sp3.Z",
                 "ftp://cddis.gsfc.nasa.gov/pub/gps/products/{GpsWeek}/igu{GpsWeek}{DayOfWeek}_12.sp3.Z",
                 "ftp://cddis.gsfc.nasa.gov/pub/gps/products/{GpsWeek}/igu{GpsWeek}{DayOfWeek}_18.sp3.Z",
            };
            this.LocalDirectories = new Dictionary<SatelliteType, List<string>>();
            this.LocalDirectories[SatelliteType.G] = new List<string>(Gdp.Setting.IgsProductLocalDirectories);
            // Gnsser.Setting.GnsserConfig.IgsProductUrlDirectories
        }

        public void SetTemplates(Time timeUtc)
        {
            var hours = timeUtc.Hour + timeUtc.Minute / 60.0;

            if (hours < 6.1) // 当天尚未更新，回滚到前一天
            {
                var models = new List<string>()
            {
                 "ftp://cddis.gsfc.nasa.gov/pub/gps/products/{GpsWeek}/igu{GpsWeek}{DayOfWeek}_18.sp3.Z",
                 "ftp://cddis.gsfc.nasa.gov/pub/gps/products/{GpsWeek}/igu{GpsWeek}{DayOfWeek}_12.sp3.Z",
            };
                var lstDay = timeUtc - TimeSpan.FromHours(24);

                var dic = ELMarkerReplaceService.GetTimeKeyWordDictionary(lstDay);
                var elService = new Gdp.ELMarkerReplaceService(dic);
                var urls = elService.Get(models);
                this.UrlTemplates[SatelliteType.G] = urls;
            }
            else if (hours < 12.1)
            {
                var models = new List<string>()
                {
                     "ftp://cddis.gsfc.nasa.gov/pub/gps/products/{GpsWeek}/igu{GpsWeek}{DayOfWeek}_18.sp3.Z",
                };
                var lstDay = timeUtc - TimeSpan.FromHours(24);

                var dic = ELMarkerReplaceService.GetTimeKeyWordDictionary(lstDay);
                var elService = new Gdp.ELMarkerReplaceService(dic);
                var urls = elService.Get(models);

                urls.Add("ftp://cddis.gsfc.nasa.gov/pub/gps/products/{GpsWeek}/igu{GpsWeek}{DayOfWeek}_00.sp3.Z");
                this.UrlTemplates[SatelliteType.G] = urls;
            }
            else if (hours < 18.1)
            {
                this.UrlTemplates[SatelliteType.G] = new List<string>()
                {
                     "ftp://cddis.gsfc.nasa.gov/pub/gps/products/{GpsWeek}/igu{GpsWeek}{DayOfWeek}_12.sp3.Z",
                     "ftp://cddis.gsfc.nasa.gov/pub/gps/products/{GpsWeek}/igu{GpsWeek}{DayOfWeek}_00.sp3.Z",
                };
            }
            else
            {
                this.UrlTemplates[SatelliteType.G] = new List<string>()
                {
                     "ftp://cddis.gsfc.nasa.gov/pub/gps/products/{GpsWeek}/igu{GpsWeek}{DayOfWeek}_18.sp3.Z",
                     "ftp://cddis.gsfc.nasa.gov/pub/gps/products/{GpsWeek}/igu{GpsWeek}{DayOfWeek}_12.sp3.Z",
                };
            }
        }

        public GnssProductUrlsBulder(Dictionary<SatelliteType, List<string>> UrlTemplates,
            Dictionary<SatelliteType, List<string>> LocalIgsProductDirectories)
        {
            this.UrlTemplates = UrlTemplates;
            this.LocalDirectories = LocalIgsProductDirectories;
        }

        public Dictionary<SatelliteType, ProductPath> GetProductPath(Time time)
        {
            this.SetTemplates(time);

            var result = new Dictionary<SatelliteType, ProductPath>();
            var prod = GetUrlsToBeDownload(time);
            foreach (var item in prod)
            {
                result[item.Key] = new ProductPath
                {
                    PathesExits = item.Value.First,
                    ToBeDownloads = item.Value.Second,
                };
            }
            return result;
        }

        public Dictionary<SatelliteType, Pair<List<string>>> GetUrlsToBeDownload(Time time)
        {
            //1、构建所有的URLS
            var totalUrls = BuildUrls(time);

            //2、已有的，获取需要下载的
            var urlsToBeDown = GetUrlsToBeDownload(totalUrls, time);

            return urlsToBeDown;
        }

        public Dictionary<SatelliteType, Pair<List<string>>> GetUrlsToBeDownload(Dictionary<SatelliteType, List<string>> totalUrls, Time time)
        {
            var tobeDownload = new Dictionary<SatelliteType, Pair<List<string>>>();
            foreach (var item in totalUrls)
            {
                var satType = item.Key;
                var urls = item.Value;
                var fileNames = GetFileNames(item.Value);
                var localDirectories = LocalDirectories[item.Key];
                //     List<string> localPathes = GetLocalPathes(satType, fileNames);
                tobeDownload[satType] = GetUrlsToBeDownload(urls, localDirectories);
            }
            return tobeDownload;
        }
        /// <summary>
        /// 第一个为依据存在的本地路径，第二为待下载的URL
        /// </summary>
        /// <param name="urls"></param>
        /// <param name="localDirectories"></param>
        /// <returns></returns>
        public Pair<List<string>> GetUrlsToBeDownload(List<string> urls, List<string> localDirectories)
        {
            List<string> urlsToBeDown = new List<string>();
            List<string> exist = new List<string>();
            foreach (var url in urls)
            {
                var existLocal = IsLocalExist(url, localDirectories);
                if (existLocal.Count > 0)
                    exist.AddRange(existLocal);
                else
                    urlsToBeDown.Add(url);
            }
            return new Pair<List<string>>(exist, urlsToBeDown);
        }

        public List<string> IsLocalExist(string url, List<string> localDirs)
        {
            var fileNames = GetFileNames(url);

            var localPathes = GetLocalPathes(fileNames, localDirs);

            return GetExistLocals(localPathes);
        }

        private static List<string> GetExistLocals(List<string> localPathes)
        {
            List<string> result = new List<string>();
            foreach (var path in localPathes)
            {
                var exist = Gdp.Utils.FileUtil.Exists(path);
                if (exist)
                {
                    result.Add(path);
                }
            }
            return result;
        }

        private List<string> GetLocalPathes(SatelliteType satType, List<string> fileNames)
        {
            var localDirs = GetValues(LocalDirectories, satType);
            return GetLocalPathes(fileNames, localDirs);
        }

        private static List<string> GetLocalPathes(List<string> fileNames, List<string> localDirs)
        {
            var localPathes = new List<string>();
            foreach (var dir in localDirs)
            {
                foreach (var name in fileNames)
                {
                    var path = Path.Combine(dir, name);
                    localPathes.Add(path);
                }
            }

            return localPathes;
        }

        public Dictionary<SatelliteType, List<string>> BuildUrls(Time time)
        {
            var dic = new Dictionary<SatelliteType, List<string>>();

            foreach (var item in UrlTemplates)
            {
                dic[item.Key] = BuildUrls(item.Value, time);
            }
            return dic;
        }

        public Dictionary<string, List<string>> GetFileUrls(List<string> allUrls)
        {
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            foreach (var url in allUrls)
            {
                var fileName = Path.GetFileName(url);
                if (!result.ContainsKey(fileName))
                {
                    result[fileName] = new List<string>();
                }
                result[fileName].Add(url);
            }
            return result;
        }

        /// <summary>
        /// 构建远程路径
        /// </summary>
        /// <param name="time"></param>
        /// <param name="urlModel"></param>
        /// <returns></returns>
        private List<string> BuildUrls(List<string> urlModel, Time time)
        {
            Dictionary<string, string> dic = ELMarkerReplaceService.GetTimeKeyWordDictionary(time);
            //   dic.Add(ELMarker.ProductType, fileType.ToString());
            ELMarkerReplaceService elService = new Gdp.ELMarkerReplaceService(dic);
            var url = elService.Get(urlModel);
            return url;
        }

        private List<string> GetValues(Dictionary<SatelliteType, List<string>> dic, SatelliteType SatelliteType)
        {
            if (dic.ContainsKey(SatelliteType))
                return dic[SatelliteType];
            return new List<string>();
        }

        public List<string> GetFileNames(List<string> urls)
        {
            List<string> fileNames = new List<string>();
            foreach (var url in urls)
            {
                fileNames.AddRange(GetFileNames(url));
            }
            return fileNames;
        }

        private static List<string> GetFileNames(string url)
        {
            List<string> fileNames = new List<string>();
            var fileName = Path.GetFileName(url);
            fileNames.Add(fileName);
            //去掉z结尾
            if (fileName.EndsWith(".Z") || fileName.EndsWith(".z"))
            {
                fileName = Path.GetFileNameWithoutExtension(url);
                fileNames.Add(fileName);
            }
            return fileNames;
        }

    }
}
