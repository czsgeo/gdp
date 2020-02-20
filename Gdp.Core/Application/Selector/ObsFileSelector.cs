//2016.10.28, czs, create  in hongqing, 观测文件选择选项。

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text; 

using Gdp.IO; 
using Gdp.Data.Rinex;


namespace Gdp
{

    /// <summary>
    /// 观测文件选择器
    /// </summary>
    public class ObsFileSelector
    {
        Log log = new Log(typeof(ObsFileSelector));
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Option"></param>
        /// <param name="OutputDirectory"></param>
        public ObsFileSelector(ObsFileSelectOption Option, string OutputDirectory)
        {
            this.OutputDirectory = OutputDirectory;
            this.Option = Option;
            Init();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            this.FailedPathes = new List<string>();
        }

        #region 属性
        /// <summary>
        /// 选项
        /// </summary>
        public ObsFileSelectOption Option { get; set; }
        /// <summary>
        /// 输出目录
        /// </summary>
        public string OutputDirectory { get; set; }
        /// <summary>
        /// 失败的路径集合
        /// </summary>
        public List<string> FailedPathes { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 选择并复制文件
        /// </summary>
        /// <param name="inpathes"></param>
        public void Select(IEnumerable<string> inpathes)
        {
            foreach (var item in inpathes)
            {
                Select(item);
            }
        }

        /// <summary>
        /// 选择并复制文件
        /// </summary>
        /// <param name="inpath"></param>
        /// <param name="subDirectory">子目录，若有</param>
        public bool Select(string inpath, string subDirectory = null)
        {
            if (IsMatch(inpath))
            {
                var outpath = GetOutputPath(inpath, subDirectory);
                Gdp.Utils.FileUtil.CheckOrCreateDirectory(Path.GetDirectoryName(outpath));
                try
                {
                    File.Copy(inpath, outpath, true);

                    if (this.Option.IsNavCopy)
                    {
                        var header = RinexObsFileReader.ReadHeader(inpath);
                        if (header.HasNavFile)//同时输出导航文件
                        {
                            var outpathNav = GetOutputPath(header.NavFilePath, subDirectory);
                            File.Copy(header.NavFilePath, outpathNav, true);
                        }
                    }

                    log.Info(Path.GetFileName(inpath) + " 匹配成功！ 复制到 " + outpath);
                    return true;
                }
                catch (Exception ex)
                {
                    log.Error("复制 " + inpath + " 到 " + outpath + " 发生错误！" + ex.Message);
                }
            }
            else {
                FailedPathes.Add(inpath);
                log.Info(Path.GetFileName(inpath) + " 匹配失败！" ); }
            return false;
        }

        /// <summary>
        /// 输出路径
        /// </summary>
        /// <param name="inPath"></param>
        /// <param name="subDirectory"></param>
        /// <returns></returns>
        public string GetOutputPath(string inPath, string subDirectory = null)
        {
            if (String.IsNullOrEmpty( subDirectory ))
            {
                return Path.Combine(OutputDirectory, Path.GetFileName(inPath));
            }
            return Path.Combine(OutputDirectory, subDirectory, Path.GetFileName(inPath));
        }

        /// <summary>
        /// 是否符合要求
        /// </summary>
        /// <param name="inpath"></param>
        /// <returns></returns>
        public bool IsMatch(string inpath)
        {
            var fileName = Path.GetFileName(inpath);
            var reader = new RinexObsFileReader(inpath, false);
            var header = reader.GetHeader();

            //多路径效应，此处采用TEQC分析
            if (Option.MultipathMp1.Enabled || Option.MultipathMp2.Enabled)
            {
                
                //{
                //    double Mp1 = double.Parse(result.Substring(result.IndexOf("SUM ") + 63, 4).Replace(" ", ""));
                //    double Mp2 = double.Parse(result.Substring(result.IndexOf("SUM ") + 69, 4).Replace(" ", ""));

                //    if (this.Option.MultipathMp1.Enabled)
                //    {
                //        if (Mp1 > this.Option.MultipathMp1.Value)
                //        {
                //            return false;
                //        }
                //    }
                //    if (this.Option.MultipathMp2.Enabled)
                //    {
                //        if (Mp2 > this.Option.MultipathMp2.Value)
                //        {
                //            return false;
                //        }
                //    }

                //}
                 
            }

          //  ObsAnalysisInfo anaInfo = new ObsAnalysisInfo(header);
            //if (this.Option.MultipathMp1.Enabled && anaInfo.HasMultipathFactor)
            //{
            //    if (anaInfo.MultipathFactors[FrequenceType.A] > this.Option.MultipathMp1.Value)
            //    {
            //        return false;
            //    }
            //}

            //if (this.Option.MultipathMp2.Enabled && anaInfo.HasMultipathFactor)
            //{
            //    if (anaInfo.MultipathFactors[FrequenceType.B] > this.Option.MultipathMp2.Value)
            //    {
            //        return false;
            //    }
            //}


            if (Option.IsEnableSatelliteTypes)
            {
                foreach (var satType in Option.SatelliteTypes)
                {
                    if (!header.SatelliteTypes.Contains(satType))
                    {
                        log.Info(fileName + " 不包含系统 " + satType);
                        return false;
                    }
                }
            }


            if (Option.IsEnableObsCodes)
            {
                var codes = header.ObsCodes;
                foreach (var item in Option.ObsCodes)
                {
                    foreach (var list in codes.Values)
                    {
                        if (!list.Contains(item)) {

                            log.Info(fileName + " 不包含 " + item);
                            return false; }
                    }
                }
            }

            if (Option.IsEnableMinFrequencyCount)
            {
                var codes = header.ObsCodes;

                foreach (var item in codes)
                {
                    var count = MathedCount(item.Value,"L");
                    if (count < Option.MinFrequencyCount)
                    {
                        log.Info( fileName + " 伪距数量少于 " + Option.MinFrequencyCount);
                        return false;
                    }

                    count = MathedCount(item.Value, new string[] { "C", "P" });
                    if (count < Option.MinFrequencyCount)
                    {
                        log.Info(fileName + " 载波数量少于 " + Option.MinFrequencyCount);
                        return false;
                    } 
                } 
            }


            if (Option.IsEnableMinFileSizeMB)
            {
                FileInfo info = new FileInfo(inpath);
                var minByte = Option.MinFileSizeMB * 1024L * 1024L;
                if (info.Length < minByte)
                {
                    log.Info(fileName + " 文件小于 " + Option.MinFileSizeMB + " MB");
                    return false;
                }
            }


            if (Option.IsEnableCenterRegion)
            {
                if (!Option.CenterRegion.Contains(header.ApproxXyz)) {

                    log.Info(fileName + "  " + header.ApproxXyz + " 不在指定区域内 " + Option.CenterRegion);
                    return false; }
            }

            if (Option.IsEnableExcludeSiteNames)
            {
                if (Option.ExcludeSiteNames.Contains(header.SiteName))
                {
                    log.Info(fileName + " 点名被排除了");
                    return false;
                }
            }

            if (Option.IsEnableIncludeSiteNames)
            {
                if (!Option.IncludeSiteNames.Contains(header.SiteName))
                {
                    log.Info(fileName + " 点名被排除了");
                    return false;
                }
            }
            if (Option.IsEnableTimePeriod)
            {
                if (!Option.TimePeriod.Contains(header.StartTime)) {
                    log.Info(fileName + " 不在指定时段内 " + Option.TimePeriod ); 
                    return false;
                }
            }
            //这个比较耗时，放在最后
            if (Option.IsEnableMinEpochCount)
            {
                var count = RinexObsFileReader.ReadGetEpochCount(inpath);
                if (count < Option.MinEpochCount)
                {
                    log.Info(fileName + " 历元数量 " + count + "  少于 " + Option.MinEpochCount); 
                    return false;
                }
            }

            if(Option.IsEnableMinRatioOfSatCount){
                var ratio = RinexObsFileReader.GetRatioOfSatCount(inpath, Option.MinSatCount);
                if (ratio < Option.MinRatioOfSatCount)
                {
                    log.Info(fileName + " 最小卫星数量比率 " + ratio);
                    return false;
                } 
            } 
           
            return true;
        }


        #region  工具方法
        /// <summary>
        /// 匹配数量
        /// </summary>
        /// <param name="list"></param>
        /// <param name="StartChar"></param>
        /// <returns></returns>
        public int MathedCount(List<string> list, string StartChar)
        {
            return MathedCount(list, new string[] { StartChar });
        }
        /// <summary>
        /// 匹配数量
        /// </summary>
        /// <param name="list"></param>
        /// <param name="StartChar"></param>
        /// <returns></returns>
        public int MathedCount(List<string> list, string[] StartChars)
        {
            int i = 0;
            foreach (var item in list)
            {
                foreach (var StartChar in StartChars)
                {
                    if (item.StartsWith(StartChar))
                    {
                        i++;
                    }
                }
            }
            return i;
        }
        #endregion

        #endregion
    }

}
