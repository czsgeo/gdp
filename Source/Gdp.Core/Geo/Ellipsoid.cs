//2014.05.24, czs, created 
//2019.01.09, czs, edit in hmx, 增加CGCS2000, WGS84 长半轴，扁率常量

using System;
using System.Collections.Generic;
using System.Text;


namespace Gdp
{

    /// <summary>
    /// 椭球体。
    /// </summary>
    public class Ellipsoid : IdentifiedObject//, IEllipsoid
    {

        #region Constructors
        /// <summary>
        /// 默认构造函数。什么都没有。
        /// </summary>
        public Ellipsoid() { }
        /// <summary>
        /// 椭球体
        /// </summary>
        /// <param name="semiMajorAxis">长半轴</param>
        /// <param name="flatteningOrInverse">扁率或其倒数，根据数据大小程序自动判断</param>
        /// <param name="axisUnit">坐标轴计量单位</param>
        public Ellipsoid(double semiMajorAxis, double flatteningOrInverse, LinearUnit axisUnit = null, string name = null)
        {
            //default meter
            this.AxisUnit = axisUnit == null ? LinearUnit.Metre : axisUnit;
            this.Name = name == null ? "未命名椭球" : name;

            this.SemiMajorAxis = semiMajorAxis;

            if (flatteningOrInverse < 1)//小于 1 则是扁率，大于 1 则是其倒数
            {
                this.Flattening = flatteningOrInverse;
                this.InverseFlattening = 1 / flatteningOrInverse;
            }
            else
            {
                this.Flattening = 1 / flatteningOrInverse;
                this.InverseFlattening = flatteningOrInverse;
            }

            this.SemiMinorAxis = semiMajorAxis * (1 - Flattening);
            this.PolarCurvatureSemiAxis = semiMajorAxis * semiMajorAxis / SemiMinorAxis;
            this.FirstEccentricity = Math.Sqrt(semiMajorAxis * semiMajorAxis - SemiMinorAxis * SemiMinorAxis) / semiMajorAxis;
            this.SecondEccentricity = Math.Sqrt(semiMajorAxis * semiMajorAxis - SemiMinorAxis * SemiMinorAxis) / SemiMinorAxis;
        }

        /// <summary>
        /// 由长半轴和扁率求短半轴
        /// </summary>
        /// <param name="semiMajorAxis"></param>
        /// <param name="flattening"></param>
        /// <returns></returns>
        private static double GetSemiMinorAxis(double semiMajorAxis, double flattening)
        {
            return semiMajorAxis * (1 - flattening);
        }
        /// <summary>
        /// 求扁率。 e = (a-b)/a
        /// </summary>
        /// <param name="semiMajorAxis"></param>
        /// <param name="semiMinorAxis"></param>
        /// <returns></returns>
        private static double GetFlattening(double semiMajorAxis, double semiMinorAxis)
        {
            return (semiMajorAxis - semiMinorAxis) / semiMajorAxis;
        }

        /// <summary>
        /// 返回哈希代码。
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return SemiMajorAxis.GetHashCode() * 13 + SemiMinorAxis.GetHashCode() * 5;
        }

        /// <summary>
        /// 从数值上判断是否相等。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Ellipsoid))
                return false;
            Ellipsoid e = obj as Ellipsoid;
            return (e.InverseFlattening == this.InverseFlattening &&
                    e.SemiMajorAxis == this.SemiMajorAxis &&
                    e.SemiMinorAxis == this.SemiMinorAxis &&
                    e.AxisUnit.Equals(this.AxisUnit));
        }


        #endregion

        #region property

        /// <summary>
        /// 长度单位
        /// </summary>
        public LinearUnit AxisUnit { get; set; }
        /// <summary>
        /// 长半轴
        /// </summary>
        public double SemiMajorAxis { get; set; }
        /// <summary>
        /// 短半轴
        /// </summary>
        public double SemiMinorAxis { get; set; }

        /// <summary>
        /// 扁率的倒数
        /// </summary>
        public double InverseFlattening { get; set; }
        /// <summary>
        /// 扁率
        /// </summary>
        public double Flattening { get; set; }
        /// <summary>
        /// 极曲率半径
        /// </summary>
        public double PolarCurvatureSemiAxis { get; set; }
        /// <summary>
        /// 第一偏心率
        /// </summary>
        public double FirstEccentricity { get; set; }
        /// <summary>
        /// 第二偏心率
        /// </summary>
        public double SecondEccentricity { get; set; }
        /// <summary>
        /// gravitational constant
        /// </summary>
        public double GM { get; set; }
        /// <summary>
        /// earth angular velocity (rad)
        /// </summary>
        public double AngleVelocity { get; set; }
        /// <summary>
        /// 地球平均半径  6371000
        /// </summary>
        public const double MeanRaduis = 6371000;
        #endregion
        /// <summary>
        /// 字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

        #region 常用椭球
        /// <summary>
        /// WGS84 椭球 长半轴 6378137
        /// </summary>
        public const double SemiMajorAxisOfWGS84 = 6378137;
        /// <summary>
        ///  WGS84 椭球 扁率倒数 298.257223563
        /// </summary>
        public const double InverseFlatOfWGS84 = 298.257223563;
        /// <summary>
        /// CGCS2000 椭球 长半轴 6378137.0
        /// </summary>
        public const double SemiMajorAxisOfCGCS2000 = 6378137.0;
        /// <summary>
        /// CGCS2000 椭球 扁率倒数 298.257222101
        /// </summary>
        public const double InverseFlatOfCGCS2000 = 298.257222101;
        /// <summary>
        /// CGCS 2000，中国2000国家大地坐标系采用的椭球。
        /// 以ITRF97参考框架为基准，参考历元为2000.
        /// </summary>
        public static Ellipsoid CGCS2000 => new Ellipsoid(6378137.0, 298.257222101)
                {
                    Abbreviation = "CGCS2000",
                    Name = "2000中国大地坐标系",
                    Id = "",
                    GM = 3.986004418E14,
                    AngleVelocity = 7.292115E-5

                };
        /// <summary>
        /// Glonass
        /// </summary>
        public static Ellipsoid PZ90 => new Ellipsoid(6378136.0, 298.257)
                {
                    Abbreviation = "PZ90",
                    Name = "PZ90",
                    Id = "",
                    GM = 3.9860044E14,
                    AngleVelocity = 7.292115E-5
                };
        /// <summary>
        /// 北京54坐标系
        /// </summary>
        public static Ellipsoid BeiJing54 => new Ellipsoid(6378245.0, 298.3)
                {
                    Abbreviation = "BJ54",
                    Name = "北京54椭球"
                };
        /// <summary>
        /// 西安 80 坐标系
        /// </summary>
        public static Ellipsoid XiAn80 => new Ellipsoid(6378140.0, 298.257)
                {
                    Abbreviation = "XiAn80",
                    Name = "西安80椭球"
                };

        /// <summary>
        /// WGS 84 ellipsoid
        /// </summary>
        /// <remarks>
        /// Inverse flattening derived from four defining parameters 
        /// (semi-major axis;
        /// C20 = -484.16685*10e-6;
        /// earth's angular velocity w = 7292115e11 rad/sec;
        /// gravitational constant GM = 3986005e8 m*m*m/s/s).
        /// </remarks>
        public static Ellipsoid WGS84 => new Ellipsoid(6378137, 298.257223563)
                {
                    Id = "7030",
                    Name = "WGS 84",
                    Abbreviation = "WGS84",
                    GM = 3.9860050E14,
                    AngleVelocity = 7.2921151467E-5
                };
        /// <summary>
        /// WGS 72 Ellipsoid
        /// </summary>
        public static Ellipsoid WGS72 => new Ellipsoid(6378135.0, 298.26)
                {
                    Id = "7043",
                    Name = "WGS 72",
                    Abbreviation = "WGS72"
                };


        /// <summary>
        /// GRS 1980 / International 1979 ellipsoid
        /// </summary>
        /// <remarks>
        /// Adopted by IUGG 1979 Canberra.
        /// Inverse flattening is derived from
        /// geocentric gravitational constant GM = 3986005e8 m*m*m/s/s;
        /// dynamic form factor J2 = 108263e8 and Earth's angular velocity = 7292115e-11 rad/s.")
        /// </remarks>
        public static Ellipsoid GRS80 => new Ellipsoid(6378137, 298.257222101)
                {
                    Id = "7019",
                    Name = "International 19792",
                    Abbreviation = "GRS 1980"
                };

        /// <summary>
        /// International 1924 / Hayford 1909 ellipsoid
        /// </summary>
        /// <remarks>
        /// Described as a=6378388 m. and b=6356909m. from which 1/f derived to be 296.95926. 
        /// The figure was adopted as the International ellipsoid in 1924 but with 1/f taken as
        /// 297 exactly from which b is derived as 6356911.946m.
        /// </remarks>
        public static Ellipsoid International1924 => new Ellipsoid(6378388, 297)
                {
                    Id = "7022",
                    Name = "International 1924",
                    Abbreviation = "Hayford 1909"
                };

        /// <summary>
        /// Clarke 1880
        /// </summary>
        /// <remarks>
        /// Clarke gave a and b and also 1/f=293.465 (to 3 decimal places).  1/f derived from a and b = 293.4663077
        /// </remarks>
        public static Ellipsoid Clarke1880 => new Ellipsoid(20926202, 297)
                {
                    Id = "7034",
                    Name = "Clarke 1880",
                    Abbreviation = "Clarke 1880",
                    AxisUnit = LinearUnit.ClarkesFoot
                };
        /// <summary>
        /// Clarke 1866
        /// </summary>
        /// <remarks>
        /// Original definition a=20926062 and b=20855121 (British) feet. Uses Clarke's 1865 inch-metre ratio of 39.370432 to obtain metres. (Metric value then converted to US survey feet for use in the United States using 39.37 exactly giving a=20925832.16 ft US).
        /// </remarks>
        public static Ellipsoid Clarke1866 => new Ellipsoid(6378206.4, 1.0 / GetFlattening(6378206.4, 6356583.8))
                 {
                     Id = "7008",
                     Name = "Clarke 1866",
                     Abbreviation = "Clarke 1866",
                     AxisUnit = LinearUnit.Metre
                 };

        /// <summary>
        /// Sphere
        /// </summary>
        /// <remarks>
        /// Authalic sphere derived from GRS 1980 ellipsoid (code 7019).  (An authalic sphere is
        /// one with a surface area equal to the surface area of the ellipsoid). 1/f is infinite.
        /// </remarks>
        public static Ellipsoid Sphere => new Ellipsoid(6370997.0, double.PositiveInfinity)
                {
                    Id = "7048",
                    Name = "GRS 1980 Authalic Sphere",
                    Abbreviation = "Sphere",
                    AxisUnit = LinearUnit.Metre
                };
        #endregion

        /// <summary>
        /// 根据类型，获取指定的椭球。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Ellipsoid GetEllipsoid(EllipsoidType type)
        {

            switch (type)
            {
                case EllipsoidType.CGCS2000:
                    return Ellipsoid.CGCS2000;
                case EllipsoidType.BJ54:
                    return Ellipsoid.BeiJing54;
                case EllipsoidType.WGS84:
                    return Ellipsoid.WGS84;
                case EllipsoidType.PZ90:
                    return Ellipsoid.PZ90;
                case EllipsoidType.XA80:
                    return Ellipsoid.XiAn80;
                default:
                    return Ellipsoid.WGS84;
            }
        }
        /// <summary>
        /// 根据类型，获取指定的椭球。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<Ellipsoid> GetEllipsoids()
        {
            var els = new List<Ellipsoid>();

            els.Add(Ellipsoid.CGCS2000);
            els.Add(Ellipsoid.WGS84);
            els.Add(Ellipsoid.XiAn80);
            els.Add(Ellipsoid.PZ90);
            els.Add(Ellipsoid.BeiJing54);
            return els;
        }

    }
}