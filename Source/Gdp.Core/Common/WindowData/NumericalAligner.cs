//2017.10.02, czs, create in hongqing, 数据对齐器


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gdp.IO;


namespace Gdp
{
    /// <summary>
    /// 数据对齐管理器
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TIndex"></typeparam>
    public class NumericalAlignerManager<TKey, TIndex> : BaseDictionary<TKey, NumericalAligner<TIndex>>
          where TKey : IComparable<TKey>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="MaxSan"></param>
        /// <param name="IndexToNumerial"></param>
        public NumericalAlignerManager(double MaxSan, Func<TIndex, double> IndexToNumerial)
        {
            this.MaxSan = MaxSan;
            this.IndexToNumerial = IndexToNumerial; 
        }
        /// <summary>
        /// 方法转换为数字
        /// </summary>
        public Func<TIndex, double> IndexToNumerial;
        /// <summary>
        /// 允许的最大断裂。
        /// </summary>
        public double MaxSan { get; set; }

        /// <summary>
        /// 创建一个
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override NumericalAligner<TIndex> Create(TKey key)
        {
            return new NumericalAligner<TIndex>(MaxSan, IndexToNumerial);
        }
    }

    /// <summary>
    /// 数据对齐器。
    /// </summary>
    public class NumericalAligner<TIndex>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="MaxSan"></param>
        /// <param name="IndexToNumerial"></param>
        public NumericalAligner(double MaxSan, Func<TIndex, double> IndexToNumerial)
        {
            this.MaxSan = MaxSan;
            this.IndexToNumerial = IndexToNumerial;
            this.IsReset = true;
        }
        #region 属性
        /// <summary>
        /// 方法转换为数字
        /// </summary>
        public Func<TIndex, double> IndexToNumerial;
        /// <summary>
        /// 允许的最大断裂。
        /// </summary>
        public double MaxSan { get; set; }
        /// <summary>
        /// 是否是重置
        /// </summary>
        public bool IsReset { get; set; }
        /// <summary>
        /// 上一个对齐后的数值
        /// </summary>
        public double LastAlignedValue { get; set; }
        /// <summary>
        /// 最后一个编号数值
        /// </summary>
        public double LastIndexValue { get; set; }
        /// <summary>
        /// 最后一个编号
        /// </summary>
        public TIndex LastIndex { get; set; }
        /// <summary>
        /// 调整数，对齐数。
        /// </summary>
        public double AdjustValue { get; set; }
        /// <summary>
        /// 重置编号
        /// </summary>
        public int ResetIndex { get; set; }
        #endregion

        /// <summary>
        /// 获取对齐后的
        /// </summary>
        /// <param name="index"></param>
        /// <param name="val"></param>
        /// <param name="referVal"></param>
        /// <returns></returns>
        public double GetAlignedValue(TIndex index, double val, double referVal)
        {
            if (IsReset || IsBreaked(index)) { 
                this.AdjustValue = referVal - val; 
                this.IsReset = false;
            }

            LastAlignedValue = val + AdjustValue;
            return LastAlignedValue;
        }
        /// <summary>
        /// 根据编号判断是否断裂
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool IsBreaked(TIndex index)
        {
            var newIndexVal = IndexToNumerial(index);
            var isbreaked = Math.Abs(newIndexVal - this.LastIndexValue) >= this.MaxSan;
           
            //算后更新
            UpdateValue(index, newIndexVal);
            return isbreaked;
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="index"></param>
        /// <param name="newIndexVal"></param>
        private void UpdateValue(TIndex index, double newIndexVal)
        {
            this.LastIndexValue = newIndexVal;
            this.LastIndex = index;
        }
    }

}