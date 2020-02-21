//2014.09.28， czs, create, hailutu tongliao, 创建向量接口，为后继实现自我的、快速的、大规模的矩阵计算做准备
//2014.11.16, czs, edit in namu, 实现 IEnumerable 接口，为了方便使用 Format 类。

using System;
using System.Collections.Generic;

namespace Gdp
{
    /// <summary>
    /// Geo 向量接口。
    /// 纯数字向量，无任务其它信息。
    /// </summary>
    public interface IVector :ICloneable,IEnumerable<Double> //IVectorOperation<IVector>, INumeralIndexing,  , IToTabRow
    {
        /// <summary>
        /// 维数
        /// </summary>
        int Dimension { get; }
        /// <summary>
        /// 创建一个默认的对象。
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        IVector Create(int count);
        /// <summary>
        /// 以一维数组创建。
        /// </summary>
        /// <param name="array">一维数组</param>
        /// <returns></returns>
        IVector Create(double [] array);
        /// <summary>
        /// 参数名称。
        /// </summary>
        List<string> ParamNames { get; set; }
        /// <summary>
        /// 复制一个。
        /// </summary>
        /// <returns></returns>
        //IVector Clone();

        /// <summary>
        /// 是否包含指定参数名称
        /// </summary>
        /// <param name="paramName">参数名称</param>
        /// <returns></returns>
        bool Contains(string paramName);

        /// <summary>
        /// 返回一维数组。
        /// </summary>
        double [] OneDimArray { get; }

        /// <summary>
        /// 元素数量,维数。
        /// </summary>
        int Count { get; }
        /// <summary>
        /// 向量的模/范数/长度/元素平方和的根
        /// </summary>
        double Norm { get; }

         /// <summary>
         /// 获取指定的元素。
         /// </summary>
         /// <param name="i">行编号</param> 
         /// <returns></returns>
        new  double  this[int i] { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        /// <param name="paramName"> 参数名称 </param>
        /// <returns></returns>
        double this[string paramName] {get;set;}
        /// <summary>
        /// 获取参数名称的编号。
        /// </summary>
        /// <param name="paramName">参数名称</param>
        /// <returns></returns>
        int GetIndex(string paramName);
            /// <summary>
        /// 单位向量
        /// </summary>
        IVector UnitVector { get; }
        /// <summary>
        /// 获取指定参数的向量
        /// </summary>
        /// <param name="paramNames">参数列表</param>
        /// <returns></returns>
        IVector GetVector(List<String> paramNames);
        /// <summary>
        /// 获取指定编号的向量
        /// </summary>
        /// <param name="fromIndex">起始编号</param>
        /// <param name="count">参数数量</param>
        /// <returns></returns>
        IVector GetVector(int fromIndex, int count);
        /// <summary>
        /// 向量与指定坐标轴的方向角余弦，CosX = x/r
        /// </summary>
        /// <param name="i">坐标轴/元素下标</param>
        /// <returns></returns>
        double GetCos(int i);
        /// <summary>
        /// 向量与指定坐标轴的方向角。单位：弧度
        /// </summary>
        /// <param name="i">坐标轴编号</param>
        /// <returns></returns>
        double GetDirectionAngle(int i);
        
       /// <summary>
        /// 与另一个向量的夹角余弦。
        /// </summary>
        /// <param name="another">另一个向量</param>
        /// <returns></returns>
        double GetCos(IVector another);
        /// <summary>
        /// 与另一个向量的夹角。单位：弧度
        /// </summary>
        /// <param name="another">另一个向量</param>
        /// <returns></returns>
        double GetIncludedAngle(IVector another);
        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="subVector"></param>
        /// <param name="startMainIndex"></param>
        /// <param name="startSubIndex"></param>
        /// <param name="maxSubLength"></param>
         void SetSubVector( IVector subVector, int startMainIndex = 0, int startSubIndex = 0, int maxSubLength = int.MaxValue);
         
    }
}
