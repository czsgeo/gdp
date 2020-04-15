//2014.06.24, 崔阳, edit, 修改了IsHealth(),首先判断是否满足插值的条件。
//2014.12.29,czs, edit in namu, 缓冲不易设置太大，如果设置2倍的话，会造成在星历拼接的时段结果不稳定。 
//2015.05.10, czs, edit in namu, 分离数据与服务
//2018.03.16, czs, create in hmx, 单颗卫星的星历列表


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO; 
using Gdp;

namespace Gdp.Data.Rinex
{

    /// <summary>
    /// Rinex Sp3 文件，一个对象代表一个SP3文件。采用时间和卫星编号双索引管理。
    /// 按时间顺序依次向后排列。
    /// </summary>
    public class Sp3File : BaseDictionary<Time, Sp3Section>
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public Sp3File(Sp3Header header = null)
        {
            this.Header = header ?? new Sp3Header();
            IsBuildCollection = false;
        }

       public  const string UNDEF = "UNDEF";
        /// <summary>
        /// 该星历采用的坐标系统,如 IGS08， ITR97
        /// </summary>
        public string CoordinateSystem
        {
            get
            {
                if (Header == null) return UNDEF;
                return Header.CoordinateSystem;
            }
        }
        /// <summary>
        /// 按照卫星编号管理
        /// </summary>
        public SatEphemerisCollection SatEphemerisCollection { set; get; }
        /// <summary>
        /// 指示是否构建集合
        /// </summary>
        public bool IsBuildCollection { get; set; }
        /// <summary>
        /// 时段
        /// </summary>
        public BufferedTimePeriod TimePeriod
        {
            get
            {
                CheckOrBuildIndexCollection();
                return SatEphemerisCollection.TimePeriod;
            }
        }
        /// <summary>
        /// 构建索引
        /// </summary>
        public void CheckOrBuildIndexCollection()
        {
            if (!IsBuildCollection) { BuildIndexCollection(); }
        }

        /// <summary>
        /// 数据源代码
        /// </summary>
        public string SourceCode { get { return Header.SourceName.Substring(0, 2); } }
        /// <summary>
        /// 头部信息。
        /// </summary>
        public Sp3Header Header { get; set; }
        /// <summary>
        /// 所有的PRN
        /// </summary>
        public List<SatelliteNumber> Prns
        {
            get
            {
                if (Header.PRNs != null && Header.PRNs.Count != 0)
                {
                    return Header.PRNs;
                }
                else
                {
                    CheckOrBuildIndexCollection();
                    Header.PRNs = new List<SatelliteNumber>(SatEphemerisCollection.Keys);
                    return Header.PRNs;
                }
            }
        }

        /// <summary>
        /// 添加到双索引
        /// </summary>
        /// <param name="Sp3Section"></param>
        public void Add(Sp3Section Sp3Section)
        {
            if (this.Contains(Sp3Section.Time))
            {
                return;
            }
             
            this.Add(Sp3Section.Time, Sp3Section);
       
            IsBuildCollection = false;
        }
        /// <summary>
        /// 移除其它
        /// </summary>
        /// <param name="satTypes"></param>
        public void RemoveOther(List<SatelliteType> satTypes)
        {
            foreach (var item in this)
            {
                item.RemoveOther(satTypes);
            }
            List<SatelliteNumber> prns = new List<SatelliteNumber>();

            foreach (var item in this.Header.PRNs)
            {
                if (!satTypes.Contains(item.SatelliteType))
                {
                    prns.Add(item);
                }
            }
            foreach (var prn in prns)
            {
                this.Header.PRNs.Remove(prn);
            }
        }

        /// <summary>
        /// 构建索引集合
        /// </summary>
        public void BuildIndexCollection()
        {
            if (!IsBuildCollection)
            {
                this.SatEphemerisCollection = GetSatEphemerisCollection();
                IsBuildCollection = true;
            }
        }

        /// <summary>
        /// 快速移除第一个历元的数据
        /// </summary>
        public override void RemoveFirst()
        {
            Time firstTime = TimePeriod.Start;// GetMinTime();

            base.Remove(firstTime);

            foreach (var list in this.SatEphemerisCollection)
            {
                list.Remove(firstTime);
            }
        }
      

        /// <summary>
        /// 生成集合
        /// </summary>
        /// <returns></returns>
        public SatEphemerisCollection GetSatEphemerisCollection()
        {
            SatEphemerisCollection satClockCollection = new SatEphemerisCollection(true, SourceCode); 
             
            foreach (var sec in this)
            {
                foreach (var item in sec)
                {
                    var obj = satClockCollection.GetOrCreate(item.Prn);
                    obj.Add(item);
                }
            } 
            return satClockCollection;
        }
        /// <summary>
        /// 信息显示
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Header + "";
        }
    }
}