//2016.10.15, czs, create in hongqing, 多参数插值管理器

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;  
using System.Threading.Tasks;
using System.Threading; 

namespace Gdp
{


    /// <summary>
    /// 多参数插值管理器
    /// </summary>
    public class InterpolatingManager : BaseDictionary<string, double[]>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Indexes"></param>
        public InterpolatingManager(double[] Indexes)
        {
            this.Indexes = Indexes;
        }

        /// <summary>
        /// 检索号
        /// </summary>
        public double[] Indexes { get; set; }
        /// <summary>
        /// 并行执行插值操作
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IDictionaryClass<string, RatedValue> GetInterpolateValuesParallel(double index)
        {
            var vals = new BaseDictionary<string, RatedValue>();
            var list = new List<NamedRatedValue>();

            Parallel.ForEach(this.Keys, new Action<string>(delegate (string key)
            {
                double y = 0, dydx = 0;
                Lagrange(Indexes, this[key], index, ref y, ref dydx);
                vals.Add(key, new NamedRatedValue(key, y, dydx));
            }));
            return vals;
        }
        /// <summary>
        /// 串行执行插值操作
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Dictionary<string, RatedValue> GetInterpolateValues(double index)
        {
            var vals = new Dictionary<string, RatedValue>();
            foreach (var key in this.Keys)
            {
                double y = 0, dydx = 0;
                Lagrange(Indexes, this[key], index, ref y, ref dydx);

                vals[key] = new RatedValue(y, dydx);
            }

            return vals;
        }

        static object locker = new object();
        /// <summary>
        /// 拉格朗日插值 崔阳, 2015.04
        /// </summary>
        /// <param name="xs"></param>
        /// <param name="ys"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="dydx"></param>
        /// <returns></returns>
        static public void Lagrange(double[] xs, double[] ys, double x, ref double y, ref double dydx)
        {
            if (xs.Length != ys.Length) throw new ArgumentException("数组大小必须一致.");
            
            int N = xs.Length;
            int M = (N * (N + 1)) / 2;
            double[] P = new double[N];
            double[] Q = new double[M];
            double[] D = new double[N];
            for (int i = 0; i < N; i++) { P[i] = 1.0; D[i] = 1.0; }
            for (int i = 0; i < M; i++) { Q[i] = 1.0; }

            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if (i != j)
                    {
                        P[i] *= x - xs[j];
                        D[i] *= xs[i] - xs[j];
                        if (i < j)
                        {
                            for (int k = 0; k < N; k++)
                            {
                                if (k == i || k == j) continue;
                                Q[i + (j * (j + 1)) / 2] *= x - xs[k];
                            }
                        }
                    }
                }
            }

            y = 0.0;
            dydx = 0.0;
            for (int i = 0; i < N; i++)
            {
                y += ys[i] * (P[i] / D[i]);
                //以下计算速度
                double S = 0;
                for (int k = 0; k < N; k++) if (i != k)
                {
                    if (k < i) { S += Q[k + (i * (i + 1)) / 2] / D[i]; }
                    else { S += Q[i + (k * (k + 1)) / 2] / D[i]; }
                }
                dydx += ys[i] * S;
            }
        }
    }

    //2016.08.19, czs, create in 江西上饶火车站, 同时具有平滑数据和原始数据
    /// <summary>
    /// 同时具有平滑数据和原始数据
    /// </summary>
    public class RawSmoothValue : NumeralValue
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="value"></param>
        /// <param name="smoothValue"></param>
        public RawSmoothValue(double value, double smoothValue)
            : base(value)
        {
            this.SmoothValue = smoothValue;
        }
        /// <summary>
        /// 平滑数据
        /// </summary>
        public double SmoothValue { get; set; }

    }
    //2016.10.15, czs, create in hongqing,具有名称的 值与速度
    public class NamedRatedValue : RatedValue, Namable
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="rate"></param>
        public NamedRatedValue(string name, double value, double rate)
            : base(value, rate)
        {
            this.Name = name;
        }

        public string Name { get; set; }
    }

    //2016.10.15, czs, create in hongqing, 值与速度
    /// <summary>
    /// 值与速度。
    /// </summary>
    public class RatedValue : NumeralValue
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="value"></param>
        /// <param name="rate"></param>
        public RatedValue(double value, double rate)
            : base(value)
        {
            this.Rate = rate;
        }

        /// <summary>
        /// 变化
        /// </summary>
        public double Rate { get; set; }
    }
     
}