//2016.05.10, czs, edit, 滑动窗口数据
//2016.08.04, czs, edit in fujian yongan, 修正
//2017.09.11, czs, edit in hongiqng, 重构，为了更好的使用
//2017.09.23, czs, create in hongiqng, 数值字典窗口数据
//2018.05.20, czs, edit in hmx, 增加数据多项式平滑功能
//2018.07.10, czs, edit in HMX, 增加分段拟合方法

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gdp.IO;
 

namespace Gdp
{

    //2018.05.27, czs, edit in hmx, 数值字典窗口数据管理器

    /// <summary>
    /// 数值字典窗口数据
    /// </summary>
    public class NumeralWindowDataManager<TWindowKey, TDataKey> : BaseDictionary<TWindowKey, NumeralWindowData<TDataKey>>
        where TWindowKey : IComparable<TWindowKey>
        where TDataKey : IComparable<TDataKey>
    {
        /// <summary>
        /// 数值字典窗口数据
        /// </summary>
        /// <param name="windowSize"></param>
        /// <param name="BreakKeyGap"> 最大断裂</param>
        public NumeralWindowDataManager(int windowSize, double BreakKeyGap = 5)
        {
            this.WindowSize = windowSize;
            this.BreakKeyGap = BreakKeyGap;
        }
        /// <summary>
        /// 窗口大小
        /// </summary>
        public int WindowSize { get; set; }
        /// <summary>
        /// 键值断裂大小,，如果超出，则重新建立窗口。
        /// </summary>
        public double BreakKeyGap { get; set; }
        /// <summary>
        /// 创建函数
        /// </summary>
        public Func<TDataKey, double> CreateWindowFunc;
        /// <summary>
        /// 设置窗口函数
        /// </summary>
        /// <param name="CreateFunc"></param>
        /// <returns></returns>
        public   NumeralWindowDataManager<TWindowKey, TDataKey> SetCreateWindowFunc(Func<TDataKey, double> CreateFunc = null)
        {
            this.CreateWindowFunc = CreateFunc;
            return this;
        }
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override NumeralWindowData<TDataKey> Create(TWindowKey key)
        {
            return new NumeralWindowData<TDataKey>(WindowSize) { MaxKeyGap = BreakKeyGap , CreateFunc = CreateWindowFunc };
        }
        /// <summary>
        /// 工厂方法
        /// </summary>
        /// <param name="windowSize"></param>
        /// <param name="BreakKeyGap"></param>
        /// <returns></returns>
        public static NumeralWindowDataManager<TWindowKey, TDataKey> Create(int windowSize, double BreakKeyGap = 5)
        {
            return new NumeralWindowDataManager<TWindowKey, TDataKey>(windowSize, BreakKeyGap);
        } 
    }
     


    /// <summary>
    /// 带检索关键字的数据窗口。
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class NumeralWindowData<TKey> : WindowData<TKey, double> where TKey : IComparable<TKey>
    {
        #region 构造函数和初始化函数
        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="windowSize">窗口大小</param>
        public NumeralWindowData(int windowSize) : base(windowSize) { ValueToDouble = m => m; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dic"></param>
        public NumeralWindowData(IDictionary<TKey, double> dic) : base(dic) { ValueToDouble = m => m; }
        public NumeralWindowData(BaseDictionary<TKey, double> dic) : base(dic) { ValueToDouble = m => m; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dic"></param>
        public NumeralWindowData(IDictionaryClass<TKey, double> dic)
            : base(dic) { ValueToDouble = m => m; }
        /// <summary>
        /// 初始化
        /// </summary>
        protected override void Init()
        {
            base.Init();
            PolyfitBuilders = new BaseDictionary<int, PolyfitBuilder>() { CreateFunc = new Func<int, PolyfitBuilder>(keyOrder => new PolyfitBuilder(keyOrder)) };
            //Geo.Utils.DoubleUtil.AutoSetKeyToDouble(out KeyToDouble, this.TheVeryFirstKey);//基类已设
        }
        #endregion 

        #region 操作数
        public static NumeralWindowData<TKey> operator -(double val, NumeralWindowData<TKey> data) { return (-val) + data; }
        public static NumeralWindowData<TKey> operator -(NumeralWindowData<TKey> data, double val) { return data + (-val); }
        public static NumeralWindowData<TKey> operator +(double val, NumeralWindowData<TKey> data) { return data + val; }
        public static NumeralWindowData<TKey> operator +(NumeralWindowData<TKey> data, double val)
        {
            NumeralWindowData<TKey> result = new NumeralWindowData<TKey>(data.Count);
            foreach (var item in data.Data)
            {
                result.Add(item.Key, item.Value + val);
            }
            return result;
        }

        /// <summary>
        /// 计算与此值之差的最大值。
        /// </summary>
        /// <param name="referVal"></param>
        /// <returns></returns>
        public KeyValuePair<TKey, double> GetMaxDifferReferTo(double referVal)
        {
            double maxValue = double.MinValue;
            TKey key = default(TKey);
            foreach (var kv in this.KeyValues)
            {
                var differ = Math.Abs(kv.Value - referVal);
                if(differ > maxValue)
                {
                    maxValue = differ;
                    key = kv.Key;
                }
            }
            return new KeyValuePair<TKey, double>(key,  maxValue);
        }
        
        /// <summary>
        /// 拟合后返回各项的拟合误差
        /// </summary>
        /// <param name="polyFitOrder"></param>
        /// <returns></returns>
        public NumeralWindowData<TKey> GetFitErrorWindow(int polyFitOrder)
        {
            NumeralWindowData<TKey> errors =  new NumeralWindowData<TKey>(this.WindowSize) { KeyToDouble = KeyToDouble };
            var fitter = BuildPolyfitter(polyFitOrder);

            var firstKey = this.TheVeryFirstKey;
            foreach (var item in this.OrderedData)
            {
                var x = KeyToDouble(item.Key) - KeyToDouble(firstKey);
                var error = item.Value - fitter.GetY(x);
                errors.Add(item.Key, error);
            }
            return errors;
        }

        #endregion

        #region 属性

        /// <summary>
        /// 当前拟合器。
        /// </summary>
        public BaseDictionary<int, PolyfitBuilder> PolyfitBuilders { get; set; }

        protected override void OnDataClearing()
        {
            base.OnDataClearing();
            PolyfitBuilders.Clear();
        }
        /// <summary>
        /// 获取后一个减去前一个，迭代相减的值，高次差值。
        /// </summary>
        /// <returns></returns>
        public double LoopDifferValue { get { return Gdp.Utils.DoubleUtil.GetLoopDifferValue(this.Values.ToArray()); } }

        /// <summary>
        /// 求绝对值平均
        /// </summary>
        public double AbsAverageValue { get { return Gdp.Utils.DoubleUtil.AbsAverage(this); } }
        /// <summary>
        /// 求平均
        /// </summary>
        public double AverageValue { get { return Gdp.Utils.DoubleUtil.Average(this); } }

        /// <summary>
        /// 具有权值的平均值
        /// </summary>
        public RmsedNumeral Average
        {
            get
            {
                var val = Gdp.Utils.DoubleUtil.GetAverageWithStdDev(this);
                return new RmsedNumeral(val[0], val[1]);
            }
        }

        /// <summary>
        /// 上一个平均值,在调用清空前赋值。
        /// </summary>
        public RmsedNumeral LastAverage { get; set; }
        #endregion

        #region  方法
        /// <summary>
        ///清空
        /// </summary>
        public override void Clear()
        {
            this.LastAverage = this.Average;
            base.Clear();
        }

        /// <summary>
        /// 探测的粗差或周跳，首先按照Key进行分离成多个子窗体，分别探测。其中Key断裂口，也认为是粗差。
        /// </summary>
        /// <param name="funcKeyToDouble">将Key转换为数值</param> 
        /// <param name="maxTimeSpan"></param>
        /// <param name="PolyFitOrder"></param>
        /// <param name="MaxErrorTimes"></param>
        /// <param name="MinWindowSize"></param>
        /// <param name="differTime"></param>
        /// <param name="IsFirstWindow"></param>
        /// <returns></returns>
        public List<TKey> SplitAndGetKeysOfGrossError(Func<TKey, double> funcKeyToDouble, double maxTimeSpan, int PolyFitOrder, double MaxErrorTimes, double MinWindowSize, double differTime = 0, bool IsFirstWindow = true)
        {
            List<TKey> grossTimes = new List<TKey>();
            //首先，按照允许的时间跨度，将其分解为多时段子窗口
            var subWins = this.Split(funcKeyToDouble, maxTimeSpan);
            int i = 0;
            foreach (var win in subWins)
            {
                var subWin = win;
                //探测本窗口粗差
                for (int j = 0; j < differTime; j++)
                {
                    subWin = subWin.GetDifferWindow();
                }

                var errorKeys = subWin.GetKeysOfGrossError(funcKeyToDouble, PolyFitOrder, MaxErrorTimes, MinWindowSize);
                //将Key断裂口，也认为是粗差。
                if (!errorKeys.Contains(win.FirstKey))
                {
                    //如果起始窗口，则添加，否则跳过
                    if (!IsFirstWindow && i == 0) { continue; }

                    grossTimes.Add(win.FirstKey);
                }
                grossTimes.AddRange(errorKeys);
                i++;
            }
            return grossTimes;
        }

        /// <summary>
        /// 多项式拟合检核数据.
        /// 首先采用大阈值滑动平均，进行粗探，结果分离成多个子窗口，再采用多项式拟合细探测。
        /// </summary> 
        /// <param name="funcKeyToDouble">将Key转换为数值</param>
        /// <param name="order"></param>
        /// <param name="errorTimes"></param>
        /// <param name="minWindowSize"></param>
        /// <returns></returns>
        public List<TKey> GetKeysOfGrossError(Func<TKey, double> funcKeyToDouble, int order = 2, double errorTimes = 3, double minWindowSize = 3)
        {
            var errTimes = 10;
            var subWindows = SplitByMovingAverage(errTimes);

            //分别计算每一个分段的粗差。
            List<TKey> totalKeys = new List<TKey>();
            foreach (var window in subWindows)
            {
                //如果分段太小，则全部认为粗差。
                if (window.Count < minWindowSize)
                {
                    totalKeys.AddRange(window.Keys);
                    continue;
                }
                //分段计算
                var windowKeys = new List<TKey>();
                var wds = window.Split(10);
                foreach (var item in wds)
                {
                    var keys = Gdp.Utils.DoubleUtil.GetKeysOfGrossError(item.Data, funcKeyToDouble, errorTimes, order, true);
                    windowKeys.AddRange(keys);
                }
                //默认第一个，为粗差。
                if (!windowKeys.Contains(window.FirstKey)) { windowKeys.Insert(0, window.FirstKey); }

                totalKeys.AddRange(windowKeys);
            }

            return totalKeys;
        }
        #region 分离
        /// <summary>
        /// 按照指定长度截断。最后如果太小，则直接追加到上一个。
        /// </summary>
        /// <param name="aboutSize">大致大小</param>
        /// <returns></returns>
        private List<NumeralWindowData<TKey>> Split(int aboutSize)
        {
            if (this.Count <= aboutSize)
            {
                return new List<NumeralWindowData<TKey>>() { this };
            }

            var initSelects = new List<NumeralWindowData<TKey>>();
            var window = new NumeralWindowData<TKey>(aboutSize);

            foreach (var kv in this.Data)
            {
                if (window.IsFull)
                {
                    initSelects.Add(window);
                    window = new NumeralWindowData<TKey>(aboutSize);
                }

                window.Add(kv.Key, kv.Value);
            }

            //检查，最后如果太小，则直接追加到上一个。
            if (window.Count < aboutSize / 2)
            {
                var last = initSelects.Last();
                last.WindowSize += window.Count;
                last.Add(window.Data);
            }
            else
            {
                initSelects.Add(window);
            }

            return initSelects;
        }

        /// <summary>
        /// 按键间隔分离小窗口。如果间之间的间距超过了指定宽度，则分离成两个或多个子窗口。
        /// </summary>
        /// <param name="funcKeyToDouble">将Key转换为数值</param>
        /// <param name="maxKeySpan"></param>
        /// <returns></returns>
        public List<NumeralWindowData<TKey>> Split(Func<TKey, double> funcKeyToDouble, double maxKeySpan)
        {
            var initSelects = new List<NumeralWindowData<TKey>>();
            var window = new NumeralWindowData<TKey>(Int16.MaxValue);
            initSelects.Add(window);

            var lastKeyVal = Double.MinValue;
            foreach (var kv in this.Data)
            {
                var key = kv.Key;
                var val = kv.Value;
                var keVal = funcKeyToDouble(kv.Key);

                if (window.Count < 1) { window.Add(kv.Key, kv.Value); lastKeyVal = keVal; continue; }

                var differ = Math.Abs(lastKeyVal - keVal);

                var isPassed = differ <= maxKeySpan;
                if (isPassed)
                {
                    window.Add(kv.Key, kv.Value);
                }
                else
                {
                    //update window size
                    window.WindowSize = window.Count;
                    //create new
                    window = new NumeralWindowData<TKey>(Int16.MaxValue);
                    initSelects.Add(window);
                }
                //update
                lastKeyVal = keVal;
            }
            window.WindowSize = window.Count;
            return initSelects;
        }
        /// <summary>
        /// 采用滑动平均值方法，对值进行比较，如果超限则就地分解。
        /// </summary>
        /// <param name="errorTimes"></param>
        /// <param name="judgeWindowSize"></param>
        /// <returns></returns>
        public List<NumeralWindowData<TKey>> SplitByMovingAverage(int errorTimes, int judgeWindowSize = 10)
        {
            //1.初选，采用平均法，放大方差要求，将数据进行分段。
            //遍历，
            var initSelects = new List<NumeralWindowData<TKey>>();
            var window = new NumeralWindowData<TKey>(Int16.MaxValue);
            initSelects.Add(window);

            foreach (var kv in this.Data)
            {
                var key = kv.Key;
                var val = kv.Value;

                if (window.Count < 1) { window.Add(kv.Key, kv.Value); continue; }

                RmsedNumeral ave = window.SubNumeralWindow(window.Count - judgeWindowSize, judgeWindowSize).Average;
                var differ = Math.Abs(ave.Value - kv.Value);
                var rms = ave.Rms == 0 ? double.MaxValue : ave.Rms;

                var errorThreshold = errorTimes * rms;
                var isPassed = differ <= errorThreshold;
                if (isPassed)
                {
                    window.Add(kv.Key, kv.Value);
                }
                else
                {
                    window.WindowSize = window.Count;
                    window = new NumeralWindowData<TKey>(Int16.MaxValue);
                    initSelects.Add(window);
                }
            }
            window.WindowSize = window.Count;
            return initSelects;
        }
        /// <summary>
        /// 截取子窗口
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public virtual NumeralWindowData<TKey> SubNumeralWindow(int startIndex, int len)
        {
            var bva = base.SubWindow(startIndex, len);
            return new NumeralWindowData<TKey>(bva.Data);
        }

        /// <summary>
        /// 采用平均值方法，对值进行比较，如果超限则就地分解。
        /// </summary>
        /// <param name="errorTimes"></param>
        /// <returns></returns>
        public List<NumeralWindowData<TKey>> SplitByAverage(int errorTimes)
        {
            //1.初选，采用平均法，放大方差要求，将数据进行分段。
            //遍历，
            var initSelects = new List<NumeralWindowData<TKey>>();
            var window = new NumeralWindowData<TKey>(Int16.MaxValue);
            initSelects.Add(window);

            foreach (var kv in this.Data)
            {
                var key = kv.Key;
                var val = kv.Value;

                if (window.Count < 1) { window.Add(kv.Key, kv.Value); continue; }

                var ave = window.Average;
                var differ = Math.Abs(ave.Value - kv.Value);
                var rms = ave.Rms == 0 ? double.MaxValue : ave.Rms;

                var errorThreshold = errorTimes * rms;
                var isPassed = differ <= errorThreshold;
                if (isPassed)
                {
                    window.Add(kv.Key, kv.Value);
                }
                else
                {
                    window.WindowSize = window.Count;
                    window = new NumeralWindowData<TKey>(Int16.MaxValue);
                    initSelects.Add(window);
                }
            }
            window.WindowSize = window.Count;
            return initSelects;
        }
        #endregion

        /// <summary>
        /// 获取差分指定次的数据窗口结果，后减前
        /// </summary>
        /// <returns></returns>
        public NumeralWindowData<TKey> GetDifferWindow(int differCount)
        {
            NumeralWindowData<TKey> result = this;
            for (int i = 0; i < differCount; i++)
            {
                result = result.GetDifferWindow();
            }
            return result;
        }

        /// <summary>
        /// 获取差分一次的数据窗口结果。键值存储为后一个。
        /// </summary>
        /// <returns></returns>
        public NumeralWindowData<TKey> GetDifferWindow()
        {
            var keys = this.OrderedKeys;
            var length = this.Count;
            Dictionary<TKey, double> dic = new Dictionary<TKey, double>();
            for (int i = 1; i < length; i++)
            {
                var keyPrev = keys[i - 1];
                var key = keys[i];

                dic[key] = this[key] - this[keyPrev];
            }

            return new NumeralWindowData<TKey>(dic) { KeyToDouble = this.KeyToDouble };
        }


        /// <summary>
        /// 判断最后一个数据是否超限。通过值与多项式拟合值之差判断。
        /// 这个方法很不理想！！！2018.05.28，czs， hmx
        /// </summary> 
        /// <param name="maxRmsTimes">最大的偏差，绝对值</param>
        /// <param name="order">拟合阶次</param>
        /// <returns></returns>
        public bool IsLastOverLimited(int order = 2, double maxRmsTimes = 3.0)
        {
            var fitter = BuildPolyfitter(order);
            var lastKey = LastKey;
            var lastVal = this[lastKey];
            double lastX = this.Count -1;
            double fitVal = 0;
            if (KeyToDouble != null)
            {
                lastX = KeyToDouble(lastKey) - KeyToDouble(this.TheVeryFirstKey);
            }
            fitVal = fitter.GetY(lastX);

            var delta = Math.Abs(lastVal - fitVal);
            var isOvered = delta > maxRmsTimes * fitter.StdDev;

             

            return isOvered;
        }

        /// <summary>
        /// 判断这个数据是否超限，通过前期的拟合值比较之差判断。
        /// 如果由于时间断裂，或数量不足阶次+1，也认为超限。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="newVal">待判断的数据</param>
        /// <param name="maxRmsTimes">最大的偏差，绝对值</param>
        /// <param name="order">拟合阶次</param>
        /// <returns></returns>
        public bool IsOverLimitedByPrevPolyfit(TKey key, double newVal, int order = 2, double maxRmsTimes = 3.0)
        {
            var fitVal = this.GetPolyFitValue(key, order);
            var delta = Math.Abs(newVal - fitVal.Value);
            var isOvered = delta > maxRmsTimes * fitVal.Rms;
            if (isOvered)
            {
                int ii = 0;
            }
            return isOvered;

            //var added = this.Clone();
            //added.Add(key, newVal);
            //if(added.Count <= order + 1) { return false; }
            //return added.IsLastOverLimited(order, maxRmsTimes);
        }


        /// <summary>
        /// 判断这个数据是否超限。通过下一个值与当前平均值之差判断。
        /// </summary>
        /// <param name="newVal">待判断的数据</param>
        /// <param name="maxDelta">最大的偏差，绝对值</param>
        /// <param name="isRelativeError">是否是相对误差，即百分数。</param>
        /// <returns></returns>
        public bool IsOverLimitedByAverage(double newVal, double maxDelta = 0.1, bool isRelativeError = true)
        {
            var delta = Math.Abs(newVal - AverageValue);
            if (isRelativeError)
            {
                delta /= Math.Abs(AverageValue);
            }
            var result = delta > maxDelta;
            return result;
        }

        /// <summary>
        /// 是否是水平数据的粗差，通过已知数据求平均判断，并不将其参与计算。
        /// </summary>
        /// <param name="newVal"></param> 
        /// <param name="errorTimes"></param>
        /// <returns></returns>
        public bool IsGrossOfLeveling(double newVal, double errorTimes = 3)
        {
            return (Gdp.Utils.DoubleUtil.IsGross(this, newVal, errorTimes));
        }
        /// <summary>
        /// 平均数和RMS检核新数据，通过则添加，返回ture，否则清空，添加该数据，并返回false。
        /// 如果未满，则直接添加，不检核，并返回true。
        /// 如果断裂，返回 false。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="newVal"></param> 
        /// <param name="errorTimes"></param> 
        /// <returns></returns>
        public bool AverageCheckAddOrClear(TKey key, double newVal, double errorTimes = 3)
        {
            if (!this.IsFull) { this.Add(key, newVal); return true; }
            var ave = this.Average;
            var error = Math.Abs(ave.Value - newVal);
            var maxError = ave.Rms * errorTimes;
            if (error > maxError)
            {
                this.Clear();
                this.Add(key, newVal);
                return false;
            }

            this.Add(key, newVal);
            return true;
        }
        /// <summary>
        /// 建立一个副本
        /// </summary>
        /// <returns></returns>
        public new NumeralWindowData<TKey> Clone()
        {
            return new NumeralWindowData<TKey>(base.Clone()) { MaxKeyGap = this.MaxKeyGap, KeyToDouble = KeyToDouble };
        }
        /// <summary>
        /// 具有差分功能的检查。1）数量不足或合格则直接添加，返回true；2）不合格，则清空后添加，返回 false；
        ///3） 如果时间超限，则清空添加，返回true。 
        /// </summary>
        /// <param name="time"></param>
        /// <param name="newVal"></param>
        /// <param name="maxRmsTimes"></param>
        /// <param name="differCount"></param>
        /// <returns></returns>
        public bool DifferCheckAddOrClear(TKey time, double newVal, double maxRmsTimes = 10, int differCount = 1)
        {
            if (!this.IsFull)
            {
                this.Add(time, newVal);
                return true;
            }
            //添加后的差分结果
            var after = this.Clone();
            after.Add(time, newVal);
            var afterWindow = after.GetDifferWindow(differCount);

            //当前差分结果
            var differWindow = this.GetDifferWindow(differCount);
            var aveVal = differWindow.Average;
            var differVal = Math.Abs(differWindow.Last - afterWindow.Last);
            //  log.Info("differVal: " + differVal + "  Rms:" + aveVal.Rms + " maxRmsTimes:" + maxRmsTimes);
            if (differVal > aveVal.Rms * maxRmsTimes)
            {
                this.Clear();
                this.Add(time, newVal);
                return false;
            }

            this.Add(time, newVal);
            return true;
        }

        /// <summary>
        /// 得到剔除粗差（默认3倍中误差）的数据列表。采用平均的方法。
        /// </summary>
        /// <param name="errorTimes"></param>
        /// <returns></returns>
        public NumeralWindowData<TKey> GetNeatlyWindowData(double errorTimes = 3)
        {
            if (this.Count == 0) return new NumeralWindowData<TKey>(this.WindowSize);

            var dic = Gdp.Utils.DoubleUtil.GetNeatlyData<TKey>(this.OrderedData, errorTimes);
            return Create(dic);
        }

        /// <summary>
        /// 得到剔除粗差（默认3倍中误差）的数据列表。
        /// </summary>
        /// <param name="fitOrder">拟合阶次</param>
        /// <param name="errorTimes"></param>
        /// <param name="isLoop">是否循环</param>
        /// <returns></returns>
        public NumeralWindowData<TKey> GetNeatlyWindowData(int fitOrder = 2, double errorTimes = 3, bool isLoop = true)
        {
            if (this.Count == 0) return new NumeralWindowData<TKey>(this.WindowSize);

            var dic = Gdp.Utils.DoubleUtil.GetNeatlyData<TKey>(this.OrderedData, this.KeyToDouble, errorTimes, fitOrder, isLoop);
            return Create(dic);
        }

        private NumeralWindowData<TKey> Create(IDictionary<TKey, double> dic)
        {
            return new NumeralWindowData<TKey>(dic) { KeyToDouble = this.KeyToDouble };
        }

        /// <summary>
        /// 获取最近的窗口数据。舍去多余数据。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="count">数量，如果大于总数，则等于总数</param>
        /// <returns></returns>
        public virtual NumeralWindowData<TKey> GetNearstWindowData( TKey key, int count)
        {
            //全部返回
            if (count >= this.Count) return this;
            var orderedKeys = this.OrderedKeys;

            var keys = Gdp.Utils.ComparableUtil.GetNearst<TKey>(orderedKeys, key, count, KeyToDouble);
            Dictionary<TKey, double> dic = new Dictionary<TKey, double>();
            foreach (var k in keys)
            {
                dic.Add(k, this[k]);
            }

            return Create(dic);
        }

        /// <summary>
        ///得到粗差编号（默认3倍中误差）列表。。
        /// </summary>
        /// <param name="errorTimes"></param>
        /// <param name="loop">是否循环</param>
        /// <returns></returns>
        public List<TKey> GetKeysOfGrossError(double errorTimes = 3, bool loop = true)
        {
            return Gdp.Utils.DoubleUtil.GetKeysOfGrossError(this.OrderedData, errorTimes, loop);
        }
        /// <summary>
        /// 具有差分功能的多项式拟合检查。1）数量不足或合格则直接添加，返回true；2）不合格，则清空后添加，返回 false；
        ///3） 如果时间超限，则清空添加，返回true。 
        /// </summary>
        /// <param name="time"></param>
        /// <param name="newVal"></param>
        /// <param name="maxRmsTimes"></param>
        /// <param name="differCount"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public bool DifferPloyfitCheckAddOrClear(TKey time, double newVal, int order = 2, double maxRmsTimes = 10, int differCount = 1)
        {
            if (!this.IsFull)
            {
                this.Add(time, newVal);
                return true;
            }
            //添加后的差分结果
            var after = this.Clone();
            after.Add(time, newVal);
            var afterWindow = after.GetDifferWindow(differCount); 

            if (afterWindow.Count > 1 && afterWindow.IsLastOverLimited(order, maxRmsTimes))
            {
                this.Clear();
                this.Add(time, newVal);
                return false;
            }

            this.Add(time, newVal);
            return true;
        }

        /// <summary>
        /// 多项式拟合检核新数据，通过则添加，返回ture，否则清空，添加该数据，并返回false。
        /// 如果未满，则直接添加，不检核，并返回true。
        /// 如果key断裂，返回false。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="newVal"></param>
        /// <param name="order"></param>
        /// <param name="maxRmsTimes"></param> 
        /// <returns>添加则返回true，否则false</returns>
        public bool PolyfitCheckAddOrClear(TKey key, double newVal, int order, double maxRmsTimes = 3)
        {
            if (!this.IsFull) { bool isOvered = this.IsKeyBreaked(key); this.Add(key, newVal); return isOvered; }

            if (IsOverLimitedByPrevPolyfit(key, newVal, order, maxRmsTimes))
            {
                this.Clear();
                this.Add(key, newVal);
                return false;
            }

            this.Add(key, newVal);
            return true;
        }
        /// <summary>
        /// 修复效果不理想。2017.10.10
        /// 多项式拟合检核新数据，通过则直接添加，返回修正后的该值
        /// 如果未满，则直接添加，不检核。
        /// 如果数据达到最大断裂数，则清空之前数据重新对齐添加。
        /// 采用Tag存储对齐数据。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="newVal"></param>
        /// <param name="order"></param>
        /// <param name="maxRmsTimes"></param>
        /// <param name="referVal">参考值</param>
        /// <returns>添加则返回true，否则false</returns>
        public double FitCheckAddAndAlign(TKey key, double newVal, double referVal = 0, int order = 2, double maxRmsTimes = 10)
        {
            //对齐功能,第一次
            if (this.Tag == null || this.Count == 0) { this.Tag = (referVal - newVal); }
            var correctedNewVal = newVal + (double)(Tag);

            if (!this.IsFull) { this.Add(key, correctedNewVal); return correctedNewVal; }
            //干净拟合数据   
            var neatWin = GetNeatlyWindowData(order, 3, true);
            RmsedNumeral nextFitVal = neatWin.GetPolyFitValue(key, order);

            var differ = Math.Abs(nextFitVal.Value - correctedNewVal);
            if (differ > nextFitVal.Rms * maxRmsTimes)
            {
                //采用拟合值修复采用参考值修复
                Tag = (nextFitVal.Value - newVal);

                correctedNewVal = newVal + (double)(Tag);
                this.Clear();
            }

            Add(key, correctedNewVal);
            return correctedNewVal;
        }

        /// <summary>
        ///具有认为阶梯的对齐。
        /// 多项式拟合检核新数据，通过则直接添加，返回修正后的该值
        /// 如果未满，则直接添加，不检核。
        /// 如果数据达到最大断裂数，则清空之前数据重新对齐添加。
        /// 采用Tag存储对齐数据。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="newVal"></param>
        /// <param name="order"></param>
        /// <param name="maxRmsTimes"></param>
        /// <param name="referVal">参考值</param>
        /// <returns>添加则返回true，否则false</returns>
        public double FitCheckAddAndAlignWithStep(TKey key, double newVal, out bool isReseted, double referVal = 0, int order = 2, double maxRmsTimes = 10, double step = 100)
        {
            //对齐功能,第一次
            if (this.Tag == null || this.Count == 0) { this.Tag = Math.Round(referVal - newVal); isReseted = true; }//整数对齐,第一次
            var correctedNewVal = newVal + (double)(Tag);

            if (!this.IsFull)
            {
                //在未满前，也应该做出判断。此处采用参考值判断。
                if (Math.Abs(correctedNewVal - referVal) > 1000)
                {
                    Tag = GetAligner(newVal, referVal, step, 0);
                    correctedNewVal = newVal + (double)(Tag);
                    isReseted = true;
                }
                else
                {
                    isReseted = false;
                }

                this.Add(key, correctedNewVal);

                return correctedNewVal;
            }

            //干净拟合数据

            var neatWin = GetNeatlyWindowData(order, 3, true);
            RmsedNumeral nextFitVal = neatWin.GetPolyFitValue(key, order);

            var differ = Math.Abs(nextFitVal.Value - correctedNewVal);
            if (differ > nextFitVal.Rms * maxRmsTimes)
            {
                //避免和前一个修正采用相同方向的偏移。但围绕参考值，不至于太远。
                var fitVal = nextFitVal.Value;
                Tag = GetAligner(newVal, referVal, step, fitVal);
                correctedNewVal = newVal + (double)(Tag);
                this.Clear();
                isReseted = true;
            }
            else
            {
                isReseted = false;
            }

            Add(key, correctedNewVal);
            return correctedNewVal;
        }
        /// <summary>
        /// 计算对齐参数
        /// </summary>
        /// <param name="newVal"></param>
        /// <param name="referVal"></param>
        /// <param name="step"></param>
        /// <param name="fitVal"></param>
        /// <returns></returns>
        private static double GetAligner(double newVal, double referVal, double step, double fitVal = 0)
        {
            var alignVal = Math.Round(referVal - newVal);

            if (fitVal > referVal)
            {
                alignVal -= step;
            }
            else
            {
                alignVal += step;
            }
            return alignVal;
        }
        /// <summary>
        ///  拟合一个数据。以指定Key为中心，拟合数据数为半径选择数据进行拟合求值，若在边界，则选择靠边界的拟合数据进行拟合。
        ///  返回的中误差为平差的单位权均方差.
        /// </summary>
        /// <param name="newKey">自变量 X 值</param>
        /// <param name="order"></param> 
        /// <param name="fitDataCount">拟合数据个数，以X为中心，两边扩展</param>
        /// <returns></returns>
        public virtual RmsedNumeral GetPolyFitValue(TKey newKey, int order = 2, int fitDataCount = int.MaxValue)
        {
            if (this.Count == 0) return RmsedNumeral.NaN;
            if (this.Count == 1) return new RmsedNumeral(this.First, 0);

            var fit = BuildPolyfitBuilder(order, newKey, fitDataCount, false);

            //double nextIndexVal = this.Count;
            //if (this.KeyToDouble != null)
            //{
            //    nextIndexVal = this.KeyToDouble(newKey) - this.KeyToDouble(this.TheVeryFirstKey);
            //}
            var x = this.KeyToDouble(newKey);
            var val = fit.GetPolyfitValue(x, false);
            return val;
        }

        /// <summary>
        /// 加权平均值。
        /// </summary>
        /// <param name="newKey"></param>
        /// <param name="fitDataCount"></param>
        /// <param name="isNeedRms"></param>
        /// <returns></returns>
        public virtual RmsedNumeral GetAdaptiveLinearFitValue(TKey newKey, int fitDataCount = int.MaxValue, bool isNeedRms  =true)
        {
            if (this.Count == 0) return RmsedNumeral.NaN;
            if (this.Count == 1) return new RmsedNumeral(this.First, 0);

            var indexs = FindIndexRange(newKey, fitDataCount);
            
            int startIndex = indexs[0];
            int endIndex = indexs[1];

            if (endIndex > this.Count - 1 || endIndex < 1)
            {
                endIndex = this.Count - 1;
            }

            bool hasKeyToDouble = KeyToDouble != null;
            startIndex = Gdp.Utils.IntUtil.GetPositiveOrZero(startIndex);
            var keys = this.OrderedKeys;

            double newKeyV = KeyToDouble(newKey);

            //重新生成，以当前Key为基准
            List<double[]> data = new List<double[]>();
            for (int i = startIndex; i <= endIndex; i++)
            {
                var key = keys[i];
                double keyV = i;
                if (hasKeyToDouble) { keyV = KeyToDouble(key); }

                var val = this[key];
                double distance = Math.Abs(keyV - newKeyV);
                //这样中间三个具有相同的权值
                if(distance == 0 ) {
                    if (Interval == 0) { distance = 1; }
                    else { distance = Interval; }
                }

                double p = 1.0 / distance;//距离的倒数当权

                data.Add(new double[] { val,  p});
            }

            return Gdp.Utils.DoubleUtil.GetWeightedAverage(false, data.ToArray());
        }

        /// <summary>
        /// 构建最小二乘拟合器。
        /// 以参考键的前边界点中心点，以指定的拟合数量为整个拟合窗口，提供最为准确的拟合结果。
        /// 若在边界，则选择靠边界的拟合数据进行拟合。
        /// </summary>
        /// <param name="order">拟合阶次</param>
        /// <param name="referKey">参考键，以此为中部选择拟合数</param> 
        /// <param name="fitDataCount">拟合数据量</param>
        /// <param name="isFromTheVeryFirstKey">是否从第一个注册的时间开始计算拟合的 X，其余拟合值需要减去它，否则以FirstKey替代</param>
        public virtual PolyfitBuilder BuildPolyfitBuilder(int order, TKey referKey, int fitDataCount = int.MaxValue, bool isFromTheVeryFirstKey = true)
        {
            if (order > this.Count - 1) { throw new Exception("数据不足！"); } 
             
           var indexs =  FindIndexRange(referKey, fitDataCount);

            //还需要把关判断Index，不允许大于大，小于小则不行，此步骤留给最后的生成去判断吧
            return BuildPolyfitBuilder(order, indexs[0], indexs[1], isFromTheVeryFirstKey);
        }
        /// <summary>
        /// 查找距离指定Key最近的数据范围。从小到大
        /// </summary>
        /// <param name="referKey"></param>
        /// <param name="fitDataCount"></param>
        /// <returns></returns>
        public int [] FindIndexRange(TKey referKey, int fitDataCount)
        {
            int startIndex = 0, endIndex = 0;
            if (fitDataCount == int.MaxValue || fitDataCount > this.Count) { fitDataCount = this.Count; }
            int midCount = fitDataCount / 2;
            int currentIndex = Gdp.Utils.ListUtil.ClosestToIndexOf(this.OrderedKeys, referKey, KeyToDouble);
            //以当前为中心
            startIndex = currentIndex - midCount;
            endIndex = currentIndex + midCount;
            //小于，则等于
            if (startIndex < 0)
            {
                startIndex = 0;
                endIndex = startIndex + fitDataCount - 1;
            }
            //大于，则等于
            if (endIndex > this.Count - 1)
            {
                endIndex = this.Count - 1;
                startIndex = endIndex - fitDataCount + 1;
            }
            return new int[] { startIndex, endIndex };
        }
         

      
        /// <summary>
        /// 构建，确保数字正确。从当前键的前边界处开始拟合。
        /// </summary>
        /// <param name="order"></param>
        /// <param name="polyfitCount"></param>
        /// <param name="marginCount">拟合边界，是指拟合时，加入一些边界数据，但计算时并不使用之。</param>
        /// <param name="currentIndex"></param>
        public PolyfitBuilder BuildPolyfitBuilder(int order, int currentIndex, int polyfitCount, int marginCount = -1)
        {
            if(marginCount == -1) { marginCount = polyfitCount / 4; }

            int startIndex = Gdp.Utils.IntUtil.GetPositiveOrZero(currentIndex - marginCount);
            int endIndex = startIndex + polyfitCount - 1;

            //如果已经是最最后的阶段了，则只用最后Edge部分
            //if (currentIndex >= this.Count - marginCount)
            //{
            //    startIndex = Geo.Utils.IntUtil.GetPositiveOrZero(this.Count - marginCount);
            //}

            return  BuildPolyfitBuilder(order, startIndex, endIndex, true);
        }

        /// <summary>
        /// 直接根据所指数据构建拟合器。由 PolyfitBuilders 统一管理。
        /// 注意：此处的键值是相对于最初的拟合窗口键 TheVeryFirstKey 作差的。
        /// </summary>
        /// <param name="order"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="isFromTheVeryFirstKey">是否从第一个注册的时间开始计算拟合的 X，其余拟合值需要减去它，否则以FirstKey替代</param>
        public LsPolyFit BuildPolyfitter(int order, int startIndex = 0, int endIndex = int.MaxValue, bool isFromTheVeryFirstKey = true)
        {
            PolyfitBuilder builder = BuildPolyfitBuilder(order, startIndex, endIndex, isFromTheVeryFirstKey);
            return builder.Currrent;
        }
        /// <summary>
        /// 直接根据所指数据构建拟合器。由 PolyfitBuilders 统一管理。
        /// </summary>
        /// <param name="order"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="isFromTheVeryFirstKey"></param>
        /// <returns></returns>
        public PolyfitBuilder BuildPolyfitBuilder(int order, int startIndex=0, int endIndex=int.MaxValue, bool isFromTheVeryFirstKey=true)
        {
            if (endIndex > this.Count - 1 || endIndex < 1)
            {
                endIndex = this.Count - 1;
            }
            bool hasKeyToDouble = KeyToDouble != null;
            startIndex = Gdp.Utils.IntUtil.GetPositiveOrZero(startIndex);
            var keys = this.OrderedKeys;

            var builder = PolyfitBuilders.GetOrCreate(order);

            //检查是否已有，避免重复创建
            if ((hasKeyToDouble && builder.Currrent != null
                && builder.FirstKey == KeyToDouble(keys[startIndex])
                && builder.LastKey == KeyToDouble(keys[endIndex]))                
                )
            {
                int ii = 0;
            }
            else
            {
                if(startIndex == endIndex) //防止最后历元 
                {
                    return builder;
                }

                //重新生成，以当前Key为基准
                var dic = new Dictionary<double, double>();

                for (int i = startIndex; i <= endIndex; i++)
                {
                    var item = keys[i];
                    double keyV = i;
                    if (hasKeyToDouble) { keyV = KeyToDouble(item); }
                    dic.Add(keyV, this[item]);
                }

                builder.SetData(dic).SetIsFromTheVeryFirstKey(isFromTheVeryFirstKey);
                builder.Build();
            }

            if(builder.Currrent.StdDev > 10)
            {
                log.Warn("拟合 StdDev 大啊：" + builder.Currrent.StdDev + ", " + builder.Currrent.ToString());
            }

            return builder;
        }
        #endregion
    }
}