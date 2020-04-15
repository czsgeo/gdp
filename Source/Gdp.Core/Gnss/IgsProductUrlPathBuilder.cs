//2015.12.07, czs, create in hongqing.IGS日产品地址生成器
//2015.12.22, czs, edit in hognqing, 增加目录。
//2015.12.29, czs, edit in hognqing, 修正地址模板可编辑。
//2018.04.27, czs, edit in hmx, 多系统支持
//2017.05.27, czs,  edit in hmx, 可以直接以ftp开头，将自动切断前面的目录

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 


namespace Gdp.Data
{
    /// <summary>
    /// IGS（日，默认）产品地址生成器
    /// </summary>
    public class IgsProductUrlPathBuilder : AbstractBuilder<string[]>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="urlDirectories"></param>
        /// <param name="SourceNames"></param>
        /// <param name="urlModels"></param>
        /// <param name="ProductType"></param>
        /// <param name="TimeIntervalInSeconds"></param>
        public IgsProductUrlPathBuilder(
            string [] urlDirectories,
            Dictionary<SatelliteType, List<string>> SourceNames,
            string[] urlModels  ,
            IgsProductType ProductType = IgsProductType.Sp3,
            int TimeIntervalInSeconds = 86400)
        {
            this.ProductType = ProductType;
            this.UrlDirectories = urlDirectories;

            if (TimeIntervalInSeconds == 604800)
            {
                var len = urlModels.Length;
                for (int i = 0; i < len; i++)
                {
                    urlModels[i] = urlModels[i].Replace("{DayOfWeek}", "7");
                }
            }             
             
            this.UrlModels = urlModels;

            //Gnsser.Winform.Setting.GnsserConfig.IgsProductUrlModels;
            this.SourceNameDic = SourceNames;// new string[] { "igs", "wum", "com", "gbm", "qzf", "tum" };
            this.TimeIntervalInSeconds = TimeIntervalInSeconds;
            this.SatelliteType = SatelliteType.U;
        }
        /// <summary>
        /// 构造函数,采用内置固定URL参数。
        /// </summary>
        /// <param name="ProductType"></param>
        /// <param name="TimeIntervalInSeconds"></param>
        public IgsProductUrlPathBuilder(IgsProductType ProductType = IgsProductType.Sp3, int TimeIntervalInSeconds = 86400)
        {
            this.ProductType = ProductType;
            this.UrlDirectories = new string[]{
               "ftp://igs.ensg.eu/pub/igs/products",
               "ftp://cddis.gsfc.nasa.gov/pub/gps/products"
            };
            var urlModels = new string[]{
               "{UrlDirectory}/{FileDirectory}/{FileName}"};
            if (TimeIntervalInSeconds == 604800) urlModels = new string[]{
               "{UrlDirectory}/{FileDirectory}/{FileDirectory}/{FileName}"};           
             
            this.UrlModels = urlModels;

            //Gnsser.Winform.Setting.GnsserConfig.IgsProductUrlModels;
            this.SourceNameDic = new Dictionary<SatelliteType, List<string>>();

            SourceNameDic[ SatelliteType.U] =new List<string>(  new string[] { "igs" });
            // wum,gbm,qzf,com, 
            this.TimeIntervalInSeconds = TimeIntervalInSeconds;
            this.SatelliteType = SatelliteType.U;
        }


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="UrlDirectories"></param>
        /// <param name="UrlModels"></param>
        /// <param name="SourceNames"></param>
        /// <param name="timeFrom"></param>
        /// <param name="timeTo"></param>
        /// <param name="ProductType"></param>
        /// <param name="TimeIntervalInSeconds"></param>
        public IgsProductUrlPathBuilder(string[] UrlDirectories, string[] UrlModels, string[] SourceNames, DateTime timeFrom, DateTime timeTo, IgsProductType ProductType, int TimeIntervalInSeconds = 86400)
        {
            this.ProductType = ProductType;
            this.UrlModels = UrlModels;
            this.UrlDirectories = UrlDirectories;
            this.SourceNameDic = new Dictionary<SatelliteType, List<string>>();
            if (SourceNames == null || SourceNames.Length == 0)
            { 
            }
            else
            {
                SourceNameDic[SatelliteType.U] = new List<string>(SourceNames);
            }
            this.TimeFrom = timeFrom;
            this.TimeTo = timeTo;
            this.TimeIntervalInSeconds = TimeIntervalInSeconds;
            this.SatelliteType = SatelliteType.U;
        }

        #region 属性
        IgsProductDirectoryModel IgsProductDirectoryModel = IgsProductDirectoryModel.Instance;
        IgsProductFileNameModel IgsProductFileNameModel = IgsProductFileNameModel.Instance;
        /// <summary>
        /// 时间间隔，单位秒，日为86400，周为604800，时段服务信息
        /// </summary>
        public int TimeIntervalInSeconds { get; set; }
        /// <summary>
        /// 数据产品类型。
        /// </summary>
        public IgsProductType ProductType { get; set; }
        /// <summary>
        /// 地址模板
        /// </summary>
        public string[] UrlDirectories { get; set; }
        /// <summary>
        /// 地址模板
        /// </summary>
        public string[] UrlModels { get; set; }
        /// <summary>
        /// 数据源名称
        /// </summary>
        //public string[] SourceNames { get; set; }
        /// <summary>
        /// 起始时间
        /// </summary>
        public DateTime TimeFrom { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime TimeTo { get; set; }
        /// <summary>
        /// 多系统数据源字典
        /// </summary>
        protected Dictionary<SatelliteType, List<string>> SourceNameDic { get; set; }
        /// <summary>
        /// 卫星系统类型，若不指定的话，为U
        /// </summary>
        public SatelliteType SatelliteType { get; set; }
        /// <summary>
        /// 测站名称，观测文件有效
        /// </summary>
        public List<string> SiteNames { get; set; }
        #endregion

        public IgsProductUrlPathBuilder SetSatelliteType(SatelliteType satType)
        {
            this.SatelliteType = satType;
            return this;
        }

        /// <summary>
        /// 生成
        /// </summary>
        /// <returns></returns>
        public override string[] Build()
        {
            var urlModels = BuildWithUrlDirectory(new List<String>(UrlModels));
            urlModels = BuildWithProductType(urlModels);
            urlModels = BuildWithTime(urlModels);
            urlModels = BuildWithSourceName(urlModels);
            urlModels = BuildWithSiteNames(urlModels);
            urlModels = RemoveRedundancy(urlModels);

            return urlModels.Distinct().ToArray();
        }
        /// <summary>
        /// 构建
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public string [] Build(Time time)
        {
            var urlModels = BuildWithUrlDirectory(new List<String>(UrlModels));
            urlModels = BuildWithProductType(urlModels);
            urlModels = ELMarkerReplaceService.BuildWithTime(urlModels, time);
            urlModels = BuildWithSourceName(urlModels);
            urlModels = BuildWithSiteNames(urlModels);
            urlModels = RemoveRedundancy(urlModels);

            return urlModels.ToArray();
        }

        #region 生成细节
        /// <summary>
        /// 移除多余的东西
        /// </summary>
        /// <param name="pathes"></param>
        /// <returns></returns>
        public List<string> RemoveRedundancy(List<string> pathes)
        {
            List<string> newPathes = new List<string>();
            List<string> newPathesLowerCase = new List<string>();//用于辅助判断
            foreach (var item in pathes)
            {
                var path = item;
                int lastIndexOfHead = item.ToLower().LastIndexOf("ftp://");
                if ( lastIndexOfHead != 0)
                {
                    path = item.Substring(lastIndexOfHead);
                }
                if (!newPathesLowerCase.Contains(path.ToLower()))
                {
                    newPathesLowerCase.Add(path.ToLower());
                    newPathes.Add(path);
                }
                 
            }
            return newPathes;
        }

        /// <summary>
        /// 第一步替换
        /// </summary>
        /// <param name="urlModels"></param>
        /// <returns></returns>
        private List<string> BuildWithUrlDirectory(List<string> urlModels)
        {
            if (UrlDirectories == null) return urlModels;

            List<string> pathes = new List<string>();
            
            foreach (var UrlDirectory in UrlDirectories)
            {
                foreach (var model in urlModels)
                {
                    var path = model.Replace(ELMarker.UrlDirectory, UrlDirectory);
                   
                    var val = this.IgsProductDirectoryModel.Get(this.ProductType);
                    path = path.Replace(ELMarker.FileDirectory, val);
                    
                    //fileName
                    val = this.IgsProductFileNameModel.Get(this.ProductType);
                    path = path.Replace(ELMarker.FileName, val); 

                    pathes.Add(path);

                }
            }
            return pathes;
        }

        private List<string> BuildWithSourceName(List<string> urlModels)
        {
            if (SourceNameDic  == null || !SourceNameDic.ContainsKey(SatelliteType)) return urlModels;

            List<string> pathes = new List<string>();
            var SourceNames = SourceNameDic[this.SatelliteType];
            foreach (var sourceName in SourceNames)
            {
                foreach (var model in urlModels)
                {
                    var path = model
                     .Replace(ELMarker.SourceName, sourceName.Trim());
                    pathes.Add(path);
                }
            }
            return pathes;
        }
        private List<string> BuildWithProductType(List<string> urlModels)
        {
            List<string> pathes = new List<string>();

            foreach (var model in urlModels)
            {   
                string path  = model
                    .Replace(ELMarker.ProductType, ProductType.ToString().ToLower()); 
                pathes.Add(path);
            }
            return pathes;
        }
        /// <summary>
        /// 替换测站名称
        /// </summary>
        /// <param name="urlModels"></param>
        /// <returns></returns>
        private List<string> BuildWithSiteNames(List<string> urlModels)
        {
            if(SiteNames == null || SiteNames.Count ==0) { return urlModels; }

            List<string> pathes = new List<string>(); 
            foreach (var siteName in SiteNames)
            {
                foreach (var model in urlModels)
                {
                    var path = model
                     .Replace(ELMarker.SiteName, siteName.Trim());
                    pathes.Add(path);
                }
            }
            return pathes;
        }

        private List<string> BuildWithTime(List<string> urlModels)
        {
            Time from = Time.Parse(TimeFrom);
            Time to = Time.Parse(TimeTo);
            List<string> pathes = new List<string>();
            for (Time ifrom = from; ifrom <= to; ifrom = ifrom + TimeIntervalInSeconds)
            {
                List<string> subPathes = ELMarkerReplaceService.BuildWithTime(urlModels, ifrom);
                pathes.AddRange(subPathes);
            }
            return pathes.Distinct().ToList();
        }

        #endregion
    }
      
}
