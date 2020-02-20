//2015.05.12, czs, create in namu, 字典接口
//2016.11.18, czs, create in hongqing, 线程安全的字典类
//2016.11.18, czs, refactor in hongqing, 提取抽象字典。

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using System.Linq;
using System.Collections;
using Gdp.IO;

namespace Gdp
{
    /// <summary>
    /// 具有关键字的数据存储结构。核心存储为字典。属于管理者模式应用。
    /// </summary>
    /// <typeparam name="TKey">关键字</typeparam>
    /// <typeparam name="TValue">值</typeparam>
    public abstract class AbstractBaseDictionary<TKey, TValue> : IEnumerable<TValue>, IDisposable, IDictionaryClass<TKey, TValue>
    // where TKey : IComparable<TKey>
    {
       protected  Log log = new Log(typeof(AbstractBaseDictionary<TKey, TValue>));
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public AbstractBaseDictionary() { OrderedKeys = new List<TKey>(); TheVeryFirstKey = default(TKey); IsOrderedKeyEnabled = true; }
        /// <summary>
        /// 构造函数，自带建立方法。
        /// </summary>
        /// <param name="CreateFunc"></param>
        public AbstractBaseDictionary(Func<TKey, TValue> CreateFunc) { this.CreateFunc = CreateFunc; OrderedKeys = new List<TKey>(); TheVeryFirstKey = default(TKey); IsOrderedKeyEnabled = true; }

        #region 属性
        /// <summary>
        /// 数据内容是否发生了变化
        /// </summary>
        public bool IsChanged { get; set; }
        /// <summary>
        /// 事件，数据即将被清空。
        /// </summary>
        public event Action<AbstractBaseDictionary<TKey, TValue>> DataClearing;

        /// <summary>
        /// 移出事件
        /// </summary>
        public event Action<TKey, TValue> ItemRemoved;
        /// <summary>
        /// 移出后触发
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        protected void OnItemRemoved(TKey key, TValue val)
        {
            LastRemovedItem = new KeyValuePair<TKey, TValue>(key, val);
            if (ItemRemoved != null) { ItemRemoved(key, val); }
        }
        /// <summary>
        /// 最后一个被移出的项目。这里相当于回收站
        /// </summary>
        public KeyValuePair<TKey, TValue> LastRemovedItem { get; set; }

        /// <summary>
        /// 建立方法
        /// </summary>
        public Func<TKey, TValue> CreateFunc { get; set; }
        /// <summary>
        /// 核心数据返回。
        /// </summary>
     //   [Obsolete("请不要直接在此属性上执行添加、删除等操作，直接使用对象方法操作")]
        public abstract IDictionary<TKey, TValue> Data { get; }
        /// <summary>
        /// 数量。
        /// </summary>
        public int Count { get { return Data.Count; } }

        /// <summary>
        /// 名称
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// 以列表形式返回内容。
        /// </summary>
        public List<TValue> Values { get { return new List<TValue>(Data.Values); } }

        /// <summary>
        /// 最大的值
        /// </summary>
        public TKey MaxKey { get { return this.Keys.Max(); } }
        /// <summary>
        /// 最大的值
        /// </summary>
        public TKey MinKey { get { return this.Keys.Min(); } }
     
        /// <summary>
        /// 前面第几个最大的,从0开始，0表示最大的一个。
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public TKey GetMaxKey(int order)
        {
            var keys = this.Keys;
            keys.Sort();

            return keys[this.Count - 1 - order];
        }

        /// <summary>
        /// 前面第几个最小的,从0开始，0表示最小的一个。
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public TKey GetMinKey(int order)
        {
            var keys = this.Keys;
            keys.Sort();

            return keys[order];
        }

        /// <summary>
        /// 排序后的值，按照Add先后顺序排序。依靠OrderedKeys，如果直接修改了Data请维护之。
        /// </summary>
        public List<TValue> OrderedValues
        {
            get
            {
                List<TValue> vals = new List<TValue>();
                List<TKey> notExistKeys = new List<TKey>();
                foreach (var key in OrderedKeys)
                {
                    if (this.Contains(key))
                    {
                        vals.Add(this[key]);
                    }
                    else
                    {
                        notExistKeys.Add(key);
                    }                  
                }
                if (notExistKeys.Count > 0)
                {
                    this.OrderedKeys.RemoveAll(m => notExistKeys.Contains(m));
                }
                return vals;
            }
        }

        /// <summary>
        /// 核心数据按照插入顺序返回。不可以在此直接操作。
        /// </summary>
        public virtual IDictionary<TKey, TValue> OrderedData
        {
            get
            {
                List<TKey> notExistKeys = new List<TKey>();
                Dictionary<TKey, TValue> Data = new Dictionary<TKey, TValue>();
                foreach (var key in OrderedKeys)
                {
                    if (this.Contains(key))
                    {
                        Data.Add(key, this[key]);
                    }
                    else
                    {
                        notExistKeys.Add(key);
                    }
                }
                if (notExistKeys.Count > 0)
                {
                    this.OrderedKeys.RemoveAll(m => notExistKeys.Contains(m));
                }
                return Data;
            }
        }
        /// <summary>
        /// 以列表形式返回key。
        /// </summary>
        public List<TKey> Keys { get { return new List<TKey>(Data.Keys); } }
        /// <summary>
        /// 排序后的Key，按照Add先后顺序排序。依靠手动维护，如果直接修改了Data请维护之。
        /// </summary>
        public List<TKey> OrderedKeys { get; set; }

        /// <summary>
        /// 检索器
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual TValue this[TKey key] { get { return Data[key]; } set { Set(key, value); } }
        /// <summary>
        /// 第一个值
        /// </summary>
        public TValue First
        {
            get
            {
                if (this.Count > 0) { return this[FirstKey]; }
                return default(TValue);
            }
        }      
        /// <summary>
        /// 第2个值
        /// </summary>
        public TValue Second
        {
            get
            {
                if (this.Count > 1) { return this[SecondKey]; }
                return default(TValue);
            }
        }
        /// <summary>
        /// 开天辟地第一个，永不改变，与对象共存亡。
        /// </summary>
        public TKey TheVeryFirstKey {
            get;
            private set; }

        /// <summary>
        /// 是否设置了第一个键值。
        /// </summary>
        public bool IsSetTheVeryFirstKey { get; set; }
        /// <summary>
        /// 检查并设置第一个键值
        /// </summary>
        /// <param name="key"></param>
        public void CheckOrSetTheVeryFirstKey(TKey key)
        {
            if (!IsSetTheVeryFirstKey) { TheVeryFirstKey = key; IsSetTheVeryFirstKey = true; }
        }
        /// <summary>
        /// 第一个键
        /// </summary>
        public TKey FirstKey
        {
            get
            {
                CheckOrInitOrderedKeys();
                foreach (var item in this.OrderedKeys)
                {
                    return item;
                }
                foreach (var item in this.Keys)
                {
                    return item;
                }
                return default(TKey);
            }
        }

        /// <summary>
        /// 检查并初始化有顺序的键值
        /// </summary>
        public void CheckOrInitOrderedKeys()
        {
            if (this.OrderedKeys == null || this.OrderedKeys.Count == 0 || this.OrderedKeys.Count != this.Count)
            {
                log.Warn("键值顺序不对，重新初始化！");
                InitOrderedKeys();
            }
        }
        /// <summary>
        /// 采用键初始化列表。
        /// </summary>
        protected void InitOrderedKeys()
        {
            this.OrderedKeys = new List<TKey>(this.Keys);
        }

        /// <summary>
        /// 第2个键
        /// </summary>
        public TKey SecondKey
        {
            get
            {
                CheckOrInitOrderedKeys();
                int i = 0;
                foreach (var item in this.OrderedKeys)
                {
                    i++;
                    if(i ==2)
                    return item;
                }
                return default(TKey);
            }
        }
        /// <summary>
        /// 最后个
        /// </summary>
        public TValue Last
        {
            get
            {
                if (Count > 0)
                {
                    return this.Values[Count - 1];
                }
                return default(TValue);
            }
        }
        /// <summary>
        /// 最后个
        /// </summary>
        public KeyValuePair<TKey, TValue> LastKeyValue
        {
            get
            {
                if (Count > 0)
                {
                    return Data.LastOrDefault();
                }
                return default(KeyValuePair<TKey, TValue>);
            }
        }
        /// <summary>
        /// 第一个
        /// </summary>
        public KeyValuePair<TKey, TValue> FirstKeyValue
        {
            get
            {
                if (Count > 0)
                {
                    return Data.FirstOrDefault();
                }
                return default(KeyValuePair<TKey, TValue>);
            }
        }
        /// <summary>
        /// 最后一个键
        /// </summary>
        public TKey LastKey
        {
            get
            {
                CheckOrInitOrderedKeys();
                if (Count > 0)
                {
                    return this.OrderedKeys[Count - 1];
                }
                return default(TKey);
            }
        }
        /// <summary>
        /// 最后第2个键
        /// </summary>
        public TKey LastSecondKey
        {
            get
            {
                if (Count > 1)
                {
                    return this.OrderedKeys[Count - 2];
                }
                return default(TKey);
            }
        }
        /// <summary>
        /// 倒数第2个值
        /// </summary>
        public TValue SecondLast
        {
            get
            {
                if (this.Count > 1) { return this[LastSecondKey]; }
                return default(TValue);
            }
        }
        /// <summary>
        /// 存储额外数据。
        /// </summary>
        public Object Tag { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 添加，若有保存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public virtual void Add(TKey key, TValue val)
        {
            CheckOrSetTheVeryFirstKey(key);
            this.OrderedKeys.Add(key);
            Data.Add(key, val);
        }
        /// <summary>
        /// 是否采用顺序键值
        /// </summary>
        public bool IsOrderedKeyEnabled { get; set; }
        /// <summary>
        /// 设置，直接替换，所有的设置，必须通过此接口，否则后果自付！！
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public virtual void Set(TKey key, TValue val)
        {
            CheckOrSetTheVeryFirstKey(key);
            if(IsOrderedKeyEnabled && !this.OrderedKeys.Contains(key) ){ this.OrderedKeys.Add(key);}
            Data[key] = val;
        }
        /// <summary>
        /// 获取或创建
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual TValue GetOrCreate(TKey key)
        { 
            if (!this.Contains(key))
            {
                var val = Create(key);
                this[key] = val;
                return val;
            }

            return this[key];
        }
 
        /// <summary>
        /// 创建默认
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual TValue Create(TKey key)
        {
            TValue val = default(TValue);
            if (CreateFunc != null)
            {
                val = CreateFunc(key);
            }
            this[key] = val;
            return val;
        }

        /// <summary>
        /// 是否包含
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual bool Contains(TKey key)
        {
            return Data.ContainsKey(key);
        }
        /// <summary>
        /// 获取，若无则返回默认实例 ，如默认值，如null
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual TValue Get(TKey key)
        {
            if (Data.ContainsKey(key)) return Data[key];
            return default(TValue);
        }
        /// <summary>
        /// 移除第一个
        /// </summary> 
        public virtual void RemoveFirst()
        {
            this.Remove(FirstKey);
        }
        /// <summary>
        /// 移除一个
        /// </summary>
        /// <param name="key"></param>
        public virtual void Remove(TKey key)
        {
            var val = Get(key);
             
            this.OrderedKeys.Remove(key);
            if (Data.ContainsKey(key)) { Data.Remove(key); }

            if(val != null)
            {
                OnItemRemoved(key, val);
            }
            IsChanged = true;
        }

        /// <summary>
        /// 批量移除
        /// </summary>
        /// <param name="keys"></param>
        public virtual void Remove(IEnumerable<TKey> keys)
        {
            foreach (var key in keys)
            {
                Remove(key);
            } 
        }
        /// <summary>
        /// 数据即将被清空前激发。
        /// </summary>
        protected virtual void OnDataClearing()
        {
            if (DataClearing != null) { DataClearing(this); }
        }

        /// <summary>
        /// 清空
        /// </summary>
        public virtual void Clear()
        {
            OnDataClearing();
            LastRemovedItem = new KeyValuePair<TKey, TValue>(default(TKey), default(TValue));
            this.OrderedKeys.Clear();
            this.Data.Clear();

            IsChanged = true;
        }


        #region IEnumerator
        /// <summary>
        /// IEnumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<TValue> GetEnumerator()
        {
            return Data.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<TValue>)Values).GetEnumerator();
        }
        #endregion
        /// <summary>
        /// 返回第一个匹配上的，只要key包含该关键字就可以啦。
        /// </summary>
        /// <param name="containedKey"></param>
        /// <returns></returns>
        public virtual TValue GetFirstMatched(string containedKey)
        {
            foreach (var item in Keys)
            {
                if (item.ToString().Contains(containedKey))
                {
                    return this[item];
                }
            }
            return default(TValue);
        }

        /// <summary>
        /// 字符串显示
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name + "[" + Count + "]";
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public virtual void Dispose()
        {
            foreach (var item in this)
            {
                if (item is IDisposable) { ((IDisposable)item).Dispose(); }
            }
            this.Clear();
        }


        #endregion
    }
}