//2018.03.16, czs, create in hmx, 多文件多系统组合的星历服务

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO; 


namespace Gdp.Data.Rinex
{



    /// <summary>
    ///多文件多系统组合的星历服务
    /// </summary>
    public class CombinedSp3FileEphService : FileEphemerisService
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="source"></param>
        /// <param name="MinSequentialSatCount"></param>
        /// <param name="MaxBreakingCount">允许星历断裂次数</param>
        public CombinedSp3FileEphService(SatEphemerisCollection source, int MinSequentialSatCount = 11, int MaxBreakingCount= 5, int InterpolateOrder = 10)
        {
            this.MinSequentialSatCount = MinSequentialSatCount;
            this.MaxBreakingCount = MaxBreakingCount;
            this.Order = InterpolateOrder;
            SetSatEphemerisCollection(source);
        }


        /// <summary>
        /// 允许星历断裂的最大次数（采样率为间隔）
        /// </summary>
        public int MaxBreakingCount { get; set; }
        /// <summary>
        /// 初始化插值器
        /// </summary>
        private void Init()
        {
            var interval = SatEphemerisCollection.Interval == 0 ? 1 : SatEphemerisCollection.Interval;// Sp3Reader.Header.EpochInterval;
            WarnedPrns = new List<SatelliteNumber>();

            EphemerisManager = new EphemerisManager(interval, MaxBreakingCount);
            Order = 10;
            SatEphemerisCollection.TimePeriod.SetSameBuffer(interval * 0.2); //外推0.2采样率

            foreach (var prn in SatEphemerisCollection.Prns)
            {
                var storage = EphemerisManager.GetOrCreate(prn);

                var all = SatEphemerisCollection.Get(prn);
                if (all == null || all.Count == 0) { continue; }

                storage.Add(all.Values);
            }
        }
        /// <summary>
        /// 修改数据源，重新初始化。
        /// </summary>
        /// <param name="Sp3File"></param>
        public void SetSatEphemerisCollection(SatEphemerisCollection Sp3File)
        {
            this.SatEphemerisCollection = Sp3File;
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
        public SatEphemerisCollection SatEphemerisCollection { get; private set; }

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
        /// 该星历采用的坐标系统,如 IGS08， ITR97
        /// </summary>
        public override string CoordinateSystem { get { return SatEphemerisCollection.CoordinateSystem; } }

        /// <summary>
        /// 有效时段，缓冲不易设置太大，如果设置2倍的话，会造成在星历拼接的时段结果不稳定。
        /// </summary>
        public override BufferedTimePeriod TimePeriod { get { return SatEphemerisCollection.TimePeriod; } }
       /// <summary>
        /// 卫星数量，通过头文件获取
        /// </summary>
        public override int SatCount { get { return Prns.Count; } }
        /// <summary>
        /// 所有的卫星编号
        /// </summary>
        public override List<SatelliteNumber> Prns { get { return SatEphemerisCollection.Prns; } }
        /// <summary>
        ///一颗卫星只警告一次，避免卡顿
        /// </summary>
        List<SatelliteNumber> WarnedPrns { get; set; }

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
            var data  = SatEphemerisCollection.Get(prn);
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
            var entities = new List<Gdp.Ephemeris>();
            foreach (var sec in SatEphemerisCollection) { entities.AddRange(sec); }
            return entities;
        }
        /// <returns></returns>
        public override Ephemeris Get(SatelliteNumber prn, Time time)
        {
            if (!TimePeriod.BufferedContains(time))
            {
                log.Error(prn + "," + time + " 已经超出星历服务时段范围 " + TimePeriod);
                return null;
            }

            var ephes = EphemerisManager.GetOrCreate(prn);
            if (ephes == null || ephes.Count == 0)
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
             
            var entities =  storage.GetGetNearst(time, Order);

            //插值 
            using (var interpolator = new EphemerisInterpolator(entities, Order))
            {
                var result = interpolator.GetEphemerisInfo(time);
                if (result.XYZ.Length > 100000000)
                {
                    log.Error("尚不支持轨道半径" + ((result.XYZ.Length) / 1000).ToString("0 000 000.00") + " KM 的 GNSS 卫星 " + result.XYZ);
                }
                return result;
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
