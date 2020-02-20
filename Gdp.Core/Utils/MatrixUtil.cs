//2012, czs, create in zz, 矩阵工具类，基于二维数组

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Gdp.Utils
{
    /// <summary>
    /// 矩阵、向量工具。
    /// </summary>
    public static class MatrixUtil
    {
        #region  矩阵的创建
        /// <summary>
        /// 通过一维列向量获取二维数组 n x 1 或 1 x n。
        /// </summary>
        /// <param name="colVector"></param>
        /// <returns></returns>
        static public double[][] GetMatrix(double[] colVector, bool isColOrRow = true)
        {
            int len = colVector.Length;
            int row = isColOrRow ? len : 1;
            int col = isColOrRow ? 1 : len;

            double[][] matrix = new double[row][]; 

            for (int i = 0; i < row; i++) {
                matrix[i] = new double[col];
            }
            int k = 0;
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    matrix[i][j] = colVector[k];
                    k++;
                }
            }
            return matrix;
        }

        /// <summary>
        /// 复制矩阵。
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static double[][] Clone(double[][] matrix)
        {
            int row = matrix.Length;
            int col = matrix[0].Length;
            double[][] newMatrix = Create(row, col);
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    newMatrix[i][j] = matrix[i][j];
                }
            }
            return newMatrix;
        }

        /// <summary>
        /// 通过一维列向量获取二维数组 n x 1。
        /// </summary>
        /// <param name="colVector"></param>
        /// <returns></returns>
        static public double[][] Create(double[] colVector)
        {
            double[][] matrix = new double[colVector.Length][];
            for (int i = 0; i < colVector.Length; i++) matrix[i] = new double[1];
            for (int i = 0; i < colVector.Length; i++) matrix[i][0] = colVector[i];
            return matrix;
        } 
        /// <summary>
        /// 创建只有一个元素的矩阵。
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static double[][] Create(double val)
        {
            double[][] sub = Create(1, 1);
            sub[0][0] = val;
            return sub;
        }

        public static double[][] Create(double[,] p)
        {
            int row = p.GetLength(0);
            int col = p.GetLength(1);
            double[][] matrix = new double[row][];
            for (int i = 0; i < row; i++)
            {
                matrix[i] = new double[col];
                for (int j = 0; j < col; j++)
                {
                    matrix[i][j] = p[i, j];
                }
            }
            return matrix;
        }
        /// <summary>
        /// 构建以两个矩阵为对角线的大矩阵。
        /// 默认2个矩阵为方形矩阵。
        /// </summary>
        /// <param name="coeffOfParams"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double[][] BuildMatrix(double[][] a, double[][] b)
        {
            int row = a.Length + b.Length;
            int col = row;
            int aLen = a.Length;
            double[][] c = new double[row][];
            for (int i = 0; i < row; i++) c[i] = new double[col];

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    if (i < aLen && j < aLen)
                        c[i][j] = a[i][j];
                    if (i >= aLen && j >= aLen)
                        c[i][j] = b[i - aLen][j - aLen];
                }
            }
            return c;
        }
        /// <summary>
        /// 创建一个方阵。并指定所有元素的初始值。
        /// </summary>
        /// <param name="rowCol">行数量</param>
        /// <param name="initVal">列数量</param>
        /// <returns></returns>
        public static double[][] Create(int rowCol, double initVal = 0.0) { return Create(rowCol, rowCol, initVal); }
        /// <summary>
        /// 创建一个矩阵。并指定所有元素的初始值。
        /// </summary>
        /// <param name="row">行数量</param>
        /// <param name="col">列数量</param>
        /// <param name="initVal">元素默认值</param>
        /// <returns></returns>
        public static double[][] Create(int row, int col, double initVal = 0.0)
        {
            return CreateT<double>(row, col, initVal);
        }

        /// <summary>
        /// 采用 单精度型计算。
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="initVal"></param>
        /// <returns></returns>
        public static float[][] CreateFloat(int row, int col, float initVal = 0.0f)
        {
            return CreateT<float>(row, col, initVal);
        }   
        
        /// <summary>
        /// 采用 单精度型计算。
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="initVal"></param>
        /// <returns></returns>
        public static Decimal[][] CreateDecimal(int row, int col, Decimal initVal = 0.0m)
        {
            return CreateT<Decimal>(row, col, initVal);
        } 
        /// <summary>
        /// 创建一个矩阵。并指定所有元素的初始值。
        /// </summary>
        /// <param name="row">行数量</param>
        /// <param name="col">列数量</param>
        /// <param name="initVal">元素默认值</param>
        /// <returns></returns>
        public static T[][] CreateT<T>(int row, int col, T initVal = default(T))
        {
            var defaultVal = default(T);
            var isSetInitVal = initVal.Equals(defaultVal);

            T [][] matrix = new T[row][];
            for (int i = 0; i < row; i++) matrix[i] = new T[col];

            if (isSetInitVal) { for (int i = 0; i < row; i++) for (int j = 0; j < col; j++) matrix[i][j] = initVal; }

            return matrix;
        }
        /// <summary>
        /// 创建一个 n*n 的单位对角阵
        /// </summary>
        /// <param name="len">对角阵阶次</param>
        /// <returns></returns>
        static public double[][] CreateIdentity(int len) { return CreateDiagonal(len); }
        /// <summary>
        /// 创建对角阵。
        /// </summary>
        /// <param name="len"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        static public double[][] CreateDiagonal(int len, double val = 1.0)
        {
            double[][] matrix = Create(len);
            for (int i = 0; i < len; i++) matrix[i][i] = val;
            return matrix;
        }
        /// <summary>
        /// 用对角一维数组，创建一个二维矩阵。
        /// </summary>
        /// <param name="vector">对角一维数组</param>
        /// <returns></returns>
        public static double[][] CreateWithDiagonal(double[] vector)
        {
            int len = vector.Length;
            double[][] matrix = Create(len);
            for (int i = 0; i < len; i++) matrix[i][i] = vector[i];
            return matrix;
        }
        /// <summary>
        /// 创建方阵，并以指定范围的随机数填充。
        /// </summary>
        /// <param name="rowCol"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static double[][] CreateRandom(int rowCol, double from = 0.0, double to = 1.0)
        {
            return CreateRandom(rowCol, rowCol, from, to);
        }
        static Random random = new Random();
        /// <summary>
        /// 创建矩阵，并以指定范围的随机数填充。
        /// </summary>
        /// <param name="row">行</param>
        /// <param name="col">列</param>
        /// <param name="from">随机数最小值</param>
        /// <param name="to">随机数最大值</param>
        /// <returns></returns>
        public static double[][] CreateRandom(int row, int col, double from = 0.0, double to = 1.0)
        {
            double len = (to - from);
            double[][] matrix = new double[row][];
            for (int i = 0; i < row; i++) matrix[i] = new double[col];

            for (int i = 0; i < row; i++)
                for (int j = 0; j < col; j++) matrix[i][j] = random.NextDouble() * len + from;

            return matrix;
        }
        /// <summary>
        /// 创建一组随机的向量
        /// </summary>
        /// <param name="dimension">维数</param>
        /// <param name="from">最小</param>
        /// <param name="to">最大</param>
        /// <returns></returns>
        public static double[] CreateRandomVector(int dimension, double from = 0.0, double to = 1.0)
        {
            double len = (to - from);
            double[] vector = new double[dimension];

            for (int i = 0; i < dimension; i++)
                vector[i] = random.NextDouble() * len + from;

            return vector;
        }
        /// <summary>
        /// 随机数数组。
        /// </summary>
        /// <param name="dimension"></param>
        /// <returns></returns>
        public static double[] CreateRandomVector(int dimension)
        {
            double[] array = new double[dimension];
            Random random = new Random(dimension);
            for (int i = 0; i < dimension; i++)
            {
                array[i] = random.NextDouble();
            }
            return array;
        }
        /// <summary>
        /// 随机数矩阵
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="colCount"></param>
        /// <returns></returns>
        public static double[][] CreateRandom(int rowCount, int colCount)
        {
            double[][] array = Gdp.Utils.MatrixUtil.Create(rowCount, colCount);

            Random random = new Random(rowCount * colCount);
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    array[i][j] = random.NextDouble();
                }
            }
            return array;
        }
        #endregion

        #region 矩阵，子矩阵，向量转换
        #region 对称变换
        /// <summary>
        /// 对称矩阵重新排列，按照给定列表的索引顺序排列，即 i 行和 indeses[i] 行互换。
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="indexes"></param>
        public static void SymmetricExchange(double[][] matrix, List<int> indexes)
        {
            int row = indexes.Count;
            for (int i = 0; i < row; i++)
            {
                SymmetricExchange(matrix, i, indexes[i]);
            }
        }

        /// <summary>
        /// 对称矩阵行列互换。对矩阵本身操作。
        /// 适用场合：权阵、协方差阵的参数变换。
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="indexA"></param>
        /// <param name="indexB"></param>
        /// <returns></returns>
        public static void SymmetricExchange(double[][] matrix, int indexA, int indexB)
        {
            if (indexA == indexB) return;

            // 交换行
            ExchangeRow(matrix, indexA, indexB);
            //交换列
            ExchangeCol(matrix, indexA, indexB);
        }

        /// <summary>
        /// 指定列互换
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="indexA"></param>
        /// <param name="indexB"></param>
        public static void ExchangeCol(double[][] matrix, int indexA, int indexB)
        {
            int len = matrix.Length;
            for (int i = 0; i < len; i++)
            {
                double temp = matrix[i][indexA];
                matrix[i][indexA] = matrix[i][indexB];
                matrix[i][indexB] = temp;
            }
        }
        /// <summary>
        /// 指定行元素互换
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="indexA"></param>
        /// <param name="indexB"></param>
        public static void ExchangeRow(double[][] matrix, int indexA, int indexB)
        {
            double[] row = matrix[indexA];
            matrix[indexA] = matrix[indexB];
            matrix[indexB] = row;
        }

        /// <summary>
        /// 交换向量元素。对向量本身操作。
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="indexA"></param>
        /// <param name="indexB"></param>
        public static void SymmetricExchange(double[] vector, int indexA, int indexB)
        {
            if (indexA == indexB) return;

            double tmp = vector[indexA];
            vector[indexA] = vector[indexB];
            vector[indexB] = tmp;
        }
        #endregion
        /// <summary>
        /// 将子矩阵插入到主矩阵中。
        /// </summary>
        /// <param name="mainMatrix"></param>
        /// <param name="subMatrix"></param>
        /// <param name="startMainRowIndex"></param>
        /// <param name="startMainColIndex"></param>
        /// <param name="startSubRowIndex">含</param>
        /// <param name="startSubColIndex"></param>
        /// <param name="maxSubRowLen">最大长度(含)</param>
        /// <param name="maxSubColLen">最大长度(含)</param>
        public static void SetSubMatrix(
            double[][] mainMatrix,
            double[][] subMatrix,
            int startMainRowIndex = 0,
            int startMainColIndex = 0,
            int startSubRowIndex = 0,
            int startSubColIndex = 0,
            int maxSubRowLen = int.MaxValue,
            int maxSubColLen = int.MaxValue
            )
        {
            for (int i = 0;
                   i + startMainRowIndex < mainMatrix.Length //主矩阵行编号不可大于其长度
                && i + startSubRowIndex < subMatrix.Length //
                && i < maxSubRowLen; i++)
            {
                for (int j = 0;
                    startMainColIndex + j < mainMatrix[0].Length
                    && j + startSubColIndex < subMatrix[0].Length
                    && j < maxSubColLen; j++)
                {
                    mainMatrix[startMainRowIndex + i][startMainColIndex + j] = subMatrix[i + startSubRowIndex][j + startSubColIndex];
                }
            }
        }
        /// <summary>
        /// 将子向量的值插入到主向量中。
        /// </summary>
        /// <param name="mainVector">主向量</param>
        /// <param name="subVector">子向量</param>
        /// <param name="startMainIndex">插入位置</param> 
        /// <param name="startSubIndex">不含</param>
        /// <param name="maxSubLength">最大长度(含)</param>
        public static void SetSubVector(double[] mainVector, double[] subVector, int startMainIndex = 0, int startSubIndex = 0, int maxSubLength = int.MaxValue)
        {
            for (int i = 0; i + startSubIndex < subVector.Length && i < maxSubLength; i++)
            {
                mainVector[startMainIndex + i] = subVector[i + startSubIndex];
            }
        }
        /// <summary>
        /// 获取对角向量。
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static double[] GetDiagonal(double[][] matrix)
        {
            int row = matrix.Length;
            int col = matrix[0].Length;
            if (row != col) throw new ArgumentException("必须是方阵！");

            double[] vector = new double[row];
            for (int i = 0; i < row; i++)
            {
                vector[i] = matrix[i][i];
            }
            return vector;
        } 
        /// <summary>
        /// 下三角函数，以一维数组表示。
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static double[] GetDownTriangle(double[][] matrix)
        {
            int row = matrix.Length;
            int col = matrix[0].Length;
            if (row != col) throw new ArgumentException("必须是方阵！");

            int count = (row + 1) * row / 2;

            double[] a = new double[count];
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    a[i * (i + 1) / 2 + j] = matrix[i][j];
                }
            }

            return a;
        }
        /// <summary>
        /// 清理方矩，将Index对应的行和列删除，对矩阵瘦身。
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="indexesTobeRemoved"></param>
        /// <returns></returns>
        public static double[][] ShrinkMatrix(double[][] matrix, List<int> indexesTobeRemoved)
        {
            double[][] matrixOk = Create(matrix.Length - indexesTobeRemoved.Count);

            for (int i = 0, iOk = 0; i < matrix.Length; i++)
            {
                if (indexesTobeRemoved.Contains(i)) continue;
                for (int j = 0, jOk = 0; j < matrix.Length; j++)
                {
                    if (indexesTobeRemoved.Contains(j)) continue;
                    matrixOk[iOk][jOk] = matrix[i][j];
                    jOk++;
                }
                iOk++;
            }
            return matrixOk;
        }

        /// <summary>
        ///  以一维数组形式返回矩阵的指定列，以 0 开始编号。默认返回矩阵的第一列。
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="colIndex"></param>
        /// <returns></returns>
        public static double[] GetColVector(double[][] matrix, int colIndex = 0)
        {
            double[] Vector = new double[matrix.Length];
            for (int i = 0; i < matrix.Length; i++) Vector[i] = matrix[i][colIndex];
            return Vector;
        }

        /// <summary>
        /// 获取子矩阵
        /// </summary>
        /// <param name="mainMatrix"></param>
        /// <param name="rowLen"></param>
        /// <param name="colLen"></param>
        /// <param name="fromRow"></param>
        /// <param name="fromCol"></param>
        /// <returns></returns>
        public static double[][] GetSubMatrix(double[][] mainMatrix, int rowLen, int colLen, int fromRow = 0, int fromCol = 0)
        {
            double[][] sub = Create(rowLen, colLen);
            for (int i = fromRow; i < mainMatrix.Length && i < rowLen + fromRow; i++)
            {
                for (int j = fromCol; j < mainMatrix[0].Length && j < colLen + fromCol; j++)
                {
                    sub[i - fromRow][j - fromCol] = mainMatrix[i][j];
                }
            }
            return sub;
        }

        /// <summary>
        /// 获取子矩阵
        /// </summary>
        /// <param name="mainMatrix"></param>
        /// <param name="rowLen"></param>
        /// <param name="colLen"></param>
        /// <param name="fromRow"></param>
        /// <param name="fromCol"></param>
        /// <returns></returns>
        public static decimal[][] GetSubMatrixDecimal(decimal[][] mainMatrix, int rowLen, int colLen, int fromRow = 0, int fromCol = 0)
        {
            decimal[][] sub = CreateDecimal(rowLen, colLen);
            for (int i = fromRow; i < mainMatrix.Length && i < rowLen + fromRow; i++)
            {
                for (int j = fromCol; j < mainMatrix[0].Length && j < colLen + fromCol; j++)
                {
                    sub[i - fromRow][j - fromCol] = mainMatrix[i][j];
                }
            }
            return sub;
        }
        /// <summary>
        /// 设置指定行的值。
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="row"></param>
        /// <param name="val"></param>
        public static void SetRowValue(double[][] matrix, int row, double val = 0.0)
        {
            int col = matrix[0].Length;
            for (int i = 0; i < col; i++)
            {
                matrix[row][i] = val;
            }
        }
        /// <summary>
        /// 设置指定列的值
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="col"></param>
        /// <param name="val"></param>
        public static void SetColValue(double[][] matrix, int col, double val = 0.0)
        {
            int row = matrix.Length;
            for (int i = 0; i < row; i++)
            {
                matrix[i][col] = val;
            }
        }
        /// <summary>
        /// 设置指定行和列的值。
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="rowCol"></param>
        /// <param name="val"></param>
        public static void SetRowColValue(double[][] matrix, int rowCol, double val = 0.0)
        {
            SetRowValue(matrix, rowCol, val);
            SetColValue(matrix, rowCol, val);
        }
        /// <summary>
        /// 交行指定行和列。
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="fromRowCol"></param>
        /// <param name="toRowCol"></param>
        public static void Exchage(double[][] matrix, int fromRowCol, int toRowCol)
        {
            ExchangeRow(matrix, fromRowCol, toRowCol);
            ExchangeCol(matrix, fromRowCol, toRowCol);
        }

        /// <summary>
        /// 清理矩阵，将Index对应的行删除，对矩阵瘦身。
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="indexesTobeRemoved"></param>
        /// <returns></returns>
        public static double[][] ShrinkMatrixRow(double[][] matrix, List<int> indexesTobeRemoved)
        {
            double[][] matrixOk = new double[matrix.Length - indexesTobeRemoved.Count][];
            for (int i = 0; i < matrixOk.Length; i++) matrixOk[i] = new double[matrix.Length];

            for (int i = 0, iOk = 0; i < matrix.Length; i++)
            {
                if (indexesTobeRemoved.Contains(i)) continue;
                for (int j = 0, jOk = 0; j < matrix.Length; j++)
                {
                    //   if (indexesTobeRemoved.Contains(j)) continue;
                    matrixOk[iOk][jOk] = matrix[i][j];
                    jOk++;
                }
                iOk++;
            }
            return matrixOk;
        }


        #endregion

        #region 矩阵计算
        /// <summary>
        /// 对数组本身数据进行求幂运算。
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="pow"></param>
        public static void Pow(double[] vector, double pow)
        {
            int len = vector.Length;
            for (int i = 0; i < len; i++) vector[i] = Math.Pow(vector[i], pow);
        }
        /// <summary>
        /// 对数组本身数据进行求幂运算。
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="pow"></param>
        public static void Pow(double[][] matrix, double pow)
        {
            int len = matrix.Length;
            for (int i = 0; i < len; i++) Pow(matrix[i], pow);
        }
        /// <summary>
        /// 求幂运算，返回结果。
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="pow"></param>
        /// <returns></returns>
        public static double[] GetPow(double[] vector, double pow)
        {
            int len = vector.Length;
            double[] v = new double[len];
            for (int i = 0; i < len; i++) v[i] = Math.Pow(vector[i], pow);
            return v;
        }
        /// <summary>
        /// 求幂运算，返回结果。
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="pow"></param>
        public static double[][] GetPow(double[][] matrix, double pow)
        {
            int len = matrix.Length;
            double[][] m = MatrixUtil.Create(len, matrix[0].Length);
            for (int i = 0; i < len; i++) m[i] = GetPow(matrix[i], pow);
            return m;
        }

        /// <summary>
        /// 所有元素求和。
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static double SumOfCells(double[] vector)
        {
            double sum = 0;
            int len = vector.Length;
            for (int i = 0; i < len; i++) sum += vector[i];
            return sum;
        }
        /// <summary>
        /// 所有元素求和。
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static double SumOfCells(double[][] matrix)
        {
            double sum = 0; int len = matrix.Length;
            for (int i = 0; i < matrix.Length; i++) sum += SumOfCells(matrix[i]);
            return sum;
        }

        /// <summary>
        /// 判断两个矩阵是否相等。
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static bool IsEqual(double[][] A, double[][] B, double tolerance = 1E-13)
        {
            if (A.Length != B.Length || A[0].Length != B[0].Length) return false;
            int row = A.Length;
            int col = A[0].Length;

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    double a = A[i][j];
                    double b = B[i][j];
                    if (Math.Abs(a - b) > tolerance)
                        return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 判断向量是否相等
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static bool IsEqual(double[] p1, double[] p2, double tolerance)
        {
            if (p1.Length != p2.Length) return false;
            int len = p1.Length;
            for (int i = 0; i < len; i++)
            {
                if (Math.Abs(p1[i] - p2[i]) > tolerance) return false;
            }
            return true;
        }
        /// <summary>
        /// 矩阵的转置。
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static double[][] Transpose(double[][] matrix)
        {
            int row = matrix.Length;
            int col = matrix[0].Length;
            double[][] trans = Create(col, row);
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    trans[j][i] = matrix[i][j];
                }
            }
            return trans;
        }
        #region 矩阵的算术操作


        /// <summary>
        /// 与一个数相乘,更新当前数组。
        /// </summary>
        /// <param name="matrixA"></param>
        /// <param name="varFactorA"></param>
        public static void Multiply(double[][] matrixA, double varFactorA) { for (int i = 0; i < matrixA.Length; i++) for (int j = 0; j < matrixA[0].Length; j++)   matrixA[i][j] *= varFactorA; }
        /// <summary>
        /// 与一个数相乘,更新当前数组。
        /// </summary>
        /// <param name="matrixA"></param>
        /// <param name="varFactorA"></param>
        public static void Multiply(double[] matrixA, double varFactorA) { for (int i = 0; i < matrixA.Length; i++)  matrixA[i] *= varFactorA; }

        /// <summary>
        /// 返回乘积后的矩阵。
        /// </summary>
        /// <param name="matrixA"></param>
        /// <param name="varFactorA"></param>
        /// <returns></returns>
        public static double[][] GetMultiply(double[][] matrixA, double varFactorA)
        {
            double[][] matrix = Create(matrixA.Length, matrixA[0].Length);
            for (int i = 0; i < matrixA.Length; i++)
                for (int j = 0; j < matrixA[0].Length; j++)
                    matrix[i][j] = matrixA[i][j] * varFactorA;
            return matrix;
        }
        /// <summary>
        /// 加
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="matrixB"></param>
        /// <returns></returns>
        public static double[][] GetPlus(double[] vector, double[][] matrixB)
        {
            return GetPlus( Create(vector) , matrixB);
        }
        /// <summary>
        /// 返回两个矩阵相加结果。可以处理null。
        /// </summary>
        /// <param name="matrixA"></param>
        /// <param name="matrixB"></param>
        /// <returns></returns>
        public static double[][] GetPlus(double[][] matrixA, double[][] matrixB)
        {
            if (matrixA == null) return Clone(matrixB);
            if (matrixB == null) return Clone(matrixA);

            if (matrixA.Length != matrixB.Length || matrixA[0].Length != matrixB[0].Length) throw new ArgumentException("两个矩阵阶次不一致。");
            int row = matrixA.Length;
            int col = matrixA[0].Length;
            double[][] matrixC = Create(matrixA.Length, matrixA[0].Length);

            for (int i = 0; i < row; i++) for (int j = 0; j < col; j++) matrixC[i][j] = matrixA[i][j] + matrixB[i][j];

            return matrixC;
        }
        /// <summary>
        /// 返回两个矩阵相减结果。
        /// </summary>
        /// <param name="matrixA"></param>
        /// <param name="matrixB"></param>
        /// <returns></returns>
        public static double[][] GetMinus(double[][] matrixA, double[][] matrixB)
        {
            if (matrixA.Length != matrixB.Length || matrixA[0].Length != matrixB[0].Length) throw new ArgumentException("两个矩阵阶次不一致。");
            int row = matrixA.Length;
            int col = matrixA[0].Length;
            double[][] matrixC = Create(matrixA.Length, matrixA[0].Length);

            for (int i = 0; i < row; i++) for (int j = 0; j < col; j++) matrixC[i][j] = matrixA[i][j] - matrixB[i][j];

            return matrixC;
        }

        /// <summary>
        /// 两个矩阵的乘法,原始的计算方法。
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static double[][] Multiply(double[][] left, double[][] right)
        {
            int aRows = left.Length; int aCols = left[0].Length;
            int bRows = right.Length; int bCols = right[0].Length;
            if (aCols != bRows)
                throw new Exception("Out of shape matrices");
            double[][] result = Create(aRows, bCols);
            for (int i = 0; i < aRows; ++i) // each row of MatrixOne
            {
                var resultRow = result[i];
                var leftRow = left[i];
                for (int j = 0; j < bCols; ++j) // each col of MatrixTwo
                {
                    for (int k = 0; k < aCols; ++k)
                    {
                        resultRow[j] += leftRow[k] * right[k][j];
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 两个矩阵的乘法。2013.06.05： creating
        /// </summary>
        /// <param name="a">矩阵A</param>
        /// <param name="b">矩阵B</param>
        /// <returns></returns>
        public static double[][] GetMultiply(double[][] a, double[][] b)
        {
            if (a[0].Length != b.Length) throw new ArgumentException("左矩阵的列数必须等于右矩阵的行数");

            int newRow = a.Length;
            int newCol = b[0].Length;
            int size = a[0].Length;//两矩阵共同尺寸
            double[][] c = Create(newRow, newCol);
            //B 转置，将列向量转换为行向量，方便获取。
            double[][] bT = Transpose(b);

            for (int i = 0; i < newRow; i++)
            {
                double[] rowA = a[i];//A的行

                for (int j = 0; j < newCol; j++)
                {
                    double[] colB = bT[j];//B的列
                    c[i][j] = GetSumOfMultiply(rowA, colB);
                }
            }
            return c;
        }
        #endregion
        #endregion

        #region 向量的算术操作

        public static double[] GetMultiply(double[] p, double right)
        {
            double[] v = new double[p.Length];
            for (int i = 0; i < v.Length; i++)
            {
                v[i] = p[i] * right;
            }
            return v;
        }
        /// <summary>
        /// 两个向量对应元素相乘，向量乘积 Hadamard 积。
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double[] GetMultiply(double[] a, double[] b)
        {
            if (a.Length != b.Length) throw new ArgumentException("两个向量的维数必须相等！");
            int size = a.Length;
            double[] c = new double[size];
            for (int i = 0; i < size; i++) c[i] = a[i] * b[i];
            return c;
        }
        /// <summary>
        /// 两向量对应元素相乘，再相加。相当于 VTV。
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double GetSumOfMultiply(double[] a, double[] b)
        {
            if (a.Length != b.Length) throw new ArgumentException("两个向量的维数必须相等！");
            int size = a.Length;
            double s = 0;
            for (int i = 0; i < size; i++)
                s += a[i] * b[i];
            return s;
        }
        /// <summary>
        /// 两个向量对应元素相除
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double[] GetDivision(double[] a, double[] b)
        {
            if (a.Length != b.Length) throw new ArgumentException("两个向量的维数必须相等！");
            int size = a.Length;
            double[] c = new double[size];
            for (int i = 0; i < size; i++) c[i] = a[i] / b[i];
            return c;
        }
        /// <summary>
        /// 两个向量相减
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double[] GetMinus(double[] a, double[] b)
        {
            if (a.Length != b.Length) throw new ArgumentException("两个向量的维数必须相等！");
            int size = a.Length;
            double[] c = new double[size];
            for (int i = 0; i < size; i++) c[i] = a[i] - b[i];
            return c;
        }
        /// <summary>
        /// 获取相反数
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static double[] GetOpposite(double[] a)
        {
            int size = a.Length;
            double[] c = new double[size];
            for (int i = 0; i < size; i++) c[i] = -a[i];
            return c;
        }
        /// <summary>
        /// 两个向量相加
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double[] GetPlus(double[] a, double[] b)
        {
            if (a == null) return Clone(b);
            if (b == null) return Clone(a);

            if (a.Length != b.Length) throw new ArgumentException("两个向量的维数必须相等。");
            int size = a.Length;
            double[] c = new double[size];

            for (int i = 0; i < size; i++) c[i] = a[i] + b[i];

            return c;
        }
        /// <summary>
        /// 深拷贝
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double[] Clone(double[] b)
        {
            double[] a = new double[b.Length];
            for (int i = 0; i < b.Length; i++)
            {
                a[i] = b[i];
            }
            return a;
        }
        #endregion

        #region 矩阵IO

        #region 二进制IO
        /// <summary>
        /// 以二进制文件形式保存。
        /// </summary>
        /// <param name="matrix">矩阵</param>
        /// <param name="path">路径</param>
        public static void SaveToBinary(double[][] matrix, string path)
        {
        using (BinaryWriter bw = new BinaryWriter(new FileStream(path, FileMode.Create, FileAccess.Write)))
            {
                SaveToBinary(matrix, bw);
                bw.Close();
            }
        }

        /// <summary>
        /// 将矩阵以二进制形式保存
        /// 格式定义：1. 前两个位int，分别指行和列；2.后面为双精度。
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="bw"></param>
        public static void SaveToBinary(double[][] matrix, BinaryWriter bw)
        {
            int row = matrix.Length;
            int col = matrix[0].Length;
            bw.Write(row);
            bw.Write(col);
            //BitConverter.GetBytes(row);

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    bw.Write(matrix[i][j]);
                }
            }
        }
        /// <summary>
        /// 从二进制文件中读取。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static double[][] ReadFromBinary(string path)
        {
        using (BinaryReader br = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read)))
            {
                return ReadFromBinary(br);
            }
        }

        /// <summary>
        /// 从二进制读取矩阵。
        /// 格式定义：1. 前两个位int，分别指行和列；2.后面为双精度。
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        public static double[][] ReadFromBinary(BinaryReader br)
        {
            int row = br.ReadInt32();
            int col = br.ReadInt32();
            double[][] matrix = MatrixUtil.Create(row, col);

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    matrix[i][j] = br.ReadDouble();
                }
            }
            return matrix;
        }
        #endregion

        #region 文本存储IO
        /// <summary>
        /// 以文本形式保存。
        /// 格式说明：第一行为两个整形： 行 列，以后一行为矩阵行，间隔符可以指定，默认为逗号（“，”）
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="printZero">是否打印 0， 不打印则为空</param>
        /// <param name="path"></param>
        public static void SaveToText(double[][] matrix, string path, bool printZero = false, char spliter = ',')
        {
        using (StreamWriter bw = new StreamWriter(new FileStream(path, FileMode.Create, FileAccess.Write)))
            {
                int row = matrix.Length;
                int col = matrix[0].Length;
                bw.WriteLine(row.ToString() + spliter.ToString() + col.ToString());

                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < col; j++)
                    {
                        if (!printZero && matrix[i][j] == 0) bw.Write(" " + spliter);
                        else bw.Write(matrix[i][j].ToString() + spliter);
                    }
                    bw.WriteLine();
                }
                bw.Close();
            }
        }

        /// <summary>
        /// 以文本形式保存。
        /// 格式说明：第一行为两个整形： 行 列，以后一行为矩阵行，间隔符可以指定，默认为逗号（“，”）
        /// </summary>
        /// <param name="vector">待保存向量</param>
        /// <param name="printZero">是否打印 0， 不打印则为空</param>
        /// <param name="path"></param>
        public static void SaveToText(double[] vector, string path, bool printZero = false, char spliter = ',')
        {
            SaveToText(MatrixUtil.Create(vector), path, printZero, spliter);
        }

        /// <summary>
        /// 从文本中读取。
        /// 格式说明：第一行为两个整形： 行 列，以后一行为矩阵行，间隔符可以指定，默认为逗号（“，”）
        /// </summary>
        /// <param name="path"></param>
        /// <param name="spliter"></param>
        /// <returns></returns>
        public static double[][] ReadFromText(string path, char spliter = ',')
        {
        using (StreamReader br = new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read)))
            {
                char[] spliters = new char[] { spliter };
                string line = br.ReadLine();
                string[] strings = line.Split(spliters, StringSplitOptions.RemoveEmptyEntries);
                int row = int.Parse(strings[0]);
                int col = int.Parse(strings[1]);
                double[][] matrix = MatrixUtil.Create(row, col);

                for (int i = 0; i < row; i++)
                {
                    line = br.ReadLine();
                    strings = line.Split(spliters, StringSplitOptions.RemoveEmptyEntries);
                    for (int j = 0; j < col; j++)
                    {
                        if (strings[j].Trim() != "")//若为空则为0.
                            matrix[i][j] = double.Parse(strings[j]);
                    }
                }
                return matrix;
            }
        }

        #endregion
        /// <summary>
        /// 打印数组方便查看。适用于巨大耗费内存的矩阵。
        /// 格式说明：第一行为两个整形： 行 列，以后一行为矩阵行，间隔符可以指定，默认为逗号（“，”）
        /// </summary>
        /// <param name="outStream"></param>
        /// <param name="matrix"></param>
        public static void Print(Stream outStream, double[][] matrix, char spliter = ',')
        {
            int width = 15;
        using (StreamWriter writer = new StreamWriter(outStream, Encoding.UTF8))
            {
                writer.WriteLine(matrix.Length + "" + spliter + "" + matrix[0].Length);
                foreach (var array in matrix)
                {
                    foreach (var item in array)
                    {
                        if (item == 0) writer.Write(StringUtil.FillSpace(item.ToString(), width) + spliter.ToString());
                        else writer.Write(StringUtil.FillSpace(DoubleUtil.ScientificFomate(item, "E14.10"), width) + spliter.ToString());
                    }
                    writer.WriteLine();
                }
            }
        }

        /// <summary>
        /// 格式化的字符块。
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static string GetFormatedText(double[] vector)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(vector.Length + "," + "× 1");
            foreach (var item in vector)
            {
                string formatedItem = GetForamtedDouble(item);
                sb.AppendLine(formatedItem);
            }
            return sb.ToString();
        }
        /// <summary>
        /// 格式化的字符块。
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static string GetFormatedText(double[][] matrix, char spliter = ',')
        {
            if (matrix == null) return "";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(matrix.Length + "×" + matrix[0].Length);

            foreach (var array in matrix)
            {
                foreach (var item in array)
                {
                    string formatedItem = GetForamtedDouble(item) + spliter.ToString();
                    sb.Append(formatedItem);
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
        /// <summary>
        /// 格式化双精度
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string GetForamtedDouble(double item)
        {
            string formatedItem = null;
            if (StringUtil.IsNumber( Math.Abs(item)) && item < 1e7 && item > -1e7) formatedItem = StringUtil.FillSpaceLeft(item.ToString(), 13);
            else formatedItem = DoubleUtil.ScientificFomate(item, "E13.6");
            return formatedItem;
        }
        #endregion



        /// <summary>
        /// 欧式距离。元素平方和开方。
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static double GetEuclideanDistance(double[] p)
        {
            double sum = 0;
            foreach (var item in p)
            {
                sum += item * item;
            }
            return Math.Sqrt(sum);
        }
        /// <summary>
        /// 是否是方阵
        /// </summary>
        /// <param name="matrix">待检核矩阵</param>
        /// <returns></returns>
        public static bool IsSqure(double[][] matrix)
        {
            return matrix.Length == matrix[0].Length;
        }
        /// <summary>
        /// 矩阵乘法。
        /// </summary>
        /// <param name="mat">二维矩阵</param>
        /// <param name="vector">一维向量</param>
        /// <returns></returns>
        public static double[] GetMultiply(double[][] mat, double[] vector)
        {
            if (mat[0].Length != vector.Length)
                throw new ArgumentException("矩阵的列数应该和向量的行数相等。");
            int row = mat.Length;
            int col = mat[0].Length;
            double[] result = new double[row];

            for (int i = 0; i < row; i++)
            { 
                double[] rowVec = mat[i]; 
                result[i] = GetMultiPlus(vector, rowVec);
            }
            return result;
        }
        /// <summary>
        /// 向量元素对应相乘，并相加。
        /// </summary>
        /// <param name="vec1">一维向量</param>
        /// <param name="vec2">一维向量</param>
        /// <returns></returns>
        public static double GetMultiPlus(double[] vec1, double[] vec2)
        {
            double temp = 0;
            int dim = vec1.Length;
            for (int j = 0; j < dim; j++)
            {
                temp += vec2[j] * vec1[j];
            }
            return temp;
        }
    }
}
