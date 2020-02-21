//2015.04.16, czs, edit in namu, 移除 GetYdY，提取泛型接口
//2018.06.20, czs, edit in HMX,增加时间作为X

using System;


namespace Gdp
{

    /// <summary>
    /// 简单的双精度输入与输出。
    /// </summary>
    public abstract class YGetter : IGetY
    {
        /// <summary>
        /// 获取对应的Y值
        /// </summary>
        /// <param name="xValue"></param>
        /// <returns></returns>
        public abstract double GetY(double xValue);
    }
    /// <summary>
    /// 加入速度获取量
    /// </summary>
    public interface IGetYDY : IGetY
    {
        /// <summary>
        /// 获取Y的变化，即速度
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        double GetDy(double x);
    }
    /// <summary>
    /// 用于差值或拟合,输入输出都为双精度数值。
    /// </summary>
    public interface IGetY : IGetY<Double>
    {
    }
    /// <summary>
    /// 用于差值或拟合,输入输出都为双精度数值。
    /// </summary>
    public interface ITimeXGetY : IGetY<Time, Double>
    {
    }
    /// <summary>
    /// 用于差值或拟合。
    /// </summary>
    public interface IGetY<TValue> : IGetY<TValue, TValue>
    { 
    }
    /// <summary>
    /// 用于差值或拟合。
    /// </summary>
    public interface IGetY<Tkey, TValue>
    {
        /// <summary>
        /// 输入参数，获取函数值
        /// </summary>
        /// <param name="xValue">参数</param>
        /// <returns></returns>
        TValue GetY(Tkey xValue);
    }

}
