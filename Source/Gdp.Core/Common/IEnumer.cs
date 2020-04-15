//2015.10.25, czs, create in 成都火车站 , 枚举数据源

using System;
using System.Collections.Generic; 
using Gdp.IO;

namespace Gdp
{

    /// <summary>
    /// 枚举数据源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEnumer<T> : IEnumerable<T>, IEnumerator<T>, Namable, ICancelAbale
    {
        /// <summary>
        /// 当前编号，从 0 开始。
        /// </summary>
        int CurrentIndex { get; set; }
        /// <summary>
        /// 起始编号，从0开始。
        /// </summary>
        int StartIndex { get; set; }
        /// <summary>
        /// 遍历数量，默认为最大值的一半。
        /// </summary>
        int EnumCount { get; set; }
        /// <summary>
        /// 最大的循环编号
        /// </summary>
        int MaxEnumIndex { get; }
        /// <summary>
        /// 设置遍历数量
        /// </summary>
        /// <param name="StartIndex"></param>
        /// <param name="EnumCount"></param>
        void SetEnumIndex(int StartIndex, int EnumCount);
    }

    //2016.03.27, czs, create in hongqing, 提供一个抽象的枚举器
    /// <summary>
    /// 提供一个抽象的枚举器。
    /// </summary>
    /// <typeparam name="TMaterial"></typeparam>
    public abstract class AbstractEnumer<TMaterial> : IEnumer<TMaterial>, ICancelAbale
    {
        Log log = new Log(typeof(AbstractEnumer<TMaterial>));
        /// <summary>
        /// 构造函数
        /// </summary>
        public AbstractEnumer()
        {
            this.EnumCount = int.MaxValue / 2;
         
         
        }
        #region 流程控制
        /// <summary>
        /// 外部指定，是否取消
        /// </summary>
        public virtual bool IsCancel { get; set; }
        /// <summary>
        /// 当前编号，从 0 开始。
        /// </summary>
        public virtual int CurrentIndex { get; set; }
        /// <summary>
        /// 起始编号，从0开始。
        /// </summary>
        public virtual int StartIndex { get; set; }
        /// <summary>
        /// 遍历数量，默认为最大值的一半。
        /// </summary>
        public virtual int EnumCount { get; set; }
        /// <summary>
        /// 是否该略过当前，如果当前编号小于起始编号，则略过。
        /// </summary>
        public virtual bool IsSkipCurrent { get { return CurrentIndex < this.StartIndex; } }
        /// <summary>
        /// 最大的循环编号
        /// </summary>
        public virtual int MaxEnumIndex { get { return StartIndex + EnumCount; } }
        /// <summary>
        /// 设置遍历数量
        /// </summary>
        /// <param name="StartIndex"></param>
        /// <param name="EnumCount"></param>
        public void SetEnumIndex(int StartIndex, int EnumCount) {
            this.StartIndex = StartIndex; 
            this.EnumCount = EnumCount; 

        }
        #endregion

        #region 通用实现
        /// <summary>
        /// 本身
        /// </summary>
        /// <returns></returns>
        public IEnumerator<TMaterial> GetEnumerator() { return this; }
        /// <summary>
        /// 本身
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }
        /// <summary>
        /// 当前
        /// </summary>
        public TMaterial Current { get; set; }

        /// <summary>
        /// 当前
        /// </summary>
        object System.Collections.IEnumerator.Current { get { return Current; } }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        #endregion

         
        /// <summary>
        /// 销毁
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// 移动
        /// </summary>
        /// <returns></returns>
        public virtual bool MoveNext()
        {
            #region 流程控制
            CurrentIndex++;
            if (CurrentIndex == StartIndex) { log.Debug("数据流 " + this.Name + " 开始读取数据。"); }
            if (this.IsCancel) { log.Info("数据流 " + this.Name + " 已被手动取消。"); return false; }
            if (CurrentIndex > this.MaxEnumIndex) { log.Info("数据流 " + this.Name + " 已达指定的最大编号 " + this.MaxEnumIndex); return false; }
            while (CurrentIndex < this.StartIndex) { this.MoveNext(); }
            #endregion

            return true;                
        }

        /// <summary>
        /// 重置
        /// </summary>
        public abstract void Reset();
    }


    //2018.05.20, czs, hmx, 增加简易遍历器
    /// <summary>
    /// 简易遍历器
    /// </summary>
    /// <typeparam name="TMaterial"></typeparam>
    public class SimpleEnumer<TMaterial> : AbstractEnumer<TMaterial>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="materials"></param>
        public SimpleEnumer(IEnumerable<TMaterial> materials)
        {
            CurrentIndex = -1;
            List = new List<TMaterial>(materials);
            Name = "简易无名数据流";
        }

        List<TMaterial> List { get; set; }

        /// <summary>
        /// 数据流向前
        /// </summary>
        /// <returns></returns>
        public override bool MoveNext()
        {
            bool result = base.MoveNext();

            if (result && CurrentIndex < List.Count)
            {
                this.Current = List[CurrentIndex];
                return true;
            }
            return false;
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public override void Dispose()
        {
            List.Clear();
        }
        /// <summary>
        /// 重置数据流
        /// </summary>
        public override void Reset()
        {
            CurrentIndex = -1; 
        }
    }

}
