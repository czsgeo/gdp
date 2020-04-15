//2014.09.22, czs, create, hailutu tongliao, 创建矩阵接口，为后继实现自我的、快速的、大规模的矩阵计算做准备
//2017.07.19, czs, edit in hongqing, 增加除法
//2017.08.31, czs, edit in hongqing, 实现元素求幂接口

using System;
using System.Collections.Generic;
 
using Gdp.Utils;

using System.Threading.Tasks;
using Gdp.IO;

namespace Gdp
{
    /// <summary>
    /// Geo 抽象实现矩阵，如果要采用其它存储模型请重新实现对应接口。
    /// 以向量为单位进行操作。
    /// </summary>
    public abstract class AbstractMatrix : IMatrix
    {
        protected Log log = new Log(typeof(AbstractMatrix));

        /// <summary>
        /// 默认构造函数，必须提供一个数据类型。
        /// </summary>
        /// <param name="MatrixType">数据类型</param>
        public AbstractMatrix(MatrixType MatrixType)
        {
            this.MatrixType = MatrixType;
            this.Tolerance = 1e-13;
        }
        /// <summary>
        /// 矩阵存储类型。
        /// </summary>
        public MatrixType MatrixType { get;  set;}

        #region 虚拟，可不实现
        #region 属性
        /// <summary>
        /// 行名称
        /// </summary>
        public virtual List<string> RowNames { get; set; }

        /// <summary>
        /// 列名称
        /// </summary>
        public virtual List<string> ColNames { get; set; } 

        /// <summary>
        /// 名称
        /// </summary>
        public virtual string Name { get; set; }
             
        /// <summary>
        /// 最大容忍度，用于判断两个数是否相等。
        /// </summary>
        public double Tolerance { get;set; }
        /// <summary>
        /// 列数
        /// </summary>
        public virtual int ColCount { get { return Array.Length; } }
        /// <summary>
        /// 行数
        /// </summary>
        public virtual int RowCount { get { return Array[0].Length; } }
        /// <summary>
        /// 是否是方阵。
        /// </summary>
        public virtual bool IsSquare { get { return RowCount == ColCount; } } 

        /// <summary>
        /// 行名称是否有效，可用。
        /// </summary>
        public bool IsRowNameAvailable { get => (this.RowNames != null && this.RowNames.Count == this.RowCount); }
            
        /// <summary>
        /// 列名称是否有效，可用。
        /// </summary>
        public bool IsColNameAvailable { get => (this.ColNames != null && this.ColNames.Count == this.ColCount); }
      /// <summary>
        /// 是否具有有效的行列名称
        /// </summary>
        public bool HasParamNames => IsColNameAvailable && IsRowNameAvailable;
        /// <summary>
        /// 是否是对称矩阵
        /// </summary>
        public virtual bool IsSymmetric
        {
            get
            {
                if (!this.IsSquare) { return false; } 

                var rowColCount = RowCount;
                for (int i = 0; i < rowColCount; i++)
                {
                    for (int j = 0; j <= i; j++)
                    {
                        if (Math.Abs(this[i, j] - this[j, i]) > Tolerance) return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// 是否为单位阵
        /// </summary>
        public virtual bool IsIdentity
        {
            get
            {
                if (!IsDiagonal) { return false; }
                for (int i = 0; i < RowCount; i++)
                {
                    var item = this[i, i];
                    if (Math.Abs(item - 1) > this.Tolerance*100) { return false; }
                }
                return true;
            }
        }

        /// <summary>
        /// 是否是对角阵
        /// </summary>
        public virtual bool IsDiagonal
        {
            get
            {
                if (!this.IsSquare) { return false; } 

                var rowColCount = RowCount;
                for (int i = 0; i < rowColCount; i++)
                {
                    for (int j = 0; j < rowColCount && i != j; j++)
                    {
                        if (Math.Abs(this[i, j]) > this.Tolerance) { return false; }
                    }
                }
                return true;
            }
        }
        /// <summary>
        /// 设置某行与列的值为统一的数值
        /// </summary>
        /// <param name="fromRowColIndex"></param>
        /// <param name="toRowColIndex"></param>
        /// <param name="val"></param>
        public void SetRowColValue(int fromRowColIndex, int toRowColIndex, double val)
        {
            for (int i = fromRowColIndex; i <= toRowColIndex; i++)
            {
                SetRowValue(i, val);
                SetColValue(i, val);
            }
        }

        /// <summary>
        /// 设置某行与列的值为统一的数值
        /// </summary>
        /// <param name="rowColIndex">行和列的编号</param>
        /// <param name="val">数值</param>
        public void SetRowColValue(int rowColIndex, double val)
        {
            SetRowValue(rowColIndex, val);
            SetColValue(rowColIndex, val);
        }
        /// <summary>
        /// 设置某行的值为统一的数值
        /// </summary>
        /// <param name="rowIndex">行编号</param>
        /// <param name="val">数值</param>
        public void SetRowValue(int rowIndex, double val)
        {
            for (int i = 0; i < this.ColCount; i++)
            {
                this[rowIndex, i] = val;
            }
        }

        /// <summary>
        /// 设置某列的值为统一的数值
        /// </summary>
        /// <param name="colIndex">列编号</param>
        /// <param name="val">数值</param>
        public void SetColValue(int colIndex, double val)
        {
            for (int i = 0; i < this.RowCount; i++)
            {
                this[i, colIndex] = val;
            } 
        }

        /// <summary>
        /// 获取指定参数的矩阵
        /// </summary>
        /// <param name="paramNames">参数列表</param>
        /// <returns></returns>
        public IMatrix GetMatrix(List<String> paramNames)
        {
            return GetMatrix(paramNames, paramNames);
        }

        /// <summary>
        /// 获取指定参数的矩阵
        /// </summary>
        /// <param name="rowNames">行参数列表</param>
        /// <param name="colNames">列参数列表</param>
        /// <returns></returns>
        public IMatrix GetMatrix(List<String> rowNames, List<String> colNames)
        {
            IMatrix newVector = new Matrix(rowNames.Count, rowNames.Count);
            newVector.RowNames = rowNames;
            newVector.ColNames = colNames;
            int i = 0;
            foreach (var row in rowNames)
            {
                if (!this.ContainsRowName(row)) continue;

                foreach (var col in colNames)
                {
                    if (!this.ContainsColName(col)) continue;

                    newVector[row,col] = this[row, col];
                } 
                i++;
            }
            return newVector;
        }
        /// <summary>
        /// 取矩阵的某列作为向量
        /// </summary>
        /// <param name="j"></param>
        /// <returns></returns>
        public Vector Col(int j)
        {
            Vector Res = new Vector(RowCount);
            for (int i = 0; i < RowCount; i++) Res[i] = this[i, j];
            return Res;
        }
        /// <summary>
        /// 取矩阵的某行作为向量
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public Vector Row(int i)
        {
            Vector Res = new Vector(ColCount);
            for (int j = 0; j < ColCount; j++) Res[j] = this[i,j];
            return Res;
        }



        /// <summary>
        /// 直接设置某列的值
        /// </summary>
        /// <param name="j"></param>
        /// <param name="Col"></param>
        public void SetCol(int j, Vector Col)
        {
            if (Col.Dimension != RowCount) { throw new ArgumentException("维数不符合要求！"); }
            if (j < 0 || ColCount <= j) { throw new ArgumentException("维数不符合要求！"); }

            for (int i = 0; i < RowCount; i++) { this[i, j] = Col[i]; }
        }
        /// <summary>
        /// 直接设定某行的值。
        /// </summary>
        /// <param name="i"></param>
        /// <param name="Row"></param>
        public void SetRow(int i, Vector Row)
        {
            if (Row.Dimension != ColCount) { throw new ArgumentException("维数不符合要求！"); }
            if (i < 0 || RowCount <= i) { throw new ArgumentException("维数不符合要求！"); }
            for (int j = 0; j < RowCount; j++) { this[i, j] = Row[j]; }
        }
        

        /// <summary>
        /// 是否包含行名称
        /// </summary>
        /// <param name="rowName">行名称</param>
        /// <returns></returns>
        public bool ContainsRowName(string rowName)
        {
            if(this.RowNames == null || this.RowNames.Count == 0) { return false; }
            return this.RowNames.Contains(rowName);
        }

        /// <summary>
        /// 是否包含列名称
        /// </summary>
        /// <param name="colName">列名称</param>
        /// <returns></returns>
        public bool ContainsColName(string colName)
        {
            if (this.ColNames == null || this.ColNames.Count == 0) { return false; }
            return this.ColNames.Contains(colName);
        }

        /// <summary>
        /// 获取行编号
        /// </summary>
        /// <param name="paramName">参数名称</param>
        /// <returns></returns>
        public int GetRowIndex(String paramName)
        {
            return this.RowNames.IndexOf(paramName);
        }
        /// <summary>
        /// 获取列编号
        /// </summary>
        /// <param name="paramName">参数名称</param>
        public int GetColIndex(String paramName)
        {
            return this.ColNames.IndexOf(paramName);
        }

        /// <summary>
        /// 获取指定的元素。
        /// </summary>
        /// <param name="rowName">行名称</param>
        /// <param name="colName">列名称</param>
        /// <returns></returns>
        public virtual double this[string rowName, string colName]
        {
            set { this.Array[GetRowIndex(rowName)][GetColIndex(colName)] = value; }
            get { return this.Array[GetRowIndex(rowName)][GetColIndex(colName)]; }
        } 
        /// <summary>
        /// 获取指定的元素。
        /// </summary>
        /// <param name="i">行编号</param>
        /// <param name="j">列编号</param>
        /// <returns></returns>
        public virtual double this[int i, int j]
        {
            set { this.Array[i][j] = value; }
            get { return this.Array[i][j]; }
        }

        /// <summary>返回一个完全复制品.</summary>
        public virtual IMatrix Clone()
        {
            IMatrix X = new Matrix(RowCount, ColCount);
         
            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < ColCount; j++)
                {
                    X[i,j] = this[i, j];
                }
            }

            return X;
        }
        #endregion

        #region 方法 

        /// <summary>
        /// 批量移除行，返回新矩阵
        /// </summary>
        /// <param name="rowIndexes"></param>
        /// <returns></returns>
        public virtual IMatrix GetRowRemovedMatrix(List< int> rowIndexes)
        {
            foreach (var item in rowIndexes)
            {
                if (item >= this.RowCount) { throw new Exception("索引太大，超界！"); }
            }
            var data = this.Array;
            int newRowCount = this.RowCount - rowIndexes.Count;
            var newData = new double[newRowCount][];

            for (int i = 0, j = 0; i < RowCount; i++)
            {
                if (rowIndexes.Contains( i)) { continue; }
                newData[j] = data[i];//这样直接赋值，省了代码和空间，但是和母对象公用对象，若更改可能引起连锁反映。
                j++;
            }
            return new Matrix(newData);
        }
        /// <summary>
        /// 移除指定行,返回新矩阵
        /// </summary>
        /// <param name="rowIndex"></param>
        public virtual IMatrix GetRowRemovedMatrix(int rowIndex)
        {
            if (RowCount <= rowIndex) { return this; }
            var data = this.Array;
            int newRowCount = this.RowCount - 1;
            var newData = new double[newRowCount][];

            for (int i = 0, j = 0; i < RowCount; i++)
            {
                if (i == rowIndex) { continue; }
                newData[j] = data[i];//这样直接赋值，省了代码和空间，但是和母对象公用对象，若更改可能引起连锁反映。
                j++;
            }
            return new Matrix(newData);
        }

        /// <summary>
        /// 批量移除列，返回新矩阵
        /// </summary>
        /// <param name="colndexes"></param>
        /// <returns></returns>
        public virtual IMatrix GetColRemovedMatrix(List<int> colndexes)
        {
            foreach (var item in colndexes)
            {
                if (item >= this.ColCount) { throw new Exception("索引太大，超界！"); }
            }
             
            var data = this.Array;
            int newColCount = this.ColCount - colndexes.Count;
            var newData = Gdp.Utils.MatrixUtil.Create(RowCount, newColCount);

            for (int i = 0; i < RowCount; i++)
            {
                for (int k = 0, j = 0; k < ColCount; k++)
                {
                    if (colndexes.Contains(k)) { continue; }
                    newData[i][j] = data[i][k];
                    j++;
                }
            }
            return new Matrix(newData); 
        }

        /// <summary>
        /// 移除指定列,返回新矩阵
        /// </summary>
        /// <param name="colIndex"></param>
        public virtual IMatrix GetColRemovedMatrix(int colIndex)
        {
            if (ColCount <= colIndex) { return this; }
            var data = this.Array;
            int newColCount = this.ColCount - 1;
            var newData = Gdp.Utils.MatrixUtil.Create(RowCount, newColCount);

            for (int i = 0; i < RowCount; i++)
            {
                for (int k = 0, j = 0; k < ColCount; k++)
                {
                    if (k == colIndex) { continue; }
                    newData[i][j] = data[i][k];
                    j++;
                }
            }
            return new Matrix(newData);
        }


        

        /// <summary>
        /// 矩阵加法
        /// </summary>
        /// <param name="right">右边矩阵</param>
        /// <returns></returns>
        public virtual IMatrix Plus(IMatrix right)
        {
            if (right == null) throw new ArgumentException("参数不能为空！");
            if (right.ColCount != this.ColCount || this.RowCount != right.RowCount) throw new Exception("维数相同才可以计算！");

            int rowCount = this.RowCount;
            int colCount = this.ColCount;
            double[][] array = new double[rowCount][];
            for (int i = 0; i < rowCount; i++)
            {
                var row = new double[colCount];
                for (int j = 0; j < colCount; j++)
                {
                    row[j] = this[i, j] + right[i, j];
                }
                array[i] = row;
            }
            return new Matrix(array); 
        }

        /// <summary>
        /// 乘法减法
        /// </summary>
        /// <param name="right">右边</param>
        /// <returns></returns>
        public virtual IMatrix Minus(IMatrix right)
        {
            if (right == null) throw new ArgumentException("参数不能为空！");
            if (right.ColCount != this.ColCount || this.RowCount != right.RowCount) throw new Exception("维数相同才可以计算！");

            int rowCount = this.RowCount;
            int colCount = this.ColCount;
            double[][] array = new double[rowCount][]; 
            for (int i = 0; i < rowCount; i++)
            {
                var row = new double[colCount]; 
                for (int j = 0; j < colCount; j++)
                {
                    row[j] = this[i, j] - right[i, j];
                }
                array[i] = row;
            }
            return new Matrix(array); 
        }

        /// <summary>
        /// 取负号
        /// </summary> 
        /// <returns></returns>
        public virtual IMatrix Opposite()
        { 
            int rowCount = this.RowCount;
            int colCount = this.ColCount;
            double[][] array = new double[rowCount][]; 

            for (int i = 0; i < rowCount; i++)
            {
                var row = new double[colCount]; 
                for (int j = 0; j < colCount; j++)
                {
                    row[j] = -this[i,j];
                }
                array[i] = row;
            }
            var matrix = new Matrix(array);
            return matrix;
        }
        /// <summary>
        /// 矩阵乘法。Left(m, n) * Right(n, k)
        /// </summary>
        /// <param name="right">右边乘法矩阵。</param>
        /// <returns></returns>
        public virtual IMatrix Multiply(IMatrix right)
        {
            if (right == null) throw new ArgumentException("参数不能为空！");
            if (this.ColCount != right.RowCount)
                throw new Exception("乘法左边的列必须等于右边的行");

            
            double[][] array = Gdp.Utils.MatrixUtil.Multiply(this.Array, right.Array);
            var matrix = new Matrix(array);
            return matrix; 
        }
        #region 矩阵乘法细节
        /// <summary>
        /// 乘法左边一行，乘以右边矩阵。返回一个新行。
        /// 等价于一行的矩阵乘以一个矩阵。
        /// </summary>
        /// <param name="leftRow"></param>
        /// <param name="rightArray"></param>
        /// <returns></returns>
        protected static double[] MultiplyRow(double[] leftRow, double[][] rightArray)
        {
            int col = rightArray.Length;
            double[] rowX = new double[col];
            for (int j = 0; j < col; j++)
            {
                double[] rightCol = rightArray[j];
                double sum = MultiplyAndSum(leftRow, rightCol);
                rowX[j] = sum;
            }
            return rowX;
        }
        /// <summary>
        /// 两个向量对应元素相乘后，再相加。用于乘法。
        /// </summary> 
        /// <param name="vetorA"></param>
        /// <param name="vetorB"></param>
        /// <returns></returns>
        protected static double MultiplyAndSum(double[] vetorA, double[] vetorB)
        {
            double sum = 0;
            int colsA = vetorA.Length;
            for (int k = 0; k < colsA; k++)
            {
                sum += vetorA[k] * vetorB[k];
            }
            return sum;
        }
        #endregion
        private static double[] MultiplyRow(double[] leftRow, double[] rightArray)
        {
            int col = rightArray.Length;
            double[] rowX = new double[col];
            for (int j = 0; j < col; j++)
            {
                rowX[j] = leftRow[j]*rightArray[j];
            }
            return rowX;
        }
        /// <summary>
        /// 返回新的乘法结果。
        /// </summary>
        /// <param name="right">数字</param>
        /// <returns></returns>
        public virtual IMatrix Multiply(double right)
        {
            int row = this.RowCount;
            int col = this.ColCount;
            //IMatrix matrix = this.Clone();

            //for (int i = 0; i < row; i++)
            //{
            //    for (int j = 0; j < col; j++)
            //    {
            //        matrix[i, j] = this[i, j] * right;
            //    }
            //}
            //return matrix;

            double[][] leftArray = this.Array;

            double[][] x = new double[row][];
            if (row > 50)
            {
                Parallel.For(0, row, new Action<int>(delegate(int i)
                    {
                        double[] leftArrayRow = leftArray[i];
                        x[i] = GetMultipleANumber(right, col, leftArrayRow);
                    }));
            }
            else
            {
                for (int i = 0; i < row; i++)
                {
                    double[] leftArrayRow = leftArray[i];
                    x[i] = GetMultipleANumber(right, col, leftArrayRow);
                }
            }

            return new Matrix(x);
        }

        /// <summary>
        /// 返回新的除法结果。
        /// </summary>
        /// <param name="right">数字</param>
        /// <returns></returns>
        public virtual IMatrix Divide(double right)
        {
            int row = this.RowCount;
            int col = this.ColCount;
            //IMatrix matrix = this.Clone();

            //for (int i = 0; i < row; i++)
            //{
            //    for (int j = 0; j < col; j++)
            //    {
            //        matrix[i, j] = this[i, j] / right;
            //    }
            //}
            //return matrix;

            double[][] leftArray = this.Array;

            double[][] x = new double[row][];
            if (row > 50)
            {
                Parallel.For(0, row, new Action<int>(delegate(int i)
                    {
                        double[] leftArrayRow = leftArray[i];
                        x[i] = GetDivideNumber(right, col, leftArrayRow);
                    }));
            }
            else
            {
                for (int i = 0; i < row; i++)
                {
                    double[] leftArrayRow = leftArray[i];
                    x[i] = GetDivideNumber(right, col, leftArrayRow);
                }
            }

            return new Matrix(x);
        }

        private static double[] GetDivideNumber(double right, int col,  double[] leftArrayRow)
        {
            double[] matrixArrayRow=new double[col];
            for (int j = 0; j < col; j++)
            {
                matrixArrayRow[j] = leftArrayRow[j] / right;
            }
            return matrixArrayRow;
        }
        private static double[] GetMultipleANumber(double right, int col,  double[] leftArrayRow)
        {
            double[] matrixArrayRow=new double[col];
            for (int j = 0; j < col; j++)
            {
                matrixArrayRow[j] = leftArrayRow[j] * right;
            }
            return matrixArrayRow;
        }


        public virtual Vector GetRow(int index)
        {
            return new Vector(this.Array[index]);
        }

        public virtual Vector GetCol(int index)
        {
            return new Vector(MatrixUtil.GetColVector(this.Array, index));
        }
  

        /// <summary>
        /// 转置
        /// </summary>
        /// <returns></returns>
        public virtual IMatrix Transposition
        {
            get
            {
                IMatrix trans =new Matrix(ColCount, RowCount);
                int row = this.RowCount;
                int col = this.ColCount;
                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < col; j++)
                    {
                        trans[j, i] = this[i, j];
                    }
                }
                return trans;
            }
        }
        /// <summary>
        /// 求逆,提供一个二维数组默认的实现
        /// </summary>
        public virtual IMatrix GetInverse()
        {
            return new Gdp.Matrix(this.Array).Inverse;
        }
        /// <summary>
        /// 所有元素求幂
        /// </summary>
        /// <param name="power"></param>
        /// <returns></returns>
        public virtual IMatrix Pow(double power)
        {
            Matrix mat = new Matrix(RowCount, ColCount);
            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < ColCount; j++)
                {
                    mat[i, j] = Math.Pow(this[i, j], power);
                }
            }
            return mat;
        }
        #endregion
        #endregion

        #region 抽象，必须实现或赋值
        /// <summary>
        /// 返回二维数组。
        /// </summary>
        public abstract double[][] Array { get; }  
        #endregion

        #region override
        public override bool Equals(object obj)
        {
            AbstractMatrix a = obj as AbstractMatrix;
            if (a == null) return false;

            int row = this.RowCount;
            int col = this.ColCount; 

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    if (Math.Abs(a[i, j] - this[i, j]) > Tolerance)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return MatrixUtil.GetFormatedText(this.Array) ;
        } 
        #endregion


        public abstract int ItemCount { get; }

        public abstract IMatrix SubMatrix(int fromIndex, int count);
    }
}
