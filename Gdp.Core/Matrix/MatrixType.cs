//2014.09.28， czs, create, hailutu tongliao, 创建向量接口，为后继实现自我的、快速的、大规模的矩阵计算做准备

using System;

namespace Gdp
{
    /// <summary>
    /// 矩阵数据结构类型。默认以Double形式表示。
    /// </summary>
    public enum MatrixType
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unknow,
        /// <summary>
        /// 二维数组
        /// </summary>
        Array,
        /// <summary>
        /// 一维数组
        /// </summary>
        Vector,
        /// <summary>
        /// 稀疏矩阵
        /// </summary>
        Sparse,
        /// <summary>
        /// 全部由Float数据类型表示的矩阵。
        /// </summary>
        Float,
        /// <summary>
        /// 对角线是Double，非对角线是Float类型.
        /// 即具有 float 翅膀。
        /// </summary>
        FloatWing,
        /// <summary>
        /// 对角线一维数组
        /// </summary>
        Diagonal,
        /// <summary>
        /// 对称矩阵，一般采用一维数据存储
        /// </summary>
        Symmetric,
        /// <summary>
        /// 只有一列的矩阵，也是向量。
        /// </summary>
        VectorMatrix,
        /// <summary>
        /// 全 0 矩阵
        /// </summary>
        ZeroMatrix,
        /// <summary>
        /// 全 常数 矩阵
        /// </summary>
        ConstMatrix,
        /// <summary>
        /// 可以调整大小的矩阵
        /// </summary>
        ResizeableMatrix,
    }
}
