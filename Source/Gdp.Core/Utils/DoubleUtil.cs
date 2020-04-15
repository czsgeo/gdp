//2017.03.18, czs, edit, 双精度浮点数处理，封装一些数据处理方法。

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gdp.IO;


namespace Gdp.Utils
{
    /// <summary>
    /// 双精度类型实用工具类。
    /// </summary>
    public class DoubleUtil
    {

         static Log log = new Log(typeof(DoubleUtil));

        /// <summary>
        /// 计算采样率。采样率不为负数，不为0.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="defaultInterval"></param>
        /// <returns></returns>
        public static double GetInterval(double first, double second, double defaultInterval = 10.0)
        {
            double interval = Math.Abs(second - first);
            if(interval == 0)
            {
                interval = defaultInterval;
                log.Warn("计算的采样间隔为 0 ，已采用默认的 " + defaultInterval);
            }
            return interval;
        }
        /// <summary>
        /// 平方和，再开根号。
        /// </summary> 
        /// <param name="xs"></param>
        /// <returns></returns>
        public static double SquareRootOfSumOfSquares(params double[] xs)
        {
            double val = 0;
            foreach (var item in xs)
            {
                val += item * item;
            }
            return Math.Sqrt(val);
        }

        #region 计算
        /// <summary>
        /// +
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        static public double Plus(double a, double b) { return a + b; }
        /// <summary>
        /// -
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        static public double Minus(double a, double b) { return a - b; }
        /// <summary>
        /// /
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        static public double Div(double a, double b) { return a / b; }
        /// <summary>
        /// *
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        static public double Multipy(double a, double b) { return a * b; }
        #endregion

        #region 整数项无关获取小数法
        /// <summary>
        /// 返回 0-1 之间
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static List<double> GetFraction(IEnumerable<double> collection)
        {
            List<double> vals = new List<double>();
            foreach (var item in collection)
            {
                vals.Add(GetFraction(item));
            }
            return vals;
        }
        /// <summary>
        /// 返回 0-1 之间
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static double GetFraction(double val)
        {
            int integer = (int)val;
            if (val < 0) { integer--; }

            return val - integer;
        }
        /// <summary>
        /// 返回最接近指定数值的整数，保证数值偏差在0.5以内。
        /// 如 2.7 和 0.1，2.7 应该取整为 3 而不是 2 
        /// </summary>
        /// <param name="val"></param>
        /// <param name="toBeNear"></param>
        /// <returns></returns>
        public static int GetNearstIntBias(double val, double toBeNear)
        {
            int integer = (int)val;
            if (val < 0) { integer--; } //让小数在 0-1 之间

            var fraction = val - integer;
            double differ = fraction - toBeNear;
            while (differ > 0.5)
            {
                integer++;

                fraction = val - integer;
                differ = fraction - toBeNear;
            }
            while (differ < -0.5)
            {
                integer--;

                fraction = val - integer;
                differ = fraction - toBeNear;
            }


            return integer;
        }
        /// <summary>
        /// RMS 相加
        /// </summary>
        /// <param name="rms1"></param>
        /// <param name="rms2"></param>
        /// <returns></returns>
        public static double RmsPlus(double rms1, double rms2)
        {
            return Math.Sqrt(rms1 * rms1 + rms2 * rms2);
        }

        /// <summary>
        /// 获取小数部分。
        /// 整数项无关获取小数法。
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static double GetIntFreeFraction(IEnumerable<double> collection, IEnumerable<double> weiths = null)
        {
            double totalUp = 0;
            double totalDown = 0;
            double weight = 1.0;
            List<double> ws = null;
            if (weiths != null) { ws = weiths.ToList(); }
            int i = 0;
            foreach (var floatNum in collection)
            {
                if (ws != null)
                {
                    weight = ws[i];
                }
                double twoPiMulti = 2.0 * GeoConst.PI * floatNum;

                double up = weight * Math.Sin(twoPiMulti);
                double down = weight * Math.Cos(twoPiMulti);

                totalUp += up;
                totalDown += down;
                i++;
            }
            double val = Math.Atan2(totalUp, totalDown) / (2 * GeoConst.PI);
            return val;
        }

        /// <summary>
        /// 返回正小数部分
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static RmsedNumeral GetRoundFraction(RmsedNumeral value)
        {
            return new RmsedNumeral(GetRoundFraction(value.Value), value.Rms);
        }

        #endregion

        /// <summary>
        /// 后一个减去前一个，反复做差，返回最后一个差分结果。
        /// 输入n个，则做n-1次差。
        /// 如果输入两个，则做一次差。
        /// 如果结果值较大，则认为最后一个数据异常。
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static double GetLoopDifferValue(params double[] array)
        {
            int length = array.Length;
            if (length == 1) return array[0];

            double[] differs = new double[length - 1];
            for (int i = 0; i < length - 1; i++)
            {
                differs[i] = array[i + 1] - array[i];
            }

            return GetLoopDifferValue(differs);
        }

        /// <summary>
        /// 获取平均的正小数。首先归类一个区间再平均。过滤04-0.6直接的数据。
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="dicNumerials"></param>
        /// <returns></returns>
        public static Dictionary<TKey, RmsedNumeral> GetRoundFractionAverageValues<TKey>(AbstractBaseDictionary<TKey, List<RmsedNumeral>> dicNumerials)
        {
            return GetRoundFractionAverageValues<TKey>(dicNumerials.Data);
        }

        /// <summary>
        /// 获取四舍五入小数。首先归类一个区间再平均。过滤04-0.6直接的数据。
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="dicNumerials"></param>
        /// <returns></returns>
        public static Dictionary<TKey, RmsedNumeral> GetRoundFractionAverageValues<TKey>(IDictionary<TKey, List<RmsedNumeral>> dicNumerials)
        {
            Dictionary<TKey, List<RmsedNumeral>> positiveFractions = GetRoundFraction(dicNumerials);

            var result = new Dictionary<TKey, RmsedNumeral>();
            foreach (var kv in positiveFractions)
            {
                if (kv.Value == null || kv.Value.Count == 0) { continue; }
                var ave = Gdp.Utils.DoubleUtil.GetAverageInOnePeriod(kv.Value);
                var posVal = Gdp.Utils.DoubleUtil.GetRoundFraction(ave.Value);
                result[kv.Key] = new RmsedNumeral(posVal, ave.Rms);
            }

            return result;
        }


        /// <summary>
        /// 过滤超限 RMS 后，获取四舍五入小数部分，然后求平均
        /// </summary>
        /// <param name="list"></param>
        /// <param name="maxRms"></param>
        /// <param name="maxErrorTimes"></param>
        /// <returns></returns>
        public static RmsedNumeral GetAverageRoundFraction(List<RmsedNumeral> list, double maxRms, double maxErrorTimes = 3.0)
        {
            if (list == null && list.Count == 0) { return null; }

            //保留小数部分
            List<RmsedNumeral> fractions = new List<RmsedNumeral>();
            foreach (var d in list)
            {
                if (d.Rms > maxRms) { continue; }//误差太大，直接舍去

                var frac = Gdp.Utils.DoubleUtil.GetRoundFraction(d.Value);//在[-0.5, 0.5]区间
                fractions.Add(new RmsedNumeral(frac, d.Rms));
            }

            if (fractions.Count <= 0) { return null; }   //没有则略过
            if (fractions.Count == 1)                 //只有一个，则直接返回
            {
                return fractions[0];
            }
            //有可能出现 5.4 和 5.6的情况，前者为 0.4 后者为 -0.4，平均后变成了 0.，可以采用如下算法
            //方法1：三角无关算法
            double check = GetRoundFractionIntFree( list);
            //方法2：GNSSer 算法
            RmsedNumeral ave = Gdp.Utils.DoubleUtil.GetAverageInOnePeriod(fractions, 1, maxErrorTimes);
            if (ave.Value > 0.5 || ave.Value < -0.5)
            {
                ave.Value = GetRoundFractionIntFree(ave.Value);
            }

            return ave;
        }

        /// <summary>
        /// 整数无关小数平均，返回平均值，[-0.5- 0.5]
        /// 采用三角函数实现。
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        static public double GetRoundFractionIntFree(List<RmsedNumeral> list)
        {
            double up = 0;
            double down = 0;
            double twoPi = 2.0 * Math.PI;
            foreach (var item in list)
            {
                var val = item.Value;
                up += Math.Sin(twoPi * val);
                down += Math.Cos(twoPi * val);
            }

            double frac = Math.Atan(up / down) / twoPi;
            return frac;
        }

        /// <summary>
        /// 归算到 [-0.5, 0.5] 区间，去掉整数部分
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        static public double GetRoundFractionIntFree(double val)
        {
            double twoPi = 2.0 * Math.PI; 
            double up = Math.Sin(twoPi * val);
            double down = Math.Cos(twoPi * val); 

            double frac = Math.Atan(up / down) / twoPi;
            return frac;
        }
         

        /// <summary>
        /// 四舍五入小数部分
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="fractions"></param>
        /// <returns></returns>
        public static Dictionary<TKey, List<RmsedNumeral>> GetRoundFraction<TKey>(IDictionary<TKey, List<RmsedNumeral>> fractions)
        {
            Dictionary<TKey, List<RmsedNumeral>> positiveFractions = new Dictionary<TKey, List<RmsedNumeral>>();
            foreach (var kv in fractions)
            {
                if (kv.Value == null || kv.Value.Count == 0) { continue; }
                var list = new List<RmsedNumeral>();
                positiveFractions[kv.Key] = list;
                foreach (var item in kv.Value)
                {
                    var posFrac = Gdp.Utils.DoubleUtil.GetRoundFraction(item.Value);
                    list.Add(new RmsedNumeral(posFrac, item.Rms));
                }
            }

            return positiveFractions;
        }
        /// <summary>
        /// 获取数浮点正小数部分,在 [0,1) 区间。
        /// </summary>
        /// <param name="floatValue">浮点数</param>
        /// <returns></returns>
        public static double GetPositiveFraction(double floatValue)
        {
            var frac = floatValue - (int)Math.Floor(floatValue);
            if (frac == 1) { return 0; }

            return frac;
        }

        /// <summary>
        /// 获取数浮点小数部分，为其与四舍五入的整数之差,在 [-0.5, 0.5] 区间。
        /// </summary>
        /// <param name="floatValue">浮点数</param>
        /// <returns></returns>
        public static double GetRoundFraction(double floatValue)
        {
            var frac = floatValue - Math.Round(floatValue);
            return frac;
        }

        /// <summary>
        /// 归算到一个区间的加权平均。选择RMS最小的一个作为基准，将其它的数据归算到同一区间后加权平均。
        /// 若RMS相同，则取平均值作为基准。
        /// 如果与基准相差大于0.4个周期，则移除。
        /// </summary>
        /// <param name="fractions">输入数据</param>
        /// <param name="period">周期</param>
        /// <param name="maxRmsTimes">粗差倍数</param>
        /// <returns></returns>
        public static RmsedNumeral GetAverageInOnePeriod(List<RmsedNumeral> fractions, double period = 1, double maxRmsTimes = 3.0)
        {
            if (fractions == null || fractions.Count == 0) { return null; }
            if(fractions.Count == 1) { return fractions[0]; } //直接返回

            //首先找到RMS最小的一个
            fractions.Sort((m, n) => m.Rms.CompareTo(n.Rms));

            double maxDiffer = 0.4 * period;

            List<RmsedNumeral> inOnePeriods = new List<RmsedNumeral>();

            //以第一个为基准，进行比较，确保在同一个小数区间
            int length = fractions.Count;
            var referValue = fractions[0].Value;
            if (fractions[0].Rms == fractions[length - 1].Rms) //RMS 排序已经没有意义了
            {
                referValue = WeightedAverageWithRms(fractions).Value;//平均值作为基准。
            }

            length = fractions.Count;
            for (int i = 0; i < length; i++)
            {
                var current = fractions[i];

                //归算到一个周期
                if (current.Value != referValue)
                {
                    current.Value = ConvertToSamePeriod(current.Value, referValue, period);
                }

                double differ = current.Value - referValue; //第二个相差多少，
                if (Math.Abs(differ) < maxDiffer)//加权平均做为下一个
                {
                    inOnePeriods.Add(current);
                }
                else//舍弃,与基准差别在 [0.4-0.6]
                {
                    log.Debug("周期内加权平均，舍弃：" + current + ", 偏差：" + differ);
                    break;//跳出循环，不要此数据
                }
            }

            if (inOnePeriods.Count == 1) {
                return inOnePeriods[0];
            }
            RmsedNumeral result = null;
            if (true)
            {
                result = WeightedAverageWithRms(inOnePeriods, maxRmsTimes); //加权平均， 并去除3倍中误差
            }
            else
            {
                var sum = RmsedNumeral.Zero;
                foreach (var item in inOnePeriods)
                {
                    sum += item;
                }
                result = sum / (inOnePeriods.Count);
            }
            return result;
        }

        /// <summary>
        /// 转换到同一个区间
        /// </summary>
        /// <param name="vals"></param>
        /// <param name="referVal"></param>
        /// <param name="period"></param>
        /// <returns></returns>
        public static List<double> ConvertToSamePeriod(IEnumerable<double> vals, double referVal, double period)
        {
            List<double> list = new List<double>();
            foreach (var item in vals)
            {
                list.Add(ConvertToSamePeriod(item, referVal, period));
            }

            return list;
        }

        /// <summary>
        /// 将数据与参考数值转换到同一个周期中。
        /// </summary>
        /// <param name="val"></param>
        /// <param name="referVal"></param>
        /// <param name="period"></param>
        /// <returns></returns>
        public static double ConvertToSamePeriod(double val, double referVal, double period)
        {
            var halfPeriod = Math.Abs( period / 2.0);
            var min = referVal - halfPeriod;
            var max = referVal + halfPeriod;
            while(val < min)
            {
                val += period;
            }

            while(val > max)
            {
                val -= period;
            }
            return val;
        }

        /// <summary>
        /// 四舍五入法固定模糊度
        /// </summary>
        /// <param name="floatDiffAmbi">模糊度浮点解</param>
        /// <param name="intAmbi">模糊度整数解</param>
        /// <param name="maxDiffer">浮点解与整数解允许的最大偏差</param>
        /// <returns></returns>
        public static bool TryRoundToFixAmbiguiy(double floatDiffAmbi, out int intAmbi,  double maxDiffer = 0.3)
        {
            intAmbi = (int)Math.Round(floatDiffAmbi);
            var differ = Math.Abs(floatDiffAmbi - intAmbi);
            if(differ > maxDiffer)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取多次差分的结果，后减前
        /// </summary>
        /// <param name="array"></param>
        /// <param name="differCount"></param>
        /// <returns></returns>
        public static double[] GetDiffer(double[] array, int differCount)
        {
            int length = array.Count();
            if (length <= differCount) { throw new ArgumentException("数据不足，差分次数太多，无法差分完成！"); }

            double[] differs = array;

            for (int i = 0; i < differCount; i++)
            {
                differs = GetDiffer(differs);
            }
            return differs;
        }
        /// <summary>
        /// 自动设置键为数字的函数
        /// </summary>
        public static void AutoSetKeyToDouble<TKey>(out Func<TKey, double> KeyToDouble)
        {
            KeyToDouble = null;

            var typeOfKey = typeof(TKey);

            //KeyToDouble = new Func<TKey, double>(key => Convert.ToDouble(key));

            if (typeOfKey == typeof(int))
            {
                KeyToDouble = new Func<TKey, double>(key => Convert.ToInt32(key));
            }
            if (typeOfKey == typeof(double))
            {
                KeyToDouble = new Func<TKey, double>(key => Convert.ToDouble(key));
            }
            if (typeOfKey == typeof(long))
            {
                KeyToDouble = new Func<TKey, double>(key => Convert.ToInt64(key));
            }
            if (typeOfKey == typeof(float))
            {
                KeyToDouble = new Func<TKey, double>(key => Convert.ToSingle(key));
            }
            if (typeOfKey == typeof(DateTime))
            {
                //KeyToDouble = new Func<TKey, double>(key => (Convert.ToDateTime(key) - Convert.ToDateTime(FirstKey)).TotalSeconds);
                KeyToDouble = new Func<TKey, double>(key => (Convert.ToDateTime(key).TimeOfDay).TotalSeconds);
            }
            if (typeOfKey == typeof(Time))
            {
                //KeyToDouble = new Func<TKey, double>(key => (Time.Parse(key) - Time.Parse(FirstKey)));
                KeyToDouble = new Func<TKey, double>(key => (Time.Parse(key).SecondsOfWeek));
            }
            if (typeOfKey == typeof(RmsedNumeral))
            {
                //KeyToDouble = new Func<TKey, double>(key => (Convert.ToDateTime(key) - Convert.ToDateTime(FirstKey)).TotalSeconds);
                KeyToDouble = new Func<TKey, double>(key => ((key as RmsedNumeral).Value));
            }
        }
        /// <summary>
        /// 自动设置键为数字的函数
        /// </summary>
        public static void AutoSetKeyToDouble<TKey>(out Func<TKey, double> KeyToDouble, TKey firstKey)
        {
            KeyToDouble = null;

            var typeOfKey = typeof(TKey);
            //KeyToDouble = new Func<TKey, double>(key => Convert.ToDouble(key));
            if (typeOfKey == typeof(int))
            {
                KeyToDouble = new Func<TKey, double>(key => Convert.ToInt32(key));
            }
            if (typeOfKey == typeof(double))
            {
                KeyToDouble = new Func<TKey, double>(key => Convert.ToDouble(key));
            }
            if (typeOfKey == typeof(long))
            {
                KeyToDouble = new Func<TKey, double>(key => Convert.ToInt64(key));
            }
            if (typeOfKey == typeof(float))
            {
                KeyToDouble = new Func<TKey, double>(key => Convert.ToSingle(key));
            }
            if (typeOfKey == typeof(DateTime))
            {
                //KeyToDouble = new Func<TKey, double>(key => (Convert.ToDateTime(key) - Convert.ToDateTime(FirstKey)).TotalSeconds);
                KeyToDouble = new Func<TKey, double>(key => (Convert.ToDateTime(key) - Convert.ToDateTime(firstKey)).TotalSeconds);
            }
            if (typeOfKey == typeof(Time))
            {
                //KeyToDouble = new Func<TKey, double>(key => (Time.Parse(key) - Time.Parse(FirstKey)));
                KeyToDouble = new Func<TKey, double>(key => (Time.Parse(key) - Time.Parse(firstKey)));
            }

            //try
            //{
            //    KeyToDouble = new Func<TKey, double>(key => Convert.ToDouble(key));
            //}catch(Exception ex)
            //{
            //    log.Warn("Key 转换失败！" + ex.Message);
            //}
        } 
        /// <summary>
          /// 利用自身数据进行加权平均，单变量加权平均，返回中误差
          /// </summary>
          /// <param name="values"></param>
          /// <param name="maxRmsTimes">如果有超过的，则过滤，重新计算</param>
          /// <returns></returns>
        public static RmsedNumeral WeightedAverageWithRms(List<double> values, double maxRmsTimes = 3.0)
        {
            int len = values.Count;
            var ave = GetAverageWithRms(values);

            var rms = ave.Rms;

            //粗差剔除后继续
            List<int> tobeRemoves = new List<int>();
            for (int i = 0; i < len; i++)
            {
                var val = values[i];
                var differ = val - ave.Value;
                var times = Math.Abs(differ) / rms;
                if (times > maxRmsTimes)
                {
                    tobeRemoves.Add(i);
                }
            }
            if (tobeRemoves.Count > 0)
            {
                Gdp.Utils.ListUtil.RemoveAt(values, tobeRemoves);
                return WeightedAverageWithRms(values, maxRmsTimes);
            }

            return ave;
        }

        /// <summary>
        /// 单变量加权平均，返回中误差
        /// </summary>
        /// <param name="values"></param>
        /// <param name="maxRmsTimes">如果有超过的，则过滤，重新计算</param>
        /// <returns></returns>
        public static RmsedNumeral WeightedAverageWithRms(List<WeightedNumeral> values, double maxRmsTimes = 3.0)
        {
            int len = values.Count;
            var ave = WeightedAverage(values);

            double totalUp = 0.0; 
            for (int i = 0; i < len; i++)
            {
                var val = values[i];
                var p = val.Weight;

                var v = val.Value - ave;
                var vv = v * v * p;  

                totalUp += vv; 
            }

            var rms = Math.Sqrt( totalUp / (len - 1));

            //粗差剔除后继续
            List<int> tobeRemoves = new List<int>();
            for (int i = 0; i < len; i++)
            {
                var val = values[i]; 
                var differ =val.Value - ave;
                var times = Math.Abs( differ) / rms; 
                if(times > maxRmsTimes)
                {
                    tobeRemoves.Add(i);
                }
            }
            if (tobeRemoves.Count > 0)
            {
                Gdp.Utils.ListUtil.RemoveAt(values, tobeRemoves);
                return WeightedAverageWithRms(values, maxRmsTimes);
            }

            return new RmsedNumeral( ave, rms);
        }


        /// <summary>
        /// 单变量加权平均
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static double WeightedAverage(List<WeightedNumeral> values)
        {
            double totalUp = 0.0;
            double totalDown = 0.0;
            int len = values.Count;
            for (int i = 0; i < len; i++)
            {
                var val = values[i];
                var weight = val.Weight;
                totalUp += val.Value * weight;
                totalDown += weight;
            }

            var ave = totalUp / totalDown;
            return ave;
        }
        /// <summary>
        /// 单变量加权平均，返回中误差
        /// </summary>
        /// <param name="values"></param>
        /// <param name="maxRmsTimes"></param>
        /// <returns></returns>
        public static RmsedNumeral WeightedAverageWithRms(List<RmsedNumeral> values, double maxRmsTimes = 3.0)
        {
            int len = values.Count;
            var ave = WeightedAverage(values);
             
            double totalUp = 0.0;
            for (int i = 0; i < len; i++)
            {
                var val = values[i];
                var p = 1.0 / val.Variance;  //1 为权和方差的比例常数
                if (!Gdp.Utils.DoubleUtil.IsValid(p))
                {
                    p = 1;
                }

                var v = val.Value - ave;
                var vv = v * v * p;

                totalUp += vv;
            }

            var u = Math.Sqrt(totalUp / (len - 1));

            //粗差剔除后继续
            List<int> tobeRemoves = new List<int>();
            for (int i = 0; i < len; i++)
            {
                var val = values[i];
                var differ = val.Value - ave;
                var times = Math.Abs(differ) / u;
                if (times > maxRmsTimes)
                {
                    tobeRemoves.Add(i);
                }
            }
            if (tobeRemoves.Count > 0)
            {
                Gdp.Utils.ListUtil.RemoveAt(values, tobeRemoves);
                return WeightedAverageWithRms(values, maxRmsTimes);
            }

            return new RmsedNumeral(ave, u);
        }
        /// <summary>
        /// 单变量加权平均
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static double WeightedAverage(List<RmsedNumeral> values)
        {
            double totalUp = 0.0;
            double totalDown = 0.0;
            int len = values.Count;
            for (int i = 0; i < len; i++)
            {
                var val = values[i];
                var weight =  1.0 / val.Variance; //1 为权和方差的比例常数
                if ( !Gdp.Utils.DoubleUtil.IsValid(weight))
                {
                    weight = 1.0;
                }
                totalUp += val.Value * weight;
                totalDown += weight;
            }

            var ave = totalUp / totalDown;
            return ave;
        }

        /// <summary>
        /// 计算加权平均值
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="distanceToVal1">与值1的距离</param>
        /// <param name="distanceToVal2">与值2的距离</param>
        /// <returns></returns>
        public static double WeightedAverage(double value1, double value2, double distanceToVal1, double distanceToVal2)
        {
            double totalLen = distanceToVal1 + distanceToVal2;
            //double weithOf1 = 1 - distanceToVal2 / totalLen;
            return (value1 * distanceToVal2 + value2 * distanceToVal1) / totalLen;
            
        }
        /// <summary>
        /// 单个数据的加权平均
        /// </summary>
        /// <param name="values"></param>
        /// <param name="weights"></param>
        /// <returns></returns>
        public static double WeightedAverage(double [] values, double [] weights)
        {
            double totalUp = 0.0;
            double totalDown = 0.0;
            int len = values.Length;
            for (int i = 0; i < len; i++)
            {
                var weight =  weights[i];
                totalUp += values[i] * weight;
                totalDown += weight;
            }

            return totalUp / totalDown;

        }
        /// <summary>
        /// 获取一次差分值，后减前
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static double[] GetDiffer(double[] array)
        {
            int length = array.Count();
            if (length <= 1) { throw new ArgumentException("数据太少，无法差分！"); }

            double[] differs = new double[length - 1];

            for (int i = 0; i < length - 1; i++)
            {
                differs[i] = array[i + 1] - array[i];
            }
            return differs;
        }


        #region 计算平均数

        /// <summary>
        /// 计算平均值，通过以往平均数据计算，滤波。
        /// </summary>
        /// <param name="newVal"></param>
        /// <param name="oldAverageValue"></param>
        /// <param name="currentValCount"></param>
        /// <returns></returns>
        public static Double GetAverageValue(double newVal, double oldAverageValue, int currentValCount)
        {
            return oldAverageValue + (newVal - oldAverageValue) / currentValCount;
        }


        /// <summary>
        /// 0-1之间整数无关的求平均数。
        /// </summary>
        /// <param name="collection">数据集合</param>
        /// <returns></returns>
        public static double GetFractionAverage(IEnumerable<double> collection)
        {
            double result = 0; 
            int count = 0;
            foreach (var item in collection)
            {
                if (Double.IsNaN(item) || Double.IsInfinity(item)) { continue; }

                var val = GetFraction(item); //确保在 0-1 之间
                if (val - result > 0.5) //如果大于半圈，则其值减去1圈
                {
                    val--;
                }
                if (val - result < -0.5)//小于负半圈
                {
                    val++;
                }
                count++;
                result += (val - result) / count; //可以避免大数据溢出 
            }
            return result;
        }
        /// <summary>
        /// 求平均数。
        /// </summary>
        /// <param name="collection">数据集合</param>
        /// <returns></returns>
        public static double Average(params double[] collection)
        {
            double result = 0;
            //double total = 0;
            int count = 0;
            foreach (var item in collection)
            {
                if (!IsValid(item)) { continue; }

                count++;
                result += (item - result) / count; //可以避免大数据溢出
                //total += key;
            }
            //var res2 =  total / count;
            if (count == 0) { return double.NaN; }
            return result;
        }
        /// <summary>
        /// 求平均数。
        /// </summary>
        /// <param name="collection">数据集合</param>
        /// <returns></returns>
        public static double Average(IEnumerable<double> collection)
        {
            //return data.Average();

            double result = 0;
            //double total = 0;
            int count = 0;
            foreach (var item in collection)
            {
                if (Double.IsNaN(item) || Double.IsInfinity(item)) { continue; }

                count++;
                result += (item - result) / count; //可以避免大数据溢出
                //total += key;
            }
            //var res2 =  total / count;

            return result;
        }

        /// <summary>
        /// 求绝对值平均数。
        /// </summary>
        /// <param name="collection">数据集合</param>
        /// <returns></returns>
        public static double AbsAverage(IEnumerable<double> collection)
        {
            //return data.Average();
            double result = 0;
            //double total = 0;
            int count = 0;
            foreach (var item in collection)
            {
                if (Double.IsNaN(item) || Double.IsInfinity(item)) { continue; }

                count++;
                result += ( Math.Abs( item) - result) / count; //可以避免大数据溢出
                //total += key;
            }
            //var res2 =  total / count;
            return result;
        }
        /// <summary>
        /// 返回平均数和中误差，标准差（均方差mse）
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static RmsedNumeral GetAverageWithRms(IEnumerable<double> collection)
        {
            var ave = GetAverageWithStdDev(collection);
            return new RmsedNumeral(ave[0], ave[1]);
        }
        /// <summary>
        /// 返回平均数和中误差，标准差（均方差mse）,第三个为观测值数量
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static double[] GetAverageWithStdDev(IEnumerable<double> collection)
        {
            var average = Average(collection);
            int i = 0;
            double stdDev = 0;
            double totalDev = 0;
            foreach (var item in collection)
            {
                if (Double.IsNaN(item) || Double.IsInfinity(item)) { continue; }

                totalDev += Math.Pow(item - average, 2);
                i++;
            }
            stdDev = Math.Sqrt(totalDev /( i-1));

            return new double[] { average, stdDev, i };
        }

        /// <summary>
        /// 返回残差中误差，即均方差的估值,第二个为个数
        /// </summary>
        /// <param name="residuals"></param>
        /// <returns></returns>
        public static double [] GetResidualRmse(IEnumerable<double> residuals)
        {
            int i = 0;
            double stdDev = 0;
            double totalDev = 0;
            foreach (var item in residuals)
            {
                if (Double.IsNaN(item) || Double.IsInfinity(item)) { continue; }

                totalDev += Math.Pow(item, 2);
                i++;
            }
            stdDev = Math.Sqrt(totalDev / i );

            return new double[] { stdDev, i };
        }

        /// <summary>
        /// 返回残差中误差，即均方差的估值,第二个为个数
        /// </summary>
        /// <param name="residuals"></param>
        /// <returns></returns>
        public static double [] GetAbsMean(IEnumerable<double> residuals)
        {
            int i = 0; 
            double totalDev = 0;
            foreach (var item in residuals)
            {
                if (Double.IsNaN(item) || Double.IsInfinity(item)) { continue; }

                totalDev  += Math.Abs(item);
                i++;
            }
            totalDev = totalDev /= i;

            return new double[] { totalDev, i };
        }

        /// <summary>
        /// 返回平均数和中误差，标准差（均方差mse）,第三个为观测值数量
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static double[] GetAbsAverageWithStdDev(IEnumerable<double> collection)
        {
            var average = AbsAverage(collection);
            int i = 0;
            double stdDev = 0;
            double totalDev = 0;
            foreach (var item in collection)
            {
                if (Double.IsNaN(item) || Double.IsInfinity(item)) { continue; }

                totalDev += Math.Pow(item - average, 2);
                i++;
            }
            stdDev = Math.Sqrt(totalDev / (i-1));

            return new double[] { average, stdDev, i };
        }
        #endregion

        #region 数据判断
        /// <summary>
        /// 指示一个双精度数字是否有效。
        /// </summary>
        /// <param name="val">数值</param>
        /// <returns></returns>
        public static bool IsValid(double val)
        {
            bool valid = Double.IsNaN(val)
                || Double.IsInfinity(val);
            return !valid;
        }
        /// <summary>
        /// 判断数值是否为0，或无效。
        /// </summary>
        /// <param name="val">数值</param>
        /// <returns></returns>
        public static bool IsZeroOrNotValid(double val)
        {
            return val == 0 || IsValid(val);
        }
        /// <summary>
        /// 是否在范围中，含边界。
        /// </summary>
        /// <param name="val"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        static public bool IsBetween(double val, double min, double max)
        {
            return val >= min && val <= max;
        }
        #endregion


        /// <summary>
        /// 过滤掉绝对值超过限差（不含）的数。
        /// </summary>
        /// <param name="Residuals">残差数组</param>
        /// <param name="maxAbs">最大绝对值（不含）</param>
        /// <returns></returns>
        public static List<double> Filter(IEnumerable<Double> Residuals, double maxAbs) { return Residuals.Where<double>((item) => { return Math.Abs(item) < maxAbs; }).ToList(); }

        #region 数据解析
        /// <summary>
        /// 数组解析为双精度数字。
        /// </summary>
        /// <param name="lines">字符串数组</param>
        /// <returns></returns>
        public static double[] ParseLines(string[] lines, bool ignoreEmpty = true)
        {
            int len = lines.Length;
            List<double> doubles = new List<double>();
            for (int i = 0; i < len; i++)
            {
                if (ignoreEmpty && String.IsNullOrWhiteSpace(lines[i]))
                {
                    continue;
                }
                doubles.Add(DoubleUtil.TryParse(StringUtil.TrimBlank(lines[i]))); 
            }
            return doubles.ToArray();
        }

        /// <summary>
        ///  读取双精度数组集合。只返回数据部分，可设置是否忽略首行首列。
        /// </summary>
        /// <param name="lines">以分隔符分开的行</param>
        /// <param name="IsIgnoreFirstRow">是否忽略第一行</param>
        /// <param name="IsIgnoreFirstCol">是否忽略第一列</param>
        /// <returns></returns>
        public static List<double[]> ParseTable(string[] lines, bool IsIgnoreFirstRow = false, bool IsIgnoreFirstCol = false)
        {
            List<double[]> lists = new List<double[]>();
            int i = 0;
            foreach (var item in lines)
            {
                string trimed = item.Trim();
                if (i == 0 && IsIgnoreFirstRow) { i++; continue; }
                if (trimed == null || trimed.Equals("") || trimed.StartsWith("//")) continue;
                string[] strs = trimed.Split(new char[] { ',', ';', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                if (strs.Length > 0 && IsIgnoreFirstCol)
                {
                    List<string> list = new List<string>(strs);
                    list.RemoveAt(0);
                    strs = list.ToArray();
                }

                double[] doubles = DoubleUtil.ParseLines(strs);
                lists.Add(doubles);
                i++;
            }
            return lists;
        }
        /// <summary>
        /// 提取前两个数字为字典
        /// </summary>
        /// <param name="table">数据表</param>
        /// <returns></returns>
        public static Dictionary<Double, Double> ToDic(List<double[]> table)
        {
            Dictionary<Double, Double> dic = new Dictionary<double, double>();
            foreach (var item in table)
            {
                dic.Add(item[0], item[1]);
            }
            return dic;
        }
        /// <summary>
        /// 尝试解析
        /// </summary>
        /// <param name="p"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        public static double ? TryParse(object p, double? defaultVal = null)
        {
            if (p == null)
            {
                return defaultVal;
            }

            if (p is double) { return (double)p; }
            if (p is float) { return (float)p; }
            if (p is decimal) { return (double)(decimal)p; } 

            if (string.IsNullOrWhiteSpace(p + ""))
            {
                return defaultVal;
            }
            var d = TryParse(p + "", double.NaN);
            if (double.IsNaN(d))
                return defaultVal;
            return d;
        }

        /// <summary>
        /// 如果失败则返回 0 。
        /// </summary>
        /// <param name="p"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        public static double TryParse(string p, double defaultVal = 0)
        {
            if (string.IsNullOrWhiteSpace(p))
            {
                return defaultVal;
            }
            double outdouble = defaultVal;
            Double.TryParse(p, out outdouble);
            return outdouble;
        }
        /// <summary>
        /// 对Object 进行拆封，如果是数字类型则返回，否则返回 defaultValue。
        /// 如果是数字，但采用的string类型保存，也认为是非数字类型。
        /// 即一定要内存中是数字类型，才成功。
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static double TryDisclosure(object obj, double defaultValue = double.NaN)
        {
            if (ObjectUtil.IsNumerial(obj))
            {
                if (obj is double) return (double)obj;
                if (obj is int) return (int)obj;
                if (obj is float) return (float)obj;
                if (obj is long) return (long)obj;
                if (obj is decimal) return (double)((decimal)obj);
                if (obj is short) return (short)obj;

            }
            return defaultValue;
        }
        #endregion


        #region  取最大最小值
        /// <summary>
        /// 求出最大数。
        /// </summary>
        /// <param name="array">数组</param>
        /// <returns></returns>
        public static double Max(params double[] array)
        {
            double max = Double.MinValue;
            foreach (var item in array)
            {
                if (max < item) max = item;
            }
            return max;
        }
        /// <summary>
        /// 求出最大数。
        /// </summary>
        /// <param name="array">数组</param>
        /// <returns></returns>
        public static double Max(IEnumerable<double> array)
        {
            double max = Double.MinValue;
            foreach (var item in array)
            {
                if (max < item) max = item;
            }
            return max;
        }
        /// <summary>
        /// 求出绝对值最大数
        /// </summary>
        /// <param name="array">数组</param>
        /// <returns></returns>
        public static double MaxAbs(params double[] array)
        {
            double max = Double.MinValue;
            foreach (var item in array)
            {
                double tmp = Math.Abs(item);
                if (max < tmp) max = tmp;
            }
            return max;
        }
        /// <summary>
        /// 求出最小数。
        /// </summary>
        /// <param name="array">数组</param>
        /// <returns></returns>
        public static double Min(params double[] array)
        {
            double min = Double.MaxValue;
            foreach (var item in array)
            {
                if (min > item) min = item;
            }
            return min;
        }
        /// <summary>
        /// 求出最小数。
        /// </summary>
        /// <param name="array">数组</param>
        /// <returns></returns>
        public static double Min(IEnumerable<double> array)
        {
            double min = Double.MaxValue;
            foreach (var item in array)
            {
                if (min > item) min = item;
            }
            return min;
        }
        /// <summary>
        /// 求出绝对值最小数。
        /// </summary>
        /// <param name="array">数组</param>
        /// <returns></returns>
        public static double MinAbs(params double[] array)
        {
            double min = Double.MaxValue;
            foreach (var item in array)
            {
                double tmp = Math.Abs(item);
                if (min > tmp) min = tmp;
            }
            return min;
        }
        #endregion

        /// <summary>
        /// 转置
        /// </summary>
        /// <param name="table">原表</param>
        /// <returns></returns>
        public static List<List<Double>> Transpose(List<List<Double>> table)
        {
            List<List<Double>> transTable = new List<List<Double>>();

            int newCol = table.Count;
            int newRow = table[0].Count;
            for (int i = 0; i < newRow; i++)
            {
                List<double> rows = new List<double>();
                for (int j = 0; j < newCol; j++)
                {
                    double val = table[j][i];
                    rows.Add(val);
                }
                transTable.Add(rows);
            }
            return transTable;
        }

        /// <summary>
        /// 返回绝对值小于给定数值的。
        /// </summary>
        /// <param name="keyDic">字典</param>
        /// <param name="max">限差(不含)</param>
        /// <returns></returns>
        public static Dictionary<double, double> GetAbsFiltedDic(Dictionary<double, double> dic, double max)
        {
            Dictionary<double, double> results = new Dictionary<double, double>();
            foreach (var item in dic)
            {
                if (Math.Abs(item.Value) < max) results.Add(item.Key, item.Value);
            }
            return results;
        }

        #region 数值文本转换


        public class ScientificDouble
        {
            double val;
            int len;
            int power;
            bool isPositive;
            /// <summary>
            /// E前为最少的总长度，E后为有效数字
            /// </summary>
            /// <param name="val"></param>
            /// <param name="format"></param>
            public ScientificDouble(double val, string format)
            {
                this.val = val;
                this.isPositive = val >= 0;
                string[] array = format.Replace("E", "").Split(new char[] { '.' });
                this.len = int.Parse(array[0]);
                this.power = int.Parse(array[1]);
            }
            /// <summary>
            /// withZero = true
            /// 0.12566658900E+05
            /// -.12856666626E+05
            /// withZero = false 
            /// .12566658900E+05
            /// </summary>
            /// <param name="withZero"></param>
            /// <returns></returns>
            public string ToString(bool withZero = true)
            {
                string str = Math.Abs(val).ToString("E" + (power - 1));
                string[] strs = str.Split(new char[] { 'E' });
                //E之前
                if (strs[0].Equals("非数字")) return "NaN";
                string bef = "." + strs[0].Replace(".", "");
                str = bef;
                //E
                str = str + 'E';
                //E之后
                if (strs[1].Equals("非数字")) return "NaN";
                int pow = int.Parse(strs[1]);
                if (val != 0) pow = pow + 1;
                if (pow < 0) str += '-';
                else str += '+';
                str = str + StringUtil.FillZeroLeft(Math.Abs(pow), 2);
                if (withZero && isPositive) str = '0' + str;
                if (!isPositive) str = '-' + str;
                return StringUtil.FillSpaceLeft(str, len);
            }

        }
        /// <summary>
        /// 可读的文本，简洁，不长
        /// </summary>
        /// <param name="val"></param>
        /// <param name="len"></param>
        /// <param name="digital"></param>
        /// <returns></returns>
        internal static string ToReadableText(double val, int len = 9, int digital=5)
        {            
            return  Gdp.Utils.StringUtil.ToReadableText(val, len, digital);
        }


        /// <summary>
        ///  E21.14。E前为最少的总长度，E后为有效数字
        /// </summary>
        /// <param name="formate"></param>
        /// <returns></returns>
        public static string ScientificFomate(double val, string format, bool withZero = true)
        {
            ScientificDouble d = new ScientificDouble(val, format);
            return d.ToString(withZero);
        }
        /// <summary>
        /// 先转换为字符串，再转换为double
        /// </summary>
        /// <param name="val"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static double ToStringThenDouble(double val, string format="G5")
        {
            return Convert.ToDouble(val.ToString(format));
        } 

        /// <summary>
        /// 格式化双精度
        /// </summary>
        /// <param name="item"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string GetForamtedDouble(double item, string format = "E13.6")
        {
            string formatedItem = null;
            if (StringUtil.IsNumber(Math.Abs(item)) && item < 1e7 && item > -1e7) formatedItem = StringUtil.FillSpaceLeft(item.ToString(), 13);
            else formatedItem = DoubleUtil.ScientificFomate(item, format);
            return formatedItem;
        }

        /// <summary>
        /// 以Tab分割。
        /// </summary>
        /// <param name="doubles">数组</param>
        /// <param name="fractionCount">小数位数</param>
        /// <returns></returns>
        public static string ToTabString(IEnumerable<Double> doubles, int fractionCount)
        {
            string formate = StringUtil.Fill("0.", fractionCount + 2, '#', true);
            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach (var item in doubles)
            {
                if (i != 0)
                    sb.Append("\t");

                sb.Append(item.ToString(formate));

                i++;
            }
            return sb.ToString();
        }
        /// <summary>
        /// 将双精度结果转换为字符串数组。
        /// </summary>
        /// <param name="doubles">待转换双精度数</param>
        /// <returns></returns>
        public static List<String> ToStringLines(IEnumerable<Double> doubles)
        {
            List<String> results = new List<String>();
            foreach (var item in doubles)
            {
                results.Add(item.ToString());
            }
            return results;
        }

        /// <summary>
        /// 返回列
        /// </summary>
        /// <param name="doubles"></param>
        /// <returns></returns>
        public static String ToColumnString(IEnumerable<Double> doubles)
        {
            StringBuilder results = new StringBuilder();
            foreach (var item in doubles)
            {
                results.AppendLine(item.ToString());
            }
            return results.ToString();
        }
        /// <summary>
        /// 行集合
        /// </summary>
        /// <param name="keyDic">字典</param>
        /// <param name="spliter">分割点</param>
        /// <returns></returns>
        public static List<String> ToStringLines(Dictionary<double, double> dic, string spliter = "\t")
        {
            List<String> results = new List<String>();
            foreach (var item in dic)
            {
                results.Add(item.Key.ToString() + spliter + item.Value.ToString());
            }
            return results;
        }

        /// <summary>
        /// 字符串表格
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="spliter"></param>
        /// <returns></returns>
        public static string ToTableString(Dictionary<double, double> dic, string spliter = "\t")
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach (var item in dic)
            {
                sb.AppendLine(item.Key + spliter + item.Value);

                i++;
            }
            return sb.ToString();
        }
        #endregion

        #region 检索、获取
        /// <summary>
        /// 获取与指定键最近的
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dic"></param>
        /// <param name="KeyToDouble"></param>
        /// <param name="tobeNear"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        static public Dictionary<T, double> GetNearst<T>(Dictionary<T, double> dic, Func<T, double> KeyToDouble, T tobeNear, int count = 9)
        {
            var indexes = new List<double>();
            foreach (var item in dic)
            {
                indexes.Add(KeyToDouble(item.Key));
            }
            var keys =   GetNearst(indexes, KeyToDouble(tobeNear), count);
            Dictionary<T, double> result = new Dictionary<T, double>();
            foreach (var item in dic)
            {
                var val = KeyToDouble(item.Key);
                if (keys.Contains(val))
                {
                    result.Add(item.Key, item.Value);
                }
            }
            return result;     
        }
        /// <summary>
        /// 获取与数组中数值最接近的数组
        /// </summary>
        /// <param name="XArray"></param>
        /// <param name="xValue"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public static List<double> GetNearstValues(List<double> XArray, double xValue, int order = 9)
        {

            List<int> indexes = GetNearstIndexes(XArray, xValue, order);

            List<double> result = new List<double>();
            foreach (var item in indexes)
            {
                result.Add(XArray[item]);
            }

             
            return result;
        }
        /// <summary>
        /// 获取指定数组中与 X 最相近的数组编号。 
        /// </summary>
        /// <param name="XArray">递增或递减数组</param>
        /// <param name="xValue"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public static List<int> GetNearstIndexes(double[] XArray, double xValue, int order = 9)
        {
            int count = XArray.Length;

            order = Math.Min(order, count);

            int i, j, k = 0;
            List<int> indexes = new List<int>();
            //如果数量大于数组数量，则返回全部
            if (order >= count)
            {
                for (i = 0; i < order; i++)
                {
                    indexes.Add(i);
                }
                return indexes;
            }


            //找到离X最近的编号 binary search
            int index = 0;

            for (i = 0, j = count - 1; i < j;)
            {
                k = (i + j) / 2;
                if ((XArray[k] - xValue) < 0.0)
                { i = k + 1; }
                else { j = k; }
            }

            index = i <= 0 ? 0 : i - 1;

            //polynomial interpolation for orbit
            i = index - (order + 1) / 2;

            if (i < 0) i = 0; else if ((i + order) >= count) i = count - order - 1;

            for (j = 0; j <= order; j++)
            {
                indexes.Add(i + j);
            }

            return indexes;

        }
        /// <summary>
        /// 获取最接近的数字列表
        /// </summary>
        /// <param name="XArray">待选</param>
        /// <param name="tobeNear">指定值</param>
        /// <param name="count">选择数量</param>
        /// <returns></returns>
        public static List<int> GetNearstIndexes(List<double> XArray, double tobeNear, int count = 9)
        {
            List<int> indexes = new List<int>();
            //如果数量大于数组数量，则返回全部
            if (count >= XArray.Count)
            {
                for (int i = 0; i < count; i++)
                {
                    indexes.Add(i);
                }
                return indexes;
            }
            //找到离X最小的编号
            int halfCount = count / 2;
            int index = 0;
            double min = double.MaxValue;
            for (int i = 0; i < XArray.Count; i++)
            {
                double diff = Math.Abs(XArray[i] - tobeNear);
                // if (diff == 0) return YArray[time];
                if (diff < min)
                {
                    min = diff;
                    index = i;
                }
            }
            //在数组的头部
            if (index - halfCount <= 0) for (int i = 0; i < count; i++) indexes.Add(i);
            //尾部
            else if (index + halfCount >= XArray.Count - 1) for (int i = 0, j = XArray.Count - 1; i < count; i++, j--) indexes.Insert(0, j);
            //中间
            else for (int i = 0; i < count; i++) indexes.Add(index - halfCount + i);

            if (indexes[0] < 0) throw new Exception("出错了");

            return indexes;
        }
        /// <summary>
        /// 获取最接近的数字列表
        /// </summary>
        /// <param name="XArray">待选</param>
        /// <param name="tobeNear">指定值</param>
        /// <param name="count">选择数量</param>
        /// <returns></returns>
        public static List<double> GetNearst(List<double> XArray, double tobeNear, int count = 9)
        {
            List<double> indexes = new List<double>();
            //如果数量大于数组数量，则返回全部
            if (count >= XArray.Count)
            {
                return XArray;
            }

            //找到离X最小的编号
            int halfCount = count / 2;
            int index = 0;
            double min = double.MaxValue;
            for (int i = 0; i < XArray.Count; i++)
            {
                double diff = Math.Abs(XArray[i] - tobeNear);
                // if (diff == 0) return YArray[time];
                if (diff < min)
                {
                    min = diff;
                    index = i;
                }
            }
            //在数组的头部 
            int startIndex = 0;
            if (index - halfCount <= 0) { startIndex = 0; }//for (int i = 0; i < count; i++) indexes.Add(XArray[i]);
            //尾部
            else if (index + halfCount >= XArray.Count - 1) { startIndex = XArray.Count - count; }//for (int i = 0, j = XArray.Count - 1; i < count; i++, j--) indexes.Insert(0, XArray[j]);
            //中间
            else for (int i = 0; i < count; i++) { startIndex = index - halfCount; }// indexes.Add(XArray[index - halfCount + i]);

            //if (indexes[0] < 0) throw new Exception("出错了");

            indexes = XArray.GetRange(startIndex, count);

            return indexes;
        }

        /// <summary>
        /// 统计数据频域
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="fromVal"></param>
        /// <param name="waveLength"></param>
        /// <returns></returns>
        public static Dictionary<double, int> GetDataFrequence(List<double> vector, double fromVal, double waveLength)
        {
            Dictionary<double, int> result = new Dictionary<double, int>();
            vector.Sort();
            var max = vector[vector.Count - 1] + waveLength;

            for (double start = fromVal - waveLength, next = fromVal; 
                start <= max;
                start += waveLength, next += waveLength)
            {
                var count = vector.Count((val) => val >= start && val < next);
                result[start] = count;
            }

            return result;
        }
        /// <summary>
        /// 统计数据频域
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="count"></param> 
        /// <returns></returns>
        public static Dictionary<double, int> GetDataFrequence(List<double> vector, int count)
        {
            Dictionary<double, int> result = new Dictionary<double, int>();
            if(vector.Count == 0)
            {
                return result;
            }
            vector.Sort();
            var max = vector[vector.Count - 1];
            var minVal = Min(vector);
            var maxVal = Max(vector);

            var len = (maxVal - minVal) / count;

            if (len < 1)
            {
                int round = GetFirstFractionPostion(len) + 1;
                len = Math.Round(len, round);
            }
            else
            {
                len = Math.Round(len, 2);
            }


            if(minVal < 0)
            {
                int round = GetFirstFractionPostion(minVal) + 1;
                minVal = Math.Round(minVal, round) - len;
            }           
            

            return GetDataFrequence( vector, minVal, len);
        }

        /// <summary>
        /// 获取小数第一个位置，0.1，0.9 返回 1， 0.09 返回 2
        /// </summary>
        /// <param name="fraction"></param>
        /// <returns></returns>
        public static int GetFirstFractionPostion(double fraction)
        {
            fraction = GetPositiveFraction(fraction);

            int position =(int)Math.Ceiling( Math.Log10( 1.0 / fraction) );
            return position;
        }

        /// <summary>
        ///  加权平均，一个量的平差
        /// </summary>
        /// <param name="isNeedRms">是否计算RMS, 不计算，则RMS 为 0 </param>
        /// <param name="valueWithWeight">第一个为值，第二个为权</param> 
        /// <returns></returns>
        public static RmsedNumeral GetWeightedAverage( bool isNeedRms, params double[][] valueWithWeight)
        {
            double up = 0;
            double down = 0; 
            foreach (var item in valueWithWeight)
            {
                var val = item[0];
                var weight = item[1]; 
                up += weight * val;
                down += weight; 
            }

            var va = up / down;

            //下面计算单位权中误差
            double std = 0;
            if (isNeedRms)
            {
                var vtpv = 0.0;
                foreach (var item in valueWithWeight)
                {
                    double v = va - item[0];
                    var p = item[1];

                    vtpv += v * p * v;
                }
                std = Math.Sqrt(vtpv / (valueWithWeight.Length - 1)); //除以多余观测数
            }
            return new RmsedNumeral(va, std);
        }

        #endregion

        #region 数据统计、计算相关
        /// <summary>
        /// 最大偏差。正数。
        /// </summary>
        /// <param name="vals"></param>
        /// <returns></returns>
        public static double MaxDiffer(IEnumerable<double> vals)
        {
            var min = Min(vals);
            var max = Max(vals);
            return max - min;
        }
        /// <summary>
        /// 求和
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static double Sum(IEnumerable<double> list)
        {
            double sum = 0;
            foreach (var item in list)
            {
                sum += item;
            }
            return sum;
        }
        #endregion

        #region 粗差处理

        /// <summary>
        /// 拟合器
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public static LsPolyFit GetLsPolyFitter(IDictionary<double, double> dic, int order = 2)
        {
            var fit = new LsPolyFit(dic, order);
            fit.FitParameters();
            return fit;
        }

        /// <summary>
        ///  采用多项式拟合.得到粗差编号（默认3倍中误差）列表。
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="data"></param>
        /// <param name="errorTimes"></param>
        /// <param name="isLoop"></param>
        /// <returns></returns>
        static public List<TKey> GetKeysOfGrossError<TKey>(IDictionary<TKey, double> data, Func<TKey, double> funcKeyToDouble, double errorTimes = 3, int order = 2, bool isLoop = false)
        {
            var list = new List<TKey>();
            var nearty = GetNeatlyData(data, funcKeyToDouble, errorTimes, order, isLoop);
            return data.Keys.Where(m => !nearty.Keys.Contains(m)).ToList(); 
        }

        /// <summary>
        /// 得到干净的数据。
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="collection"></param>
        /// <param name="funcKeyToDouble"></param>
        /// <param name="errorTimes"></param>
        /// <param name="order"></param>
        /// <param name="isLoop"></param>
        /// <returns></returns>
        static public IDictionary<TKey, double> GetNeatlyData<TKey>(IDictionary<TKey, double> collection, Func<TKey, double> funcKeyToDouble, double errorTimes = 3, int order = 2, bool isLoop = false)
        {
            //对应
            Dictionary<double, TKey> keyDic = new Dictionary<double, TKey>();
            Dictionary<double, double> valDic = new Dictionary<double, double>();
            var initDouble = funcKeyToDouble( collection.First().Key);
            foreach (var key in collection.Keys)
            {
                var intKey = funcKeyToDouble( key) - initDouble;
                keyDic.Add(intKey, key);
                valDic.Add(intKey, collection[key]);
            }

            var grosses = Gdp.Utils.DoubleUtil.GetNeatlyData(valDic, errorTimes, order, isLoop);

            Dictionary<TKey, double> result = new Dictionary<TKey, double>();
            foreach (var item in grosses)
            {
                var key = keyDic[item.Key];
                result.Add(key, item.Value);
            }
            return result;
        }

        /// <summary>
        ///  采用多项式拟合.得到粗差编号（默认3倍中误差）列表。
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="data"></param>
        /// <param name="errorTimes"></param>
        /// <param name="isLoop"></param>
        /// <returns></returns>
        static public List<double> GetKeysOfGrossError(IDictionary<double, double> data, double errorTimes = 3, int order = 2, bool isLoop = false)
        {
            var list = new List<double>();
            var nearty = GetNeatlyData(data, errorTimes,order, isLoop);
            return data.Keys.Where(m => !nearty.Keys.Contains(m)).ToList();
            foreach (var item in data)
            {
                if (!nearty.ContainsKey(item.Key)) { list.Add(item.Key); }
            }
            return list;
        }
        /// <summary>
        ///采用多项式拟合。 得到剔除粗差（默认3倍中误差）的数据列表。
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="errorTimes"></param>
        /// <param name="order"></param>
        /// <param name="isLoop"></param>
        /// <returns></returns>
        static public IDictionary<double, double> GetNeatlyData(IDictionary<double, double> collection, double errorTimes = 3, int order = 2, bool isLoop = false)
        {
            var fitter = GetLsPolyFitter(collection, order);
            Dictionary<double, double> newDic = new Dictionary<double, double>();
            var maxDiffer = errorTimes * fitter.StdDev;
            foreach (var kv in collection)
            {
                var differ = Math.Abs(fitter.GetY(kv.Key) - kv.Value);

                if (differ <= maxDiffer)
                {
                    newDic.Add(kv.Key, kv.Value);
                }
                else
                {
                    int i = 0; 
                }
            }
             
            //如果没有发现后不需要循环探测，则直接返回。
            if (newDic.Count == collection.Count || !isLoop) { return collection; }
            if (newDic.Count == 0) { return newDic; }

            return GetNeatlyData(newDic, errorTimes, order, isLoop);
        }
        /// <summary>
        /// 得到剔除粗差（默认3倍中误差）的数据列表。
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="data"></param>
        /// <param name="errorTimes"></param>
        /// <param name="isLoop"></param>
        /// <returns></returns>
        static public Dictionary<TKey, double> GetNeatlyData<TKey>(IDictionary<TKey, double> collection, double errorTimes = 3, bool isLoop = false)
        {
            var average = GetAverageWithStdDev(collection.Values);
            var ave = average[0];
            var std = average[1];
            var maxError = std * errorTimes;
            var list = new Dictionary<TKey, double>();
            foreach (var item in collection)
            {
                var rms = Math.Abs(item.Value - ave);
                if (rms <= maxError) { list.Add(item.Key, item.Value); }
            }
            //如果没有发现后不需要循环探测，则直接返回。
            if (list.Count == collection.Count || !isLoop) { return list; }

            return GetNeatlyData<TKey>(collection, errorTimes, isLoop); 
        }
        /// <summary>
        ///  得到粗差编号（默认3倍中误差）列表。
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="data"></param>
        /// <param name="errorTimes"></param>
        /// <param name="isLoop"></param>
        /// <returns></returns>
        static public List<TKey> GetKeysOfGrossError<TKey>(IDictionary<TKey, double> data, double errorTimes = 3, bool isLoop = false)
        {
            var list = new List<TKey>();
            var nearty = GetNeatlyData<TKey>(data, errorTimes, isLoop);
            return data.Keys.Where(m => !nearty.Keys.Contains(m)).ToList();
            foreach (var item in data)
            {
                if (!nearty.ContainsKey(item.Key)) { list.Add(item.Key);}
            } 
            return list;
        }

         /// <summary>
        /// 得到剔除粗差（默认3倍中误差）的数据列表。
         /// </summary>
         /// <param name="data"></param>
         /// <param name="errorTimes"></param>
         /// <param name="isLoop"></param>
         /// <returns></returns>
        static public List<double> GetNeatlyList(IEnumerable<double> collection, double errorTimes = 3, bool isLoop = false)
        {
            var average = GetAverageWithStdDev(collection);
            var ave = average[0];
            var std = average[1];
            var maxError  =  std * errorTimes;
            List<double> list = new List<double>();
            foreach (var item in collection)
            {
                var rms =  Math.Abs(item - ave);
                if (rms <= maxError) {
                    list.Add(item);
                }
            }
            //如果没有发现后不需要循环探测，则直接返回。
            if (list.Count == collection.Count() || !isLoop) { return list; }

            return GetNeatlyList(collection, errorTimes, isLoop);  
        }
        
        /// <summary>
        /// 得到粗差编号（默认3倍中误差）列表。
        /// </summary>
        /// <param name="data"></param>
        /// <param name="errorTimes"></param>
        /// <param name="isLoop"></param>
        /// <returns></returns>
        static public List<int> GetIndexesOfGrossError(IEnumerable<double> data, double errorTimes = 3, bool isLoop = false)
        { 
            var nearty = GetNeatlyList(data, errorTimes, isLoop); 
             
            List<int> list = new List<int>();
            if(nearty.Count == data.Count()) { return list; }//没有

            int i = 0;
            foreach (var item in data)
            {
                if (!nearty.Contains(item)){ list.Add(i); }
                i++;
            }
            return list;
        }

        /// <summary>
        /// 是否是粗差，通过已知数据求平均判断，并不将其参与计算。
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="newVal"></param> 
        /// <param name="errorTimes"></param>
        /// <returns></returns>
        internal static bool IsGross(IEnumerable<double> collection, double newVal,  double errorTimes)
        {
            var average = GetAverageWithStdDev(collection);
            var ave = average[0];
            var std = average[1];
            var maxError = std * errorTimes;

            return Math.Abs(newVal - ave) > maxError; 
        }
        #endregion

        /// <summary>
        /// 获取与指定对象Compare最近的值。
        /// 如果返回只有一个值，则是刚好等于，或最小或最大边界值,或列表只有一个值。
        /// 如果不是一个值，则返回为三个值。第一个为与指定对象最近的值（与第二或三重复），第二个为小于指定值的数，第三个为大于该值的数,如果数值为null默认值，则表示没有。
        /// </summary> 
        /// <param name="list">从小到大已排序的列表</param>
        /// <param name="t"></param>
        /// <param name="IsNeedSort"></param>
        /// <returns></returns>
        public static List<Double> GetNearst(List<Double> list, Double t, bool IsNeedSort) 
        {
            if(list == null)
            {
                return new List<double>();
            }
            if(list.Count == 1)
            {
                return list;
            }
            //以防万一
            if (IsNeedSort)
            {
                list.Sort();
            }
            //list = list.OrderBy(m => m).ToList();

            var first = list.First();
            var last = list.Last();
            if (t-(first) < 0) //在之前
            {
                return new List<Double>() { first };
            }
            if (t-(last) > 0)//在之后
            {
                return new List<Double>() { last };
            }
            Double defa = default(Double);
            Double nearst = defa;
            double maxDiffer = Double.MaxValue;
            foreach (var item in list)
            {
                double differ = t-(item);
                if (differ == 0)
                {
                    return new List<Double>() { item };
                }

                double absDiffer = Math.Abs(differ);
                if (absDiffer < maxDiffer) { nearst = item; maxDiffer = differ; }
            }

            Double smaller = defa;
            Double larger = defa;
            var index = list.IndexOf(nearst);
            if (t-(nearst) < 0) //在之前
            {
                smaller = list[index - 1];
                larger = nearst;
            }
            else //在之后
            {
                smaller = nearst;
                larger = list[index + 1];
            }

            return new List<Double>()
            {
                nearst, smaller, larger
            };
        } 

        /// <summary>
        /// 是否所有的数据都相等。
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool IsAllEquals(IEnumerable<double> list)
        {
            double val = list.First(); ;
            foreach (var item in list)
            {
                if (val != item) {
                    return false;
                }
            }
            return true;
        }

        /**
      * 归算到指定区间[0, peroid)
      * @param val
      * @param peroid
      * @return
      */
        public static double RollTo(double val, double peroid)
        {
            if (peroid == 0) { throw new ArgumentException("peroid == 0"); }
            while (val >= peroid)
            {
                val -= peroid;
            }
            while (val < 0)
            {
                val += peroid;
            }
            return val;
        }
        /// <summary>
        /// 线性差值
        /// </summary>
        /// <param name="t">待差值时刻</param>
        /// <param name="t0">小时刻</param>
        /// <param name="t1">大时刻</param>
        /// <param name="val0">小值</param>
        /// <param name="val1">大值</param>
        /// <returns></returns>
        public static double Interpolate(double t, double t0, double t1, double val0, double val1)
        {
            if (t < Math.Min(t0, t1) || t > Math.Max(t0, t1)) { throw new ArgumentException("输入参数必须介于t1,t2中间！"); }
            var val = val0 + (val1 - val0) / (t1 - t0) * (t - t0);
            return val;
        }
        /// <summary>
        /// 双线性内插，平面格网内插
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="x0"></param>
        /// <param name="x1"></param>
        /// <param name="y0"></param>
        /// <param name="y1"></param>
        /// <param name="x0y0Val"></param>
        /// <param name="x0y1Val"></param>
        /// <param name="x1y0Val"></param>
        /// <param name="x1y1Val"></param>
        /// <returns></returns>
        public static double Interpolate(
            double x, double y,
            double x0, double x1,
            double y0, double y1,
            double x0y0Val, double x0y1Val,
            double x1y0Val, double x1y1Val)
        {
            double p = (x - x0) / (x1 - x0);
            double q = (y - y0)/ (y1 - y0);

            var val = (1 - p) * (1 - q) * x0y0Val
                + p * (1 - q) * x1y0Val
                + q * (1 - p) * x0y1Val
                + p * q * x1y1Val; 

            return val;
        }
        /// <summary>
        /// 转换为双精度
        /// </summary>
        /// <param name="inverse"></param>
        /// <returns></returns>
        internal static double [] GetDouble(decimal[] inverse)
        {
            double[] d = new double[inverse.Length];
            for (int i = 0; i < inverse.Length; i++)
            {
                d[i] = Convert.ToDouble(inverse[i]);
            }
            return d;
        }
    }
}
