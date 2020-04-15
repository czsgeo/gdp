// 2013.01.21, 多项式求值。

using System;
using System.Collections.Generic;
using System.Text;

namespace Gdp
{
    /// <summary>
    /// 多项式求值。
    /// </summary>
    public class PolyVal
    {
        private double[] _paramArray;

        /// <summary>
        /// 求多项式值。
        /// yi = a0 * xi^0 + a1 * xi^1 + a2 * xi^2 + a3 *  xi^3 + ... + an * xi^n 
        /// 参数顺序与matlab不同，为从低到高，即最后一位为0阶次的数字。
        /// </summary>
        /// <param name="paramArray"></param> 
        public PolyVal(double[] paramArray)
        {
            this._paramArray = paramArray;
        }

        /// <summary>
        /// 多项式对应的Y值数组。
        /// {y0, y1, y2,...,yn} 
        /// </summary>
        /// <param name="xArray"></param>
        public double[] GetYArray( double [] xArray) {
            double [] yArray = new double[xArray.Length];
            for (int i = 0; i < xArray.Length; i++)
            {
                yArray[i] = GetY(xArray[i]);
            }
            return yArray;
        }

        /// <summary>
        /// 求多项式值。
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double GetY(double x)
        {
            double result = 0;
            for (int i = 0; i < _paramArray.Length; i++)
			{
                result += _paramArray[i] * Math.Pow(x, i);
			}
            return result;
        }



        /// <summary>
        /// 求多项式值。
        /// 参数顺序与matlab不同，为从低到高，即最后一位为0阶次的数字。
        /// </summary>
        /// <param name="paramArray"></param>
        /// <param name="xArray"></param>
        /// <returns></returns>
        public static double[] GetYArray(double[] paramArray, double[] xArray)
        {
            return new PolyVal(paramArray).GetYArray(xArray);
        }
        /// <summary>
        ///  求多项式值。
        ///  参数顺序与matlab不同，为从低到高，即最后一位为0阶次的数字。
        /// </summary>
        /// <param name="paramArray"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double GetY(double[] paramArray, double x)
        {
            return new PolyVal(paramArray).GetY(x);
        }
    }
}
