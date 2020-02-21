//2014.09.08, czs, edit, 注意：不对钟差相对论误差进行改正，此处时间是卫星钟时间，是原始的观测数据，没有相对改正信息。
//2015.03, czs, edit,由于一般没有卫星速度的数据，必须通过插值坐标计算速度。
//2015.04, cy edit, 崔阳, 引入精度信息，映入新拉格朗日算法可以计算速度信息，对原始星历进行地球自转改正后差分
//2016.09.28， czs edit in namu, 进行重构，代码优化

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
using Gdp; 
using Gdp.Data.Rinex;
using System.Threading.Tasks;
using System.Threading;
using Gdp.IO;

namespace Gdp
{
    /// <summary>
    /// 星历插值器。目前采用切比雪夫和拉格朗日差值。
    /// 传入  EphemerisInfo 列表 也可是 Sp3Record 列表，可以从数据库或者直接从文件获取。
    /// 每次只能处理一个卫星，即PRN，否则容易引起混乱。
    /// 注意：传入的卫星列表默认为已经进行过筛选了,默认与待获取星历相近。将直接进行插值，而不再判断。
    /// </summary>
    public class EphemerisInterpolator : IDisposable
    {
       Log log = new  Log(typeof( EphemerisInterpolator));
            
        /// <summary>
        /// 初始化时，输入要插值的星历信息列表。
        /// 列表至少包含 2 个星历。
        /// </summary>
        /// <param name="oldInfos"></param>
        public EphemerisInterpolator(List<Ephemeris> oldInfos, int order = 10)
        {
            this.SatEphemerises = new Data.Rinex.EphemerisStorage();
            this.SatEphemerises.Add(oldInfos);
            this.Order = order;
            //InitDetect();
        } 

        /// <summary>
        /// 采用卫星列表初始化
        /// </summary>
        /// <param name="sortedRecords"></param>
        public EphemerisInterpolator(EphemerisStorage SatEphemerises, int order = 10)
        {
            this.SatEphemerises = SatEphemerises;
            this.Order = order; 
        }

        EphemerisStorage SatEphemerises { get; set; }
        public int Order { get; set; }
         
        IGetY fitX, fitY, fitZ, fitClock;

        private void Init()
        {
            int count = SatEphemerises.Count;
            double[] x = new double[count];
            double[] yX = new double[count];
            double[] yY = new double[count];
            double[] yZ = new double[count];
            double[] yClock = new double[count];
            var first = SatEphemerises.First;
            int i = 0;
            foreach (var record in SatEphemerises) 
            { 
                x[i] = (record.Time - first.Time); //Y为GPS周秒。 

                yX[i] = record.XYZ.X;
                yY[i] = record.XYZ.Y;
                yZ[i] = record.XYZ.Z;
                yClock[i] = record.ClockBias;
            }
            int order = 10;
            order = Math.Min(Order, count);

            //fitX = new PolyFit(x, yX);
            //fitY = new PolyFit(x, yY);
            //fitZ = new PolyFit(x, yZ);
            //fitClock = new PolyFit(x, yClock);
            fitX = new LagrangeInterplation(x, yX, order);
            fitY = new LagrangeInterplation(x, yY, order);
            fitZ = new LagrangeInterplation(x, yZ, order);
            // fitClock = new LagrangeInterplation(x, yClock, 2);
            fitClock = new LagrangeInterplation(x, yClock, 10);


            //fitX = new ChebyshevPolyFit(x, yX, order);
            //fitY = new ChebyshevPolyFit(x, yY, order);
            //fitZ = new ChebyshevPolyFit(x, yZ, order);
            //fitClock = new LagrangeInterplation(x, yClock, 2);
        }

        /// <summary>
        /// 获取插值后的 EphemerisInfo。
        /// 注意：不对钟差相对论误差进行改正，此处时间是卫星钟时间，没有相对信息。2014.09.08
        /// 失败，如超过时间，则返回null
        /// 由于一般没有卫星速度的数据，必须通过插值坐标计算速度。2015.03.
        /// 如果有速度信息，以下的方法就不合适了。
        /// </summary> 
        /// <param name="gpsTime">时间</param>
        /// <returns></returns>
        public Ephemeris GetEphemerisInfo(Time gpsTime)
        {
            var first = SatEphemerises.First;
            if (first == null) { return null; }

            SatelliteNumber PRN = first.Prn;
            double gpsSecond = (gpsTime - first.Time); //减去序列中第一个数

      //     gpsSecond /= 60.0;

            int count = SatEphemerises.Count;            
            int order = Math.Min(Order, count);
            
            //定义差值列表
            double[] tList = new double[order];
            double[] xList = new double[order];
            double[] yList = new double[order];
            double[] zList = new double[order];
            double[] clockList = new double[order];
            
            //初始化差值列表
            int i = 0; 
            foreach (var ephemeris in SatEphemerises)
            {
                if (i >= order) { break; }
 
                //方案1：顾及地球自转效应
                XYZ newXyz = GetSatPosWithEarthRotateCorrection(gpsTime, ephemeris);
                // 方案2：不顾及地球自转效应
           //     newXyz = ephemeris.XYZ;
                xList[i] = newXyz.X;
                yList[i] = newXyz.Y;
                zList[i] = newXyz.Z; 

                var sec = (ephemeris.Time - first.Time); //秒。
          //      sec = sec / 60.0;
                tList[i] =  sec;
                clockList[i] = ephemeris.ClockBias; 
                i++;
            }

            InterpolatingManager manager = new InterpolatingManager(tList);
            manager.Add("X", xList);
            manager.Add("Y", yList);
            manager.Add("Z", zList);
            manager.Add("C", clockList);

            #region 并行差值测试
            //测试发现，采用并行后，速度下降了至少4倍。初步估计和插值数量太少有关。czs，2016.10.15，found and tested in hongqing.

            //DateTime start = DateTime.Now;
            //var length = 20000;
            //for (int j = 0; j < length; j++)
            //{
            //    var values0 = manager.GetInterpolateValuesParallel(gpsSecond);
            //}
            //var span1 = DateTime.Now - start;
            //start = DateTime.Now;
            //for (int j = 0; j < length; j++)
            //{
            //    var values2 = manager.GetInterpolateValues(gpsSecond);
            //}
            //var span2 = DateTime.Now - start;
            #endregion

            var  values = manager.GetInterpolateValues(gpsSecond);
            //计算位置，速度和钟差
            double x = 0, y = 0, z = 0; double dtime = 0;
            double dx = 0, dy = 0, dz = 0; double ddtime = 0;

            var xVal = values["X"];
            var yVal = values["Y"];
            var zVal = values["Z"];
            var dtimeVal = values["C"];

            x = xVal.Value;
            dx = xVal.Rate;
            y = yVal.Value;
            dy = yVal.Rate;
            z = zVal.Value;
            dz = zVal.Rate;
            dtime = dtimeVal.Value;
            ddtime = dtimeVal.Rate;


            //Lagrange(tList, xList, gpsSecond, ref x, ref dx);
            //Lagrange(tList, yList, gpsSecond, ref y, ref dy);
            //Lagrange(tList, zList, gpsSecond, ref z, ref dz);
            //Lagrange(tList, clockList, gpsSecond, ref dtime, ref ddtime);
             

            //fitX.GetYdY(gpsSecond, ref x, ref dx);
            //fitY.GetYdY(gpsSecond, ref y, ref dy);
            //fitZ.GetYdY(gpsSecond, ref z, ref dz);
            //fitClock.GetYdY(gpsSecond, ref dtime, ref ddtime);

            XYZ SatXyz = new XYZ(x, y, z);

    
            //精度赋值,算法有待考虑//???2016.09.28,czs
            XYZ SatXyzSig = first.Rms;
            double clockSig = first.ClockBiasRms;
            XYZ SatXyzDotSig = first.XyzDotRms;

            //计算速度
            //double nextSecond = gpsSecond + 0.001;
            //XYZ xyzNext = new XYZ(fitX.GetY(nextSecond), fitY.GetY(nextSecond), fitZ.GetY(nextSecond));
            //XYZ speed = (xyzNext-xyz) / 0.001;


            XYZ SatSpeed = new XYZ(dx, dy, dz);
            if (SatSpeed.Length > 50000000000)
            {
                log.Error(PRN + " 卫星速度计算错误，不可能飞这么快！" + SatSpeed);
            }

            //Add relativity correction to dtime 
            //This only for consistency with GPSEphemerisInter
            //dtr=-2*dot(R,V)/(C*C)
            //dtime +=EphemerisUtil.GetRelativeCorrection(SatXyz, SatSpeed);

            return new Ephemeris()
            {
                Prn = PRN,
                XYZ = SatXyz,
                Rms = SatXyzSig,
                // Clock = fitClock.GetY(gpsSecond),
                ClockBias = dtime,
                ClockBiasRms = clockSig,
                ClockDrift = ddtime,
                Time = gpsTime,
                XyzDot = SatSpeed,
                XyzDotRms = SatXyzDotSig
            };
        }

        /// <summary>
        /// 获取指定时刻的卫星位置，考虑了地球自转改正置。
        /// </summary>
        /// <param name="gpsTime"></param>
        /// <param name="ephemeris"></param>
        /// <returns></returns>
        public static XYZ GetSatPosWithEarthRotateCorrection(Time gpsTime, Ephemeris ephemeris)
        {
            var dt = (ephemeris.Time - gpsTime); //相距参考时刻的秒数。

            #region 方案1：顾及地球自转效应
            //correction for earth rotation

            double sinL = Math.Sin(RotateVelocityOfEarth * dt);
            double cosL = Math.Cos(RotateVelocityOfEarth * dt);

            double newX = cosL * ephemeris.XYZ.X - sinL * ephemeris.XYZ.Y;
            double newY = sinL * ephemeris.XYZ.X + cosL * ephemeris.XYZ.Y;
            double newZ = ephemeris.XYZ.Z;
            XYZ newXyz = new XYZ(newX, newY, newZ);
            #endregion
            return newXyz;
        }

        /// <summary>
        /// 地球自转常数 (rad/s)
        /// </summary>
        public const double RotateVelocityOfEarth = 7.2921151467e-5;    /* earth angular velocity (IS-GPS) (rad/s) */


        public void Dispose()
        {
            SatEphemerises.Clear();
        }
    }
    /// <summary>
    /// 拉格朗日多项式插值 
    /// </summary>
    public class LagrangeInterplation : IGetY
    {
        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="yArray"></param>
        /// <param name="order"></param>
        public LagrangeInterplation(double[] yArray, int order = 10)
        {
            double[] xArray = new double[yArray.Length];
            for (int i = 0; i < yArray.Length; i++)
            {
                xArray[i] = i;
            }

            this.XArray = xArray;
            this.YArray = yArray;
            this.XCount = xArray.Length;
            this.Order = order;
        }


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="xArray">应该是递增的值</param>
        /// <param name="yArray">函数值</param>
        /// <param name="order">阶次</param>
        public LagrangeInterplation(double[] xArray, double[] yArray, int order = 10)
        {
            this.XArray = xArray;
            this.YArray = yArray;
            this.XCount = xArray.Length;
            this.Order = order;
        }

        /// <summary>
        /// X各点坐标组成的数组
        /// </summary>
        public double[] XArray { get; set; }

        /// <summary>
        /// X各点对应的Y坐标值组成的数组
        /// </summary>
        public double[] YArray { get; set; }
        /// <summary>
        /// 多项式的阶次
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// x数组或者y数组中元素的个数, 注意两个数组中的元素个数需要一样
        /// </summary>
        public int XCount { get; set; }

        /// <summary>
        /// 插值
        /// </summary>
        /// <param name="xValue"></param>
        /// <returns></returns>
        public double GetY(double xValue)
        {
            List<int> indexes = Utils.DoubleUtil.GetNearstIndexes(XArray, xValue, Order);

            List<double> xList = new List<double>();
            List<double> yList = new List<double>();
            foreach (var item in indexes)
            {
                xList.Add(XArray[item]);
                yList.Add(YArray[item]);
            }

            double y = Lagrange(xList.ToArray(), yList.ToArray(), xValue);
            return y;
        }

        /// <summary>
        /// 插值，返回 the value of Y(x) and dY(X)/dX
        /// Assumes that xValue is between X[k-1] and X[k] ,where k=N/2
        /// Warning: for use with the precise(sp3) ephemeris only when velocity is not avilable estimates of velocity , and especially clock drift, not as accurate.
        /// Cui Yang, 2014.06.09
        /// </summary>
        /// <param name="xValue"></param>
        /// <param name="Y">位置</param>
        /// <param name="dYdX">速度，单位Km</param>
        public void GetYdY(double xValue, ref double Y, ref double dYdX)
        {
            List<int> indexes = Utils.DoubleUtil.GetNearstIndexes(XArray, xValue, Order);

            List<double> xList = new List<double>();
            List<double> yList = new List<double>();
            foreach (var item in indexes)
            {
                xList.Add(XArray[item]);
                yList.Add(YArray[item]);
            }
            double y = 0.0;
            double dydx = 0.0;
            Lagrange(xList.ToArray(), yList.ToArray(), xValue, ref y, ref dydx);
            Y = y;
            dYdX = dydx;
        }

        public void GetYdY(double xValue, double xValue1, ref double Y, ref double dYdX)
        {
            List<int> indexes = Utils.DoubleUtil.GetNearstIndexes(XArray, xValue, Order);

            List<double> xList = new List<double>();
            List<double> yList = new List<double>();
            foreach (var item in indexes)
            {
                xList.Add(XArray[item] - XArray[indexes[0]]);
                yList.Add(YArray[item]);
            }

            xValue = xValue - XArray[indexes[0]] - xValue1;

            double y = 0.0;
            double dydx = 0.0;
            Lagrange(xList.ToArray(), yList.ToArray(), xValue, ref y, ref dydx);
            Y = y;
            dYdX = dydx;
        }


        /// <summary>
        /// Linear interpolation at coeffOfParams single point x.
        /// </summary>
        /// <param name="x">double</param>
        /// <param name="x0">double</param>
        /// <param name="x1">double</param>
        /// <param name="y0">double</param>
        /// <param name="y1">double</param>
        /// <returns>double</returns>
        static public double Linear(double x, double x0, double x1, double y0, double y1)
        {
            if ((x1 - x0) == 0)
            {
                return (y0 + y1) / 2;
            }
            return y0 + (x - x0) * (y1 - y0) / (x1 - x0);
        }

        /// <summary>
        /// Lagrange polynomial interpolation at coeffOfParams single point x. Because Lagrange polynomials tend to be ill behaved, this method should be used with care.
        /// A LagrangeInterpolator object should be used if multiple interpolations are to be performed using the same data
        /// </summary>
        /// <param name="xArray">the x data</param>
        /// <param name="yArray">the y data</param>
        /// <param name="x">double</param>
        /// <returns>double</returns>
        static public double LagrangeInterp(double[] xArray, double[] yArray, double x)
        {
            if (xArray.Length != yArray.Length)
            {
                throw new ArgumentException("Arrays must be of equal length."); //$NON-NLS-1$
            }
            double sum = 0;
            for (int i = 0, n = xArray.Length; i < n; i++)
            {
                if (x - xArray[i] == 0)
                {
                    return yArray[i];
                }
                double product = yArray[i];
                for (int j = 0; j < n; j++)
                {
                    if ((i == j) || (xArray[i] - xArray[j] == 0))
                    {
                        continue;
                    }
                    product *= (x - xArray[j]) / (xArray[i] - xArray[j]);
                }
                sum += product;
            }
            return sum;
        }
        /// <summary>
        /// 拉格朗日插值
        /// </summary>
        /// <param name="xs"></param>
        /// <param name="ys"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double Lagrange(double[] xs, double[] ys, double x)
        {
            if (xs.Length != ys.Length) throw new ArgumentException("数组大小必须一致.");

            double val = 0;
            int count = xs.Length;
            for (int i = 0; i < count; i++)
            {
                if (x - xs[i] == 0) return ys[i];

                double y = ys[i];
                double f = 1;
                for (int j = 0; j < count; j++)
                {
                    if (j == i || (xs[i] - xs[j]) == 0) continue;

                    f *= (x - xs[j]) / (xs[i] - xs[j]);
                }

                val += y * f;
            }

            return val;
        }

        /// <summary>
        /// 拉格朗日插值 崔阳
        /// </summary>
        /// <param name="xs"></param>
        /// <param name="ys"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="dydx"></param>
        /// <returns></returns>
        public static void Lagrange(double[] xs, double[] ys, double x, ref double y, ref double dydx)
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
                double S = 0;
                for (int k = 0; k < N; k++) if (i != k)
                    {
                        if (k < i) S += Q[k + (i * (i + 1)) / 2] / D[i];
                        else S += Q[i + (k * (k + 1)) / 2] / D[i];
                    }
                dydx += ys[i] * S;
            }
        }
        public static void Test()
        {
            double[] xs = new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
            double[] ys = new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
            int len = xs.Length;
            double[] result = new double[len];
            double[] result2 = new double[len];

            for (int i = 0; i < len; i++)
            {
                result[i] = LagrangeInterp(xs, ys, i + 0.5);
                result2[i] = Lagrange(xs, ys, i + 0.5);
            }
        }

    }
}