//2015.10.20, czs, create in hongqing, 列表接口类

using System;
using System.Collections.Generic;

namespace Gdp
{ 
    /// <summary>
    /// 列表接口类, 采用列表实现。属于管理者模式应用。
    /// </summary> 
    /// <typeparam name="TValue">值</typeparam>
    public class BaseList<TValue> : IEnumerable<TValue> //:  IBuffer<TValue> // BaseDictionary<int, TValue>,
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">名称</param>
        public BaseList(string name = "")
        {
            this.Name = name;
            data = new List<TValue>();
        }
        /// <summary>
        /// 以数值初始化
        /// </summary>
        /// <param name="values"></param>
        /// <param name="name"></param>
        public BaseList(IEnumerable<TValue> values, string name = "未命名列表")
        {
            this.Name = name;
            data = new List<TValue>(values);
        }
        /// <summary>
        /// 核心数据
        /// </summary>
        protected List<TValue> data { get; set; }

        /// <summary>
        /// 核心数据返回。
        /// </summary>
        public List<TValue> Data { get { return this.data; } }

        /// <summary>
        /// 存储额外数据。
        /// </summary>
        public Object Tag { get; set; }
        /// <summary>
        /// 第一个
        /// </summary>
        public TValue First { get { return this[0]; } }
        /// <summary>
        /// 第2个
        /// </summary>
        public TValue Second { get { return this[1]; } }
        /// <summary>
        /// 第一个
        /// </summary>
        public TValue Middle { get { return this[Count/2]; } }
        /// <summary>
        /// 最后个
        /// </summary>
        public TValue Last { get { return this[Count -1]; } }
        /// <summary>
        /// 数量。
        /// </summary>
        public int Count { get { return data.Count; } }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 检索器
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual TValue this[int key] { get { return data[key]; } set { data[key] = value; } }
        /// <summary>
        /// 往前移动一位
        /// </summary>
        /// <param name="val"></param>
        public void MoveUp(TValue val)
        {
            var index = this.data.IndexOf(val);
            if (index > 0)
            {
                data.RemoveAt(index);
                data.Insert(index - 1, val);
            }
        }
        /// <summary>
        /// 往后移动一位
        /// </summary>
        /// <param name="val"></param>
        public void MoveDown(TValue val)
        {
            var index = this.data.IndexOf(val);
            if (index != -1 && index < this.Count-1)
            {
                data.RemoveAt(index);
                data.Insert(index + 1, val);
            }
        }
        /// <summary>
        /// 截取自列表
        /// </summary>
        /// <param name="indexFrom"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public BaseList<TValue> GetSubList(int indexFrom, int count)
        {
            var newList = new BaseList<TValue>("SubOf" + Name);
            for (int i = 0; i < count && indexFrom + i < Count; i++)
            {
                newList.Add(this[indexFrom + i]);
            }
            return newList;
        }


        /// <summary>
        /// 获取最后的部分
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public  List<TValue> GetLast(int count)
        {
            int totalCount = this.Count;
            int fromIndex = totalCount - count;
            var newList = new List<TValue>();// ("SubOf" + Name);
            for (int i = fromIndex; i < totalCount; i++)
            {
                newList.Add(this[i]);
            }
            return newList;
        }
        /// <summary>
        /// 添加， 
        /// </summary> 
        /// <param name="val"></param>
        public virtual void Add(TValue val)
        {
            data.Add(val);
        }
        /// <summary>
        /// 添加， 
        /// </summary> 
        /// <param name="val"></param>
        public virtual void Add(IEnumerable<TValue> val)
        {
            data.AddRange(val);
        }
        /// <summary>
        /// 添加，若有保存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public virtual void Insert(int key, TValue val)
        {
            data.Insert(key, val);
        }
        /// <summary>
        /// 设置，直接替换
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public virtual void Set(int key, TValue val)
        {
            data[key] = val;
        }
        /// <summary>
        /// 是否包含
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public virtual bool Contains(TValue val)
        {
            return data.Contains(val);
        }
        /// <summary>
        /// 获取，若无则返回默认实例 ，如null
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual TValue Get(int index)
        {
            if (data.Count > index) return data[index];
            return default(TValue);
        }  
        /// <summary>
        /// 移除一个
        /// </summary>
        /// <param name="val"></param>
        public virtual bool Remove(TValue val)
        {
            if (!data.Contains(val)) return false;
           return  data.Remove(val); 
        }
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="vals"></param>
        public virtual void Remove(IEnumerable<TValue> vals)
        {
            foreach (var val in vals)
            {
                Remove(val);
            }
        }
        /// <summary>
        /// 移除范围
        /// </summary>
        /// <param name="indexStart"></param>
        /// <param name="endIndex"></param>
        public void RemoveAt(int indexStart, int endIndex)
        {
            for (int i = endIndex; i > -1; i--)
            {
                RemoveAt(i);
            }
        }

        /// <summary>
        /// 清空
        /// </summary> 
        public virtual void Clear( )
        {
            data.Clear();
        }  
        #region IEnumerator
        /// <summary>
        /// IEnumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<TValue> GetEnumerator()
        {
            return data.GetEnumerator();
        }
        /// <summary>
        /// IEnumerator
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return data.GetEnumerator();
        }
        #endregion

        /// <summary>
        /// 移除指定引用
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            data.RemoveAt(index);
        }

        /// <summary>
        /// 弹出最先进入的一个
        /// </summary>
        /// <returns></returns> 
        public TValue Pop()
        {
            if (Count == 0) { return default(TValue); }
            var val = data[0];
            data.RemoveAt(0);
            return val;
        }
        /// <summary>
        /// 到列表。
        /// </summary>
        /// <returns></returns>
        public List<TValue> ToList() { return new List<TValue>(Data); }
        /// <summary>
        ///显示
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Count: " + this.Count;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            foreach (var item in this)
            {
                if (item is IDisposable) { ((IDisposable)item).Dispose(); }
            }
            this.Clear();
        }


        public TValue GetOrCreate(int key)
        {
            throw new NotImplementedException();
        }
    }
}
