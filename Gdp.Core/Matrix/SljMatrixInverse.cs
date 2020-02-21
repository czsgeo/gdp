//2014.07， cy，  转换
//2018.10.27, czs, craete in hmx, 代码整理，重构

namespace Gdp
{
    using System;
    using Gdp.IO;

    /// <summary>
    /// 借鉴的宋立杰的对称矩阵求逆算法。
    /// </summary>
    public class SljMatrixInverse
    {
        Log log = new Log(typeof(SljMatrixInverse));

        public SljMatrixInverse(Matrix A)
        {
            LA = A;
            int rows = A.RowCount;
            int columns = A.Columns;
        }

        /// <summary>
        /// 待求系数阵
        /// </summary>
        public Matrix LA { get; set; }

        /// <summary>Returns if the matrix is non-singular.</summary>
        [Obsolete("判断方式可能有问题，下列程序只判断了 a[0,0],有待解决？？？2017.09.06, kyc@czs")]
        public bool IsNonSingular
        {
            get
            {
                for (int j = 0; j < LA.Columns; j++)
                    if (LA[j, j] == 0 || LA[j, j] + 1 == 1)
                        return false;
                return true;
            }
        }

        /// <summary>Solves a set of equation systems of type <c>A * X = B</c>.</summary>
        /// <param name="B">Right hand side matrix with as many rows as <c>A</c> and any number of columns.</param>
        /// <returns>Matrix <c>X</c> so that <c>L * U * X = B</c>.</returns>
        public Matrix Solve(Matrix B)
        {
            if (B.RowCount != LA.RowCount) throw new ArgumentException("Invalid matrix dimensions.只能计算对称矩阵");
            // if (!IsNonSingular) throw new InvalidOperationException("Matrix is singular 单数的；单一的；非凡的；异常的");

            int i, j, k;
            int rows = LA.RowCount;
            int columns = LA.Columns;
            Matrix result = new Matrix(rows, columns);
            int count = (rows + 1) * rows / 2;
            //仅存下三角元素
            double[] lowerTriangular = new double[count];
            for (i = 0; i < rows; i++)
            {
                for (j = 0; j <= i; j++)
                {
                    lowerTriangular[i * (i + 1) / 2 + j] = LA[i, j];
                }
            }

            double[] a0 = new double[rows];

            int n = rows;

            for (k = 0; k < rows; k++)
            {
                double a00 = lowerTriangular[0];
                if (Math.Abs(a00) < 1E-40)
                {
                    var msg = "a00 太小 == 0, in current precision Inverse is wrong";
                    log.Error(msg);
                    throw new Exception(msg);
                }
                for (i = 1; i < n; i++)
                {
                    double ai0 = lowerTriangular[i * (i + 1) / 2];
                    if (i <= n - k - 1) { a0[i] = -ai0 / a00; }
                    else { a0[i] = ai0 / a00; }

                    for (j = 1; j <= i; j++)
                    {
                        lowerTriangular[(i - 1) * i / 2 + j - 1] = lowerTriangular[i * (i + 1) / 2 + j] + ai0 * a0[j];
                    }
                }
                for (i = 1; i < n; i++)
                {
                    lowerTriangular[(n - 1) * n / 2 + i - 1] = a0[i];
                }
                lowerTriangular[n * (n + 1) / 2 - 1] = 1.0 / a00;
            }

            //返回求逆结果
            for (i = 0; i < rows; i++)
            {
                for (j = 0; j <= i; j++)
                {
                    result[i, j] = lowerTriangular[i * (i + 1) / 2 + j];
                }
            }

            //对称
            for (i = 0; i < rows; i++)
            {
                for (j = i; j < columns; j++)
                {
                    result[i, j] = result[j, i];
                }
            }

            return result;
        }
    }
}
