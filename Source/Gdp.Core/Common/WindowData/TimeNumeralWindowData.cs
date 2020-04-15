//2016.05.10, czs, edit， 滑动窗口数据
//2016.08.04, czs, edit in fujian yongan, 修正
//2017.09.11, czs, edit in hongiqng, 重构，为了更好的使用

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gdp.IO;





namespace Gdp
{
    /// <summary>
    /// 时间数值字典窗口数据
    /// </summary>
    public class TimeNumeralWindowDataManager : TimeNumeralWindowDataManager<string>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="windowSize"></param>
        /// <param name="breakSpanSeconds"></param>
        public TimeNumeralWindowDataManager(int windowSize, double breakSpanSeconds = 121):base(windowSize, breakSpanSeconds)
        { 
        }
    }

    //2017.09.23, czs, create in hongiqng, 数值字典窗口数据
    /// <summary>
    /// 时间数值字典窗口数据
    /// </summary>
    public class TimeNumeralWindowDataManager<TKey> : BaseDictionary<TKey, TimeNumeralWindowData> 
        where TKey : IComparable<TKey>
    {
        /// <summary>
        /// 数值字典窗口数据
        /// </summary>
        /// <param name="windowSize"></param>
        /// <param name="breakSpanSeconds"> 最大时间断裂，秒 </param>
        public TimeNumeralWindowDataManager(int windowSize, double breakSpanSeconds  =121)
        {
            this.WindowSize = windowSize;
            this.BreakKeyGap = breakSpanSeconds;
        }
        /// <summary>
        /// 窗口大小
        /// </summary>
        public int WindowSize { get; set; }
        /// <summary>
        /// 值断裂大小
        /// </summary>
        public double BreakKeyGap { get; set; }


        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override TimeNumeralWindowData Create(TKey key)
        {
            return new TimeNumeralWindowData(WindowSize) {  MaxKeyGap = BreakKeyGap };
        }
    }
     


    //2017.09.23, czs, create in hongiqng, 数值字典窗口数据

    /// <summary>
    /// 时间作为索引的窗口数据。
    /// </summary>
    public class TimeNumeralWindowData : NumeralWindowData<Time>
    { /// <summary>
      /// 构造函数。
      /// </summary>
      /// <param name="windowSize">窗口大小</param>
      /// <param name="MaxBreakingSeconds"> 最大时间断裂，秒 </param>
        public TimeNumeralWindowData(int windowSize, double MaxBreakingSeconds = double.MaxValue) : base(windowSize) { KeyToDouble = k => k.SecondsOfWeek; this.MaxKeyGap = MaxBreakingSeconds; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dic"></param>
        public TimeNumeralWindowData(IDictionary<Time, double> dic)
            : base(dic) { KeyToDouble = k => k.SecondsOfWeek; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dic"></param>
        public TimeNumeralWindowData(IDictionaryClass<Time, double> dic)
            : base(dic) { KeyToDouble = k => k.SecondsOfWeek; } 


        #region 操作数
        public static TimeNumeralWindowData operator -(double val, TimeNumeralWindowData data) { return (-val) + data; }
        public static TimeNumeralWindowData operator -(TimeNumeralWindowData data, double val) { return data + (-val); }
        public static TimeNumeralWindowData operator +(double val, TimeNumeralWindowData data) { return data + val; }
        public static TimeNumeralWindowData operator +(TimeNumeralWindowData data, double val)
        {
            TimeNumeralWindowData result = new TimeNumeralWindowData(data.Count);
            foreach (var item in data.Data)
            {
                result.Add(item.Key, item.Value + val);
            }
            return result;
        }
        #endregion

        /// <summary>
        /// 直接以Time作为X,若数据数量太少，返回null
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public virtual TimedLsPolyFit GetTimedLsPolyFit(int order)
        {
            if (order >= this.Count)
            {
                return null;
            }
            var fit = new TimedLsPolyFit(this.Data, order);
            fit.Init();
            return fit;
        }
        /// <summary>
        /// 多项式拟合值，将尽量查找接近指定时刻的拟合，若数据数量太少，返回null
        /// </summary>
        /// <param name="time">时间，历元</param>
        /// <param name="order">拟合阶次</param>
        /// <param name="dataCount">参与计算的数量</param>
        /// <returns></returns>
        //public virtual RmsedNumeral GetPolyFitValue(Time time, int order, int dataCount)
        //{

        //    base.GetPolyFitValue()
        //    if (order >= this.Count)
        //    {
        //        return null;
        //    }

        //    TimedLsPolyFit fit = GetPolyFit(time, order, dataCount);
        //    return fit.GetRmsedY(time);
        //}

        public TimedLsPolyFit GetPolyFit(Time time, int order, int dataCount)
        {
            if (order >= this.Count)
            {
                return null;
            }
            var window = GetNearstWindowData(time, dataCount);

            var fit = new TimedLsPolyFit(window.Data, order);
            fit.Init();
            return fit;
        }

        /// <summary>
        /// 多项式拟合检核数据.
        /// 剔除粗差，分隔跳跃差为多个干净的小窗口,小窗口数据再检核。
        /// </summary> 
        /// <param name="order"></param>
        /// <param name="errorTimes"></param>
        /// <param name="minWindow"></param>
        /// <returns></returns>
        //public List<Time> GetKeysOfGrossError(int order = 2, double errorTimes = 3, double minWindow = 3)
        //{
        //    return base.GetKeysOfGrossError(m => m.SecondsOfWeek, order, errorTimes, minWindow); 
        //}      
    }
}