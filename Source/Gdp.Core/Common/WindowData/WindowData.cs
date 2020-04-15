//2016.05.10, czs, edit， 滑动窗口数据
//2016.08.04, czs, edit in fujian yongan, 修正
//2016.08.29, czs, create in hongqing，提取窗口数据接口
//2017.09.11, czs, edit in hongqing, 重构，移除SkipCount属性，此应该在窗口外判断
//2017.09.23, czs, create in hongqing, 增加窗口数据管理器,采用字典实现带检索的窗口

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gdp.IO;

namespace Gdp
{

    /// <summary>
    /// 窗口数据管理器
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class WindowDataManager<TKey, TValue> : BaseDictionary<TKey, WindowData<TValue>>
         where TKey : IComparable<TKey>
    {
        /// <summary>
        /// 窗口数据管理器
        /// </summary>
        /// <param name="WindowSize"></param>
        public WindowDataManager(int WindowSize)
        {
            this.WindowSize = WindowSize;
        }
        /// <summary>
        /// 窗口大小。
        /// </summary>
        public int WindowSize { get; set; }
        /// <summary>
        /// 是否已经具有满窗口。
        /// </summary>
        public bool HasFull
        {
            get
            {
                foreach (var item in this.Values)
                {
                    if (item.IsFull) return true;
                }
                return false;
            }
        }
        /// <summary>
        /// 创建一个。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override WindowData<TValue> Create(TKey key)
        {
            return new WindowData<TValue>(WindowSize);
        }
    }

    /// <summary>
    /// 采用字典维护的窗口数据，可以对数据进行编号。
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class WindowData<TKey, TValue> : BaseDictionary<TKey, TValue>, IWindowData
        where TKey : IComparable<TKey>
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        /// <param name="WindowSize"></param>
        /// <param name="maxKeyGap">Key 允许的最大断裂，否则清空，重新开始</param>
        public WindowData(int WindowSize,double maxKeyGap = double.MaxValue):base()
        {
            this.WindowSize = WindowSize;
            this.MaxKeyGap = maxKeyGap;
        }
        /// <summary>
        /// 采用字典维护的窗口数据
        /// </summary>
        /// <param name="values"></param>
        /// <param name="maxKeyGap">Key 允许的最大断裂，否则清空，重新开始</param>
        public WindowData(IDictionary<TKey, TValue> values, double maxKeyGap = double.MaxValue)
            : base(values)
        {
            this.WindowSize = values.Count();
            this.MaxKeyGap = maxKeyGap;
        }
        /// <summary>
        /// 采用字典维护的窗口数据
        /// </summary>
        /// <param name="values"></param>
        /// <param name="maxKeyGap">Key 允许的最大断裂，否则清空，重新开始</param>
        public WindowData(IDictionaryClass<TKey, TValue> values, double maxKeyGap = double.MaxValue)
            : base(values)
        {
            this.WindowSize = values.Count();
            this.MaxKeyGap = maxKeyGap;
        }
        /// <summary>
        /// 采用字典维护的窗口数据
        /// </summary>
        /// <param name="values"></param>
        /// <param name="maxKeyGap">Key 允许的最大断裂，否则清空，重新开始</param>
        public WindowData(IEnumerable<KeyValuePair<TKey, TValue>> values, double maxKeyGap = double.MaxValue)
            : base(values)
        {
            this.WindowSize = values.Count();
            this.MaxKeyGap = maxKeyGap;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        protected override void Init()
        {
            base.Init();
            Gdp.Utils.DoubleUtil.AutoSetKeyToDouble(out KeyToDouble, this.TheVeryFirstKey);
        }

        #region 委托与属性
        /// <summary>
        /// 委托，关键字转向数值
        /// </summary>
        public Func<TKey, double> KeyToDouble;
        /// <summary>
        /// 委托，值转向数值
        /// </summary>
        public Func<TValue, double> ValueToDouble;

        /// <summary>
        /// 允许的键最大断裂值。需要将Key转换为double计算。
        /// 如果超过此值，则清空重来。如未设置@see KeyToDouble转换，则忽略之。
        /// </summary>
        public double MaxKeyGap { get; set; }
        /// <summary>
        /// 窗口大小。
        /// </summary>
        public int WindowSize { get; set; }
        /// <summary>
        /// 窗口是否已满
        /// </summary>
        public bool IsFull
        {
            get { return this.Count >= WindowSize; }
        }
        /// <summary>
        /// 采样间隔，第二减去第一，若无设置KeyToDouble则返回1。
        /// </summary>
        public double Interval
        {
            get
            {
                if (KeyToDouble == null)
                {
                    return 1;
                }
                else
                {
                    return KeyToDouble(this.SecondKey) - KeyToDouble(this.FirstKey);
                }
            }
        }

        /// <summary>
        /// 为了方便调试输出数据而生
        /// </summary>
        public string ValueStringLines
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (var item in this)
                {
                    sb.AppendLine(item.ToString());
                }

                return sb.ToString();
            }
        }
        #endregion
             
        /// <summary>
        /// 增加一个。如果Key值断裂，则清空当前数据。激发清空数据事件。
        /// </summary>
        /// <param name="item"></param>
        /// <param name="key"></param>
        public override void Add(TKey key, TValue item)
        {
            if (this.Contains(key)) { return; }

            if (IsKeyBreaked(key))
            {
                this.Clear(); 
            }

            while (IsFull && WindowSize > 0) {
                this.Remove(FirstKey);
            }

            base.Add(key, item);
        }

        /// <summary>
        /// 判断输入Key和最后的Key只差，若超过最大MaxKeyGap,则认为断裂。 
        /// 需设置 KeyToDouble， 如果没有设置，则永远返回false。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsKeyBreaked(TKey key)
        {
            if (this.Count < 1 ) { return false; }
            if (KeyToDouble == null)
            {
                log.Warn("使用 IsKeyBreaked 方法，必须设置 KeyToDouble");
                return false;
            }

            var lastKeyVal = KeyToDouble(LastKey);
            var nowKeyVal = KeyToDouble(key);
   
            var differ = Math.Abs(lastKeyVal - nowKeyVal);

            bool isBreaked = differ > MaxKeyGap;
            if (isBreaked)
            {
                int ii = 0;
            }
            return isBreaked;
        }
        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="val"></param>
        public void Add(IDictionary<TKey, TValue> val)
        {
            foreach (var item in val)
            {
                Add(item.Key, item.Value);
            }
        }
        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="dics"></param>
        public void Add(AbstractBaseDictionary<TKey, TValue> dics)
        {
            foreach (var item in dics.Data)
            {
                Add(item.Key, item.Value);
            }
        }

        /// <summary>
        /// 返回一个满足条件的新对象。
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public WindowData<TKey, TValue> GetNew(Func<KeyValuePair<TKey, TValue>, bool> selector)
        {
            return new WindowData<TKey, TValue>(this.Data.Where(selector));
        }

        /// <summary>
        /// 返回一个满足条件的新对象。
        /// </summary>
        /// <param name="startKey"></param>
        /// <param name="endKey"></param>
        /// <returns></returns>
        public WindowData<TKey, TValue> SubWindow(TKey startKey, TKey endKey)
        {
            var orderedKeys = this.OrderedKeys;
            orderedKeys.Sort();
            int indexOfEnd = orderedKeys.IndexOf(endKey);
            if (indexOfEnd == -1)
            {
                return null;
            }
            int indexOfStart = orderedKeys.IndexOf(startKey);
            if (indexOfStart == -1)
            {
                return null;
            }
            Dictionary<TKey, TValue> dic = new Dictionary<TKey, TValue>();
            for (int i = indexOfStart; i <= indexOfEnd; i++)
            {
                var key = orderedKeys[i];
                var val = this[key];
                dic.Add(key, val);
            }

            return Create(dic); 
        }

        /// <summary>
        /// 从小到大， 返回一个满足条件的新对象。
        /// </summary>
        /// <param name="endKey"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public WindowData<TKey, TValue> SubWindow(int count, TKey endKey)
        {
            var orderedKeys = this.OrderedKeys;
            orderedKeys.Sort();
            int indexOfEnd = orderedKeys.IndexOf(endKey);
            if (indexOfEnd == -1)
            {
                return null;
            }
            int starIndex = Math.Max(0, indexOfEnd - count);

            Dictionary<TKey, TValue> dic = new Dictionary<TKey, TValue>();
            for (int i = starIndex; i <= indexOfEnd  ; i++)
            {
                var key = orderedKeys[i];
                var val = this[key];
                dic.Add(key, val);
            }

            return Create(dic);
        }

        /// <summary>
        /// 从小到大， 返回一个满足条件的新对象。
        /// </summary>
        /// <param name="startKey"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public WindowData<TKey, TValue> SubWindow(TKey startKey, int count)
        {
            var orderedKeys = this.OrderedKeys;
            orderedKeys.Sort();
            int indexOfStart = orderedKeys.IndexOf(startKey);
            if(indexOfStart == -1)
            {
                return null;
            }

            count = Math.Min(count, this.Count - indexOfStart);

            Dictionary<TKey, TValue> dic = new Dictionary<TKey, TValue>();
            for (int i = indexOfStart; i < count; i++)
            {
                var key = orderedKeys[i];
                var val = this[key];
                dic.Add(key, val);
            }

            return Create(dic);
        }

        /// <summary>
        /// 创建一个新的
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public   WindowData<TKey, TValue> Create(Dictionary<TKey, TValue> dic)
        {
            return new WindowData<TKey, TValue>(dic) { KeyToDouble = KeyToDouble };
        }

        /// <summary>
        /// 返回一个满足条件的新对象。
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public virtual WindowData<TKey, TValue> SubWindow(int startIndex, int len)
        {
            int entIndex = startIndex + len;
            Dictionary<TKey, TValue> dic = new Dictionary<TKey, TValue>();
            int i = 0;
            foreach (var item in this.GetData())
            {
                if (i >= startIndex && i <= entIndex)
                {
                    dic.Add(item.Key, item.Value);
                }
                if (i > entIndex) { break; }
                i++;
            }

            return Create(dic);
        }

        /// <summary>
        ///显示
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return FirstKey + "-" + LastKey + "，Count: " + this.Count;
        }
    }

    /// <summary>
    /// 滑动窗口数据。只存储固定的数据，如果超出，则剔除先进入的数据。
    /// </summary>
    public class WindowData<TValue> : BaseList<TValue> , IWindowData<TValue>
    {
        /// <summary>
        /// 日志
        /// </summary>
        protected Log log = new Log(typeof(WindowData<TValue>));
        /// <summary>
        /// 默认构造函数。
        /// </summary>
        /// <param name="WindowSize">窗口大小</param>
        public WindowData(int WindowSize)
        {
            this.WindowSize = WindowSize;
        }
        /// <summary>
        /// 以数值列表初始化，数据默认为窗口大小。
        /// </summary>
        /// <param name="values"></param>
        /// <param name="name"></param>
        public WindowData(IEnumerable<TValue> values, string name = "未命名窗口数据")
            : base(values, name)
        {
            this.WindowSize = values.Count();
        }

        #region  属性
        /// <summary>
        /// 窗口大小。
        /// </summary>
        public int WindowSize { get; set; }
        /// <summary>
        /// 是否存满了
        /// </summary>
        public bool IsFull { get { return this.Count >= WindowSize; } }
        /// <summary>
        /// 为了方便调试输出数据而生
        /// </summary>
        public string ValueStringLines
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (var item in this)
                {
                    sb.AppendLine(item.ToString());
                }

                return sb.ToString();
            }
        }

        #endregion

        #region  基本方法
        /// <summary>
        /// 增加一个。
        /// </summary>
        /// <param name="item"></param>
        public override void Add(TValue item)
        {
            while (IsFull && this.Count > 0) { Data.RemoveAt(0); }

            Data.Add(item);
        }

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="val"></param>
        public override void Add(IEnumerable<TValue> val)
        {
            foreach (var item in val)
            {
                Add(item);
            }
        }

        /// <summary>
        /// 返回一个满足条件的新对象。
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public WindowData<TValue> GetNew(Func<TValue, bool> selector)
        {
            return new WindowData<TValue>(this.Where(selector), selector + "_Find");
        }

        /// <summary>
        ///显示
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this.Count > 0) { return First + " - " + Last + ", Count: " + this.Count; }
            return "Count: " + this.Count;
        }
        #endregion

        #region 分析
        /// <summary>
        /// 按照指定条件分解成小窗口集合，比较附近两个元素是否满足条件。
        /// </summary>
        /// <param name="funcConditionToSplit">如果返回真，则表示分离</param>
        /// <returns></returns>
        public List<WindowData<TValue>> Split(Func<TValue, TValue, bool> funcConditionToSplit)
        {
            List<WindowData<TValue>> list = new List<WindowData<TValue>>();
            WindowData<TValue> window = new WindowData<TValue>(Int16.MaxValue);
            list.Add(window);

            TValue prev = default(TValue);
            int i = 0;
            foreach (var current in this)
            {
                if (i == 0) { window.Add(current); prev = current; i++; continue; }

                if (funcConditionToSplit(prev, current))//满足条件，则分裂，新建。
                {
                    window.WindowSize = window.Count;

                    window = new WindowData<TValue>(Int16.MaxValue);
                    list.Add(window);
                }

                window.Add(current);
                prev = current;
                i++;
            }

            window.WindowSize = window.Count;
            return list;
        }

        IListClass<TValue> IListClass<TValue>.GetSubList(int indexFrom, int count)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}