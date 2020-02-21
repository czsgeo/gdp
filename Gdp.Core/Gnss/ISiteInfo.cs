//2014.12.03, czs,edit, jinxingliangmao shuangliao, 实现 IToTabRow
//2015.05.28, czs , add in namu, 增加估值坐标属性
//2015.10.15, czs , create in hongqing, 增加测站某次观测信息 IObservationInfo 类


using System; 
using Gdp.Data;
using System.Collections.Generic; 

namespace Gdp
{
    /// <summary>
    /// GNSS 测站信息，包括接收机和天线信息。是比较固定的信息，短时间内不会改变。
    /// </summary>
    public interface ISiteInfo :  ICloneable
    {
        /// <summary>
        /// 自定义测站标识，为 SiteName + "_" + SiteNumber
        /// </summary>
        string SiteId { get; }
        /// <summary>
        /// 测站名称,MarkerName
        /// </summary>
        string SiteName { get; set; }
        /// <summary>
        /// 测站编号
        /// </summary>
        string MarkerNumber { get; set; }
        /// <summary>
        /// 采用手动设置初始坐标。此方法是为了避免和区分手动输入造成的错误。
        /// </summary>
        /// <param name="xyz"></param>
        void SetApproxXyz(XYZ xyz);
        /// <summary>
        /// 初始概略坐标,必须要有赋值！设置请用 SetApproxXyz 
        /// </summary>
        XYZ ApproxXyz { get;  }
        /// <summary>
        /// 计算估值坐标,必须要有赋值！如果没有初始化则采用概略坐标 ApproxXyz。
        /// 在计算中，估值坐标常常会被更新，以获得更好的精度。可以是本历元内迭代，也可以是时间序列传递。
        /// </summary>
        XYZ EstimatedXyz { get; set; } 

        /// <summary>
        /// 坐标精度指示。用于决定初始坐标是否精确。
        /// </summary>
        XYZ EstimatedXyzRms { get; set; }
        //  RmsedXYZ RmsedXyz { get; set; }
        /// <summary>
        /// 概略坐标
        /// </summary>
        GeoCoord ApproxGeoCoord { get; set; }
        /// <summary>
        /// 概略坐标
        /// </summary>
        GeoCoord EstimatedGeoCoord { get;   } 

        /// <summary>
        /// 提供的坐标是否精确。一般在厘米级别以内认为精确。具体还要看实际应用。
        /// 如果是精确坐标，则不用过多的循环迭代，不用此次更新，只需要求偏差就行了；
        /// 否则需要多次迭代，并采用每次的平差值作为下一次的近似值计算。
        /// </summary>
        bool IsPrecisePosition { get; }

        /// <summary>
        /// 接收机编号
        /// </summary>
        string ReceiverNumber { get; set; }
        /// <summary>
        /// 接收机类型
        /// </summary>
        string ReceiverType { get; set; }
        /// <summary>
        /// 接收机型号
        /// </summary>
        string ReceiverVersion { get; set; }

        /// <summary>
        /// 天线编号
        /// </summary>
        string AntennaNumber { get; set; }
        /// <summary>
        /// 天线类型
        /// </summary>
        string AntennaType { get; set; }
        /// <summary>
        /// 天线坐标偏移 天线相位中心与天线参考点 ARP(Antenna Reference Point)
        /// 采用 HEN 坐标描述，与 RINEX 头文件一致。
        /// 计算时需要转换到 NEU。
        /// </summary>
        HEN Hen { get; set; }

        /// <summary>
        /// 天线对象。需要后继赋值才能使用。
        /// </summary>
        IAntenna Antenna { get; set; }

        /// <summary>
        /// 天线 XYZ 偏差，对应 RINEXANTENNA_DELTA_XYZ
        /// </summary>
        XYZ AntDeltaXyz { get; set; }
    }
 
}
