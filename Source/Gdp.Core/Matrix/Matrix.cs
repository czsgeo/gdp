//2017.07.18, czs, refactor ib hongqing, 重命名为 ArrayMatrix

using System;
using System.IO;
using System.IO.Compression;//压缩
using System.Threading.Tasks;
using System.Collections.Generic;
using Gdp.Utils;

namespace Gdp
{ 
    /// <summary>
    /// 二维数组表示的矩阵，最通用的矩阵类
    /// </summary>
    public class Matrix : Gdp.AbstractMatrix
    { 
        private double[][] data;
        private int rows;
        private int columns;
        
        public Matrix(int rows, int columns)
            : base(MatrixType.Array)
        {
            this.rows = rows;
            this.columns = columns;
            this.data = new double[rows][];
            for (int i = 0; i < rows; i++)
            {
                this.data[i] = new double[columns];
            } 
        }
         
        /// <summary>
        /// 采用向量初始化一个 N x 1 或 1x N 的矩阵。
        /// </summary>
        /// <param name="vector">向量</param>
        /// <param name="isColOrRow">是否列或行向量</param>
        public Matrix(IVector vector, bool isColOrRow=true)
            : this(MatrixUtil.GetMatrix(vector.OneDimArray, isColOrRow)) {
            if(vector.ParamNames != null)
            {
                if (isColOrRow)
                {
                    this.RowNames = vector.ParamNames;
                    this.ColNames =  new List<string>() { "Value" };
                }
                else
                {
                    this.RowNames = new List<string>() { "Value" };
                    this.ColNames = vector.ParamNames;
                }
            }
        }

          
        /// <summary>Constructs a matrix from the given array.</summary>
        /// <param name="data">The array the matrix gets constructed from.</param>
        public Matrix(double[][] data)
            : base(MatrixType.Array)
        {
            this.rows = data.Length;
            this.columns = data[0].Length;

            for (int i = 0; i < rows; i++)
            {
                if (data[i].Length != columns)
                {
                    throw new ArgumentException();
                }
            }

            this.data = data; 
        }

         
        /// <summary>
        /// 二维数组
        /// </summary>
        public override double[][] Array=> this.data; 
        public int Rows=> this.rows; 
         
        public int Columns => this.columns;
        /// <summary>
        /// 总共元素数量。
        /// </summary>
        public override int ItemCount
        {
            get { return ColCount * RowCount; }
        }
         
        public IMatrix Submatrix(int i0, int i1, int j0, int j1)
        {
            if ((i0 > i1) || (j0 > j1) || (i0 < 0) || (i0 >= this.rows) || (i1 < 0) || (i1 >= this.rows) || (j0 < 0) || (j0 >= this.columns) || (j1 < 0) || (j1 >= this.columns))
            {
                throw new ArgumentException();
            }

            Matrix X = new Matrix(i1 - i0 + 1, j1 - j0 + 1);
            double[][] x = X.Array;
            for (int i = i0; i <= i1; i++)
            {
                for (int j = j0; j <= j1; j++)
                {
                    x[i - i0][j - j0] = data[i][j];
                }
            }
            if (this.ColNames != null && this.ColNames.Count > 0) { X.ColNames = this.ColNames.GetRange(i0, i1 - i0+1); }
            if (this.RowNames != null && this.RowNames.Count > 0) { X.RowNames = this.RowNames.GetRange(j0, j1 - j0+1); }

            return X;
        }
            

        /// <summary>Creates a copy of the matrix.</summary>
        public new Matrix Clone()
        {
            Matrix X = new Matrix(rows, columns);
            double[][] x = X.Array;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    x[i][j] = data[i][j];
                }
            }

            return X;
        }

        /// <summary>Returns the transposed matrix.</summary>
        public Matrix Transpose()
        {
            Matrix X = new Matrix(columns, rows);
            double[][] x = X.Array;  

            for (int i = 0; i < rows; i++)
            { 
                var row = data[i];
                for (int j = 0; j < columns; j++)
                { 
                    x[j][i] = row[j];
                }
            }

            return X;
        }
          

        /// <summary>Unary minus.</summary>
        public static Matrix operator -(Matrix a)
        {
            int rows = a.Rows;
            int columns = a.Columns;
            double[][] data = a.Array;

            Matrix X = new Matrix(rows, columns);
            double[][] x = X.Array;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    x[i][j] = -data[i][j];
                }
            }

            return X;
        }
        /// <summary>Matrix addition.</summary>
        public static Matrix operator +(Matrix a, IMatrix b)
        {
            int rows = a.Rows;
            int columns = a.Columns;
            double[][] data = a.Array;

            if ((rows != b.RowCount) || (columns != b.ColCount))
            {
                throw new ArgumentException("Matrix dimension do not match.");
            }

            Matrix X = new Matrix(rows, columns);
            double[][] x = X.Array;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    x[i][j] = data[i][j] + b[i, j];
                }
            }
            return X;
        }

        /// <summary>Matrix subtraction.</summary>
        public static Matrix operator -(Matrix a, IMatrix b)
        {
            int rows = a.Rows;
            int columns = a.Columns;
            double[][] data = a.Array;

            if ((rows != b.RowCount) || (columns != b.ColCount))
            {
                throw new ArgumentException("Matrix dimension do not match.");
            }

            Matrix X = new Matrix(rows, columns);
            double[][] x = X.Array;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    x[i][j] = data[i][j] - b[i, j];
                }
            }
            return X;
        } 
        public static Matrix operator *(Matrix a, double s)
        {
            int rows = a.Rows;
            int columns = a.Columns;
            double[][] data = a.Array;

            Matrix X = new Matrix(rows, columns);

            double[][] x = X.Array;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    x[i][j] = data[i][j] * s;
                }
            }

            return X;
        }
       
        public static Matrix operator *(Matrix left, IMatrix right)
        {
            if (right.RowCount != left.columns)
            {
                throw new ArgumentException("Matrix dimensions are not valid.");
            }

            int rowCount = left.RowCount; 
            double[][] x = new double[rowCount][];// X.Array;
            double[][] aArray = left.Array;           
            double[][] bArray = right.Transposition.Array;
            if (rowCount <= 500)
            { 
                for (int i = 0; i < rowCount; i++)
                { 
                    x[i] = MultiplyRow(aArray[i], bArray);
                }
            }
            else
            {
                Parallel.For(0,  rowCount, new Action<int>(delegate(int i){
                      x[i] = MultiplyRow(aArray[i], bArray);
                }));
            }
            return  new Matrix(x); 
        } 
         
        public Matrix Solve(Matrix rhs)
        {
            Matrix inv = null;
            if (rows == columns && this.IsSymmetric)
            {
                inv = new SljMatrixInverse(this).Solve(rhs);
                return inv;
            }
            inv = new QrDecomposition(this).Solve(rhs);
            return inv;
        }
         
        public Matrix Inverse
        {
            get
            {
                return this.Solve(Diagonal(rows, rows, 1.0));
            }
        }

     
        /// <summary>Returns a diagonal matrix of the given aboutSize.</summary>
        public static Matrix Diagonal(int rows, int columns, double value)
        {
            Matrix X = new Matrix(rows, columns);
            double[][] x = X.Array;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    x[i][j] = ((i == j) ? value : 0.0);
                }
            }
            return X;
        }

        /// <summary>Returns the matrix in a textual form.</summary>
        public override string ToString()
        {
        using (StringWriter writer = new StringWriter())
            {
                writer.Write("Matrix "); 

               writer.Write( MatrixUtil.GetFormatedText(this.Array)); 

                return writer.ToString();
            }
        }
        
        #region override
        /// <summary>
        /// 列数
        /// </summary>
        public override int ColCount { get { return Columns; } }
        /// <summary>
        /// 行数
        /// </summary>
        public override int RowCount { get { return Rows; } }
        public override Gdp.IMatrix GetInverse()
        {
            return this.Inverse;
        }
        /// <summary>
        /// 转置。
        /// </summary>
        public override Gdp.IMatrix Transposition { get { return this.Transpose(); } }

        #endregion
        /// <summary>
        /// 提取方阵
        /// </summary>
        /// <param name="fromIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override IMatrix SubMatrix(int fromIndex, int count)
        {
            int minRowOrCol = Math.Min(this.RowCount, this.ColCount);
            int maxEndIndexIncluded = Math.Min(fromIndex + count - 1, minRowOrCol - 1);
            return this.Submatrix(fromIndex, maxEndIndexIncluded, fromIndex, maxEndIndexIncluded);
        }
    }


    /// <summary>
    ///	 QR 分解。 
    /// </summary>
    public class QrDecomposition
    {
        private Matrix QR;
        private double[] Rdiag;

        /// <summary>Construct a QR decomposition.</summary>	
        public QrDecomposition(Matrix A)
        {
            QR = (Matrix)A.Clone();
            double[][] qr = QR.Array;
            int m = A.RowCount;
            int n = A.Columns;
            Rdiag = new double[n];

            for (int k = 0; k < n; k++)
            {
                // Compute 2-norm of k-th column without under/overflow.
                double nrm = 0;
                for (int i = k; i < m; i++)
                    nrm = this.Hypotenuse(nrm, qr[i][k]);

                if (nrm != 0.0)
                {
                    // Form k-th Householder vector.
                    if (qr[k][k] < 0)
                        nrm = -nrm;
                    for (int i = k; i < m; i++)
                        qr[i][k] /= nrm;
                    qr[k][k] += 1.0;

                    // Apply transformation to remaining columns.
                    for (int j = k + 1; j < n; j++)
                    {
                        double s = 0.0;
                        for (int i = k; i < m; i++)
                            s += qr[i][k] * qr[i][j];
                        s = -s / qr[k][k];
                        for (int i = k; i < m; i++)
                            qr[i][j] += s * qr[i][k];
                    }
                }
                Rdiag[k] = -nrm;
            }
        }

        public Matrix Solve(Matrix rhs)
        {
            if (rhs.RowCount != QR.RowCount) throw new ArgumentException("Matrix row dimensions must agree.");
            if (!IsFullRank) throw new InvalidOperationException("Matrix is rank deficient.");

            // Copy right hand side
            int count = rhs.Columns;
            Matrix X = (Matrix)rhs.Clone();
            int m = QR.RowCount;
            int n = QR.Columns;
            double[][] qr = QR.Array;

            // Compute Y = transpose(Q)*B
            for (int k = 0; k < n; k++)
            {
                for (int j = 0; j < count; j++)
                {
                    double s = 0.0;
                    for (int i = k; i < m; i++)
                        s += qr[i][k] * X[i, j];
                    s = -s / qr[k][k];
                    for (int i = k; i < m; i++)
                        X[i, j] += s * qr[i][k];
                }
            }

            // Solve R*X = Y;
            for (int k = n - 1; k >= 0; k--)
            {
                for (int j = 0; j < count; j++)
                    X[k, j] /= Rdiag[k];

                for (int i = 0; i < k; i++)
                    for (int j = 0; j < count; j++)
                        X[i, j] -= X[k, j] * qr[i][k];
            }

            return (Matrix)X.Submatrix(0, n - 1, 0, count - 1);
        }

        public bool IsFullRank
        {
            get
            {
                int columns = QR.Columns;
                for (int j = 0; j < columns; j++)
                {
                    var Val = Rdiag[j];
                    if (Val == 0)
                        return false;
                }
                return true;
            }
        }

        /// <summary>
        /// 直角三角形的斜边
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private double Hypotenuse(double a, double b)
        {
            if (Math.Abs(a) > Math.Abs(b))
            {
                double r = b / a;
                return Math.Abs(a) * Math.Sqrt(1 + r * r);
            }

            if (b != 0)
            {
                double r = a / b;
                return Math.Abs(b) * Math.Sqrt(1 + r * r);
            }

            return 0.0;
        }
    }
}
