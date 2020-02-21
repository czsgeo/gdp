//2015.04.15, czs, edit in namu, 增加注释

using System;
using System.Collections.Generic;
using System.Text;

namespace Gdp
{
    /// <summary>
    /// 角度转换
    /// </summary>
    public static class AngularConvert
    {
        /// <summary>
        /// 度到弧度乘法因子
        /// </summary>
        public const double DegToRadMultiplier = 0.017453292519943295769236907684886;
        /// <summary>
        /// 弧度到度乘法因子
        /// </summary>
        public const double RadToDegMultiplier = 57.295779513082320876798154814105;

        /// <summary>
        /// 通过赋值大小，自动判断角度取值。注意如果角度小于6分，则可能出错。
        /// </summary>
        /// <param name="degreeOrDmg_s"></param>
        /// <returns></returns>
        public static double GetDegree(double degreeOrDms_s)
        {
            double lon = degreeOrDms_s;
            if (degreeOrDms_s > 360 || degreeOrDms_s < -360)
            {
                lon = new DMS(lon, AngleUnit.DMS_S).Degrees;//AngularConvert.Dms_sToDeg(lon);
            }
            return lon;
        }

        /// <summary>
        /// 角度转换
        /// </summary>
        /// <param name="val">待转换值</param>
        /// <param name="from">从格式</param>
        /// <param name="to">到格式</param>
        /// <returns></returns>
        public static double ConvertDegree(double val, AngleUnit from, AngleUnit to = AngleUnit.Degree)
        {
            if (from == to) return val;
            if (from == AngleUnit.DMS_S && to == AngleUnit.Degree)
            {
                return D_msToDeg(val);
            }
            if (from == AngleUnit.Degree && to == AngleUnit.DMS_S)
            {
                return DegToDms_s(val);
            }
            throw new NotImplementedException("请更新算法！"); 
        }
        /// <summary>
        /// 度小数到度分秒
        /// </summary>
        /// <param name="deg"></param>
        /// <returns></returns>
        public static double DegToDms_s(double deg)
        {
            DMS dms = new DMS(deg);
            return dms.Dms_s;
        }
        /// <summary>
        /// 度小数到度.分秒
        /// </summary>
        /// <param name="deg"></param>
        /// <returns></returns>
        public static double DegToD_ms(double deg)
        {
            DMS dms = new DMS(deg);
            return dms.D_ms;
        }

        /// <summary>
        /// 度转换为弧度
        /// </summary>
        /// <param name="deg"></param>
        /// <returns></returns>
        public static double DegToRad(double deg)
        {
            return deg * DegToRadMultiplier;
        }

        /// <summary>
        /// 弧度转换为度.
        /// </summary>
        /// <param name="rad"></param>
        /// <returns></returns>
        public static double RadToDeg(double rad)
        {
            return rad * RadToDegMultiplier;
        }

        /// <summary>
        /// 度分秒转换为度。1093000
        /// </summary>
        /// <param name="dms_s"></param>
        /// <returns></returns>
        public static double Dms_sToDeg(double dms_s)
        {
            return new DMS(dms_s, AngleUnit.DMS_S).Degrees;
            //double deg = (int)(dms_s / 10000);
            //double min = (int)((dms_s % 10000) / 100);//120.3030
            //double sec = dms_s % 100;
            //double lonDeg = deg + min / 60.0 + sec / 3600.0;
            //return lonDeg;
        }
        /// <summary>
        /// 度分秒转换为度。1093000
        /// </summary>
        /// <param name="hms_s"></param>
        /// <returns></returns>
        public static double Hms_sToDeg(double hms_s)
        {
            return new DMS(hms_s, AngleUnit.HMS_S).Degrees; 
        }
        /// <summary>
        /// 度分秒转换为度。
        /// </summary>
        /// <param name="d_ms"></param>
        /// <returns></returns>
        public static double D_msToDeg(double d_ms)
        {
            DMS dms = new DMS(d_ms, AngleUnit.D_MS);
            return dms.Degrees;

            //double deg = (int)(d_ms);
            //double min = (int)((d_ms * 100) % 100);//120.3030
            //double sec = (d_ms * 10000) % 100;
            //double lonDeg = deg + min / 60.0 + sec / 3600.0;
            //return lonDeg;
        }

        /// <summary>
        /// 角度转换
        /// </summary>
        /// <param name="val">值</param>
        /// <param name="from">原单位</param>
        /// <param name="to">目标单位</param>
        /// <returns></returns>
        public static double Convert(double val, AngleUnit from, AngleUnit to)
        {
            if (from == to) { return val; }
            switch (from)
            {
                case AngleUnit.Degree:
                    switch (to)
                    {
                        case AngleUnit.Degree: return (val);
                        case AngleUnit.Radian: return DegToRad(val);
                        case AngleUnit.HMS_S: return new DMS(val, AngleUnit.Degree).Hms_s;
                        case AngleUnit.DMS_S: return DegToDms_s(val);
                        case AngleUnit.D_MS: return DegToD_ms(val);
                        default: throw new ArgumentException("请指名角度单位");
                    }
                case AngleUnit.Radian:
                    switch (to)
                    {
                        case AngleUnit.Degree: return RadToDeg(val);
                        case AngleUnit.Radian: return (val);
                        case AngleUnit.HMS_S: return new DMS(val, AngleUnit.Radian).Hms_s;
                        case AngleUnit.DMS_S: return DegToDms_s(RadToDeg(val));
                        case AngleUnit.D_MS: return DegToD_ms(RadToDeg(val));
                        default: throw new ArgumentException("请指名角度单位");
                    } 
                case AngleUnit.DMS_S:
                    switch (to)
                    {
                        case AngleUnit.Degree: return Dms_sToDeg(val);
                        case AngleUnit.Radian: return DegToRad(Dms_sToDeg(val));
                        case AngleUnit.HMS_S: return new DMS(val, AngleUnit.DMS_S).Hms_s;
                        case AngleUnit.DMS_S: return ((val));
                        case AngleUnit.D_MS: return DegToD_ms(Dms_sToDeg(val));
                        default: throw new ArgumentException("请指名角度单位");
                    }
                case AngleUnit.HMS_S:
                    switch (to)
                    {
                        case AngleUnit.Degree: return Hms_sToDeg(val);
                        case AngleUnit.Radian: return DegToRad(Hms_sToDeg(val));
                        case AngleUnit.HMS_S: return ((val));
                        case AngleUnit.DMS_S: return (Hms_sToDeg(val));
                        case AngleUnit.D_MS: return DegToD_ms(Hms_sToDeg(val));
                        default: throw new ArgumentException("请指名角度单位");
                    }
                case AngleUnit.D_MS:
                    switch (to)
                    {
                        case AngleUnit.Degree: return D_msToDeg(val);
                        case AngleUnit.Radian: return DegToRad(D_msToDeg(val));
                        case AngleUnit.HMS_S: return new DMS(val, AngleUnit.D_MS).Hms_s;
                        case AngleUnit.DMS_S: return DegToD_ms(D_msToDeg(val));
                        case AngleUnit.D_MS: return ((val));
                        default: throw new ArgumentException("请指名角度单位");
                    }
                default: throw new ArgumentException("请指名角度单位");
            }
        }

        /// <summary>
        /// 转换为弧度
        /// </summary>
        /// <param name="val"></param>
        /// <param name="from"></param>
        /// <returns></returns>
        public static double ToRad(double val, AngleUnit from) { return Convert(val, from, AngleUnit.Radian); }
        /// <summary>
        /// 弧度转换为其它角度单位
        /// </summary>
        /// <param name="val"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static double RadTo(double val, AngleUnit to) { return Convert(val, AngleUnit.Radian,to); }
        /// <summary>
        /// 转换为度小数
        /// </summary>
        /// <param name="val"></param>
        /// <param name="from"></param>
        /// <returns></returns>
        public static double ToDegree(double val, AngleUnit from) { return Convert(val, from, AngleUnit.Degree); }
        /// <summary>
        /// 转换为 DMS_S
        /// </summary>
        /// <param name="val"></param>
        /// <param name="from"></param>
        /// <returns></returns>
        public static double ToDms_s(double val, AngleUnit from) { return Convert(val, from, AngleUnit.DMS_S); }
        public static double GetDms_s(double degreeOrDms_s)
        {
            if (degreeOrDms_s > 360 || degreeOrDms_s < -360) return degreeOrDms_s;

            return new DMS(degreeOrDms_s, AngleUnit.Degree).Dms_s;
        }
    }
}
