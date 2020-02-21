//2016.05.10, czs, edit， 滑动窗口数据
//2016.08.04, czs, edit in fujian yongan, 修正
//2016.08.29, czs, create in hongqing，提取窗口数据接口
//2017.09.11, czs, edit in hongiqng, 重构，为了更好的使用

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gdp.IO;

namespace Gdp
{


    /// <summary>
    /// 具有检错的窗口数据，可以判断是否连续而重置的窗口数据。
    /// 检索数据默认按照一定的顺序，支持从大到校和从小到大
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class IndexedWindowData<TValue> : WindowData<TValue>
    {
        /// <summary>
        /// 具有检错的窗口数据构造函数
        /// </summary>
        /// <param name="windowSize"></param>
        /// <param name="IsResetWhenIndexBreak">当断裂时，是否重置 清空</param>
        /// <param name="MaxBreakCount">允许最大的断裂数量</param>
        public IndexedWindowData(int windowSize, bool IsResetWhenIndexBreak = true, int MaxBreakCount = 5)
            : base(windowSize)
        {
            this.IsResetWhenIndexBreaked = IsResetWhenIndexBreak;
            this.MaxBreakCount = MaxBreakCount;
        }
         
         /// <summary>
        /// 具有检错的窗口数据 以数值初始化
        /// </summary>
        /// <param name="values"></param>
        /// <param name="IsResetWhenIndexBreak">当断裂时，是否重置 清空</param>
        /// <param name="MaxBreakCount">允许最大的断裂数量</param>
        /// <param name="name"></param>
        public IndexedWindowData(IEnumerable<TValue> values,bool IsResetWhenIndexBreak = true, int MaxBreakCount = 5, string name = "未命名检索窗口数据")
            : base(values, name)
        { 
            this.IsResetWhenIndexBreaked = IsResetWhenIndexBreak;
            this.MaxBreakCount = MaxBreakCount;
        }

        #region 编号相关属性
        /// <summary>
        /// 允许的最大的非连续数量。
        /// </summary>
        public int MaxBreakCount { get; set; }

        /// <summary>
        /// 当数据编号断裂（超过最大非连续数量）时，是否重置数据。
        /// </summary>
        public bool IsResetWhenIndexBreaked { get; set; }

        /// <summary>
        /// 当前数据编号
        /// </summary>
        public int CurrentIndex { get; set; }
        #endregion

        /// <summary>
        /// 编号是否断裂
        /// </summary>
        /// <param name="newIndex"></param>
        /// <returns></returns>
        public Boolean IsIndexBreaked(int newIndex) { return Math.Abs(newIndex - CurrentIndex) > MaxBreakCount; }

        /// <summary>
        /// 添加一个具有编号的数据。
        /// </summary>
        /// <param name="index"></param>
        /// <param name="val"></param>
        public void Add(int index, TValue val)
        {
            if (IsResetWhenIndexBreaked && IsIndexBreaked(index))
            {
                this.Clear();
            }
            //update
            CurrentIndex = index;

            this.Add(val);
        }
    }

}