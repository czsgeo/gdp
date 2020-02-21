//2014.10.02， czs, create, hailutu tongliao, 创建向量接口，为后继实现自我的、快速的、大规模的矩阵计算做准备
//2014.10.08， czs, edit in hailutu, 将核心变量数组改成了列表，这样可以方便更改维数。
//2017.07.19, czs, edit in hongqing, 合并了 Orbit Vector 部分函数

using System;
using System.Text;
using System.Collections.Generic;

using Gdp.Utils;
using System.Linq;
using System.Linq.Expressions;

namespace Gdp
{

    /// <summary>
    /// Geo 向量，以一维列表形式实现。
    /// 是一串纯粹的数字，没有其它任何物理意义。
    /// 与矩阵相同，向量也有多种存储方式，但是主要还是一维列表比较方便,如改变向量空间的维数。
    /// </summary>
    [Serializable]
    public class Vector : Gdp.AbstractVector, ICloneable//, IReadable
    {
        #region 构造函数
        /// <summary>
        /// 构建一个二维向量
        /// </summary>
        /// <param name="prevObj">第一元素</param>
        /// <param name="second">第二元素</param>
        public Vector(double first, double second) : this(new double[] { first, second }) { }

        /// <summary>
        /// 构建一个三维向量
        /// </summary>
        /// <param name="prevObj">第一元素</param>
        /// <param name="second">第二元素</param>
        /// <param name="third">第3元素</param>
        public Vector(double first, double second, double third) : this(new double[] { first, second, third }) { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dimension">维数</param>
        /// <param name="initVal">初始数据</param>
        public Vector(int dimension, double initVal = 0)
        {
            this.SetDimension(dimension);

            if (initVal != 0)
            {
                for (int i = 0; i < dimension; i++)
                {
                    this.Data[i] = initVal;
                }
            }
        }
        /// <summary>
        /// 矩阵向量
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="isColOrRow">列向量或行向量</param>
        public Vector(Matrix matrix, bool isColOrRow):this()
        {
            if (isColOrRow)
            {
                int length = matrix.RowCount;

                for (int i = 0; i < length; i++)
                {
                    this.Add(matrix[i, 0], matrix.RowNames[i]);
                }
            }
            else
            {

                int length = matrix.ColCount;

                for (int i = 0; i < length; i++)
                {
                    this.Add(matrix[0, i], matrix.ColNames[i]);
                }
            }
        }

        /// <summary>
        /// 构造函数。自动忽略null数据。
        /// </summary>
        /// <param name="dic"></param>
        public Vector(Dictionary<string, Object> dic)
        {
            this.ParamNames = new List<string>();
            this.Data = new List<double>();

            Dictionary<string, double> values = new Dictionary<string, double>();
            foreach (var item in dic)
            {
                if (item.Value == null) { continue; }
                if (item.Value is Double)
                {
                    this.Data.Add((double)item.Value);
                }
                else
                {
                    //Geo.Utils.DoubleUtil.TryParse(key.Value, double.NaN);
                    this.Data.Add(Double.Parse(item.Value.ToString()));
                }
               
                this.ParamNames.Add(item.Key);
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="array"></param> 
        public Vector(double[] array) :this(array, array.Length)
        { 
        }

        /// <summary>
        /// 构造函数，只取N部分
        /// </summary>
        /// <param name="array"></param>
        /// <param name="dimension"></param>
        public Vector(double[] array, int dimension)        // Array copy 
        {
            this.SetDimension(dimension);

            for (int i = 0; i < dimension; i++) { Data[i] = array[i]; this.ParamNames[i] = i + ""; }
        }

        /// <summary>
        /// 6 维向量
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Z"></param>
        public Vector(double x, double y, double z,   // 6dim-Vector
                       double X, double Y, double Z): this(new double[] { x, y, z,X, Y,Z }) { } 

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="vector">一维数组</param>
        public Vector(double[] vector, string [] names = null)
        {
            this.Data = new List<double>(vector);
            if (names == null && this.ParamNames == null)
            { 
                this.ParamNames = new List<string>(vector.Length);
            }
            else
            {
                this.ParamNames = new List<string>(names);
            }
        }
       /// <summary>
       /// 采用字典初始化
       /// </summary>
       /// <param name="values"></param>
        public Vector(Dictionary<string, double> values)
        {
            this.ParamNames = new List<string>(values.Keys);
            this.Data = new List<double>(values.Values);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="vector">一维数组</param>
        /// <param name="names">名称</param>
        public Vector(IVector vector)
        {
            this.Data = new List<double>(vector.OneDimArray);
            this.ParamNames = new List<string>( vector.ParamNames.ToArray());
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="vector">列表</param>
        /// <param name="names">名称</param>
        public Vector(List<double> vector, List<string> names = null)
        {
            this.Data = vector; 
            if (names == null)
            {
                this.ParamNames = new List<string>(vector.Count);
            }
            else
            {
                this.ParamNames = new List<string>(names);
            }
        }
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public Vector()
        {
            this.Data = new List<double>();
            this.ParamNames = new List<string>();
        }
        #endregion

        #region 核心变量
        /// <summary>
        /// 核心存储
        /// </summary>
        public List<double> Data {get;set;}
        #endregion

        #region Vector 自我方法

        /// <summary>
        /// b追加到a后面Concatenation
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        static public Vector Stack(IVector a, IVector b)
        {
            int i;
            Vector c = new Vector(a.Dimension + b.Dimension);
            for (i = 0; i < a.Dimension; i++) c[i] = a[i];
            for (i = 0; i < b.Dimension; i++) c[i + a.Dimension] = b[i];
            return c;
        }
        /// <summary>
        /// 追加一个向量。
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public Vector Stack(IVector b)
        {
            return Stack(this, b);
        }
        // Component access
        /// <summary>
        /// 截取新向量。
        /// </summary>
        /// <param name="prevObj">起始编号（含）</param>
        /// <param name="last">最后编号（含）</param>
        /// <returns></returns>
        public Vector Slice(int first, int last)
        {
            Vector Aux = new Vector(last - first + 1);
            for (int i = first; i <= last; i++) Aux.Data[i - first] = Data[i];
            return Aux;
        }

        /// <summary>
        /// 单位方向向量
        /// </summary>
        /// <returns></returns>
        public Vector DirectionUnit() { return this / Norm(); }

        /// <summary>
        ///  Vector from polar angles。
        ///  站心坐标。极坐标转空间直角坐标。
        /// </summary>
        /// <param name="azim"></param>
        /// <param name="elev"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        static public Vector VecPolar(double azim, double elev, double r = 1.0)
        {
            return new Vector(r * Math.Cos(azim) * Math.Cos(elev), r * Math.Sin(azim) * Math.Cos(elev), r * Math.Sin(elev));
        }

        // Dot product, norm, cross product

        public static double Dot(Vector left, Vector right)
        {
            double Sum = 0.0;
            for (int i = 0; i < left.Dimension; i++) Sum += left.Data[i] * right.Data[i];
            return Sum;
        }
        /// <summary>
        /// 点乘
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        public double Dot(Vector right)
        {
            return Dot(this, right);
        }

        /// <summary>
        /// 二次范数
        /// </summary>
        /// <returns></returns>
        public double Norm()
        {
            return Math.Sqrt(Dot(this, this));
        }

        public Vector Cross3D(Vector right)
        {
            return Cross3D(this, right);
        }

        /// <summary>
        /// 叉乘
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector Cross3D(Vector left, Vector right)
        {
            if ((left.Dimension != 3) || (right.Dimension != 3))
            {
                throw new ArgumentException("维数不符合要求！");
            }
            Vector Result = new Vector(3);
            Result.Data[0] = left.Data[1] * right.Data[2] - left.Data[2] * right.Data[1];
            Result.Data[1] = left.Data[2] * right.Data[0] - left.Data[0] * right.Data[2];
            Result.Data[2] = left.Data[0] * right.Data[1] - left.Data[1] * right.Data[0];
            return Result;
        }
        /// <summary>
        /// 获取值和名称都相同的向量
        /// </summary>
        /// <param name="fixedAmbiguities"></param>
        /// <returns></returns>
        public Vector GetSame(Vector fixedAmbiguities)
        {
            if(fixedAmbiguities ==null){return new Vector();}

            List<double> vals = new List<double>();
            var names = new List<string>(); 

            foreach (var item in this.ParamNames)
            {
                if (fixedAmbiguities.Contains(item) && fixedAmbiguities[item] == this[item])
                {
                    vals.Add(this[item]);
                    names.Add(item);
                }
            }
            return new Vector(vals, names);

        }
        /// <summary>
        /// 获取指定编号的向量
        /// </summary>
        /// <param name="fromIndex">起始编号</param>
        /// <param name="count">参数数量</param>
        /// <returns></returns>
        public override IVector GetVector(int fromIndex, int count) { return new Vector(this.Data.GetRange(fromIndex, count), this.ParamNames.GetRange(fromIndex, count)); }
        /// <summary>
        /// 按名称顺序排序
        /// </summary>
        /// <returns></returns>
        public Vector SortByName()
        {
            Dictionary<string, double> dic = GetDictionary();
            var ordered = (from entry in dic
                           orderby entry.Key ascending
                           select entry).ToDictionary(pair => pair.Key, pair => pair.Value);
            return new Vector(ordered);
        }
        /// <summary>
        /// 按名称逆序排序
        /// </summary>
        /// <returns></returns>
        public Vector SortByNameDescending()
        {
            Dictionary<string, double> dic = GetDictionary();
            var ordered = (from entry in dic
                           orderby entry.Key descending
                           select entry).ToDictionary(pair => pair.Key, pair => pair.Value);
            return new Vector(ordered);
        }

        /// <summary>
        /// 按值顺序排序
        /// </summary>
        /// <returns></returns>
        public Vector Sort()
        {
            Dictionary<string, double> dic = GetDictionary();
            var ordered = (from entry in dic
                           orderby entry.Value ascending
                           select entry).ToDictionary(pair => pair.Key, pair => pair.Value);
            return new Vector(ordered);
        }
        /// <summary>
        /// 逆序排序
        /// </summary>
        /// <returns></returns>
        public Vector SortDescending()
        {
            Dictionary<string, double> dic = GetDictionary();
            var dic1Asc = dic.OrderByDescending(o => o.Value).ToDictionary(o => o.Key, p => p.Value);
            return new Vector(dic1Asc);
        }
        /// <summary>
        /// 获取字典。
        /// </summary>
        /// <returns></returns>
        public  Dictionary<string, double> GetDictionary()
        {
            Dictionary<string, double> dic = new Dictionary<string, double>();
            var count = this.Count;
            for (int i = 0; i < count; i++)
			{
                dic.Add(this.ParamNames[i], this[i]);
            }  

            return dic;
        }


        /// <summary>
        /// 设置向量空间的维数。只设置增大。
        /// </summary>
        /// <param name="dimension">新维数</param>
        public void SetDimension(int dimension)
        {
            this.Data = ListUtil.SetDimension<double>(Data, dimension);
            this.ParamNames = ListUtil.SetDimension<string>(ParamNames, dimension); 
        }



        #endregion

        #region override

        /// <summary>
        /// 获取指定的元素。
        /// </summary>
        /// <param name="i">编号</param>
        /// <returns></returns>
        public override double this[int i] { get { return Data[i]; } set { Data[i] = value; } }
        /// <summary>
        /// 设置数据。
        /// </summary>
        /// <param name="i"></param>
        /// <param name="val"></param>
        /// <param name="name"></param>
        public void Set(int i, double val, string name) { this[i] = val; this.ParamNames[i] = name; }
        /// <summary>
        /// 增加一个数
        /// </summary>
        /// <param name="val"></param>
        /// <param name="name"></param>
        public void Add(double val, string name)
        {
            this.Data.Add(val);
            this.ParamNames.Add(name);
        }

        /// <summary>
        /// 一维数组
        /// </summary>
        public override double[] OneDimArray { get { return Data.ToArray(); } }

        public override IVector Create(int count)
        {
            return new Vector(count);
        }

        /// <summary>
        /// 与原点的距离半径。
        /// </summary>
        /// <returns></returns>
        public virtual double Radius() { return this.Norm(); }
        public override Object Clone()
        {
            int length = this.Count;
            double[] other = new double[length];
            for (int i = 0; i < length; i++)
            {
                other[i] = this[i];
            }
            return new Vector(other);
        }


        public override int GetHashCode() { return Data.GetHashCode(); }

        #endregion

        /// <summary>
        /// 逐个参数幂乘
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public  Vector Pow(double value)
        { 
            return Vector.Pow( this ,value);
        }
        #region operater
        public static Vector Pow(Vector V, double value)
        {
            Vector Aux = new Vector(V.Dimension);
            for (int i = 0; i < V.Dimension; i++) { Aux.Data[i] = Math.Pow(V.Data[i], value); }
            Aux.ParamNames = V.ParamNames;
            return Aux;
        }
        public static Vector operator *(double value, Vector V)
        {
            Vector Aux = new Vector(V.Dimension);
            for (int i = 0; i < V.Dimension; i++) Aux.Data[i] = value * V.Data[i];
            Aux.ParamNames = V.ParamNames;
            return Aux;
        }
        /// <summary>
        /// +
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static IVector operator +(Vector left, IVector right) { return left.Plus(right); }
        /// <summary>
        /// -
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static IVector operator -(Vector left, IVector right) { return left.Minus(right); }
        /// <summary>
        /// -
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector operator -(Vector left) { return left.Opposite(); }
        public static Vector operator /(Vector left, double val) { return left.Divide(val); }

        //public static Vector operator /(Vector V, double value)
        //{
        //    Vector Aux = new Vector(V.Dimension);
        //    for (int i = 0; i < V.Dimension; i++) Aux.Data[i] = V.Data[i] / value;
        //    return Aux;
        //}

        /// <summary>
        /// *
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector operator *(Vector left, IVector right) { return new Vector( left.Multiply(right)); }
        /// <summary>
        /// +
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>

        public static Vector operator +(Vector left, Vector right) { return left.Plus(right); }
        /// <summary>
        /// -
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector operator -(Vector left, Vector right) { return left.Minus(right); }
        /// <summary>
        /// *
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector operator *(Vector left, Vector right) { return left.Multiply(right); }
        /// <summary>
        /// *
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector operator *(Vector left, double right) { return left.Multiply(right); }
        /// <summary>
        /// +
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        public virtual Vector Plus(Vector right)
        {
            if (this.Count != right.Count)
                throw new Exception("维数相同才可以计算！");

            Vector reslult = new Vector(this.Count);
            int length = this.Count;
            for (int i = 0; i < length; i++)
            {
                reslult[i] = this[i] + right[i];
            }
            return reslult;
        }
        /// <summary>
        /// 追加
        /// </summary>
        /// <param name="vector"></param>
        public void AddRange(Vector vector )
        {
            var i = 0;
            foreach (var item in vector)
            {
                var name = vector.ParamNames[i];
                this.Add(item, name);
                i++;
            } 
        }

        /// <summary>
        /// -
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        public virtual Vector Minus(Vector right)
        {
            if (this.Count != right.Count)
                throw new Exception("维数相同才可以计算！");

            Vector reslult = new Vector(this.Count) { ParamNames = this.ParamNames };
            int length = this.Count;
            for (int i = 0; i < length; i++)
            {
                reslult[i] = this[i] - right[i];
            }
            return reslult;
        }
        /// <summary>
        /// 相反数
        /// </summary>
        /// <returns></returns>
        public virtual Vector Opposite()
        {
            Vector reslult = new Vector(this.Count);
            int length = this.Count;
            for (int i = 0; i < length; i++)
            {
                reslult[i] = -this[i];
            }
            return reslult;
        }

        /// <summary>
        /// Corss
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        public virtual Vector Multiply(Vector right)
        {
            if (this.Count != right.Count)
                throw new Exception("维数相同才可以计算！");

            //三维
            if (Count == 3)
            {
                double cp0 = this[1] * right[2] - this[2] * right[1];
                double cp1 = this[2] * right[0] - this[0] * right[2];
                double cp2 = this[0] * right[1] - this[1] * right[0];
                double[] cp = new double[] { cp0, cp1, cp2 };
                return new Vector(cp);
            }
            //  throw new NotImplementedException("暂不支持其它维数的叉乘计算" + Count);

            //以下有待验证
            Vector reslult = new Vector(this.Count);
            int length = this.Count;
            for (int i = 0; i < length; i++)
            {

                double tmp = 0;
                for (int j = 0; j < length; j++)
                {
                    if (i == j) continue; //同轴叉乘为 0。
                    tmp = this[i] * right[j] - right[i] * this[j];
                }
                reslult[i] = tmp;
            }
            return reslult;
        }
        /// <summary>
        /// *
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        public virtual Vector Multiply(double right)
        {
            Vector reslult = new Vector(this.Count);
            int length = this.Count;
            for (int i = 0; i < length; i++)
            {
                reslult[i] = this[i] * right;
                reslult.ParamNames[i] = this.ParamNames[i];
            }
            return reslult;
        }
        /// <summary>
        /// 除以
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        public virtual Vector Divide(double right)
        {
            Vector reslult = new Vector(this.Count);
            int length = this.Count;
            for (int i = 0; i < length; i++)
            {
                reslult[i] = this[i] / right;
                reslult.ParamNames[i] = this.ParamNames[i];
            }
            return reslult;
        }
        #endregion

        /// <summary>
        /// 创建一个实例
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public override IVector Create(double[] array) { return new Vector(array); }

        /// <summary>
        /// 返回迭代
        /// </summary>
        /// <returns></returns>
        public override IEnumerator<double> GetEnumerator() { return this.Data.GetEnumerator(); }

     

        /// <summary>
        /// 移除指定参数
        /// </summary>
        /// <param name="paramNames"></param>
        public virtual void  Remove( IEnumerable<string> paramNames)
        {
            foreach (var item in paramNames)
            {
                Remove(item);
            }
        }
        /// <summary>
        /// 移除指定参数
        /// </summary>
        /// <param name="paramName"></param>
        public void Remove(string paramName)
        {
            var index = this.ParamNames.IndexOf(paramName);
            this.ParamNames.RemoveAt(index);
            this.Data.RemoveAt(index);
        }
        
        /// <summary>
        /// 获取子向量
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public Vector GetSubVector(int startIndex =0, int len= Int16.MaxValue)
        {
            Vector Vector = new Vector();
            for (int i = startIndex; i >= 0 && i < len + startIndex && i< this.Count; i++)
            {
                Vector.Add(this[i], this.ParamNames[i]);
            }
            return Vector;
        }

        /// <summary>
        /// 向量是否为空
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static bool IsEmpty(IVector vector)
        {
            return vector == null || vector.Count == 0;
        }
        /// <summary>
        /// 是否所有数字有效
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static bool IsValid(IVector vector)
        {
            if (IsEmpty(vector)) { return false; }
            foreach (var item in vector)
            {
                if (!Gdp.Utils.DoubleUtil.IsValid(item)) { return false; }
            }
            return true;
        }
        /// <summary>
        /// 随机生成
        /// </summary>
        /// <param name="dimension"></param>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static Vector Random(int dimension, int seed = 1)
        {
            Random rd = new Random(seed);
            //rd.next(1, 10)(生成1~10之间的随机数，不包括10)
            Vector vec = new Vector(dimension);
            for (int i = 0; i < dimension; i++)
            {
                vec[i] = rd.NextDouble();
            }
            return vec;
        }


        #region  IO

        /// <summary>
        /// 提供可读String类型返回
        ///说明行数据格式定义
        ///RowCount × ColCount, content
        /// </summary>
        /// <param name="splitter"></param>
        /// <returns></returns>
        public virtual string ToReadableText(string splitter="\n")
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            sb.AppendLine(this.Count + "×1");
            foreach (var item in this)
            {
                if (i != 0) { sb.Append(splitter); }
                sb.Append(Gdp.Utils.DoubleUtil.ToReadableText(item)); 
                i++;
            }
            sb.AppendLine();

          
            if (this.ParamNames != null)
            {
                sb.Append(paramNameMarker);
                int j = 0;
                foreach (var item in this.ParamNames)
                {
                    if(j != 0) { sb.Append(","); }
                    sb.Append(item);
                    j++;
                }
            }
            sb.AppendLine();

            return sb.ToString();
        }
        const string paramNameMarker = "ParamNames:";
        /// <summary>
        /// 解析字符串，默认为 \t \n \r , 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="splitter"></param>
        /// <returns></returns>
        public static Vector Parse(string text, string[] splitter = null)
        {
            if (splitter == null)
            {
                splitter = new string[] { "\n", "\r", ",", ";", "\t", " " };
            }
            string[] lines = text.Split(splitter, StringSplitOptions.RemoveEmptyEntries);

            return Parse(lines);
        }

        /// <summary>
        /// 解析打散的字符串.其中，第一个可以为含 × 号的字符中，如 5 × 1，表示向量的大小。
        /// </summary>
        /// <param name="items">元素不可为空的数组</param>
        /// <returns></returns>
        public static Vector Parse(string[] items, string [] splitter = null)
        {
            int startLineIndex = 0; 
            int rowCount = items.Length;
            int colCount = 1;
            //找到含有×号的句子，认为是数量标注 
            foreach (var line in items)
            {
                if (line.Contains("×"))
                {
                    var strs = line.Split(new string[] { "×" }, StringSplitOptions.RemoveEmptyEntries);
                    rowCount = int.Parse(strs[0]);
                    if (strs.Length >= 2)
                    {
                        colCount = int.Parse(strs[1]);
                    }
                    startLineIndex ++;
                    break;
                } 
            }
            if(startLineIndex >= items.Length) { startLineIndex = 0; }//如果没有乘号则认为全是数据。

            Vector vector = new Vector(rowCount);
            for (int vecIndex = 0, lineIndex = startLineIndex; vecIndex < rowCount; lineIndex++, vecIndex++)
            {
                vector[vecIndex] = Gdp.Utils.DoubleUtil.TryParse(items[lineIndex], 0);
            } 

            vector.ParamNames = Gdp.Utils.StringUtil.ExtractNames(items, paramNameMarker, splitter);//  paramNames;

            return vector;
        }
        #endregion
    }
}
