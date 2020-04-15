//2014.06.24, czs, edit, 增加一些便利方法
//2014.08.19, czs, edit, 移除了Read方法 ，实现了 IBlockObservationDataSource 接口。
//2015.05.09, czs, edit in namu, 增加移除观测类型的方法
//2015.10.15, czs, edit in hongqing, 接口调整

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;



namespace Gdp.Data.Rinex
{
    /// <summary>
    /// 观测数据文件，一个测站所有的观测信息。
    /// 所有的文件都保存在这个类中。
    /// </summary>
    public class RinexObsFile : IEnumerable<RinexEpochObservation>, IDisposable
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public RinexObsFile()
        {
            _data = new List<RinexEpochObservation>();
        }

        #region 属性,字段
        #region 核心数据存储
        /// <summary>
        /// 记录内容。
        /// </summary>
        private List<RinexEpochObservation> _data { get; set; }
        /// <summary>
        /// 头文件
        /// </summary>
        public RinexObsFileHeader Header { get; set; }
        #endregion
        /// <summary>
        /// 数据是否改变，否则直接采用上一次结果。
        /// </summary>
        public bool IsChanged { get; set; }
        /// <summary>
        /// 测站信息。
        /// </summary>
        public ISiteInfo SiteInfo { get { return Header.SiteInfo; } }

        /// <summary>
        /// 索引器获取历元观测信息
        /// </summary>
        /// <param name="index">编号</param>
        /// <returns></returns>
        public RinexEpochObservation this[int index]
        {
            set { _data[index] = value; }
            get { return _data[index]; }
        }

        /// <summary>
        /// 采用内容时间更新头文件时间信息，并返回。
        /// </summary>
        /// <returns></returns>
        public TimePeriod UpdateAndGetHeaderTimePeriodWithContentTime()
        {
            this.Header.StartTime = StartTime;
            this.Header.EndTime = EndTime;
            return Header.TimePeriod;
        }
        /// <summary>
        /// 采用观测内容更新采用率
        /// </summary>
        /// <returns></returns>
        public double CheckOrUpdateAndGetHeaderIntervalWithContent()
        {
            var differ = Math.Abs(this._data[1].ReceiverTime - this._data[0].ReceiverTime);
            if (differ - this.Header.Interval < 0.001)
            {
                return this.Header.Interval;
            }
            this.Header.Interval = differ;

            return this.Header.Interval;
        }


        /// <summary>
        /// 首次观测时间，若无则返回  Time.Default.
        /// </summary>
        public Time StartTime
        {
            get
            {
                if (_data != null && Count > 0)
                    return _data[0].ReceiverTime;
                else return Time.Default;
            }
        }
        /// <summary>
        /// 最后观测时间，若无则返回 null
        /// </summary>
        public Time EndTime
        {
            get
            {
                if (_data != null && Count > 0)
                    return _data[_data.Count - 1].ReceiverTime;
                else return Time.Default;
            }
        }

        /// <summary>
        /// 观测历元数量。
        /// </summary>
        public int Count
        {
            get { return _data.Count; }
        }

        /// <summary>
        /// 采样间隔,单位：秒
        /// </summary>
        public double Interval
        {
            get { return Header.Interval; }
        }
        #endregion

        #region 核心方法
        /// <summary>
        /// 追加
        /// </summary>
        /// <param name="recs"></param>
        public void Add(RinexObsFile recs)
        {
            this.Header.EndTime = recs.EndTime;
            Add(recs._data);
        }
        /// <summary>
        /// 追加
        /// </summary>
        /// <param name="recs"></param>
        public void Add(IEnumerable<RinexEpochObservation> recs)
        {
            foreach (var rec in recs)
            {
                Add(rec);
            }
        }
        /// <summary>
        /// 增加一个
        /// </summary>
        /// <param name="rec"></param>
        public void Add(RinexEpochObservation rec)
        {
            _data.Add(rec);
        }
        /// <summary>
        /// 清空
        /// </summary>
        public void Clear() { _data.Clear(); }

        #region 检索，获取
        /// <summary>
        /// 获取指定编号的观测数据。如果失败则返回null
        /// </summary>
        /// <param name="gpsTime">时刻</param>
        /// <returns></returns>
        public RinexEpochObservation Get(int index)
        {
            if (index > -1 && index < _data.Count)
            {
                return _data[index];
            }
            return null;
        }

        /// <summary>
        /// 获取指定时刻的观测数据。
        /// </summary>
        /// <param name="gpsTime">时刻</param>
        /// <returns></returns>
        public RinexEpochObservation GetEpochObservation(Time gpsTime, double toleranceSeccond = 1e-15)
        {
            return _data.Find(m => (Double)Math.Abs(m.ReceiverTime - gpsTime) < toleranceSeccond);
        }

        /// <summary>
        /// 获取一部分原始数据
        /// </summary>
        /// <param name="fromIndex">从编号，含</param>
        /// <param name="count">数量</param>
        /// <returns></returns>
        public List<RinexEpochObservation> GetItems(int fromIndex, int count)
        {
            if (Count < fromIndex) return new List<RinexEpochObservation>();
            List<RinexEpochObservation> items = new List<RinexEpochObservation>();
            for (int i = fromIndex; i < Count && i < fromIndex + count; i++)
            {
                items.Add(_data[i]);
            }
            return items;
        }
        List<SatelliteNumber> totalPrns { get; set; }
        /// <summary>
        /// 获取出现过的卫星编号列表。
        /// </summary>
        /// <returns></returns>
        public List<SatelliteNumber> GetPrns()
        {
            if (totalPrns != null && !IsChanged) { return totalPrns; }

            totalPrns = new List<SatelliteNumber>();
            foreach (RinexEpochObservation obs in _data)
            {
                foreach (SatelliteNumber prn in obs.Prns)
                {
                    if (!totalPrns.Contains(prn)) totalPrns.Add(prn);
                }
            }
            totalPrns.Sort();
            return totalPrns;
        }
        /// <summary>
        /// 卫星
        /// </summary>
        /// <returns></returns>
        public Dictionary<SatelliteType, List<SatelliteNumber>> GetSatTypedPrns()
        {
            Dictionary<SatelliteType, List<SatelliteNumber>> dic = new Dictionary<SatelliteType, List<SatelliteNumber>>();
            List<SatelliteNumber> list = GetPrns();
            list.Sort();
            foreach (var sat in list)
            {
                if (!dic.ContainsKey(sat.SatelliteType)) { dic[sat.SatelliteType] = new List<SatelliteNumber>(); }
                dic[sat.SatelliteType].Add(sat);
            }

            return dic;
        }

        /// <summary>
        /// 通过卫星编号获取该卫星所有的观测值列表。
        /// </summary>
        /// <param name="prn">卫星编号</param>
        /// <returns></returns>
        public List<RinexSatObsData> GetEpochObservations(SatelliteNumber prn)
        {
            List<RinexSatObsData> list = new List<RinexSatObsData>();
            foreach (var item in _data)
            {
                if (item.Contains(prn)) list.Add(item[prn]);
            }
            return list;
        }
        /// <summary>
        /// 返回单星观测列表
        /// </summary>
        /// <param name="prn"></param>
        /// <returns></returns>
        public List<TimedRinexSatObsData> GetEpochTimedObservations(SatelliteNumber prn)
        {
            List<TimedRinexSatObsData> list = new List<TimedRinexSatObsData>();
            foreach (var item in _data)
            {
                if (item.Contains(prn)) list.Add(new TimedRinexSatObsData(item.ReceiverTime, item[prn]));
            }
            return list;
        }

        #endregion

        #region 遍历器
        /// <summary>
        /// 遍历器
        /// </summary>
        /// <returns></returns>
        public IEnumerator<RinexEpochObservation> GetEnumerator()
        {
            return _data.GetEnumerator();
        }
        /// <summary>
        /// 遍历器
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _data.GetEnumerator();
        }
        #endregion

        #endregion

        #region 扩展方法 

        #region 移除指定的观测类型
        /// <summary>
        /// 移除指定的观测类型
        /// </summary> 
        /// <param name="obsTypesTobeRemove"></param>
        public void RemoveObsType(Dictionary<SatelliteType, List<string>> obsTypesTobeRemove)
        {
            foreach (var item in obsTypesTobeRemove)
            {
                RemoveObsType(item.Key, item.Value);
            }
        }
        /// <summary>
        /// 移除指定的观测类型
        /// </summary>
        /// <param name="obsType"></param>
        public void RemoveObsType(SatelliteType satType, string obsType)
        {
            //移除头部
            Header.GetOrInitObsCodes(satType).Remove(obsType);

            foreach (var epochObs in this)//逐个历元
            {
                epochObs.RemoveObsType(satType, obsType);
            }
        }
        /// <summary>
        /// 移除指定的观测类型
        /// </summary>
        /// <param name="satType"></param>
        /// <param name="obsTypes"></param>
        public void RemoveObsType(SatelliteType satType, List<string> obsTypes)
        {
            //移除头部
            Header.GetOrInitObsCodes(satType).RemoveAll(m => obsTypes.Contains(m));

            foreach (var epochObs in this)//逐个历元
            {
                epochObs.RemoveObsType(satType, obsTypes);
            }
        }
        #endregion

        #endregion



        public void Dispose()
        {
            _data.Clear();
        }
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="prns"></param>
        /// <param name="epochs"></param>
        public void Remove(List<SatelliteNumber> prns, TimePeriod epochs)
        { 
            foreach (var prn in prns)
            {
                Remove(prn, epochs);
            }
        }
        public void Remove(List<SatelliteType> prns, TimePeriod epochs = null)
        {
            if(epochs == null)
            {
                epochs = this.Header.TimePeriod;
            }
            foreach (var prn in prns)
            {
                Remove(prn, epochs);
            }
        }
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="prn"></param>
        /// <param name="epochs"></param>
        public void Remove(SatelliteType prn, TimePeriod epochs)
        {
            foreach (var item in this._data)
            {
                if (!epochs.Contains(item.ReceiverTime)) { continue; }
                 
                  item.Remove(prn); 
            }
            IsChanged = true;
        }
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="prn"></param>
        /// <param name="epochs"></param>
        public void Remove(SatelliteNumber prn, TimePeriod epochs)
        {
            foreach (var item in this._data)
            {
                if (!epochs.Contains(item.ReceiverTime)) { continue; }
                if (item.Contains(prn))
                {
                    item.Remove(prn);
                }
            }
            IsChanged = true;
        }
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="prn"></param>
        /// <param name="epochs"></param>
        public void Remove(SatelliteNumber prn, List<Time> epochs)
        {
            foreach (var item in this._data)
            {
                if (!epochs.Contains(item.ReceiverTime)) { continue; }
                if (item.Contains(prn))
                {
                    item.Remove(prn);
                }
            }
            IsChanged = true;
        }
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="prn"></param>
        public void Remove(SatelliteNumber prn)
        {
            foreach (var item in this._data)
            {
                item.Remove(prn);
            }
            IsChanged = true;
        }


        /// <summary>
        /// 针对某一个卫星绘制
        /// </summary>
        /// <param name="prn"></param>
        /// <returns></returns>
        public ObjectTableStorage BuildObjectTable(SatelliteNumber prn, bool show1Only)
        {
            List<TimedRinexSatObsData> records = this.GetEpochTimedObservations(prn);
            ObjectTableStorage table = new ObjectTableStorage();
            foreach (var record in records)
            {
                table.NewRow();
                table.AddItem("Epoch", record.Time);
                foreach (var item in record.SatObsData)
                {
                    if (show1Only && !item.Key.Contains("1"))
                    {
                        continue;
                    }
                    table.AddItem(item.Key, item.Value.Value);
                }
            }
            return table;
        }

        /// <summary>
        /// 构建数据表，绘制相位数据
        /// </summary>
        /// <param name="isDrawAllPhase"></param>
        /// <returns></returns>
        public ObjectTableStorage BuildObjectTable(bool isDrawAllPhase)
        {
            ObjectTableStorage table = new ObjectTableStorage(this.Header.FileName);
            foreach (var epochInfo in this)
            {
                table.NewRow();
                //加下划线，确保排序为第一个
                table.AddItem("_Epoch", epochInfo.ReceiverTime);
                foreach (var sat in epochInfo)
                {
                    if (isDrawAllPhase)
                    {
                        foreach (var phase in sat)
                        {
                            var val = phase.Value.Value;
                            if (Gdp.Utils.DoubleUtil.IsValid(val) || val == 0)
                            {
                                table.AddItem(sat.Prn + "_" + phase.Key, val);
                            }
                        }
                    }
                    else if (sat.FirstAvailable != null && Gdp.Utils.DoubleUtil.IsValid(sat.FirstAvailable.Value))
                    {
                        table.AddItem(sat.Prn, sat.FirstAvailable.Value);
                    }
                }
                table.EndRow();
            }
            return table;
        }
    }
}
