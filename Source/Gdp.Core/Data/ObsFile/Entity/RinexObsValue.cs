//2018.07.18, czs, edit in HMX, 增加了观测码 ObservationCode


using System;
using System.Collections.Generic;

using System.Text;

namespace Gdp.Data.Rinex
{ 
    /// <summary>
    /// 0-7 为rinex自有。8-9位Gdp数据指示，其中8表示OK，9表示Bad。
    /// 信号失锁指示，信号质量描述。Loss of lock indicator (LLI).0 or blank: OK or not known
    /// Bit 0 set : Lost lock between previous and current observation: Cycleslip possible. For phase observations only.
    /// Bit 1 set : Half-cycle ambiguity/
    /// possible. Software not capable of  handling half cycles should skip this observation. Valid for the current epoch only.
    /// Bit 2 set : Galileo BOC-tracking of an MBOC-modulated signal (may suffer from increased noise).
    /// </summary>
    public enum LossLockIndicator
    {
        /// <summary>
        /// OK or not known
        /// </summary>
        OKOrNotKnown = 0,
        /// <summary>
        ///  01
        /// </summary>
        CyclePossible1 = 1,
        /// <summary>
        ///  Bit 2 set : Galileo BOC-tracking of an MBOC-modulated signal (may suffer from increased noise).
        /// </summary>
        Two = 2,
        /// <summary>
        /// 可能周跳
        /// </summary>
        CyclePossible3 = 3,
        /// <summary>
        /// 第 4
        /// </summary>
        Four = 4,
        /// <summary>
        /// 可能周跳
        /// </summary>
        CyclePossible5 = 5,
        /// <summary>
        /// 第 6 
        /// </summary>
        Six = 6,
        /// <summary>
        /// 可能周跳
        /// </summary>
        CyclePossible7 = 7,
        /// <summary>
        /// 没问题
        /// </summary>
        OK = 8,
        /// <summary>
        /// 9
        /// </summary>
        Bad = 9,
    }

    /// <summary>
    /// RINEX 观测值数据
    /// OBSERVATIONS
    /// </summary>
    public class RinexObsValue
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="observationCode"></param>
        public RinexObsValue(double Value, string observationCode)
        {
            this.Value = Value;
            this.ObservationCode = ObservationCode.Parse(observationCode);
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="ObservationCode"></param>
        public RinexObsValue(double Value, ObservationCode ObservationCode)
        {
            this.Value = Value;
            this.ObservationCode = ObservationCode;
        }
        /// <summary>
        /// 观测码
        /// </summary>
        public ObservationCode ObservationCode { get; set; }
        /// <summary>
        /// 观测数值
        /// </summary>
        public double Value { get; set; }
        /// <summary>
        /// 失锁指示(LLI)
        /// LLI 的范围为0~7。0或空格表示正常或未知；
        /// bit 0 若设置，表示可能有周跳；即 1 3 5 7 可能有周跳
        /// bit 1 设置表示可能有半周跳；
        /// bit 2 Galileo BOC-tracking of an MBOC-modulated signal (may suffer from increased noise).
        /// </summary>
        public LossLockIndicator LossLockIndicator { get; set; } 

        /// <summary>
        /// 信号强度
        /// 在RINEX格式中，用1~9表示信号强度：1表示可能的最小信号强度，5表示良好S/N比的阈值，9表示可能的最大信号强度，0或空格表示未知或未给出。
        /// </summary>
        public int SignalStrength { get; set; }

        /// <summary>
        /// 是否产生了周跳。
        /// </summary>
        /// <returns></returns>
        public bool IsLossLock
        {
            get=> LossLockIndicator == LossLockIndicator.Bad
                || LossLockIndicator == LossLockIndicator.CyclePossible1
                || LossLockIndicator == LossLockIndicator.CyclePossible3
                || LossLockIndicator == LossLockIndicator.CyclePossible5
                || LossLockIndicator == LossLockIndicator.CyclePossible7;
        }
        /// <summary>
        /// 字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Value + "";
        }
        /// <summary>
        /// 值为0。
        /// </summary>
        public static RinexObsValue Zero { get { return new RinexObsValue(0, ObservationCode.Empty); } }

    }
}
