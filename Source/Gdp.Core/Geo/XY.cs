using System;
using System.Collections.Generic;
using System.Text;

namespace Gdp
{
    public class XY
    {
        public XY(double x=0, double y=0)
        {
            this.X = x;
            this.Y = y; 
        }

        public static XYZ Zero => new XYZ(0, 0, 0);
        public double X { get; set; }
        public double Y { get; set; } 

        public double Length => Math.Sqrt(X*X+ Y*Y);

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
        public static  XYZ Parse(string toString) { return Parse(toString, new char[] { ',', ';', ' ', '\t', '\n', '\r' }); }

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
            return X + "," + Y ;
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

            return X == xyz.X && Y == xyz.Y;
        }
        /// <summary>
        ///  哈希数
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return X.GetHashCode() + 5 * Y.GetHashCode();
        }
         

    }
}
