//2014.09.18, czs, create, hailutu tongliao, 创建矩阵接口，为后继实现自我的、快速的、大规模的矩阵计算做准备
//2015.01.14, czs, edit, 增加行列名称
//2017.07.19,, czs, edit in hongqing, 增加除法
//2017.08.31, czs, edit in hongqing, 增加元素求幂接口


using System;
using System.Collections.Generic;

namespace Gdp
{
    /// <summary>
    /// Geo 矩阵接口。
    /// </summary>
    public interface IMatrix //: IArithmeticOperation<IMatrix>
    {
        /// <summary>
        /// 行名称
        /// </summary>
          List<string> RowNames { get; set; }

        /// <summary>
        /// 列名称
        /// </summary>
          List<string> ColNames { get; set; }
        /// <summary>
        /// 行名称是否有效，可用。
        /// </summary>
        bool IsRowNameAvailable { get; }
        /// <summary>
        /// 列名称是否有效，可用。
        /// </summary>
        bool IsColNameAvailable { get; }
        /// <summary>
        /// 是否具有有效的行列名称
        /// </summary>
        bool HasParamNames { get; }
        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; set; }
        
        /// <summary>
        /// 是否包含行名称
        /// </summary>
        /// <param name="rowName">行名称</param>
        /// <returns></returns>
        bool ContainsRowName(string rowName);

        /// <summary>
        /// 是否包含列名称
        /// </summary>
        /// <param name="colName">列名称</param>
        /// <returns></returns>
        bool ContainsColName(string colName);

        /// <summary>
        /// 获取指定参数的向量
        /// </summary>
        /// <param name="paramNames">参数列表</param>
        /// <returns></returns>
        IMatrix GetMatrix(List<String> paramNames);

          /// <summary>
        /// 获取指定参数的向量
        /// </summary>
        /// <param name="rowNames">行参数列表</param>
        /// <param name="colNames">列参数列表</param>
        /// <returns></returns>
        IMatrix GetMatrix(List<String> rowNames, List<String> colNames);
        /// <summary>
        /// 批量移除行，返回新矩阵
        /// </summary>
        /// <param name="rowIndexes"></param>
        /// <returns></returns>
        IMatrix GetRowRemovedMatrix(List<int> rowIndexes);
        /// <summary>
        /// 移除指定行,返回新矩阵
        /// </summary>
        /// <param name="rowIndex"></param>
        IMatrix GetRowRemovedMatrix(int rowIndex);
        /// <summary>
        /// 批量移除列，返回新矩阵
        /// </summary>
        /// <param name="colIndexes"></param>
        /// <returns></returns>
         IMatrix GetColRemovedMatrix(List<int> colIndexes);
        /// <summary>
        /// 移除指定列,返回新矩阵
        /// </summary>
        /// <param name="colIndex"></param>
       IMatrix GetColRemovedMatrix(int colIndex);
        /// <summary>
        /// 矩阵类型，用于快速判断其存储结构。
        /// </summary>
        MatrixType MatrixType { get; }
        /// <summary>
        /// 两个数字最大差，认为相等。
        /// </summary>
        double Tolerance { get; set; }
        /// <summary>
        /// 返回二维数组。
        /// </summary>
        double[][] Array { get; }
        /// <summary>
        /// 所有元素总和。指有效的内容表示数，矩阵内容必须的数。
        /// </summary>
        int ItemCount { get; }
        /// <summary>
        /// 列数
        /// </summary>
        int ColCount { get; }
        /// <summary>
        /// 行数
        /// </summary>
        int RowCount { get; }
        /// <summary>
        /// 是否是方阵。
        /// </summary>
        bool IsSquare { get; }
        /// <summary>
        /// 是否是对称矩阵
        /// </summary>
        bool IsSymmetric { get; }
        /// <summary>
        /// 是否对角阵
        /// </summary>
        bool IsDiagonal { get; }

        //public IMatrix operator +(IMatrix left, IMatrix right);
         /// <summary>
         /// 获取指定的元素。
         /// </summary>
         /// <param name="i">行编号</param>
         /// <param name="j">列编号</param>
        /// <returns></returns>
        double this[int i, int j] { get; set; }
        /// <summary>
        /// 获取指定的元素
        /// </summary>
        /// <param name="rowName">行名称</param>
        /// <param name="colName">列名称</param>
        /// <returns></returns>
        double this[string rowName, string colName] { get; set; }

        /// <summary>
        /// 设置某行的值为统一的数值
        /// </summary>
        /// <param name="rowIndex">行编号</param>
        /// <param name="val">数值</param>
        void SetRowValue(int rowIndex, double val);

        /// <summary>
        /// 设置某列的值为统一的数值
        /// </summary>
        /// <param name="colIndex">列编号</param>
        /// <param name="val">数值</param>
        void SetColValue(int colIndex, double val);

        /// <summary>
        /// 获取行编号
        /// </summary>
        /// <param name="paramName">参数名称</param>
        /// <returns></returns>
        int GetRowIndex(String paramName);
        /// <summary>
        /// 获取列编号
        /// </summary>
        /// <param name="paramName">参数名称</param>
        int GetColIndex(String paramName);
        /// <summary>
        /// 行向量
        /// </summary>
        /// <param name="colIndex">编号</param>
        /// <returns></returns>
        Vector GetRow(int colIndex);
        /// <summary>
        /// 列向量
        /// </summary>
        /// <param name="rowIndex">编号</param>
        /// <returns></returns>
        Vector GetCol(int rowIndex); 
        /// <summary>
        /// 获取子矩阵,方阵。
        /// </summary>
        /// <param name="fromIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        IMatrix SubMatrix(int fromIndex, int count);

        #region 计算 
        /// <summary>
        /// 求幂
        /// </summary>
        /// <param name="power"></param>
        /// <returns></returns>
        IMatrix Pow(double power);
        /// <summary>
        /// 转置
        /// </summary>
        /// <returns>返回新的结果矩阵</returns>
        IMatrix Transposition { get; }

        /// <summary>
        /// 求逆
        /// </summary>
        IMatrix GetInverse();
        /// <summary>
        /// 全克隆
        /// </summary>
        IMatrix Clone();


        #endregion

    }
}
