//2014.07.31, czs, 分离导航，SP3等头文件，只做观测文件的头文件。
//2015.05.25, czs, edit in namu,设计了头部系统类型表示法，其中 SatelliteTypeMarker对整个文档进行描述，而SatelliteTypes表示的为具体的系统类型，新模型对RINEX 2 和 3,采用相同核心描述。考虑了RINEX 2 类型为 M 的情况。
//2015.10.15, czs, edit in 西安五路口袁记肉夹馍店, 提取文件信息类。
//2015.10.28, czs, edit in hongqing, 对RINEX 2 和 3 进行融合设计。
//2016.11.25, czs, edit in hongqing, 提取 SiteObsInfo

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Gdp.Utils;


namespace Gdp.Data.Rinex
{         /// <summary>
          /// Time system:- GPS (=GPS time system)
          /// - GLO (=UTC time system)
          /// - GAL (=Galileo System Time)
          /// - QZS (= QZSS time system)
          /// - BDT (=BDS Time system)
          /// </summary>
  //  public String TimeSystem { get; set; }
    /// <summary>
    /// 时间系统
    /// </summary>
    public enum TimeSystem
    {
        GPS,
        BDT,
        GLO,
        GAL,
        QZS, 
    }

    /// <summary>
    /// Rinex 观测文件头文件。
    /// </summary>
    public class RinexObsFileHeader : SiteObsInfo, ICloneable
    {
        /// <summary>
        /// 以基本信息初始化
        /// </summary>
        /// <param name="ObsInfo"></param>
        /// <param name="SiteInfo"></param>
        public RinexObsFileHeader(IObsInfo ObsInfo, ISiteInfo SiteInfo)
            : base(SiteInfo, ObsInfo)
        { 
            this.FileInfo = new FileInfomation
            {
                CreationAgence = "Gdp",
                CreationDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                CreationProgram = "Gdp"
            };
            CountryCode = "000";
            GlonassSlotFreqNums = new Dictionary<SatelliteNumber, int>();
            SystemPhaseShift = new Dictionary<SatelliteNumber, Dictionary<ObservationCode, double>>();
            GlonassCodePhaseBias = new Dictionary<ObservationCode, double>();
        }

        /// <summary>
        /// Rinex 头文件。
        /// </summary>
        public RinexObsFileHeader()
        {
            this.Version = 3.02;
            this.Comments = new List<string>();
            this.FileInfo = new FileInfomation();
            this.StartTime = Time.Default;
            this.EndTime = Time.Default; CountryCode = "000";
            GlonassSlotFreqNums = new Dictionary<SatelliteNumber, int>();
            SystemPhaseShift = new Dictionary<SatelliteNumber, Dictionary<ObservationCode, double>>();
            GlonassCodePhaseBias = new Dictionary<ObservationCode, double>();
        }

        #region 属性
        /// <summary>
        /// 是否具有导航文件
        /// </summary>
        public bool HasNavFile => File.Exists(NavFilePath);
        /// <summary>
        /// 导航文件路径，如果有的话。
        /// </summary>
        public string NavFilePath { get; set; }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get { return FileInfo.FilePath; } set { FileInfo.FilePath = value; } }
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get => FileInfo.FileName; }
        /// <summary>
        /// GLONASS_SLOT_FRQ
        /// </summary>
        public Dictionary<SatelliteNumber, int> GlonassSlotFreqNums { get; set; }
        /// <summary>
        /// Phase shift correction used to generate phases consistent w/r to cycle shifts
        /// </summary>
        public Dictionary< SatelliteNumber ,Dictionary<ObservationCode, double>> SystemPhaseShift { get; set; }
        public double GetSystemPhaseShift(SatelliteNumber prn, ObservationCode code)
        {
            return SystemPhaseShift[prn][code];
        }

        /// <summary>
        /// 用于RINEX输出
        /// </summary>
        /// <returns></returns>
        public Dictionary<SatelliteType, Dictionary<ObservationCode, List<SatelliteNumber>>> GetCodeSatTypedPrns()
        {
            var result = new Dictionary<SatelliteType, Dictionary<ObservationCode, List<SatelliteNumber>>>();
            foreach (var kv in SystemPhaseShift)
            {
                if (!result.ContainsKey(kv.Key.SatelliteType))
                {
                    result[kv.Key.SatelliteType] = new Dictionary<ObservationCode, List<SatelliteNumber>>();
                }
                var dic = result[kv.Key.SatelliteType];
                foreach (var item in kv.Value)
                {
                    if (!dic.ContainsKey(item.Key))
                    {
                        dic[item.Key] = new List<SatelliteNumber>();
                    }
                    dic[item.Key].Add(kv.Key);
                }            
            }
            return result;
        }

        /// <summary>
        /// GLONASS COD/PHS/BIS
        /// </summary>
        public Dictionary<ObservationCode, double> GlonassCodePhaseBias { get; set; }

        #region ObsInfo 属性
        /// <summary>
        /// 测站名称
        /// </summary>
        public string MarkerName { get { return SiteInfo.SiteName; } set { SiteInfo.SiteName = value; } }

        /// <summary>
        /// 天线偏差 ANTENNA_DELTA_XYZ
        /// </summary>
        public XYZ AntDeltaXyz { get { return SiteInfo.AntDeltaXyz; } set { SiteInfo.AntDeltaXyz = value; } }
        /// <summary>
        /// 概略坐标.
        /// </summary>
        public XYZ ApproxXyz { get { return SiteInfo.ApproxXyz; } set { SiteInfo.SetApproxXyz(value); } }
        /// <summary>
        /// 天线坐标偏移
        /// </summary>
        public HEN Hen { get { return SiteInfo.Hen; } set { SiteInfo.Hen = value; } }


        #endregion

        #region 观测信息 ObservationInfo 快捷属性
        /// <summary>
        /// 时间系统
        /// </summary>
        public TimeSystem TimeSystem { get; set; }
        /// <summary>
        /// 第一次观测时间
        /// </summary>
        public Time StartTime { get { return ObsInfo.StartTime; } set { ObsInfo.StartTime = value; } }

        /// <summary>
        /// 最后一次观测时间
        /// </summary>
        public Time EndTime { get { return ObsInfo.EndTime; } set { ObsInfo.EndTime = value; } }
        /// <summary>
        /// 时段
        /// </summary>
        public TimePeriod TimePeriod { get => new TimePeriod(StartTime, EndTime); }

        /// <summary>
        /// 采样率
        /// </summary>
        public double Interval { get { return ObsInfo.Interval; } set { ObsInfo.Interval = value; } }
        #endregion
        #region 扩充属性
         
        /// <summary>
        /// 国家代码，为从RINEX3文件名提取，通常为3个字符的代码
        /// </summary>
        public string CountryCode { get; set; }

        #endregion

        #region Rinex 头文件独有属性
        /// <summary>
        /// 版本号
        /// </summary>
        public double Version { get; set; }
        /// <summary>
        /// 文件类型
        /// </summary>
        public RinexFileType FileType { get; set; }
        /// <summary>
        /// 卫星系统，单一卫星系统起作用，如果为多系统，代码为M，则需要进一步判断。
        /// 这只是一个前期头部记录器。
        /// 这是一个简单的标记、记录，需要赋值判断。
        /// </summary>
        public SatelliteType SatTypeMarker { get; set; }
      /// <summary>
        /// 文件信息，通常是创建信息。
        /// </summary>
        public FileInfomation FileInfo { get; set; }
 

        /// <summary>
        /// RINEX 3.0
        /// </summary>
        public string MarkerType { get; set; } 
        /// <summary>
        /// 观测值数量,V2 对于所有系统均采用同样的观测类型
        /// 如果 M 设置了，则不统计它。
        /// </summary>
        public int GetObsTypesCount(SatelliteType SatelliteType)
        {
            int i = 0;
            foreach (var item in this.ObsCodes)
            {
                //如果有 M，则表示为 RINEX2.X 数据。
                //因为 RINEX 3.X 回分别采用 G，C等。
                if (item.Key == SatelliteType) { return item.Value.Count; }
              //  time += key.Value.Count;
            }
            return i;
        }
        
        /// <summary>
        /// 闰秒
        /// </summary>
        public int LeapSeconds { get; set; }
        public bool IsEndTimeRead { get; internal set; }


        #endregion

        #endregion
        /// <summary>
        /// V2 的区别是:对所有的系统采用同样的观测码类型，如C1,C2
        /// 但此处只是简单记录，并不所有都赋予，系统将在读取内容数据前进行甄别和更新。
        /// 此处采用的是懒加载的方式。避免 TotallObserTypesCount 出现统计错误。
        /// </summary>
        /// <param name="typeOfObs"></param>
        public void SetTypeOfObservesV2(string[] typeOfObs)
        {
            //此处怎样判断卫星系统类型？？？？
            //如果是 M，则对应的GPS，GLONASS（如果有的话），都应该在此获取观测码类型序号。
            ObsCodes[SatTypeMarker] = new List<string>(typeOfObs);
        }
        /// <summary>
        /// 尝试采用文件名称设置国家代码
        /// </summary>
        public string TrySetAndGetContryCodeWithFileName()
        {
            if (String.IsNullOrWhiteSpace(FileName) || FileName.Length < 35)
            {
                this.CountryCode = "000";
            }
            else
            {
                this.CountryCode = FileName.Substring(6, 3); 
            }
            return this.CountryCode;
        }

        #region 方法

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            var clone = new RinexObsFileHeader
            { 
                SiteInfo = SiteInfo.Clone() as ISiteInfo,
                ObsInfo = ObsInfo.Clone() as IObsInfo,
                Version = Version,
                FileType = FileType,
                SatTypeMarker = SatTypeMarker,
                FileInfo = FileInfo,
                Comments = Comments,
                MarkerType = MarkerType, 
                LeapSeconds = LeapSeconds,
                TimeSystem = TimeSystem, 
                GlonassCodePhaseBias = this.GlonassCodePhaseBias,
                GlonassSlotFreqNums = GlonassSlotFreqNums,
                SystemPhaseShift=SystemPhaseShift,
            };
            return clone;
        }

        #endregion

    }
}
