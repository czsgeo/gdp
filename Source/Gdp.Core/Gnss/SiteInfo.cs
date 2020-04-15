//2014.12.03, czs,edit, jinxingliangmao shuangliao, 实现 IToTabRow
//2015.05.28, czs , add in namu, 增加估值坐标属性
//2015.10.15, czs , create in hongqing, 增加测站某次观测信息 IObservationInfo 类

using System; 
using System.Collections.Generic;
using System.Text; 

namespace Gdp
{
    /// <summary>
    /// GNSS 测站信息，包括接收机和天线信息。是比较固定的信息，短时间内不会改变。
    /// </summary>
    public class SiteInfo : ISiteInfo
    {
        /// <summary>
        /// 构造函数。
        /// </summary>
        public SiteInfo()
        {
            this.EstimatedXyzRms = XYZ.Zero;
            this.SiteName = "UNKOWN";
            this.MarkerNumber = "UNKOWN";
        }

        #region 属性

        /// <summary>
        /// 自定义测站标识，为 SiteName + "_" + SiteNumber
        /// </summary>
        public string SiteId { get { return SiteName + "_" + MarkerNumber; } } 
        /// <summary>
        /// 属性排序
        /// </summary>
        public List<string> OrderedProperties { get; protected set; }
        /// <summary>
        /// 测站名称
        /// </summary>
        public string SiteName { get; set; }
        /// <summary>
        /// 测站编号
        /// </summary>
        public string MarkerNumber { get; set; } 

        /// <summary>
        /// 天线偏差
        /// </summary>
        public XYZ AntDeltaXyz { get; set; }

        /// <summary>
        /// 估计值概略坐标
        /// </summary>
        public GeoCoord EstimatedGeoCoord { get { return CoordTransformer.XyzToGeoCoord(EstimatedXyz); } } 

        XYZ approxXyz;
        /// <summary>
        /// 概略坐标.
        /// </summary>
        public XYZ ApproxXyz
        {
            get { return approxXyz; }
            protected  set
            { 
                this.approxXyz = value;
                if (value != null)
                    this.ApproxGeoCoord = CoordTransformer.XyzToGeoCoord(value);
                else this.ApproxGeoCoord = null;
            }
        }
        XYZ estimatedXyz;
        /// <summary>
        /// 估值坐标，如果没有设置，则采用概略坐标。.此属性是为了方便坐标更新使用。
        /// </summary>
        public XYZ EstimatedXyz
        {
            get { if (estimatedXyz == null) estimatedXyz = ApproxXyz; return estimatedXyz; }
            set
            {
                
                this.estimatedXyz = value;
            }
        }

        /// <summary>
        /// 坐标精度指示。用于决定初始坐标是否精确。
        /// </summary>
        public XYZ EstimatedXyzRms { get; set; }
        

        /// <summary>
        /// 概略坐标
        /// </summary>
        public GeoCoord ApproxGeoCoord { get; set; } 

        /// <summary>
        /// 提供的坐标是否精确。一般在厘米级别以内认为精确。具体还要看实际应用，有的认为米级别就是精确了。
        /// 如果是CORS ，则是提供的精确坐标。
        /// 如果是精确坐标，则不用过多的循环迭代，不用此次更新，只需要求偏差就行了；
        /// 否则需要多次迭代，并采用每次的平差值作为下一次的近似值计算。
        /// </summary>
        public bool IsPrecisePosition
        {
            get
            {
                //对概略坐标进行简要的判断
                if (this.EstimatedXyz == null || this.EstimatedXyz.IsZero)
                    return false;
                //如果没有设置精度信息，则默认为精确
                if (this.EstimatedXyzRms == null || this.EstimatedXyzRms.IsZero)
                    return true;

                return this.EstimatedXyzRms.Length <= 20;//偏离小于20米,认为精确（伪距定位）。
            }
        }
        /// <summary>
        /// 接收机编号
        /// </summary>
        public string ReceiverNumber { get; set; }
        /// <summary>
        /// 接收机类型
        /// </summary>
        public string ReceiverType { get; set; }
        /// <summary>
        /// 接收机型号
        /// </summary>
        public string ReceiverVersion { get; set; }

        /// <summary>
        /// 天线编号
        /// </summary>
        public string AntennaNumber { get; set; }
        /// <summary>
        /// 天线类型
        /// </summary>
        public string AntennaType { get; set; }
        /// <summary>
        /// 天线坐标偏移
        /// </summary>
        public HEN Hen { get; set; }
        /// <summary>
        /// 字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return SiteName + " " + MarkerNumber;
        }
        private Data.IAntenna _antenna;
        public Data.IAntenna Antenna
        {
            get
            {
                return _antenna;
            }
            set
            {
                _antenna = value;
            }
        }

        

        #endregion
         

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            var clone = new SiteInfo
            {
                AntDeltaXyz = AntDeltaXyz,
                Antenna = Antenna,
                AntennaNumber = AntennaNumber,
                ApproxXyz = ApproxXyz,
                AntennaType = AntennaType,
                ApproxGeoCoord = ApproxGeoCoord,
                EstimatedXyz = EstimatedXyz,
                Hen = Hen,
                SiteName = SiteName,
                MarkerNumber = MarkerNumber,
                ReceiverNumber = ReceiverNumber,
                ReceiverType = ReceiverType,
                ReceiverVersion = ReceiverVersion,
                EstimatedXyzRms = EstimatedXyzRms

            };
            return clone;
        }



        public void SetApproxXyz(XYZ xyz)
        {
            this.ApproxXyz = xyz;
        }
    }
}
