//2016.10.28, czs, create  in hongqing, 观测文件选择选项。

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace Gdp
{ 
    
    /// <summary>
    /// 观测文件选择选项
    /// </summary>
    public class ObsFileSelectOption
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ObsFileSelectOption()
        {
            this.CenterRegion = new XyzCenterRegion(new XYZ(0, 0, 0), 1e10);
            this.IncludeSiteNames = new List<string>();
            this.ExcludeSiteNames = new List<string>();
            this.ObsCodes = new List<string>() { "P1" };
            this.TimePeriod = new TimePeriod(Time.MinValue, Time.MaxValue);
            MinFileSizeMB = 1;
            MinEpochCount = 2000;
            MinSatCount = 4;
            MinRatioOfSatCount = 0.5;
            MinFrequencyCount = 2;
            MultipathMp1 = new EnableFloat(0.5);
            MultipathMp2 = new EnableFloat(0.5);
            SatelliteTypes = new List<SatelliteType>();
            IsNavCopy = true;
        }
        /// <summary>
        /// 是否将对应的导航文件一起选择复制
        /// </summary>
        public bool IsNavCopy { get; set; }
        /// <summary>
        /// 多路径设置
        /// </summary>
        public EnableFloat MultipathMp1 { get; set; }
        /// <summary>
        /// 多路径设置
        /// </summary>
        public EnableFloat MultipathMp2 { get; set; }

        /// <summary>
        /// 最小频率数量
        /// </summary>
        public int MinFrequencyCount { get; set; }
        /// <summary>
        /// 启用
        /// </summary>
        public bool IsEnableMinFrequencyCount { get; set; }
        /// <summary>
        /// 最小卫星数量
        /// </summary>
        public int MinSatCount { get; set; }
        /// <summary>
        /// 最小卫星数量的比例
        /// </summary>
        public double MinRatioOfSatCount { get; set; }
        /// <summary>
        /// 是否启用 最小卫星数量的比例
        /// </summary>
        public bool IsEnableMinRatioOfSatCount { get; set; }

        /// <summary>
        /// 文件最小的大小
        /// </summary>
        public double MinFileSizeMB { get; set; }
        /// <summary>
        /// 是否启用最小历元数。
        /// </summary>
        public bool IsEnableMinFileSizeMB { get; set; }
        /// <summary>
        /// 最小历元数，少于此则历元数量。
        /// </summary>
        public int MinEpochCount { get; set; }
        /// <summary>
        /// 是否启用最小历元数。
        /// </summary>
        public bool IsEnableMinEpochCount { get; set; }

        /// <summary>
        /// 时段信息
        /// </summary>
        public TimePeriod TimePeriod { get; set; }
        /// <summary>
        /// 是否启用时段信息
        /// </summary>
        public bool IsEnableTimePeriod { get; set; }
        /// <summary>
        /// 卫星系统类型选择
        /// </summary>
        public List<SatelliteType> SatelliteTypes { get; set; }
        /// <summary>
        /// 卫星系统类型选择
        /// </summary>
        public bool IsEnableSatelliteTypes { get; set; }

        /// <summary>
        /// 观测码
        /// </summary>
        public List<string> ObsCodes { get; set; }
        /// <summary>
        /// 启用观测码
        /// </summary>
        public bool IsEnableObsCodes { get; set; }
        /// <summary>
        /// 中心区域
        /// </summary>
        public XyzCenterRegion CenterRegion { get; set; }
        /// <summary>
        /// 启用
        /// </summary>
        public bool IsEnableCenterRegion { get; set; }
        /// <summary>
        /// 应该包含的测站名
        /// </summary>
        public List<string> IncludeSiteNames { get; set; }
        /// <summary>
        /// 启用
        /// </summary>
        public bool IsEnableIncludeSiteNames { get; set; }
        /// <summary>
        /// 应该包含的测站名
        /// </summary>
        public List<string> ExcludeSiteNames { get; set; }
        /// <summary>
        /// 启用+
        /// </summary>
        public bool IsEnableExcludeSiteNames { get; set; }
    }

}
