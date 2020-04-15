//2014.5.22, cy, edit, 新增了方法
//2014.06.24， czs, edit, 添加Comment，移动Read方法到 RinexReader
//2014.08.19， czs, edit,  添加注释，程序修改为 ObservationData 的遍历器
//2015.05.09, czs, edit in namu, 增加移除观测类型的方法
//2018.09.08, czs, edit in hmx, 增加移除指定频率编号

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace Gdp.Data.Rinex
{ 
    /// <summary>
    /// 一组原始的RINEX观测记录。记录了在一个时刻，一个测站观测了多个卫星的值。
    /// 由多个 ObservationData 组成。
    /// </summary>
    public class RinexEpochObservation : BaseDictionary<SatelliteNumber, RinexSatObsData>, IEpochObsData
    {
        /// <summary>
        /// 默认构造函数，初始化参数。
        /// </summary>
        public RinexEpochObservation()
        {
            Comments = new List<string>();
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="time"></param>
        public RinexEpochObservation(Time time)
        {
            this.RawTime = time;
            Comments = new List<string>();
        }
        /// <summary>
        /// 增加一个
        /// </summary>
        /// <param name="RinexSatObsData"></param>
        public void Add(RinexSatObsData RinexSatObsData) { this.Add(RinexSatObsData.Prn, RinexSatObsData); }

        #region 字段、 属性
        /// <summary>
        /// 文件名称
        /// </summary>
        public override string Name
        {
            get
            {
                if (Header != null)
                return Header.FileName;
                return "无名";
            } 
        }
        /// <summary>
        /// 观测文件头文件。
        /// </summary>
        public RinexObsFileHeader Header { get; set; }
        /// <summary>
        /// 获取GPS时间
        /// </summary>
        /// <returns></returns>
        public Time GetGpsTime()
        {
            if(Header == null) { return RawTime; }
            switch(Header.TimeSystem)
            {
                case TimeSystem.BDT:
                    return RawTime + 14;
                default:
                    return RawTime;
            }
        }
        /// <summary>
        /// 原始时间，需结果Header的时间系统才能判断。
        /// </summary>
        public Time RawTime { get; set; }

        /// <summary>
        /// GNSS GPS 时间。
        /// </summary>
        public Time ReceiverTime => GetGpsTime();
        /// <summary>
        /// 历元标志：0表示正常，1表示在前一历元与当前历元之间发生了电源故障，>1为事件标志
        /// 8 为Gdp 钟跳标记。
        /// </summary>
        public int EpochFlag { get; set; }
        /// <summary>
        /// 卫星编号
        /// </summary>
        public List<SatelliteNumber> Prns { get { return  Keys; } }

        /// <summary>
        /// 注释行。
        /// </summary>
        public List<String> Comments { get; set; }

        /// <summary>
        /// 接收机钟差（可选） receiver clock offset (fraction, optional) 
        /// </summary>
        public double ReceiverClockOffset { get; set; }
        #endregion

        #region 数据管理方法
        /// <summary>
        /// 移除其它系统
        /// </summary>
        /// <param name="satelliteTypes"></param>
        public void RemoveOther(List<SatelliteType> satelliteTypes)
        {
            List<SatelliteNumber> others = new List<SatelliteNumber>();
            foreach (var item in this.Keys)
            {
                if (!satelliteTypes.Contains(item.SatelliteType))
                {
                    others.Add(item);
                }
            }
            this.Remove(others);
        }
        /// <summary>
        /// 移除指定频率编号
        /// </summary>
        /// <param name="frequenceNumToBeRemoved"></param>
        internal void RemoveFrequences(List<int> frequenceNumToBeRemoved)
        {
            foreach (var item in this)
            {
                item.RemoveFrequences(frequenceNumToBeRemoved);
            }
        }
        /// <summary>
        /// 移除指定系统的观测类型
        /// </summary>
        /// <param name="satType">系统类型</param>
        /// <param name="obsType">观测类型</param>
        public void RemoveObsType(SatelliteType satType, String obsType)
        {
            foreach (var item in this)
            {
                if (item.Prn.SatelliteType == satType)
                    item.Remove(obsType);
            }
        }
        /// <summary>
        /// 移除非此系统
        /// </summary>
        /// <param name="satTypes"></param>
        public void RemoveOthers(List<SatelliteType> satTypes)
        {
            var list = new List<SatelliteNumber>();
            foreach (var item in this)
            {
                if ( !satTypes.Contains( item.Prn.SatelliteType))
                    list.Add(item.Prn);
            }
            this.Remove(list);
        }

        /// <summary>
        /// 移除其它。
        /// </summary>
        /// <param name="obsTypes"></param>
        public void RemoveOthers(List<ObsTypes> obsTypes)
        {
            var types = new List<string>();
            foreach (var item in obsTypes)
            {
                types.Add(item.ToString());
            }
            foreach (var sat in this)
            {
                var list = new List<string>();
                foreach (var code in sat.ObsTypes)
                {
                    if (types.Contains(code[0].ToString()))
                    {
                        list.Add(code);
                    }
                }
                sat.RemoveOthers(list);
            }
        }
        /// <summary>
        /// 移除指定系统的卫星观测
        /// </summary>
        /// <param name="satType">系统类型</param> 
        public void Remove(SatelliteType satType)
        {
            var list = new List<SatelliteNumber>();
            foreach (var item in this)
            {
                if (item.Prn.SatelliteType == satType)
                    list.Add(item.Prn);
            }
            this.Remove(list);
        }
        /// <summary>
        /// 移除指定系统的观测类型
        /// </summary>
        /// <param name="satType">系统类型</param>
        /// <param name="obsType">观测类型</param>
        public void RemoveObsType(SatelliteType satType, List<String> obsType)
        {
            foreach (var item in this)
            {
                if (item.Prn.SatelliteType == satType)
                {
                    item.Remove(obsType);
                }
            }
        }

        /// <summary>
        /// 字符表示的对象信息
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("时间：" + ReceiverTime + "，卫星数量：" + Count);

            foreach (var item in Prns)
            {
                sb.Append(item.ToString() + " ");
            }
            sb.AppendLine();

            return sb.ToString();
        }
        #endregion

       
    }
}
