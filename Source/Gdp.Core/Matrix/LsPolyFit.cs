// 2013.01.21, czs,  最小二乘多项式拟合
//2016.11.11, double, add in zhengzhou, 添加了拟合sigma这一变量
//2018.05.20, czs, edit in hmx, 修正起始位置加1的bug
//2018.05.27, czs, edit in hmx, 重构，初始化和估值分开

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Linq;
using Gdp.Utils;

namespace Gdp
{
    /// <summary>
    /// 最小二乘多项式拟合 
    /// </summary>
    public class LsPolyFit// : IGetY
    {
        #region 构造函数
        /// <summary>
        /// 拟合器
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public LsPolyFit(int order = 2)
        {
            SetOrder(order);
        }

        /// <summary>
        /// 拟合器
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public LsPolyFit(IDictionary<double, double> dic, int order = 2)
        {  
            SetData(dic).SetOrder(order);
        }
        /// <summary>
        /// 多项式拟合
        /// </summary>
        /// <param name="yArray"></param>
        /// <param name="order"></param>
        public LsPolyFit(double[] yArray, int order = 2)
        { 
            SetData(yArray).SetOrder(order);
        }

        /// <summary>
        /// 多项式拟合。
        /// 阶次排序，从低到高。
        /// </summary>
        /// <param name="arrayX"></param>
        /// <param name="arrayY"></param>
        /// <param name="order"></param>
        public LsPolyFit(double[] arrayX, double[] arrayY, int order=2)
        {          
            if (arrayY.Length < order) throw new ArgumentException("拟合阶次应该小于数组的长度。");
          
            SetData(arrayX, arrayY).SetOrder(order);
        }
        #endregion

        #region  赋值初始化
        /// <summary>
        /// 设置数据并拟合参数
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="keys"></param>
        /// <param name="keyToNumerial"></param>
        /// <param name="ys"></param>
        public void InitAndFitParams<TKey>(IEnumerable<TKey> keys, Func<TKey, double> keyToNumerial, IEnumerable<double> ys)
        {
            List<double> list = new List<double>();

            foreach (var item in keys)
            {
                list.Add(keyToNumerial(item));
            }
            this.ArrayX = list.ToArray();
            this.ArrayY = ys.ToArray();
            FitParameters();
        }
        /// <summary>
        /// 设置数据并拟合参数
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="dic"></param>
        /// <param name="keyToNumerial"></param>
        public double[] InitAndFitParams<TKey>(Dictionary<TKey,double> dic, Func<TKey, double> keyToNumerial)
        {
            List<double> list = new List<double>();

            foreach (var item in dic)
            {
                list.Add(keyToNumerial(item.Key));
            } 
            this.SetData(list.ToArray(), dic.Values.ToArray());
            return  FitParameters();
        }
        /// <summary>
        /// 设置数据并拟合参数。
        /// </summary>
        /// <param name="arrayX"></param>
        /// <param name="arrayY"></param>
        public double[] InitAndFitParams(double[] arrayX, double[] arrayY)
        {
            SetData(arrayX, arrayY);
            return   FitParameters();
        }
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="dic"></param> 
        /// <returns></returns> 
        public LsPolyFit SetData(IDictionary<double, double> dic)
        {
            double[] ys = dic.Values.ToArray(); 
            double[] xs = dic.Keys.ToArray();

            return SetData(xs, ys);
        }

        /// <summary>
        /// 多项式拟合
        /// </summary>
        /// <param name="yArray"></param> 
        public LsPolyFit SetData(double[] yArray)
        {
            double[] xArray = new double[yArray.Length];
            for (int i = 0; i < yArray.Length; i++)
            {
                xArray[i] = i;
            }
            return SetData(xArray, yArray);
        }
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="arrayX"></param>
        /// <param name="arrayY"></param>
        /// <returns></returns>
        public LsPolyFit SetData(double[] arrayX, double[] arrayY)
        {
            if (arrayY.Length != arrayX.Length) throw new ArgumentException("输入的数组X,Y的长度应该相等！");
            this.ArrayX = arrayX;
            this.ArrayY = arrayY;
            return this;
        }
        /// <summary>
        /// 设置数据
        /// </summary> 
        /// <param name="order"></param>
        /// <returns></returns>
        public LsPolyFit SetOrder(int order)
        {
            if (order < 1) throw new ArgumentException("最高此次幂必须大于0。");
            this.Order = order; 
            return this;
        }
        #endregion

        #region 核心属性
        /// <summary>
        /// 次数
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// 参数数量,比次数多一个。
        /// </summary>
        public int ParamCount { get { return Order + 1; } }
        /// <summary>
        /// 多项式参数,数组表示。
        /// </summary>
        public double[] Parameters { get; private set; }
        /// <summary>
        /// 单位权中误差
        /// </summary>
        public double StdDev { get; private set; }
        /// <summary>
        /// X 集合
        /// </summary>
        private double[] ArrayX { get;  set; }
        /// <summary>
        /// Y 集合
        /// </summary>
        private double[] ArrayY { get;  set; }

        /// <summary>
        /// 第一个 X
        /// </summary>
        public double FirstX { get { return ArrayX[0]; } }
        /// <summary>
        /// 第一个 Y
        /// </summary>
        public double FirstY { get { return ArrayY[0]; } }
        /// <summary>
        /// 最后一个 X
        /// </summary>
        public double LastX { get { return ArrayX[ArrayX.Length - 1]; } }
        /// <summary>
        /// 最后一个Y
        /// </summary>
        public double LastY { get { return ArrayY[ArrayY.Length - 1]; } }
        /// <summary>
        /// 多项式求值
        /// </summary>
        PolyVal PolyVal { get; set; }
        #endregion

        #region 计算方法
        /// <summary>
        /// 多项式求值。
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double GetY(double x)
        {
            if (PolyVal == null) PolyVal = new PolyVal(Parameters);
            return PolyVal.GetY(x);
        }
        /// <summary>
        /// 多项式求值。
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public RmsedNumeral GetRmsedY(double x)
        {
            if (PolyVal == null) PolyVal = new PolyVal(Parameters);
            return new RmsedNumeral( PolyVal.GetY(x), StdDev);
        }
        /// <summary>
        /// 拟合参数
        /// </summary>
        public double [] FitParameters()
        {
            int rowA = ArrayY.Length;//行
            int colA = ParamCount;

            //coeffOfParams(m,n)         
            double[][] A = MatrixUtil.Create(rowA, colA);
            //让 X 不变，如输出相同。！！！！2018.05.20，czs 
            for (int i = 0; i < rowA; i++)
            {
                for (int j = 0; j < colA; j++)
                {
                    var x = ArrayX[i]; 
                    A[i][j] = Math.Pow(x, j);
                }
            }
             
            double[][] L = MatrixUtil.Create(rowA, 1); 
            for (int i = 0; i < rowA; i++)
            {
                L[i][0] = ArrayY[i];
            }             

            ParamAdjuster ParamAdjuster = new ParamAdjuster();
            var ad = ParamAdjuster.Run(new Matrix(A),new Matrix( L));
            Parameters = ad.OneDimArray;
            StdDev = ParamAdjuster.StdDev;
            return Parameters;
        }

        /// <summary>
        /// 多项式拟合。
        /// 参数与乘积为从低到高。
        /// 注意：拟合参数数量 = 阶次 + 1
        /// </summary>
        /// <param name="arrayX">数组</param>
        /// <param name="arrayY">数组</param>
        /// <param name="order">阶次</param>
        /// <returns>拟合参数</returns>
        public static double[] GetFitParams(double[] arrayX, double[] arrayY, int order)
        {
            LsPolyFit fit = new LsPolyFit(arrayX, arrayY, order);
            return fit.FitParameters();
        }
        /// <summary>
        /// 字符串显示
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("DataCount:" + this.ArrayX.Length);
            sb.Append(", ParamCount: " + ParamCount);
            sb.Append(", StdDev: " + StdDev.ToString("G6"));
            if (Parameters != null)
            {
                sb.Append(", Parameters:");
                foreach (var item in Parameters)
                {
                    sb.Append(", " + item);
                }
            }
            return sb.ToString();
        }
        #endregion
    }
}
