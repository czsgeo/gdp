// 2013.03.28.09.11 ,czs, 修正正负号问题。
//2017.07.07, czs, edit in hongiqng, 修正构造函数。

using System;
using System.Collections.Generic;
using System.Text;

namespace Gdp
{
    /// <summary>
    /// 以度分秒表示的角度。
    /// </summary>
    public class DMS
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dval"></param>
        /// <param name="degreeFormat"></param>
        public DMS(double dval, AngleUnit degreeFormat)
        { 
            switch (degreeFormat)
            {
                case AngleUnit.Radian:
                    InitDegree(AngularConvert.RadToDeg(dval));
                    break;
                case AngleUnit.Degree:
                    InitDegree(dval);
                    break;
                case AngleUnit.DMS_S:
                    {
                        this.IsPlus = dval > 0;
                        double d = Math.Abs(dval);
                        var str = d.ToString("0.00000000000");
                        var indexOfPt = str.IndexOf(".");

                        if (indexOfPt > 4)
                        {
                            var degr = str.Substring(0, indexOfPt - 4);
                            this.Degree = int.Parse(degr);
                        }
                        if (indexOfPt > 2)
                        {
                            var minStr = str.Substring(indexOfPt-4, 2);
                            this.Minute = int.Parse(minStr);
                        } 
                        var secStr = str.Substring(indexOfPt - 2);
                        this.Second = Double.Parse(secStr); 
                    }
                    break;
                case AngleUnit.D_MS://120.30300
                    {
                        this.IsPlus = dval > 0;
                        double d = Math.Abs(dval);
                        var str = d.ToString("0.00000000000");
                        var indexOfPt = str.IndexOf(".");
                        var degr = str.Substring(0, indexOfPt);
                        this.Degree = int.Parse(degr);
                        this.Minute = int.Parse(str.Substring(indexOfPt + 1, 2));
                        var secStr =str.Substring(indexOfPt + 3, 2)+  "." + str.Substring(indexOfPt+5);
                        this.Second = Double.Parse(secStr);
                    }
                    break;
                case AngleUnit.HMS_S:
                    {
                        this.IsPlus = dval > 0;
                        double d = Math.Abs(dval);
                        var str = d.ToString("0.00000000000");
                        var indexOfPt = str.IndexOf(".");

                        if (indexOfPt > 4)
                        {
                            var degr = str.Substring(0, indexOfPt - 4);
                            this.Degree = int.Parse(degr) * 15;
                        }
                        if (indexOfPt > 2)
                        {
                            var minStr = str.Substring(indexOfPt - 4, 2);
                            this.Minute = int.Parse(minStr);
                        }
                        var secStr = str.Substring(indexOfPt - 2);
                        this.Second = Double.Parse(secStr);
                    }
                    //this.IsPlus = dval > 0;
                    //double deg0 = Math.Abs(dval);
                    //var hour = (int)(deg0 / 10000);
                    //var minute = (int)((deg0) / 100.0) - Degree * 100;
                    //var second = deg0 % 100;
                    //var hours = hour + minute / 60.0 + second / 3600.0;

                    //var degs = hours * 15.0;
                    //InitDegree(degs);
                    break;
                default:
                    throw new ArgumentException("暂不支持这种类型 " + degreeFormat);
            }
        }

        /// <summary>
        /// 以小数度初始化。
        /// </summary>
        /// <param name="deg"></param>
        public DMS(double deg, bool isSeconds = false)
        { 
            if (isSeconds)
            {
                deg = deg /3600.0;
            }
            InitDegree(deg);
        }

        private void InitDegree(double deg)
        {
            this.IsPlus = deg >= 0;

            deg = Math.Abs(deg);
            this.Degree = (int)deg;
            this.Minute = (int)((deg - Degree) * 60.0);
            this.Second = (deg - Degree - Minute / 60.0) * 3600.0;
        }

        /// <summary>
        /// 以度分秒和正负号初始化。
        /// </summary>
        /// <param name="deg"></param>
        /// <param name="second"></param>
        /// <param name="second"></param>
        /// <param name="isPlus"></param>
        public DMS(int deg, int minute, double second, bool isPlus)
        {
            IsPlus = isPlus; 
            this.Degree = Math.Abs(deg);
            this.Minute = Math.Abs(minute);
            this.Second = Math.Abs(second);
        }

        /// <summary>
        /// 无符号度。
        /// </summary>
        public int Degree { get; set; }
        /// <summary>
        /// 无符号 分。
        /// </summary>
        public int Minute { get; set; }
        /// <summary>
        /// 无符号 秒。
        /// </summary>
        public double Second { get; set; }
        /// <summary>
        /// 是正是负号，由这个管理。其它的数字都应该化为正数来计算。
        /// </summary>
        public bool IsPlus { get; set; }
        /// <summary>
        /// 度小数。
        /// </summary>
        public double Degrees
        {
            get
            {
                var deg = Degree + Minute / 60.0 + Second / 3600.0;
                if (IsPlus) return deg;
                return -1.0 * deg; 
            }
        }

        /// <summary>
        /// 全部转化为秒
        /// </summary>
        public double Seconds
        {
            get
            {
                double seconds = Degree * 3600 + Minute * 60 + Second;
                if (IsPlus) return seconds;
                return -1.0 * seconds; 
            }
        }

        public override bool Equals(object obj)
        {
            DMS o = obj as DMS;
            if (o == null) return false;

            return IsPlus == o.IsPlus
                && Degree == o.Degree
                && Minute == o.Minute
                && Second == o.Second;
        }
        /// <summary>
        /// 时角转换为度分秒
        /// </summary>
        /// <returns></returns>
        public DMS HourAngleToDms()
        {
            double decimalHour = (this.Degree + Minute / 60.0 + Second / 3600.0) / 15;
            return new DMS(decimalHour * 15);
        }
        /// <summary>
        /// 度分秒转换为度
        /// </summary>
        /// <returns></returns>
        public DMS DmsToHourAngle()
        {
            double decimalDegree = (this.Degree + Minute / 60.0 + Second / 3600.0) / 15;
            return new DMS(decimalDegree / 15);
        }

        public override int GetHashCode()
        {
            return (int)(Seconds * 100000);
        }
        /// <summary>
        /// 如 -1112233.444444
        /// </summary>
        /// <returns></returns>
        public string ToDms_sString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(IsPlus ? "" : "-");
            sb.Append(Degree.ToString("00"));
            sb.Append(Minute.ToString("00"));
            sb.Append(Second.ToString("00.00####"));
            return sb.ToString();
        }
        /// <summary>
        /// 如 -111.2233444444
        /// </summary>
        /// <returns></returns>
        public string ToD_msString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(IsPlus ? "" : "-");
            sb.Append(Degree.ToString("00"));
            sb.Append(".");
            sb.Append(Minute.ToString("00"));
            sb.Append(Second.ToString("00.00000000000").Replace(".", ""));
            return sb.ToString();
        }
        /// <summary>
        /// 时角转换
        /// 如 -112233.444444
        /// </summary>
        /// <returns></returns>
        public string ToHms_sString()
        {
            var degs = this.Degrees;
            var hourDeci = Degrees / 15.0;
            var hour = (int)(hourDeci);
            //var minuteDec = (hourDeci - hour) * 60.0;
            //var minute = (int)minuteDec;
            //var second = (hourDeci - minute / 60.0) * 3600.0;

            StringBuilder sb = new StringBuilder();
            sb.Append(IsPlus ? "" : "-");
            sb.Append(hour.ToString("00"));
            sb.Append(Minute.ToString("00"));
            sb.Append(Second.ToString("00.00####"));
            return sb.ToString();
        }
        /// <summary>
        /// 40°07'58.00000″
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToReadableDms();
        }
        /// <summary>
        ///  40°07'58.00000″
        /// </summary>
        /// <returns></returns>
        public string ToReadableDms()
        {
            string mark = "";
            if (!IsPlus) mark = "-";
            return mark + Degree + "°" + Minute.ToString("00") + "'" + Second.ToString("00.000000") + "˝";
        }
        /// <summary>
        /// 简明字符串 07'58"
        /// </summary>
        /// <returns></returns>
        public string ToConciseString()
        {
            StringBuilder sb = new StringBuilder();
            //40°07'58  ˚
            string mark = "";
            if (!IsPlus) mark = "-";
            sb.Append(mark);
            if (
                    Degree != 0
                || (Minute == 0)
                )
            {
                sb.Append(Degree + "°");
            }

            if (Minute != 0)
            {
                sb.Append(Minute.ToString("00") + "'");
                if (Second != 0)
                    sb.Append(Second.ToString("00.######") + "˝");
            }
            return sb.ToString();
        }

        public string ToHourAngleString()
        {
            string mark = "";
            if (!IsPlus) mark = "-";
            return mark + Degree + "h" + Minute.ToString("00") + "min" + Second.ToString("00.000000") + "s";
            
        }
        /// <summary>
        /// 解析度分秒
        /// </summary>
        /// <param name="val"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static DMS Parse(string val   , AngleUnit unit)
        {
            //if(unit == AngleUnit.DMS_S)
            //{
            //    return Parse(val);
            //}
            var v = Gdp.Utils.StringUtil.ParseDouble(val);
            return new DMS(v, unit);  
        }

        /// <summary>
        /// 解析度分秒，可以用符号或空格间隔.必须以度分秒的顺序排列。
        /// 
        /// </summary>
        /// <param name="d_m_s"></param>
        /// <returns></returns>
        public static DMS Parse(string d_m_s)
        {
            if (String.IsNullOrWhiteSpace(d_m_s)) { return new DMS(0); }
            d_m_s = d_m_s.Replace("min", "m");

            string[] strs = d_m_s.Split(new char[] { 'h', 'm', 's', '°', '′', '″', ',', ' ', '°', '\'', '˝', '\t' }, StringSplitOptions.RemoveEmptyEntries);
          
            int deg = int.Parse(strs[0]);
            int min = int.Parse(strs[1]);
            double sec = double.Parse(strs[2]);
            bool IsPlus = true; 
            if (deg == 0)
            {
                if (min == 0) IsPlus = sec > 0;
                else IsPlus = min > 0;
            }
            else IsPlus = deg > 0;
            return new DMS(deg, min, sec, IsPlus); 
        }
        /// <summary>
        /// 度分秒.秒
        /// </summary>
        public double Dms_s
        {
            get
            {
                return Double.Parse(ToDms_sString());
            }
        }
        /// <summary>
        /// 度.分秒
        /// </summary>
        public double D_ms
        {
            get
            {
                return Double.Parse(ToD_msString());
            }
        }
        /// <summary>
        /// 角度化为时角
        /// </summary>
        public double Hms_s
        {
            get
            {
                return Double.Parse(ToHms_sString());
            }
        }
        /// <summary>
        /// 转换为度分秒小数
        /// </summary>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static double DegreeToDms_s(double degree)
        {
            return new DMS(degree).Dms_s;
        }
        /// <summary>
        /// 转换为度小数
        /// </summary>
        /// <param name="dms_s"></param>
        /// <returns></returns>
        public static double Dms_sToDegree(double dms_s)
        {
            return new DMS(dms_s, AngleUnit.DMS_S).Degrees;
        }

    }
}
