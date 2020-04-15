//2018.08.02, czs, edit in hmx, 增加注释，整理，PVC修改为单值，非ENU

using System; 
using System.Collections.Generic;
 

namespace Gdp.Data
{
    /// <summary>
    /// 天线接口，包括接收机和卫星天线。
    /// </summary>
    public interface IAntenna
    {
        /// <summary>
        /// 按照卫星频率存储的天线数据。
        /// </summary>
        //Dictionary<RinexSatFrequency, FrequencecyAntennaData> Data { get; }
        ///// <summary>
        ///// GNSS 系统频率
        ///// </summary>
        //List<RinexSatFrequency> SatelliteFrequences { get; }
        ///// <summary>
        ///// 接收机基本的信息。
        ///// </summary>
        //AntennaHeader Header { get; } 
        /// <summary>
        /// 校验机构
        /// </summary>
        string Agency { get; set; }
        /// <summary>
        /// 天线数量
        /// </summary>
        int DataCount { get; }
        /// <summary>
        /// 天线罩
        /// </summary>
        string Radome { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        string SerialOrSatFreqCode { get; set; }
        /// <summary>
        /// 天线类型,一般都是大写。
        /// </summary>
        string AntennaType { get; set; }
        /// <summary>
        /// 校准方法
        /// </summary>
        string CalibrationMethod { get; set; }
        /// <summary>
        /// 卫星代码
        /// </summary>
        string SatCode { get; set; }
        /// <summary>
        /// 注释
        /// </summary>
        List<string> Comments { get; set; }
        /// <summary>
        /// 何年入轨，当年编号，发射编号等 COSPAR ID "YYYY-XXXA"，可选。
        /// </summary>
        string CosparID { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        string Date { get; set; }
        /// <summary>
        /// 方位角增量
        /// </summary>
        double DeltaAzimuth { get; set; }
        /// <summary>
        /// 天顶距离增量
        /// </summary>
        double DeltaZenith { get; set; }
        /// <summary>
        /// 获取平均天线相位中心偏差PCO NEU尝试获取天线偏心改正，如果失败则返回 NEU.Zero.
        /// </summary>
        /// <param name="freq">卫星频率</param>
        /// <returns></returns>
        //NEU GetPcoValue(Gdp.RinexSatFrequency freq);
        ///// <summary>
        ///// 获取天线相位变化偏差 PCV, 不依赖方位角
        ///// </summary>
        ///// <param name="freq"></param>
        ///// <param name="elevation"></param>
        ///// <returns></returns>
        //double GetPcvValue(RinexSatFrequency freq, double elevation);
        ///// <summary>
        ///// 获取天线相位变化偏差 PCV，依赖方位角
        ///// </summary>
        ///// <param name="freq"></param>
        ///// <param name="elevation"></param>
        ///// <param name="azimuth"></param>
        ///// <returns></returns>
        //double GetPcvValue(RinexSatFrequency freq, double elevation, double azimuth);
        /// <summary>
        /// 是否有效
        /// </summary>
        bool IsValid { get; }
        /// <summary>
        /// 校准的天线数量，一类型天线可能有多个。number of individual antennas calibrated
        /// </summary>
        string NumOfAntennas { get; set; }
        /// <summary>
        /// 频率数量
        /// </summary>
        int NumOfFrequencies { get; set; }

        //void SetAntennaEcc(SatelliteFrequency freq, NEU trEcc);
        //void SetAntennaNoAziPattern(SatelliteFrequency freq, List<double> pcVec);
        //void SetAntennaNoAziRms(SatelliteFrequency freq, List<double> pcRMS);
        //void SetAntennaPattern(SatelliteFrequency freq, double azi, List<double> pcVec);
        //void SetAntennaPatternRms(SatelliteFrequency freq, double azimuth, List<double> pcRmsVector);
        //void SetAntennaRmsEcc(SatelliteFrequency freq, NEU ecc);
        /// <summary>
        /// 通常为 IGS14_2000 等
        /// </summary>
        string SinexCode { get; set; }
        /// <summary>
        /// 有效起始时间
        /// </summary>
        Time ValidDateFrom { get; set; }
        /// <summary>
        /// 有效停止时间
        /// </summary>
        Time ValidDateUntil { get; set; }
        /// <summary>
        /// 起始天顶距
        /// </summary>
        double ZenithStart { get; set; }
        /// <summary>
        /// 终止天顶距
        /// </summary>
        double ZenithEnd { get; set; }
        /// <summary>
        /// 有效时段
        /// </summary>
        TimePeriod TimePeriod { get;}
    }
}
