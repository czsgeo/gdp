//2016.04.18, czs, create in hongqing, 测站观测信息
//2016.04.18, czs, edit in hongqing, 提取 ISiteObsInfo，原重命名

using System;
using System.Collections.Generic;
using System.Text;
using System.IO; 
using Gdp.Utils;

using Gdp.Data.Rinex;

namespace Gdp
{
    /// <summary>
    /// 测站观测信息
    /// </summary>
    public interface ISiteObsInfo
    {
        /// <summary>
        /// 观测信息
        /// </summary>
        IObsInfo ObsInfo { get; set; }
        /// <summary>
        /// 测站信息
        /// </summary>
        ISiteInfo SiteInfo { get; set; }
    } 
    /// <summary>
    /// 测站观测信息,组合了测站信息和观测信息，并提供了快捷访问的属性，主要用于RINEX观测数据。
    /// </summary>
    public interface IExtendSiteObsInfo : ISiteObsInfo
    {
        #region 属性
        /// <summary>
        /// 导航文件路径，如果有的话。
        /// </summary>
        string NavFilePath { get; }
        /// <summary>
        /// 测站名称
        /// </summary>
        string SiteName { get; } 
         XYZ ApproxXyz { get; set; }
        global::System.Collections.Generic.List<string> Comments { get; set; }
        Time StartTime { get; set; }
        Time EndTime { get; set; }
         

        global::Gdp.Data.Rinex.FileInfomation FileInfo { get; set; }
        HEN Hen { get; set; }
        double Interval { get; set; }
        global::System.Collections.Generic.Dictionary<global::Gdp.SatelliteType, global::System.Collections.Generic.List<string>> ObsCodes { get; set; }

        global::System.Collections.Generic.List<global::Gdp.SatelliteType> SatelliteTypes { get; }

        #endregion

        /// <summary>
        /// 移除卫星系统及其所有的观测码。
        /// </summary>
        /// <param name="type"></param>
        void Remove(SatelliteType type);
    }
}
