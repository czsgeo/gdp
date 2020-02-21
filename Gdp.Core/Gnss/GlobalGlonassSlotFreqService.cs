//2018.09.25, czs, create in hmx, 全局自动GLONASS频率服务


using System;
using System.IO;
using System.Text; 
using Gdp.Data;
using System.Collections.Generic; 
using Gdp.Data.Rinex;
using Gdp.IO;

namespace Gdp
{

  

    /// <summary>
    /// 全局自动GLONASS频率服务。
    /// </summary>
    public class GlobalGlonassSlotFreqService // : IService
    {
        Log log = new Log(typeof(GlobalGlonassSlotFreqService));

        /// <summary>
        /// 默认构造函数
        /// </summary>
        private GlobalGlonassSlotFreqService()
        {
            this.Name = "全局自动GLONASS频率服务";
            SatTimePeriodValueStoarge = PeriodNumerialStorage.Read(Setting.GlonassSlotFreqFile);

        }
        static GlobalGlonassSlotFreqService instance = new GlobalGlonassSlotFreqService();
        /// <summary>
        /// 单例模式
        /// </summary>
        public static GlobalGlonassSlotFreqService Instance
        {
            get => instance;
        }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 存储对象
        /// </summary>
        PeriodNumerialStorage SatTimePeriodValueStoarge { get; set; }

        internal double Get(SatelliteNumber prn, Time time)
        {
            double k = 0;
            k = SatTimePeriodValueStoarge.Get(prn.ToString(), time);
            if (!Gdp.Utils.DoubleUtil.IsValid(k) && Setting.GlonassSlotFrequences.ContainsKey(prn))
            {
                 k = Setting.GlonassSlotFrequences[prn];
            }
            if (!Gdp.Utils.DoubleUtil.IsValid(k))
            {
                //throw new Exception("获取GLONASS频率错误！！");
                log.Error(prn + " " + time + ", 获取GLONASS频率错误！！以 1 代替");
                k = 1;
            }
                return k;
        }
    }

}