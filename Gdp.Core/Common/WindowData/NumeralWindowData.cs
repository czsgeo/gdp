//2016.05.10, czs, edit, 滑动窗口数据
//2016.08.04, czs, edit in fujian yongan, 修正
//2017.09.11, czs, edit in hongiqng, 重构，为了更好的使用
//2018.05.20, czs, edit in hmx, 增加数据多项式平滑功能

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gdp.IO;


namespace Gdp
{  
    /// <summary>
    /// 数值窗口数据。做一些数据处理和判断工作。
    /// 加入的数值逐渐累加，如果要判断是否断裂，请在外部执行。
    /// </summary>
    public class NumeralWindowData : IndexedWindowData<double>
    {
        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="windowSize">窗口大小</param>
        public NumeralWindowData(int windowSize) : base(windowSize) { }
           
         /// <summary>
        /// 以数值初始化
        /// </summary>
        /// <param name="values"></param>
        /// <param name="name"></param>
        /// <param name="IsResetWhenIndexBreak">当断裂时，是否重置 清空</param>
        /// <param name="MaxBreakCount">允许最大的断裂数量</param>
        public NumeralWindowData(IEnumerable<double> values,bool IsResetWhenIndexBreak = true, int MaxBreakCount = 5, string name = "")
            : base(values, IsResetWhenIndexBreak, MaxBreakCount, name)
        { 
            this.IsResetWhenIndexBreaked = IsResetWhenIndexBreak;
            this.MaxBreakCount = MaxBreakCount;
        }

        #region 操作数
        public static NumeralWindowData operator -( double val, NumeralWindowData data) { return  (-val) + data; }
        public static NumeralWindowData operator -(NumeralWindowData data, double val) { return data + (-val); }
        public static NumeralWindowData operator +(double val, NumeralWindowData data) { return data + val; }
        public static NumeralWindowData operator +(NumeralWindowData data, double val)
        {
            NumeralWindowData result = new NumeralWindowData(data.Count);
            foreach (var item in data)
            {
                result.Add(item + val);
            }
            return result;
        }
        #endregion

        #region  方法
       
        /// <summary>
        /// 获取后一个减去前一个，迭代相减的值，高次差值。
        /// </summary>
        /// <returns></returns>
        public double LoopDifferValue
        {
            get { return Gdp.Utils.DoubleUtil.GetLoopDifferValue(Data.ToArray()); }
        }

        /// <summary>
        /// 获取差分一次的数据窗口结果，后减前
        /// </summary>
        /// <returns></returns>
        public NumeralWindowData GetDifferWindow(int differCount)
        { 
            return new NumeralWindowData(Gdp.Utils.DoubleUtil.GetDiffer(Data.ToArray(), differCount));
        }

        /// <summary>
        /// 获取差分一次的数据窗口结果。
        /// </summary>
        /// <returns></returns>
        public NumeralWindowData GetDifferWindow()
        {
            double[] differs = new double[WindowSize - 1];
            for (int i = 0; i < WindowSize - 1; i++)
            {
                differs[i] = this[i + 1] - this[i];
            }
            NumeralWindowData differWindow = new NumeralWindowData(differs);
            return differWindow;
        }

        /// <summary>
        /// 判断这个数据是否超限。通过下一个拟合值判断。
        /// </summary>
        /// <param name="newVal">待判断的数据</param>
        /// <param name="maxDelta">最大的偏差，绝对值</param>
        /// <param name="isRelativeError">是否是相对误差，即百分数。</param>
        /// <returns></returns>
        public bool IsOverLimited(double newVal, double maxDelta= 0.1, bool isRelativeError = true)
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
        /// 求平均
        /// </summary>
        public double AverageValue { get { return Gdp.Utils.DoubleUtil.Average(this); } }

        /// <summary>
        /// 具有权值的平均值
        /// </summary>
        public RmsedNumeral Average { get {
            var val = Gdp.Utils.DoubleUtil.GetAverageWithStdDev(this);
            return new RmsedNumeral(val[0], val[1]);        
        } }

        /// <summary>
        /// 上一个平均值,在调用清空前赋值。
        /// </summary>
        public RmsedNumeral LastAverage { get; set; }
        /// <summary>
        /// 获取平均带权值
        /// </summary>
        /// <param name="from"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public RmsedNumeral GetRmsedAverage(int from, int count)
        {
            var val = Gdp.Utils.DoubleUtil.GetAverageWithStdDev(this.GetSubList(from, count));
            return new RmsedNumeral(val[0], val[1]);        
        }

        /// <summary>
        /// 是否是水平数据的粗差，通过已知数据求平均判断，并不将其参与计算。
        /// </summary>
        /// <param name="newVal"></param> 
        /// <param name="errorTimes"></param>
        /// <returns></returns>
        public bool IsGrossOfLeveling(double newVal,  double errorTimes = 3)
        {
            return (Gdp.Utils.DoubleUtil.IsGross(this, newVal, errorTimes));
        }
        /// <summary>
        /// 平均数和RMS检核新数据，通过则添加，返回ture，否则清空，添加该数据，并返回false。
        /// 如果未满，则直接添加，不检核，并返回true。
        /// </summary>
        /// <param name="newVal"></param> 
        /// <param name="errorTimes"></param> 
        /// <param name="greenPassRms">绿色通道，避免过于苛刻</param> 
        /// <returns></returns>
        public bool AverageCheckAddOrClear(double newVal, double errorTimes = 3, double greenPassRms = 0.01)
        {
            if (!this.IsFull) { this.Add(newVal); return true; }
            var ave = this.Average;
            var error = Math.Abs(ave.Value - newVal);
            var maxError = Math.Max( ave.Rms * errorTimes,greenPassRms);
            if (error > maxError)
            {
                this.Clear();
                this.Add(newVal);
                return false;
            }

            this.Add(newVal);
            return true;
        }
        /// <summary>
        /// 清空，并保存上一个平均数。
        /// </summary>
        public override void Clear()
        {
            this.LastAverage = this.Average;
            base.Clear();
        }

        /// <summary>
        /// 多项式拟合检核新数据，通过则添加，返回ture，否则清空，添加该数据，并返回false。
        /// 如果未满，则直接添加，不检核，并返回true。
        /// </summary>
        /// <param name="newVal"></param>
        /// <param name="order"></param>
        /// <param name="errorTimes"></param>
        /// <param name="nextIndex"></param>
        /// <returns></returns>
        public bool PolyfitCheckAddOrClear(double newVal, int order, double errorTimes = 3, double nextIndex = 0)
        {
            if (!this.IsFull) { this.Add(newVal); return true; }

            RmsedNumeral rmsedVal = GetNextLsPolyFitValue(nextIndex, order);
            var error = Math.Abs(newVal - rmsedVal.Value);
            var maxError = rmsedVal.Rms * errorTimes;
            if (error > maxError)
            {
                this.Clear();
                this.Add(newVal);
                return false;
            }

            this.Add(newVal);
            return true;
        }
         
        /// <summary>
        ///  拟合下一个数据。
        /// </summary>
        /// <param name="nextIndex">下一个编号，0为推1个单位，1为外推2个单位</param>
        /// <param name="order"></param>
        /// <returns></returns>
        public RmsedNumeral GetNextLsPolyFitValue(double nextIndex = 0, int order = 2)
        {
            LsPolyFit fit = GetLsPolyFit(order);

            return new RmsedNumeral(fit.GetY(this.Count + nextIndex), fit.StdDev);
        }

        /// <summary>
        /// 获取最小二乘多项式拟合。
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public LsPolyFit GetLsPolyFit(int order)
        {
            var len = this.Count; 
            double[] yArray = this.Data.ToArray();
            double[] xArray = new double[len];
            for (int i = 0; i < len; i++) { xArray[i] = i; }

            Gdp.LsPolyFit fit = new Gdp.LsPolyFit(xArray, yArray, order);
            double[] paralist = fit.FitParameters();
            return fit;
        }

        /// <summary>
        /// 得到剔除粗差（默认2倍中误差）的数据列表。
        /// </summary>
        /// <param name="errorTimes"></param>
        /// <returns></returns>
        public NumeralWindowData GetNeatlyWindowData(double errorTimes = 3)
        {
            return new NumeralWindowData(Gdp.Utils.DoubleUtil.GetNeatlyList(this, errorTimes));
        }
        
        /// <summary>
        ///得到粗差编号（默认3倍中误差）列表。。
        /// </summary>
        /// <param name="errorTimes"></param>
        /// <returns></returns>
        public List<int> GetIndexesOfGrossError(double errorTimes = 3)
        {
            return (Gdp.Utils.DoubleUtil.GetIndexesOfGrossError(this, errorTimes));
        }

        /// <summary>
        /// 插入，并返回新列表。
        /// </summary>
        /// <param name="index"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public List<double> InsertAndReturnNew(int index, double val)
        {
            var list = this.ToList();
            list.Insert(index, val);
            return list;
        }
        #endregion
    }
}