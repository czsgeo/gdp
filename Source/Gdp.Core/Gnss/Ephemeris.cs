//2015.03.19, cy, edit, sp3c格式读取文件，除了卫星坐标，还有坐标的sdev需要读取
//2017.05.16, double add in zhengzhou, 增加了GetSimpleTabValues()
//2017.08.06, czs, refact in hongiqng, 合并RelativeCorrection与RelativeTime为一个变量。

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gdp.IO;

namespace Gdp
{
    /// <summary>
    /// 卫星位置和钟差信息。用户最后需要得到的产品。
    /// </summary>
    public class Ephemeris : SatClockBias
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Ephemeris() { XYZ = new XYZ(); XyzDot = new XYZ(); }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="prn"></param>
        /// <param name="time"></param>
        public Ephemeris(SatelliteNumber prn, Time time) : this()
        {
            this.Prn = prn;
            this.Time = time;
        }

        /// <summary>
        /// 距离地心距离
        /// </summary>
        public double Radius => XYZ.Length;

        /// <summary>
        /// 单位米。
        /// </summary>
        public XYZ XYZ { get; set; }

        /// <summary>
        /// (decim/sec)   可能为 null,卫星速度
        /// </summary>
        public XYZ XyzDot { get; set; }

        /// <summary>
        /// 精度信息，不指定则为null。 SP3文件中卫星坐标的精度估值sdev,单位是米
        /// 单位米。文件中文毫米，读取时单位转换成了米。
        /// </summary>
        public XYZ Rms { get; set; }

        /// <summary>
        /// SP3文件中卫星速度的精度估值sdev,单位是米
        /// 单位米。文件中文毫米，读取时单位转换成了米。
        /// </summary>
        public XYZ XyzDotRms { get; set; }


        /// <summary>
        /// 由于卫星相对运动，钟差的相对论改正，单位为秒。相对论时间？？？
        /// </summary>
        public double RelativeCorrection { get; set; }

        /// <summary>
        /// 卫星大地坐标。以XYZ为尊，自动设置本坐标。
        /// </summary>
        public GeoCoord GeoCoord { get { if (!XYZ.IsZero) return CoordTransformer.XyzToGeoCoord(XYZ); return null; } }

        /// <summary>
        /// 用于比较排序。
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Ephemeris other)
        {
            return (int)this.Time.CompareTo(other.Time);
        }

        /// <summary>
        /// 星历相加
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Ephemeris operator +(Ephemeris first, Ephemeris second)
        {
            if (first.Prn != second.Prn || first.Time != second.Time)
            {
                new Log(typeof(Ephemeris)).Error("不是同一卫星，不可相加，" + first + ", " + second);
                return first;
            }
            Ephemeris ephemeris = new Ephemeris(first.Prn, first.Time);
            ephemeris.XYZ = first.XYZ + second.XYZ;
            //if (first.Rms != null && second.Rms != null)
            //{
            //    ephemeris.Rms = XYZ.RmsPlus(first.Rms, second.Rms);
            //}
            ephemeris.XyzDot = first.XyzDot + second.XyzDot;
            ephemeris.ClockBias = first.ClockBias + second.ClockBias;
            ephemeris.ClockBiasRms = Utils.DoubleUtil.RmsPlus(first.ClockBiasRms, first.ClockBiasRms);
            ephemeris.ClockDrift = first.ClockDrift + second.ClockDrift;

            return ephemeris;
        }
        /// <summary>
        /// 星历相减
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Ephemeris operator -(Ephemeris first, Ephemeris second)
        {
            if (first.Prn != second.Prn || first.Time != second.Time)
            {
                new Log(typeof(Ephemeris)).Error("不是同一卫星，不可相加，" + first + ", " + second);
                return first;
            }
            Ephemeris ephemeris = new Ephemeris(first.Prn, first.Time);
            ephemeris.XYZ = first.XYZ - second.XYZ;
            if (first.Rms != null && second.Rms != null)
            {
             //   ephemeris.Rms = XYZ.RmsPlus(first.Rms, second.Rms);
            }
            ephemeris.XyzDot = first.XyzDot - second.XyzDot;
            ephemeris.ClockBias = first.ClockBias - second.ClockBias;
            ephemeris.ClockBiasRms = Utils.DoubleUtil.RmsPlus(first.ClockBiasRms, first.ClockBiasRms);
            ephemeris.ClockDrift = first.ClockDrift - second.ClockDrift;

            return ephemeris;
        }

        #region 输出
        /// <summary>
        /// 显示
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Prn + ", " + Time + ", " + XYZ;
        }

        
        public string GetSimpleTabValues()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Prn);
            sb.Append("\t");
            sb.Append(Time);

            sb.Append("\t");
            if (ClockBias == 0) { sb.Append(" "); }
            else { sb.Append(ClockBias.ToString("g")); }

            sb.Append("\t");
            if (ClockDrift == 0) { sb.Append(" "); }
            else { sb.Append(ClockDrift.ToString("g")); }



            return sb.ToString();
        }
        #endregion
    }
}
