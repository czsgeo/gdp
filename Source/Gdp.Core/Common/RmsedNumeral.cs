//2014.10.27, czs, create in numu, 具有权值的双精度数值
//2017.09.08, czs, edit in hongqing, 增加有效性判断

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace Gdp
{
    
    
    /// <summary>
    /// 加权数值
    /// </summary>
    public class WeightedNumeral
    {
        /// <summary>
        /// 加权数值
        /// </summary>
        /// <param name="val"></param>
        /// <param name="weight"></param>
        public WeightedNumeral(double val=0, double weight=0)
        {
            this.Value = val;
            this.Weight = weight;
        }
        /// <summary>
        /// 加权数值
        /// </summary>
        public double Value { get; set; }
        /// <summary>
        /// 权
        /// </summary>
        public double Weight { get; set; }
        /// <summary>
        /// 相等
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if(!(obj is WeightedNumeral)) { return false; }
            var o = (WeightedNumeral)obj;
            return this.Value == o.Value && this.Weight == o.Weight;
        }
        /// <summary>
        /// 哈希数据
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Value.GetHashCode() *3 + Weight .GetHashCode() * 7;
        }
        /// <summary>
        /// 字符串
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public virtual string ToString(string format)
        {
            return Value.ToString(format) + "(" + Weight.ToString(format) + ")";
        }
        /// <summary>
        /// 字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Value + "(" + Weight + ")";
        }
    }

    /// <summary>
    /// 具有权值的双精度数值。
    /// </summary>
    public class RmsedNumeral : RmsedValue<Double>
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public RmsedNumeral() { }
        /// <summary>
        /// 构造函数。
        /// </summary> 
        /// <param name="valWithRms"></param>
        public RmsedNumeral(Double[] valWithRms)
            : base(valWithRms[0], valWithRms[1])
        {
        }
        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="val"></param>
        /// <param name="rmsOrStdDev"></param>
        public RmsedNumeral(Double val, Double rmsOrStdDev)
            : base(val, rmsOrStdDev)
        {
        }

        /// <summary>
        /// 方差值
        /// </summary>
        public virtual Double Variance { get { return Math.Pow(Rms, 2); } }

        /// <summary>
        /// 加上
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static RmsedNumeral operator +(RmsedNumeral left, RmsedNumeral right)
        {
            return new RmsedNumeral(left.Value + right.Value, Math.Sqrt(left.Variance + right.Variance));
        }
        /// <summary>
        /// 加上
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static RmsedNumeral operator -(RmsedNumeral left, RmsedNumeral right)
        {
            return new RmsedNumeral(left.Value - right.Value, Math.Sqrt(left.Variance + right.Variance));
        }
        public static RmsedNumeral operator /(RmsedNumeral left, double right)
        {
            return new RmsedNumeral(left.Value / right, Math.Sqrt(left.Variance / right));
        }
        public static RmsedNumeral operator /(double left, RmsedNumeral right)
        {
            return new RmsedNumeral(left / right.Value, Math.Sqrt(left / right.Variance));
        }
        public static RmsedNumeral operator *(RmsedNumeral left, double right)
        {
            return new RmsedNumeral(left.Value * right, Math.Sqrt(left.Variance * right));
        }
        public static RmsedNumeral operator *(double left, RmsedNumeral right)
        {
            return new RmsedNumeral(left * right.Value, Math.Sqrt(left * right.Variance));
        }
        public static RmsedNumeral operator -(double left, RmsedNumeral right)
        {
            return new RmsedNumeral(left - right.Value, right.Rms);
        }
        public static RmsedNumeral operator -(RmsedNumeral left, double right)
        {
            return new RmsedNumeral(left.Value - right, left.Rms);
        }
        public static RmsedNumeral operator +(double left, RmsedNumeral right)
        {
            return new RmsedNumeral(left + right.Value, right.Rms);
        }

        /// <summary>
        /// 值全为 0 。
        /// </summary>
        public static RmsedNumeral Zero { get { return new RmsedNumeral(0, 0); } }

        /// <summary>
        /// NaN
        /// </summary>
        public static RmsedNumeral NaN { get { return new RmsedNumeral(Double.NaN, Double.NaN); } }

        /// <summary>
        /// 表
        /// </summary>
        public const string TabPlaceHolder = " \t ";

        /// <summary>
        /// 是否有效
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool IsValid(RmsedNumeral val)
        {
            return Gdp.Utils.DoubleUtil.IsValid(val.Value) && Gdp.Utils.DoubleUtil.IsValid(val.Rms);
        }

        /// <summary>
        /// 是否为 0 或则无效
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool IsZeroOrNotValid(RmsedNumeral val)
        {
            return IsZero(val) || !IsValid(val);
        }
        /// <summary>
        /// 是否为 0
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool IsZero(RmsedNumeral val)
        {
            return val.Rms == 0 && val.Value == 0;
        }
        /// <summary>
        /// 转换成字符串
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public virtual string ToString(string format)
        {
            return Value.ToString(format) + "(" + Rms.ToString("G5") + ")";
        }
        /// <summary>
        /// 解析字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static RmsedNumeral Parse(string str)
        {
            var strs = str.Split(new char[] { '(' }, StringSplitOptions.RemoveEmptyEntries);
            var val = Double.Parse(strs[0]);
            var rms = Double.Parse(strs[1].Replace(")", ""));
            return new RmsedNumeral(val, rms);
        }
    }
}