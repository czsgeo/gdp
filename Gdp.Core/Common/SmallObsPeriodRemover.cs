//2018.08.16, czs, create in  hmx, 小观测段数据移除器


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO; 
using System.Linq.Expressions; 

using Gdp.Data;
using Gdp.Data.Rinex; 

using Gdp.IO;

namespace Gdp
{
    /// <summary>
    /// 小观测段数据移除器
    /// </summary>
    public class SmallObsPeriodRemover<T> : Reviser<T>
        where T : IEpochObsData
    {
        new  Log log = new Log(typeof(SmallObsPeriodRemover<T>));
        /// <summary>
        /// 小观测段数据移除器
        /// </summary>
        /// <param name="BufferedStream"></param>
        /// <param name="MinEpochCount"></param>
        /// <param name="MaxBreakCount"></param> 
        /// <param name="LastEpoch">如果不正确提供，请设置为最大</param>
        public SmallObsPeriodRemover(BufferedStreamService<T> BufferedStream, int MinEpochCount, int MaxBreakCount, Time LastEpoch, double interval =0)
        {
            this.BufferedStream = BufferedStream;
            this.BufferedStream.MaterialEnded += BufferedStream_MaterialEnded;
            this.Buffers = this.BufferedStream.MaterialBuffers;

            this.MinEpochCount = MinEpochCount;
            this.MaxBreakCount = MaxBreakCount; 
            this.LastEpoch = LastEpoch;
            this.Inverval = interval;

            this.WindowDataManager = new WindowDataManager<SatelliteNumber, Time>(this.MinEpochCount + this.MaxBreakCount + 2);
        }
        /// <summary>
        /// 小观测段数据移除器
        /// </summary> 
        /// <param name="MinEpochCount"></param>
        /// <param name="MaxBreakCount"></param> 
        /// <param name="LastEpoch">如果不正确提供，请设置为最大</param>
        public SmallObsPeriodRemover(int MinEpochCount, int MaxBreakCount, Time LastEpoch, double interval = 0)
        {
            this.MinEpochCount = MinEpochCount;
            this.MaxBreakCount = MaxBreakCount; 
            this.LastEpoch = LastEpoch;
            this.Inverval = interval;

            this.WindowDataManager = new WindowDataManager<SatelliteNumber, Time>(this.MinEpochCount + this.MaxBreakCount + 2);
        }

        private void BufferedStream_MaterialEnded()
        {
            //        LastEpoch = BufferedStream.Last().Time;//czs, 2018.06.23, 这个不可用，否则末尾数据将离奇消失！！！
            //缓存小时前，还要一些判断的收尾工作要做

            JudgeAndFiltSmallSpanInBuffer(this.BufferedStream.DataSource.Current.ReceiverTime, true);
        } 

        #region  属性
        /// <summary>
        /// 数据流
        /// </summary>
        public BufferedStreamService<T> BufferedStream { get; set; }
        /// <summary>
        /// 管理窗口数据，判断是否连续
        /// </summary>
        WindowDataManager<SatelliteNumber, Time> WindowDataManager { get; set; }
        /// <summary>
        /// 最后一个历元
        /// </summary>
        public Time LastEpoch { get; set; }
        /// <summary>
        /// 采样间隔
        /// </summary>
        public double Inverval { get;
            set; }
        /// <summary>
        /// 历元最大断裂数量
        /// </summary>
        public int MaxBreakCount { get; set; }
        /// <summary>
        /// 最小历元数量
        /// </summary>
        public int MinEpochCount { get; set; }
        /// <summary>
        ///最小断裂间隔时间，单位：秒
        /// </summary>
        public double MinBreakTimeSpan
        {
            get
            {
                return this.Inverval * this.MaxBreakCount;
            }
        }
        /// <summary>
        /// 当前
        /// </summary>
        public T Current { get; set; }
        #endregion

        public override bool Revise(ref T obj)
        {
            if (obj.Equals(Current)) { return true; }

            this.Current = obj; 

            if (obj is RinexEpochObservation)
            {
                CheckAndFiltSmallSpan(obj as RinexEpochObservation);
            }

            this.Current = obj;

            return true;
        }

        public override void Complete()
        {
            base.Complete();
            if (Current != null)
            {
                JudgeAndFiltSmallSpanInBuffer(Current.ReceiverTime, true);
            }
        }

        /// <summary>
        /// 过滤掉小历元时段，在缓存中操作，确保缓存大于断裂数量
        /// </summary>
        /// <param name="info"></param>
        private void CheckAndFiltSmallSpan(RinexEpochObservation info)
        {
            //添加
            foreach (var sat in info) { WindowDataManager.GetOrCreate(sat.Prn).Add(info.ReceiverTime); }
            ////保证有足够的数量
            //if (!WindowDataManager.HasFull) { return; }

            JudgeAndFiltSmallSpanInBuffer(info.ReceiverTime, info.ReceiverTime == LastEpoch);
        } 


        /// <summary>
        /// 判断过滤
        /// </summary>
        /// <param name="currentTime"></param>
        /// <param name="isLastEpoch"></param>
        private void JudgeAndFiltSmallSpanInBuffer(Time currentTime, bool isLastEpoch)
        {

            if (this.Inverval == 0 || this.Inverval == Double.MaxValue) { this.Inverval = GetInteval(this.Buffers); }

            //执行判断,并移除历元卫星
            foreach (var prn in WindowDataManager.Keys)
            {
                var window = WindowDataManager.Get(prn);
                if (window.Count == 0) { continue; }
               

                //当前与上一个在指定历元内，且非最后，即后续还有数据需要添加后再判断，这样可以节约大量时间
                var lastSan = Math.Abs(currentTime - window.Last);
                if (lastSan < MinBreakTimeSpan && !isLastEpoch)
                {
                    continue;
                }


                //按照允许的最大间隔分离
                var gapedWindows = window.Split((prev, current) => { return Math.Abs(current - prev) > MinBreakTimeSpan; });

                int subCount = 0;
                foreach (var subWindow in gapedWindows)
                {
                    subCount++;

                    if (subWindow.Count == 0) { continue; }



                    //包括当前，且非最后，即后续还有数据需要添加后再判断
                    if (subWindow.Contains(currentTime) && !isLastEpoch)
                    {
                        continue;
                    }

                    //没有包括当前，但是窗口最后一个历元与当前不超过指定的阈值，且非最后历元，则直接继续，需要添加后判断
                    lastSan = Math.Abs(currentTime - subWindow.Last);
                    if (!subWindow.Contains(currentTime) && lastSan < MinBreakTimeSpan && !isLastEpoch)
                    {
                        continue;
                    }

                    //-----------以下数据进行判断长度并删除过短的数据-----------

                    //小于指定的数量
                    if (subWindow.Count < MinEpochCount && this.Buffers != null)
                    {
                        log.Debug(prn + " 第 " + subCount + "/" + gapedWindows.Count + " 段 " + "，在缓存中移除时段 " + subWindow.ToString() + ", 当前： " + currentTime + ", 与最后差(s)：" + lastSan + ", 是否最后历元：" + isLastEpoch);
                        int i = 0;
                        foreach (var epoch in this.Buffers)
                        {
                            if (epoch.Contains(prn) && window.Contains(epoch.ReceiverTime))
                            {
                                epoch.Remove(prn);
                                i++;
                            }
                        }

                        //处理最后一个历元
                        if (isLastEpoch)
                        {
                            var epo = Current;
                            if (epo.Contains(prn) && window.Contains(epo.ReceiverTime))
                            {
                                epo.Remove(prn);
                                i++;
                            }
                        }
                        if (i > 0)
                        {
                            log.Debug(prn + " 移除了 " + i + " 个");
                        }
                    }

                    //除了当前历元（已跳过continue），判断一次后清空过期的数据窗口，第二段重新积累，历史车轮滚滚向前
                    window.Remove(subWindow);
                }
            }

        }


        /// <summary>
        /// 计算采用间隔。单位：秒。
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public double GetInteval(IWindowData<T> buffer)
        {
            double interval = 30;
            if (buffer == null)
            {
                log.Warn("采样间隔获取失败，设为 " + interval);
                return interval;
            }

            int i = 0;
            T prev = default(T);
            foreach (var item in buffer)
            {
                if (prev == null) { prev = item; continue; }
                interval = Math.Min(interval, Math.Abs(item.ReceiverTime - prev.ReceiverTime));
                if (i > 5) break;
                i++;
            }
            return interval;
        }
    }
}
