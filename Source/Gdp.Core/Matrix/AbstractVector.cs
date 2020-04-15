//2014.09.28， czs, create, hailutu tongliao, 创建向量接口，为后继实现自我的、快速的、大规模的矩阵计算做准备
//2016.10.10, czs, editin hongqing, 增加返回指定编号的子向量
//2017.06.28, czs, eidt in hongqing, 生成一个指定精度的唯一键

using System; 
using System.Collections.Generic;
using Gdp.Utils;
using System.Text;

namespace Gdp
{
    /// <summary>
    /// Geo 向量，为一维数组。
    /// </summary>
    [Serializable]
    public abstract class AbstractVector : IVector//, IToTabRow, IArithmeticOperation<IVector>
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public AbstractVector() {
            this.Tolerance = 1e-13;
             
        }

        /// <summary>
        /// 参数名称。
        /// </summary>
        public virtual List<string> ParamNames { get; set; }
        /// <summary>
        /// 返回一维数组。
        /// Linear Array.
        /// </summary>
        public virtual double[] OneDimArray { get; set; }

        /// <summary>
        /// 元素数量
        /// </summary>
        public int Count { get { return OneDimArray.Length; } }
        /// <summary>
        /// 向量的模/范数/长度/元素平方和的根
        /// </summary>
        public virtual double Norm
        {
            get
            {
                double norm = 0;
                foreach (var item in OneDimArray)
                {
                    norm += item * item;
                }
                return Math.Sqrt(norm);
            }
        }
         

        /// <summary>
        /// 数值比较时的限差，默认为 1e-23.
        /// </summary>
        public double Tolerance { get; set; }
        /// <summary>
        /// 向量与指定坐标轴的方向余弦，CosX = x/r
        /// </summary>
        /// <param name="i">坐标轴/元素下标</param>
        /// <returns></returns>
        public double GetCos(int i) { return this[i] / Norm; }
        /// <summary>
        /// 向量与指定坐标轴的方向角[0-180]。单位：弧度。
        /// </summary>
        /// <param name="i">坐标轴编号</param>
        /// <returns></returns>
        public double GetDirectionAngle(int i) { return Math.Acos(GetCos(i)); }
        /// <summary>
        /// 获取指定参数的向量
        /// </summary>
        /// <param name="paramNames">参数列表</param>
        /// <returns></returns>
        public IVector GetVector(List<String> paramNames)
        {
            IVector newVector = Create(paramNames.Count);
            int i = 0;
            foreach (var name in paramNames)
            {
                if (this.Contains(name))
                {
                    newVector[i] = this[name];
                }
                i++;
            }
            return newVector;
        }
        /// <summary>
        /// 返回子向量，如果指定数量超限，则返回能提供的最大向量。
        /// </summary>
        /// <param name="fromIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public virtual IVector GetVector(int fromIndex, int count)
        {
            int endIndexNotInclude = Math.Min( fromIndex + count, this.Count);
            int factualCount = endIndexNotInclude - fromIndex;
            IVector newVector = Create(factualCount);
            newVector.ParamNames = this.ParamNames.GetRange(fromIndex, factualCount);
            
            for (int i = fromIndex, j = 0; i < endIndexNotInclude; i++,j++)
            {
                newVector[j] = this[i]; 
            } 
            return newVector;
        }

        /// <summary>
        /// 是否包含指定参数名称
        /// </summary>
        /// <param name="paramName">参数名称</param>
        /// <returns></returns>
        public bool Contains(string paramName)
        {
            return this.ParamNames.Contains(paramName);
        }

        /// <summary>
        /// 单位向量
        /// </summary>
        public IVector UnitVector { get { int count = this.Count; IVector vect = Create(count); for (int i = 0; i < count; i++) vect[i] = this.GetCos(i); return vect; } }
        /// <summary>
        /// 获取指定的元素。
        /// </summary>
        /// <param name="i">行编号</param>
        /// <param name="i">列编号</param>
        /// <returns></returns>
        public virtual double this[int i] { get { return OneDimArray[i]; } set { OneDimArray[i] = value; } }

        /// <summary>
        /// 是否所有数字都有效
        /// </summary>
        public virtual bool IsValid
        {
            get
            {
                foreach (var item in this.OneDimArray)
                {
                    if (!DoubleUtil.IsValid(item)) return false;
                }
                return true;
            }
        }

        /// <summary>
        /// 坐标的维数
        /// </summary>
        public virtual int Dimension { get { return Count; } }
        /// <summary>
        /// 权值，可选。
        /// </summary>
        public double Weight { get; set; }
        /// <summary>
        /// 用于保存数据。
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        /// <param name="paramName"> 参数名称 </param>
        /// <returns></returns>
        public virtual double this[string paramName] { get { return this[GetIndex(paramName)]; } set { this[GetIndex(paramName)] = value; } }
        /// <summary>
        /// 获取参数名称的编号。
        /// </summary>
        /// <param name="paramName">参数名称</param>
        /// <returns></returns>
        public int GetIndex(string paramName)
        {
            return ParamNames.IndexOf(paramName);
        }
        /// <summary>
        /// 加上
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        public virtual IVector Plus(IVector right)
        {
            if (this.Count != right.Count)
                throw new Exception("维数相同才可以计算！");

            IVector reslult = Create(this.Count);
            int length = this.Count;
            for (int i = 0; i < length; i++)
            {
                reslult[i] = this[i] + right[i];
                reslult.ParamNames[i] = this.ParamNames[i];
            }
            return reslult;
        }
         
        /// <summary>
        /// 减去
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        public virtual IVector Minus(IVector right)
        {
            if (this.Count != right.Count)
                throw new Exception("维数相同才可以计算！");

            IVector reslult = Create(this.Count);
            int length = this.Count;
            for (int i = 0; i < length; i++)
            {
                reslult[i] = this[i] - right[i];
                reslult.ParamNames[i] = this.ParamNames[i];
            }
            return reslult;
        }
        /// <summary>
        /// 相反数
        /// </summary>
        /// <returns></returns>
        public IVector Opposite()
        {
            IVector reslult = Create(this.Count);
            int length = this.Count;
            for (int i = 0; i < length; i++)
            {
                reslult[i] = -this[i];
                reslult.ParamNames[i] = this.ParamNames[i];
            }
            return reslult;
        }

        /// <summary>
        /// Corss
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        public virtual IVector Multiply(IVector right)
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
                return Create(cp);
            }
          //  throw new NotImplementedException("暂不支持其它维数的叉乘计算" + Count);

            //以下有待验证
            IVector reslult = Create(this.Count);
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
        /// 乘法
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        public virtual IVector Multiply(double right)
        {
            IVector reslult = Create(this.Count);
            int length = this.Count;
            for (int i = 0; i < length; i++)
            {
                reslult[i] = this[i] * right;
            }
            return reslult;
        }
        /// <summary>
        /// 除法
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        public virtual IVector Divide(double right)
        {
            IVector reslult = Create(this.Count);
            int length = this.Count;
            for (int i = 0; i < length; i++)
            {
                reslult[i] = this[i] / right;
            }
            return reslult;
        }
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public abstract IVector Create(int count);
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public abstract IVector Create(double[] array);
        /// <summary>
        /// 拷贝
        /// </summary>
        /// <returns></returns>
        public abstract Object Clone();
        /// <summary>
        /// 与另一个向量的夹角。单位：弧度
        /// </summary>
        /// <param name="another">另一个向量</param>
        /// <returns></returns>
        public double GetIncludedAngle(IVector another) { return Math.Acos(GetCos(another)); }
        /// <summary>
        /// 与另一个向量的夹角余弦。
        /// </summary>
        /// <param name="another">另一个向量</param>
        /// <returns></returns>
        public double GetCos(IVector another) { return Dot(another) / this.Norm / another.Norm; }
        /// <summary>
        /// 向量的点积/数量积
        /// </summary>
        /// <param name="right">另一个向量</param>
        public double Dot(IVector right)
        {
            double reslult = 0;
            int length = this.Count;
            for (int i = 0; i < length; i++)
            {
                reslult += this[i] * right[i]; 
            }
            return reslult;
        }
        /// <summary>
        /// 向量的叉乘，结果还是向量,大小为矩形面积，方向垂直此量向量所占的平面，方向符合右手规则，拇指方向
        /// </summary>
        /// <param name="right">另一个向量</param>
        /// <returns></returns>
        public IVector Cross(IVector right) { return this.Multiply(right); }
        /// <summary>
        /// 用子向量值设置向量
        /// </summary>
        /// <param name="subVector">子向量</param>
        /// <param name="startMainIndex">主向量起始编号</param>
        /// <param name="startSubIndex">子向量起始编号</param>
        /// <param name="maxSubLength">最大子向量的长度</param>
        public void SetSubVector( IVector subVector, int startMainIndex = 0, int startSubIndex = 0, int maxSubLength = int.MaxValue)
        {
            var maxMainCount = this.Count;

            for (int i = 0; startMainIndex + i <maxMainCount && i + startSubIndex < subVector.Count && i < maxSubLength; i++)
            {
                this[startMainIndex + i] = subVector[i + startSubIndex];
            }
        }
        /// <summary>
        /// 自定义格式化
        /// </summary>
        public  IFormatProvider FormatProvider { get; set; }
        /// <summary>
        /// 字符串显示
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.Count);
            sb.Append(", Values:");
            for (int i = 0; i < this.Count; i++)
            {
                if (i != 0) sb.Append(", ");              
                sb.Append(  this[i]);
            }

            if (this.ParamNames != null && this.ParamNames.Count == Count)
            {
                sb.AppendLine();
                sb.Append("Names:");
                int i = 0;
                foreach (var name in this.ParamNames)
                {
                    if (i != 0) sb.Append(", ");
                    sb.Append(name);
                    i++;
                }
            }
            return sb.ToString();
        }

        public virtual string GetTabValues() { return String.Format(FormatProvider, "{0:\t8.6}", this); }

        public virtual string GetTabTitles()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < this.Count; i++)
            {
                if (i != 0) sb.Append("\t");
                if (this.ParamNames != null)
                {
                    sb.Append(this.ParamNames[i]);
                }
                else { sb.Append(i); }
            }
            return sb.ToString();
        }
         

        /// <summary>
        /// 是否在数值限内相等。
        /// </summary>
        /// <param name="obj">待比较对象</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            AbstractVector other = obj as AbstractVector;
            if (other == null) return false;

            if (other.Count != this.Count) return false;
            int count = this.Count;
            for (int i = 0; i < count; i++)
            {
                if (Math.Abs(this[i] - other[i]) > Tolerance) return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public virtual System.Collections.Generic.IEnumerator<double> GetEnumerator() { return new List<Double>( OneDimArray).GetEnumerator();}

         System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return OneDimArray.GetEnumerator(); }

      

    }
}
