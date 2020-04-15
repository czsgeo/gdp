//2018.09.08, czs, create in hmx, 分组数据
//2018.10.20, czs, edit in hmx, 各类分类型单独存储


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks; 

namespace Gdp
{     
    /// <summary>
    /// 具有误差的数值分类存储器
    /// </summary>
    public class GroupedRmsValue<TKey, TGroup> : GroupedValue<TKey, TGroup, RmsedNumeral>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public GroupedRmsValue()
        {
        }

        /// <summary>
        /// 获取表格，以Key为列名
        /// </summary>
        /// <returns></returns>
        public ObjectTableStorage GetDetailTable(string tableName = "所有站MW计算结果", string outName = null, object outValue = null)
        {
            ObjectTableStorage table = new ObjectTableStorage(tableName);

            int index = 0;
            AddDetailRowsToTable(table, ref index, outName, outValue);
            return table;
        }
        /// <summary>
        /// 构建行
        /// </summary>
        /// <param name="table"></param>
        /// <param name="index"></param>
        /// <param name="outName"></param>
        /// <param name="outValue"></param>
        /// <returns></returns>
        public void AddDetailRowsToTable(ObjectTableStorage table, ref int index, string outName = null, object outValue = null)
        {
            foreach (var topKv in this.Data)//分组遍历
            {
                foreach (var kv in topKv.Value.Data)//一个分组一个数据
                {
                    index++;
                    table.NewRow();
                    table.AddItem("Index", index);

                    if (!String.IsNullOrWhiteSpace(outName) && outValue != null)
                    {
                        table.AddItem(outName, outValue);
                    }

                    table.AddItem("Name", topKv.Key);
                    table.AddItem("Group", kv.Key);
                    table.AddItem("Value", kv.Value.Value);
                    table.AddItem("Rms", kv.Value.StdDev);
                }
            }
        }


        /// <summary>
        /// 最大的分段
        /// </summary>
        /// <returns></returns>
        public int GetMaxSpanCount()
        {
            int count = 0;
            foreach (var item in this)
            {
                var coun = item.Count;
                if (count < coun)
                {
                    count = coun;
                }
            }
            return count;
        }


        /// <summary>
        /// 采用四舍五入法，获取本分组MW的小数部分[-0.5, 0.5]，将所有分段的MW合并，加权平均。以第RMS最小的为基准，作差比较，确保在同一个小数区间
        /// </summary>
        /// <param name="maxRms"></param>
        /// <returns></returns>
        public BaseDictionary<TKey, RmsedNumeral> GetAverageRoundFraction(double maxRms = 0.5)
        {
            var dat = new BaseDictionary<TKey, RmsedNumeral>();

            foreach (var item in this.Data)
            {
                var list = item.Value.Values;

                RmsedNumeral ave = Gdp.Utils.DoubleUtil.GetAverageRoundFraction(list, maxRms, 3);

                if (ave != null)
                {
                    dat[item.Key] = ave;
                }
            }
            return dat;
        }




        /// <summary>
        /// 获取表格
        /// </summary>
        /// <returns></returns>
        public ObjectTableStorage GetTable(string tableName = "所有站MW计算结果")
        {
            ObjectTableStorage table = new ObjectTableStorage(tableName);
            return ExtractRowToTable(table);
        }

        

        /// <summary>
        /// 提取到表格
        /// </summary>
        /// <returns></returns>
        public ObjectTableStorage ExtractRowToTable(ObjectTableStorage table)
        {
            var list = this.GetSatValueList();
            var length = this.GetMaxSpanCount();
            for (int i = 0; i < length; i++)
            {
                table.NewRow();
                table.AddItem("GroupNum", i + 1);

                var keys = list.Keys;
                keys.Sort();
                string note = "";
                foreach (var key in keys)
                {
                    var val = list.Data[key];
                    if (val.Count > i)
                    {
                        var rmsVal = val[i];
                        table.AddItem(key, rmsVal.Value.Value);
                        if (this.Count < 100)//如果文件太多，就节约内存不完全显示。
                        {
                            note += key + ": " + rmsVal.Key.ToString() + "; ";
                        }
                    }
                }
                table.AddItem("Note", note);
            }

            return table;
        }

        /// <summary>
        /// 以列表形式返回。
        /// </summary>
        /// <returns></returns>
        public BaseDictionary<TKey, List<KeyValuePair<TGroup, RmsedNumeral>>> GetSatValueList()
        {
            var list = new BaseDictionary<TKey, List<KeyValuePair<TGroup, RmsedNumeral>>>();
            foreach (var item in this.GetKeyValueList())
            {
                list.Add(item.Key, item.Value.GetKeyValueList());
            }

            return list;
        }

        // public SiteSmoothedMwValues { get; set; }
        public override BaseDictionary<TGroup, RmsedNumeral> Create(TKey key)
        {
            return new BaseDictionary<TGroup, RmsedNumeral>();
        }
    }

    /// <summary>
    /// 分组数据存储的基类
    /// </summary>
    /// <typeparam name="TKey">数据键值</typeparam>
    /// <typeparam name="TGroup">分组类型</typeparam>
    /// <typeparam name="TValue">数据类型</typeparam>
    public class GroupedValue<TKey, TGroup, TValue> : BaseDictionary<TKey, BaseDictionary<TGroup, TValue>>
    {
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override BaseDictionary<TGroup, TValue> Create(TKey key)
        {
            return new BaseDictionary<TGroup, TValue>();
        }
    }


}
