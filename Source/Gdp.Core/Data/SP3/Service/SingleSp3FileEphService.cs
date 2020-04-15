//2013.11.04, czs, edit, 采用二级缓存加快计算速度
//2014.06.24, 崔阳, edit, 修改了IsHealth(),首先判断是否满足插值的条件。判断是否满足插值的限差   Cui Yang 2014.06.24
//2014.12.29,czs, edit in namu, 缓冲不易设置太大，如果设置2倍的话，会造成在星历拼接的时段结果不稳定。
//2015.05.10, czs, edit in namu, 分离数据与服务
//2016.03.06, czs, edit in hongqing, 优化
//2016.09.27, czs, edit in hongqqing, 优化，可用性判断

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO; 


namespace Gdp.Data.Rinex
{



    /// <summary>
    /// Rinex Sp3 文件，一个对象代表一个SP3文件。
    /// 由于SP3文件通常较小，所以加载时，就直接读取。
    /// </summary>
    public class SingleSp3FileEphService : FileEphemerisService
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="MinSequentialSatCount"></param>
        /// <param name="IsAvailableOnly"></param>
        public SingleSp3FileEphService(string filePath, int MinSequentialSatCount = 11, int MaxBreakingCount = 5, bool IsAvailableOnly = true, int ephInterOder = 10)
            : this(new Sp3Reader(filePath, IsAvailableOnly).ReadAll().GetSatEphemerisCollection(), MinSequentialSatCount, MaxBreakingCount, ephInterOder)
        { 
        }
        /// <summary>
        /// 以文件初始化
        /// </summary>
        /// <param name="Sp3File"></param>
        /// <param name="MinSequentialSatCount"></param>
        /// <param name="MaxBreakingCount"></param>
        public SingleSp3FileEphService(SatEphemerisCollection Sp3File, int MinSequentialSatCount = 11, int MaxBreakingCount = 5, int ephInterOder = 10)
        { 
            this.MinSequentialSatCount = MinSequentialSatCount;
            this.MaxBreakingCount = MaxBreakingCount;
            this.Order = ephInterOder;

            SetSp3File(Sp3File);
        }
        /// <summary>
        /// 初始化插值器
        /// </summary>
        private void Init()
        {
            var interval =  Sp3File.Interval == 0 ? 1 : Sp3File.Interval;// Sp3Reader.Header.EpochInterval;
            WarnedPrns = new List<SatelliteNumber>();
            EphemerisManager = new EphemerisManager(interval, MaxBreakingCount); 
            Sp3File.TimePeriod.SetSameBuffer(interval * 10);

            foreach (var prn in Sp3File.Prns)
            {
                var storage = EphemerisManager.GetOrCreate(prn);

                var all = Sp3File.Get(prn);
                if (all == null || all.Count == 0) { continue; }

                storage.Add(all.Values);
            }
        }
        public SingleSp3FileEphService() { }

        public static SingleSp3FileEphService Empty = new SingleSp3FileEphService();

        /// <summary>
        /// 修改数据源，重新初始化。
        /// </summary>
        /// <param name="Sp3File"></param>
        public void SetSp3File(SatEphemerisCollection Sp3File)
        {
            this.Sp3File = Sp3File;
            this.Name = Sp3File.Name;
            this.Init();
        }

        #region 属性
        /// <summary>
        /// 原始SP3星历存储。
        /// </summary>
        EphemerisManager EphemerisManager { get; set; }
        /// <summary>
        /// SP3 文件 
        /// </summary>
        public SatEphemerisCollection Sp3File { get; private set; }

        /// <summary>
        /// 该星历采用的坐标系统,如 IGS08， ITR97
        /// </summary>
        public override string CoordinateSystem { get { return Sp3File.CoordinateSystem; } }
        /// <summary>
        /// 允许星历断裂的最大次数（采样率为间隔）
        /// </summary>
        public int MaxBreakingCount { get; set; }
        /// <summary>
        /// 卫星的最小连续数量
        /// </summary>
        public int MinSequentialSatCount { get; set; }
        /// <summary>
        /// 拟合阶数
        /// </summary>
        public int Order { get; set; }
        #endregion

        #region 实现接口 FileEphemerisDataSource
        /// <summary>
        /// 有效时段，缓冲不易设置太大，如果设置2倍的话，会造成在星历拼接的时段结果不稳定。
        /// </summary>
        public override BufferedTimePeriod TimePeriod { get { return Sp3File.TimePeriod; } }
        /// <summary>
        /// 星历服务类型
        /// </summary>
         /// <summary>
        /// 卫星数量，通过头文件获取
        /// </summary>
        public override int SatCount { get { return Prns.Count; } }
        /// <summary>
        /// 所有的卫星编号
        /// </summary>
        public override List<SatelliteNumber> Prns { get { return Sp3File.Prns; } }

        #region 方法
        /// <summary>
        /// 获取文件中存储的原始星历信息。
        /// </summary>
        /// <param name="prn"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public override List<Gdp.Ephemeris> Gets(SatelliteNumber prn, Time from, Time to)
        {
            if (Sp3File == null) return new List<Ephemeris>();

            var data  = Sp3File.Get(prn);
            List<Gdp.Ephemeris> prnEphes = data.Values;
            if (prnEphes == null) { return new List<Gdp.Ephemeris>(); }

            return prnEphes.FindAll(m => m.Time >= from && m.Time <= to);
        }
        /// <summary>
        /// 获取所有星历信息。
        /// </summary>
        /// <returns></returns>
        public override List<Gdp.Ephemeris> Gets()
        {
            if (Sp3File == null) return new List<Ephemeris>();
            var entities = new List<Gdp.Ephemeris>();
            foreach (var sec in Sp3File) { entities.AddRange(sec); }
            return entities;
        }
        /// <summary>
        ///一颗卫星只警告一次，避免卡顿
        /// </summary>
        List<SatelliteNumber> WarnedPrns { get; set; }

        //考虑并行情况，2014.09.14
        /// <summary>
        /// 获取指定时刻，某卫星的信息。可能需要拟合计算。 
        /// 失败则返回 null。
        /// </summary>
        /// <param name="prn"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public override Ephemeris Get(SatelliteNumber prn, Time time)
        {
            if (Sp3File == null) return  null;

            if (!TimePeriod.BufferedContains(time))
            {
                log.Error(prn + "," + time + " 已经超出星历服务时段范围 " + TimePeriod);
                return null;
            }

            var ephes = EphemerisManager.GetOrCreate(prn);
            if (ephes ==null || ephes.Count == 0)
            {
                if (!WarnedPrns.Contains(prn))
                {
                    log.Warn("当前SP3文件 " + Name + " 没有这颗卫星的信息 " + prn);
                    WarnedPrns.Add(prn);
                }
                return null;
            }

            //获取原始星历
            var storage = ephes.GetSatEphemerises(time);
            if (storage == null) { return null; }
            if (storage.Count < MinSequentialSatCount)//连续数量太少，拟合将不准确。
            {
                log.Warn(prn + "," + time + " 连续数量太少，拟合将不准确。" + storage.Count + " 至少应 " + MinSequentialSatCount);
                return null;
            }
            var secondOfWeek = time.SecondsOfWeek;
            //判断防止周日时间退减到周六
            if (time.DayOfWeek == DayOfWeek.Saturday && Math.Abs(TimeConsts.SECOND_PER_WEEK - secondOfWeek) < 2)
            {
                secondOfWeek -= TimeConsts.SECOND_PER_WEEK;//回滚到下一周
            }
            //找出最近的部分
            List<double> indexes = Utils.DoubleUtil.GetNearst(storage.Keys, secondOfWeek, Order);
            var entities = storage.GetSubEphemerises(indexes);

            //插值 
            using (var interpolator = new EphemerisInterpolator(entities, Order))
            {
                return interpolator.GetEphemerisInfo(time);
            }

            //return GetInterpolatorResult(prn, time, entities);
        }

        /// <summary>
        /// 卫星是否可用，进行简单的判断。
        /// </summary>
        /// <param name="prn"></param>
        /// <param name="satTime"></param>
        /// <returns></returns>
        public override bool IsAvailable(SatelliteNumber prn, Time satTime)
        {
            if (!TimePeriod.BufferedContains(satTime))
            {
                //log.Error(prn + "," + time + " 已经超出星历服务时段范围" + Sp3File.TimePeriod);
                return false;
            }
            return true;
        }

        #endregion
        #endregion

    }
}
