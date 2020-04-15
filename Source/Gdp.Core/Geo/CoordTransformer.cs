//2012.09.23, czs, 修改, 1.所有角度增加 AngelUnit 参数选项，默认为 度。2.增加 XyzToGeoCoord2 系列函数。

using System;
using System.Collections.Generic;
using System.Text; 
using Gdp.Utils;

namespace Gdp
{
    /// <summary>
    /// 提供便捷的坐标转换方法。 
    /// </summary>
    public static class CoordTransformer
    {    
        /// <summary>
        ///  计算卫星的站心极坐标，基于当前测站的大地坐标，即高度角基于椭球表面，当前经度为0时，请直接调用 lon lat
        /// </summary>
        /// <param name="satXyz">卫星的地心空间直角坐标</param>
        /// <param name="stationPosition">测站的地心空间直角坐标</param>
        /// <param name="unit">角度单位,表示输出的角度单位</param>
        /// <returns></returns>
        public static Polar XyzToGeoPolar(XYZ satXyz, XYZ stationPosition, AngleUnit unit = AngleUnit.Degree)
        {
            var geoCoord = XyzToGeoCoord(stationPosition);          
            return XyzToPolar(satXyz, stationPosition, geoCoord.Lon, geoCoord.Lat, unit);
        } 

        /// <summary>
        /// 计算卫星的站心极坐标,指定了经纬度，更加精确，避免重复计算。
        /// </summary>
        /// <param name="satXyz"></param>
        /// <param name="stationPosition"></param>
        /// <param name="unit">即表示输入角度的单位，也要求输出角度的单位</param>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <returns></returns>
        public static Polar XyzToPolar(XYZ satXyz, XYZ stationPosition, double lon, double lat, AngleUnit unit = AngleUnit.Degree)
        {
            XYZ deltaXyz = satXyz - stationPosition;//地面到卫星的向径。

            lon = Math.Abs(lon) < 1E-10 ? 0  : lon;

            NEU neu = XyzToNeu(deltaXyz, lat, lon, unit);
            return NeuToPolar(neu, unit);
        }
        

        /// <summary>
        ///  空间直角坐标系到，站心坐标系的转换。默认B L 单位 度，可以为球坐标和大地坐标
        ///  地心空间直角坐标系(XYZ)转换为地方左手笛卡尔直角坐标系（NEU,XYZ）
        /// </summary>
        /// <param name="vector1">测站到卫星的向径</param>
        /// <param name="lat">站点所在纬度</param>
        /// <param name="lon">站点所在经度</param>
        /// <param name="angleUnit">站点所在经度的单位</param>
        /// <returns></returns>
        public static NEU XyzToNeu(XYZ vector1, double lat, double lon, AngleUnit angleUnit = AngleUnit.Degree)
        {
            if (angleUnit != AngleUnit.Radian) //当前经度为 0 的时候，U 的转换会出现符号错误？？？！！！2017.10.12.
            {
                lat = AngularConvert.ToRad(lat, angleUnit);
                lon = AngularConvert.ToRad(lon, angleUnit);
            }
            
            XYZ v = vector1;

            double n = -v.X * Sin(lat) * Cos(lon) - v.Y * Sin(lat) * Sin(lon) + v.Z * Cos(lat);
            double e = -v.X * Sin(lon) + v.Y * Cos(lon);
            double u = v.X * Cos(lat) * Cos(lon) + v.Y * Cos(lat) * Sin(lon) + v.Z * Sin(lat);

            return new NEU(n, e, u);// { N = n, E = e, U = u };
        }
         
        /// <summary>
        /// 站心坐标系到站心极坐标系。
        /// </summary>
        /// <param name="neu"></param>
        /// <param name="unit">默认单位为度</param>
        /// <returns></returns>
        public static Polar NeuToPolar(NEU neu, AngleUnit unit = AngleUnit.Degree)
        {
            double r = neu.Length;
            double a = Math.Atan2(neu.E, neu.N);
            if (a < 0)//以北向为基准，顺时针，无负号
            {
                a += 2.0 * CoordConsts.PI;
            }

            double o = Math.Asin(neu.U / r);
            if (unit != AngleUnit.Radian)
            {
                a = AngularConvert.RadTo(a, unit);
                o = AngularConvert.RadTo(o, unit);
            }
            return new Polar(r, a, o) { Unit = unit };
        }
     

        #region 大地坐标和空间直角坐标之间的转换

        /// <summary>
        /// 大地坐标转为空间直角坐标。
        /// </summary>
        /// <param name="ellipsoidCoord"></param>
        /// <returns></returns>
        public static XYZ GeoCoordToXyz(GeoCoord ellipsoidCoord, Ellipsoid el = null)
        {
            if (el == null) el = Ellipsoid.WGS84;

            double lon = ellipsoidCoord.Lon;
            double lat = ellipsoidCoord.Lat;
            double height = ellipsoidCoord.Height;
            double a = el.SemiMajorAxis;
            double e = el.FirstEccentricity;

            return GeoCoordToXyz(lon, lat, height, a, e);
        } 

        /// <summary>
        /// 椭球坐标转为空间直角坐标。默认单位为度。
        /// </summary>
        /// <param name="lon">经度（度）</param>
        /// <param name="lat">纬度（度）</param>
        /// <param name="height">大地高</param>
        /// <param name="a">椭球半径</param>
        /// <param name="flatOrInverse">扁率或其倒数</param>
        /// <param name="unit">单位</param>
        /// <returns></returns>
        public static XYZ GeoCoordToXyz(double lon, double lat, double height, double a, double flatOrInverse, AngleUnit unit = AngleUnit.Degree)
        { 
            lon = AngularConvert.ToRad(lon, unit);
            lat = AngularConvert.ToRad(lat, unit);

            //扁率判断
            double e = flatOrInverse;
            if (flatOrInverse > 1)
            {
                e = 1.0 / e;
            } 

            double n = a / Math.Sqrt(1 - Math.Pow(e * Sin(lat), 2));

            double x = (n + height) * Cos(lat) * Cos(lon);
            double y = (n + height) * Cos(lat) * Sin(lon);
            double z = (n * (1 - e * e) + height) * Sin(lat);
            return new XYZ(x, y, z);
        }

        /// <summary>
        /// 由空间直角坐标转换为椭球坐标。默认角度单位为度。
        /// </summary>
        /// <param name="xyz"></param>
        /// <param name="ellipsoid"></param>
        /// <param name="angeUnit"></param>
        /// <returns></returns>
        public static GeoCoord XyzToGeoCoord(IXYZ xyz, Ellipsoid ellipsoid, AngleUnit angeUnit = AngleUnit.Degree)
        {
            double x = xyz.X;
            double y = xyz.Y;
            double z = xyz.Z;

            double a = ellipsoid.SemiMajorAxis;
            double e = ellipsoid.FirstEccentricity;
            return XyzToGeoCoord(x, y, z, a, e, angeUnit);
        }

        /// <summary>
        /// 由空间直角坐标转换为大地坐标，参考椭球为WGS84。
        /// 默认单位为度。
        /// </summary>
        /// <param name="xyz">空间直角坐标</param>
        /// <param name="angeUnit">角度单位，默认单位为度。</param>
        /// <returns></returns>

        public static GeoCoord XyzToGeoCoord(IXYZ xyz, AngleUnit angeUnit = AngleUnit.Degree) { return XyzToGeoCoord(xyz, Ellipsoid.WGS84, angeUnit); }

        /// <summary>
        /// 由空间直角坐标转换为椭球坐标。默认角度单位为度。
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="a"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public static GeoCoord XyzToGeoCoord(double x, double y, double z, double a, double e, AngleUnit angeUnit = AngleUnit.Degree)
        {
            double ee = e * e;
            double lon = Math.Atan2(y, x);
            double lat;
            double height;

            //iteration
            double deltaZ = 0;
            double sqrtXy = Math.Sqrt(x * x + y * y);
            double tempLat = Math.Atan2(z, sqrtXy);//初始取值
            lat = tempLat;
            //if (Math.Abs(lat) > 80.0 * CoordConverter.DegToRadMultiplier)//lat=+-90,或height特大时，采用此算法更稳定（实际有待实验，保留both代码）
            //{
            int count = 0;//避免死循环
            do
            {
                var sinLat = Sin(lat);
                deltaZ = a * ee * sinLat / Math.Sqrt(1 - Math.Pow(e * sinLat, 2));
                tempLat = lat;
                lat = Math.Atan2(z + deltaZ, sqrtXy);
                count++;
            } while (Math.Abs(lat - tempLat) > 1E-12 || count < 20);
            //}
            //else//经典算法
            //{
            //    do
            //    {
            //        double tanB = Math.Tan(lat);
            //        tempLat = lat;
            //        lat = Math.Atan2(z + a * ee * tanB / Math.Sqrt(1 + tanB * tanB * (1 - ee)), sqrtXy);
            //    } while (Math.Abs(lat - tempLat) > 1E-12);
            //}

            double n = a / Math.Sqrt(1 - Math.Pow(e * Sin(tempLat), 2));
            //double   height = Math.Sqrt(x * x + y * y + Math.Pow((z + deltaZ), 2)) - n;

            height = sqrtXy / Cos(lat) - n;

            //地心纬度
            //double dixinLat = (1 - ee * n / (n + height)) * Math.Tan(lat);
            //double dixinLatDeg = dixinLat * CoordConverter.RadToDegdMultiplier;


            lon = AngularConvert.RadTo(lon, angeUnit);
            lat = AngularConvert.RadTo(lat, angeUnit);

            return new GeoCoord(lon, lat, height);
        }
        #endregion
          
        #region 工具
        /// <summary>
        ///  Math.Cos(currentVal)
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private static double Cos(double val) { return Math.Cos(val); }
        /// <summary>
        ///  Math.Sin(currentVal)
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private static double Sin(double val) { return Math.Sin(val); }
        /// <summary>
        /// Math.Tan(currentVal)
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private static double Tan(double val) { return Math.Tan(val); }
        #endregion
    }
}
