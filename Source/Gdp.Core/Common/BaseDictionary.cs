//2015.05.12, czs, create in namu, 字典接口
//2016.11.18, czs, refactor in hongqing, 提取抽象字典。

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using System.Linq;
using System.IO;
using System.Text;

namespace Gdp
{


    //2016.10.18, czs, create in hongqing, 列表管理器
    /// <summary>
    /// 浮点数列表管理器
    /// </summary>
    public class NumerialListManager : ListManager<double> { }
    /// <summary>
    /// 列表管理器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListManager<T> : BaseDictionary<String, List<T>>
    {
        public override List<T> Create(string key)
        {
            return new List<T>();
        }
    }

    //2016.04.24, czs, create in hongqing, 增加测站相关改正数
    /// <summary>
    /// 增加测站相关改正数
    /// </summary>
    public class NumeralCorrectionManager : BaseDictionary<string, double>
    {
    }

    /// <summary>
    /// 具有关键字的数据存储结构。核心存储为字典。属于管理者模式应用。
    /// </summary>
    /// <typeparam name="TKey">关键字</typeparam>
    /// <typeparam name="TValue">值</typeparam>
    public class BaseDictionary<TKey, TValue> : AbstractBaseDictionary<TKey, TValue>//, IDisposable
       //  where TKey : IComparable<TKey>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="CreateFunc">创建函数</param>
        public BaseDictionary(string name = "", Func<TKey, TValue> CreateFunc = null):base(CreateFunc)
        {
            this.Name = name;
            this.data = new Dictionary<TKey, TValue>();
            this.InitOrderedKeys();
            Init();
        }
        /// <summary>
        /// 采用字典数据直接初始化
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="name"></param>
        public BaseDictionary(IDictionary<TKey, TValue> dic, string name = "")
        {
            this.Name = name;
            this.SetData(dic);
            Init();
        }
        /// <summary>
        /// 采用字典数据直接初始化
        /// </summary>
        /// <param name="values"></param>
        /// <param name="name"></param>
        public BaseDictionary(IEnumerable<KeyValuePair<TKey, TValue>> values, string name = "")
        {
            this.Name = name;
            data = new Dictionary<TKey, TValue>();
            foreach (var item in values)
            {
                this.Add(item.Key, item.Value);
            }
            Init();
        }
        public BaseDictionary(IDictionaryClass<TKey, TValue> values, string name = "")
        {
            this.Name = name;
            data = new Dictionary<TKey, TValue>();
            foreach (var item in values.Keys)
            {
                this.Add(item, values[item]);
            }
            Init();
        }
        /// <summary>
        /// 核心数据
        /// </summary>
        protected IDictionary<TKey, TValue> data { get; set; }

        // private readonly static ConcurrentDictionary<string, T> _dic;
        /// <summary>
        /// 初始化，在构造函数之后调用
        /// </summary>
        protected virtual void Init()
        {
            if(data != null && data.Count > 0)
            { 
                CheckOrSetTheVeryFirstKey(FirstKey);
            }
             
        }
        /// <summary>
        /// 核心数据返回。
        /// </summary>

        //  [Obsolete("请不要直接在此属性上执行添加、删除等操作，直接使用对象方法操作")]
        public override IDictionary<TKey, TValue> Data { get { return this.data; } }

      
        /// <summary>
        /// 键值对
        /// </summary>
        public  IDictionary<TKey, TValue> KeyValues { get { return this.data; } }
        /// <summary>
        /// 直接设置源数据
        /// </summary>
        /// <param name="data"></param>
        public void SetData(IDictionary<TKey, TValue> data)
        {
            this.data = data;
            this.InitOrderedKeys(); ;
        }
        /// <summary>
        /// 获取源数据，直接获取，请不要删除。
        /// </summary>
        /// <returns></returns>
        public IDictionary<TKey, TValue> GetData()
        {
            return data;
        }
        /// <summary>
        /// 返回列表
        /// </summary>
        /// <returns></returns>
        public List<KeyValuePair< TKey, TValue>> GetKeyValueList()
        {
            return Data.ToList();
        }
        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public BaseDictionary<TKey, TValue> Clone()
        {
            var d = new Dictionary<TKey, TValue>();
            
            foreach (var key in this.OrderedKeys)
            {
                d.Add(key, this[key]);
            }

            return new BaseDictionary<TKey, TValue>(d);
        }
         
    }
}
