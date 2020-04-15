using System;
using System.Collections.Generic;
using System.Text;

namespace Gdp
{
    public class XYZ : IXYZ
    {
        public XYZ(double x = 0, double y = 0, double z = 0)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public static XYZ Zero => new XYZ(0, 0, 0);
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public double Length => Math.Sqrt(X * X + Y * Y + Z * Z);

        public bool IsZero => X == 0 && Y == 0 && Z == 0;

        #region  parse


        /// <summary>
        /// 解析三维浮点数数组
        /// </summary>
        /// <param name="array">三维浮点数数组</param>
        /// <returns></returns>
        public static XYZ Parse(IEnumerable<double> array, int formIndex = 0)
        {
            XYZ xyz = new XYZ();
            int i = formIndex;
            foreach (var item in array)
            {
                if (i == formIndex) { xyz.X = item; }
                if (i == formIndex + 1) { xyz.Y = item; }
                if (i == formIndex + 2) { xyz.Z = item; break; }
                i++;
            }
            return xyz;
        }
        /// <summary>
        /// 解析三维浮点数数组
        /// </summary>
        /// <param name="array">三维浮点数数组</param>
        /// <returns></returns>
        public static XYZ Parse(double[] array) { return new XYZ(array[0], array[1], array[2]); }
        /// <summary>
        /// 解析字符串，可以解析空格、分号、换行符、回车符、Tab为分隔符的字符串
        /// </summary>
        /// <param name="toString"></param>
        /// <returns></returns>
        public static XYZ Parse(string toString) { return Parse(toString, new char[] { ',', ';', ' ', '\t', '\n', '\r' }); }

        /// <summary>
        /// (x,y) (x,y,z) (x y z) x y z
        /// </summary>
        /// <param name="toString"></param>
        /// <param name="separaters"></param>
        /// <returns></returns>
        public static XYZ Parse(string toString, char[] separaters)
        {
            toString = toString.Replace("(", "").Replace(")", "");
            string[] strs = toString.Split(separaters, StringSplitOptions.RemoveEmptyEntries);
            return Parse(strs);
        }
        /// <summary>
        /// 解析字符串数组，支持二维和三维
        /// </summary>
        /// <param name="strs"></param>
        /// <returns></returns>
        public static XYZ Parse(string[] strs)
        {
            double lon = double.Parse(strs[0]);
            double lat = double.Parse(strs[1]);
            double z = 0;
            if (strs.Length > 2) z = double.Parse(strs[2]);
            return new XYZ(lon, lat, z);
        }
        #endregion 

        /// <summary>
        /// 字符串表达
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return X + "," + Y + "," + Z;
        }
        /// <summary>
        /// 数值是否相等
        /// </summary>
        /// <param name="obj">待比较对象</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var xyz = obj as XYZ;
            if (xyz == null) return false;

            return X == xyz.X && Y == xyz.Y && Z == xyz.Z;
        }
        /// <summary>
        ///  哈希数
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return X.GetHashCode() + 5 * Y.GetHashCode() + Z.GetHashCode() * 3;
        }

        #region operator
        /// <summary>
        /// 坐标缩放
        /// </summary>
        /// <param name="first"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static XYZ operator *(XYZ first, double num)
        {
            return new XYZ(first.X * num, first.Y * num, first.Z * num);
        }
        public static XYZ operator *(double num, XYZ first)
        {
            return new XYZ(first.X * num, first.Y * num, first.Z * num);
        }
        public static XYZ operator -(XYZ first)
        {
            return new XYZ(-first.X, -first.Y, -first.Z);
        }
        /// <summary>
        /// 坐标缩放
        /// </summary>
        /// <param name="first"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static XYZ operator /(XYZ first, double num)
        {
            return new XYZ(first.X / num, first.Y / num, first.Z / num);
        }
        /// <summary>
        /// 坐标平移
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static XYZ operator +(XYZ first, XYZ second)
        {
            return new XYZ(first.X + second.X, first.Y + second.Y, first.Z + second.Z);
        }
        /// <summary>
        /// 坐标平移。
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static XYZ operator -(XYZ first, XYZ second)
        {
            return new XYZ(first.X - second.X, first.Y - second.Y, first.Z - second.Z);
        }

        public static bool IsZeroOrEmpty(XYZ approxXyz)
        {
            if (approxXyz == null) return true;
            return approxXyz.IsZero;
        }

        #endregion

    }
}
