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
    /// 具有RMS的模糊度存储器
    /// </summary>
    public class PeriodRmsedNumeralStoarge : PeriodRmsedNumeralStorage<string>
    {

        /// <summary>
        ///读取
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static PeriodRmsedNumeralStoarge Read(string path)
        {
            ObjectTableReader reader = new ObjectTableReader(path);
            var table = reader.Read();

            return Parse(table);
        }

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static PeriodRmsedNumeralStoarge Parse(ObjectTableStorage table)
        {
            var storage = new PeriodRmsedNumeralStoarge();
            foreach (var row in table.BufferedValues)
            {
                var paramName = row["Name"].ToString();
                var timePeriod = TimePeriod.Parse(row["TimePeriod"].ToString());
                double val = Double.NaN;
                double rms = Double.NaN;
                if (row.ContainsKey("Value"))
                {
                    val = double.Parse(row["Value"].ToString());
                }
                if (row.ContainsKey("Rms"))
                {
                    rms = double.Parse(row["Rms"].ToString());
                }
                storage.GetOrCreate(paramName)[timePeriod] = new RmsedNumeral(val, rms);
            }
            return storage;
        }

        /// <summary>
        /// 转换为表格
        /// </summary>
        /// <returns></returns>
        public ObjectTableStorage ToTable()
        {
            ObjectTableStorage table = new ObjectTableStorage();
            int i = 0;
            foreach (var kv in this.KeyValues)
            {
                foreach (var item in kv.Value.KeyValues)
                {
                    table.NewRow();
                    table.AddItem("Index", i);
                    table.AddItem("Name", kv.Key);
                    table.AddItem("TimePeriod", item.Key.ToString());
                    table.AddItem("Value", item.Value.Value.ToString("G8"));
                    table.AddItem("Rms", item.Value.Rms.ToString("G8"));

                    i++;
                }
            }
            return table;
        }

        /// <summary>
        ///读取
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static PeriodRmsedNumeralStoarge Combine(PeriodRmsedNumeralStoarge A, PeriodRmsedNumeralStoarge B)
        {
            PeriodRmsedNumeralStoarge result = new PeriodRmsedNumeralStoarge();
            //先加到一起
            foreach (var kv in A.KeyValues)
            {
                var dic = result.GetOrCreate(kv.Key);
                foreach (var item in kv.Value.KeyValues)
                {
                    dic[item.Key] = item.Value;
                }
            }

            foreach (var kv in B.KeyValues)
            {
                var dic = result.GetOrCreate(kv.Key);
                foreach (var item in kv.Value.KeyValues)
                {
                    dic[item.Key] = item.Value;
                }
            }

            //合并同类项
            Dictionary<string, List<Pair<TimePeriod>>> toCombies = new Dictionary<string, List<Pair<TimePeriod>>>();
            foreach (var kv in result.KeyValues)
            {
                //查找交叉，且数值相等的时段
                var crossed = new List<Pair<TimePeriod>>();
                toCombies[kv.Key] = crossed;
                List<TimePeriod> notLoopAgain = new List<TimePeriod>();
                foreach (var item1 in kv.Value.KeyValues)
                {
                    notLoopAgain.Add(item1.Key);
                    item1.Key.Tag = item1.Value;//保存数值
                    foreach (var item2 in kv.Value.KeyValues)
                    {
                        if (item1.Key == item2.Key || notLoopAgain.Contains(item2.Key) || item1.Value != item2.Value) { continue; }
                        if (item2.Key.IsIntersect(item2.Key))
                        {
                            crossed.Add(new Pair<TimePeriod>(item1.Key, item2.Key));
                        }
                    }
                }
            }
            //移除
            foreach (var item in toCombies)
            {
                foreach (var kv in item.Value)
                {
                    result.Remove(item.Key, kv.First);
                    result.Remove(item.Key, kv.Second);
                }
            }

            //合并，再添加 
            foreach (var item in toCombies)
            {
                var data = result.GetOrCreate(item.Key);
                foreach (var kv in item.Value)
                {
                    var newP = kv.First.Exppand(kv.Second);
                    data[newP] = (RmsedNumeral)kv.First.Tag;
                }
            }
            return result;
        }

    }

    /// <summary>
    /// 模糊度存储器
    /// </summary>
    public class PeriodNumerialStorage : PeriodNumerialStorage<string>
    {

        /// <summary>
        ///读取
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static PeriodNumerialStorage Combine(PeriodNumerialStorage A, PeriodNumerialStorage B)
        {
            PeriodNumerialStorage result = new PeriodNumerialStorage();
            //先加到一起
            foreach (var kv in A.KeyValues)
            {
                var dic = result.GetOrCreate(kv.Key);
                foreach (var item in kv.Value.KeyValues)
                {
                    dic[item.Key] = item.Value;
                }
            }

            foreach (var kv in B.KeyValues)
            {
                var dic = result.GetOrCreate(kv.Key);
                foreach (var item in kv.Value.KeyValues)
                {
                    dic[item.Key] = item.Value;
                }
            }

            //合并同类项
            Dictionary<string, List<Pair<TimePeriod>>> toCombies = new Dictionary<string, List<Pair<TimePeriod>>>();
            foreach (var kv in result.KeyValues)
            {
                //查找交叉，且数值相等的时段
                var crossed = new List<Pair<TimePeriod>>();
                toCombies[kv.Key] = crossed;
                List<TimePeriod> notLoopAgain = new List<TimePeriod>();
                foreach (var item1 in kv.Value.KeyValues)
                {
                    notLoopAgain.Add(item1.Key);
                    item1.Key.Tag = item1.Value;//保存数值
                    foreach (var item2 in kv.Value.KeyValues)
                    {
                        if (item1.Key == item2.Key || notLoopAgain.Contains(item2.Key) || item1.Value != item2.Value) { continue; }
                        if (item2.Key.IsIntersect(item2.Key))
                        {
                            crossed.Add(new Pair<TimePeriod>(item1.Key, item2.Key));
                        }
                    }
                }
            }
            //移除
            foreach (var item in toCombies)
            {
                foreach (var kv in item.Value)
                {
                    result.Remove(item.Key, kv.First);
                    result.Remove(item.Key, kv.Second);
                }
            }

            //合并，再添加
            KeyValuePair<TimePeriod, double> kvs = new KeyValuePair<TimePeriod, double>();

            foreach (var item in toCombies)
            {
                var data = result.GetOrCreate(item.Key);
                foreach (var kv in item.Value)
                {
                    var newP = kv.First.Exppand(kv.Second);
                    data[newP] = (double)kv.First.Tag;
                }
            }
            return result;
        }
    }

    /// <summary>
    /// 时段RMS数值存储器
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class PeriodRmsedNumeralStorage<TKey> : PeriodValueStorage<TKey, RmsedNumeral>
    {
        /// <summary>
        /// 获取差分结果
        /// </summary>
        /// <param name="time"></param>
        /// <param name="nameA"></param>
        /// <param name="nameB"></param>
        /// <returns></returns>
        public RmsedNumeral GetDifferValue(Time time, TKey nameA, TKey nameB)
        {
            var valA = this.GetValue(nameA, time);
            var valB = this.GetValue(nameB, time);
            return valA - valB;
        }

        /// <summary>
        /// 默认数据
        /// </summary>
        /// <returns></returns>
        public override RmsedNumeral GetDefaultValue()
        {
            return RmsedNumeral.NaN;
        }

    }


    /// <summary>
    /// 时段数值存储器
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class PeriodNumerialStorage<TKey> : PeriodValueStorage<TKey, double>
    {
        /// <summary>
        /// 默认数据
        /// </summary>
        /// <returns></returns>
        public override double GetDefaultValue()
        {
            return double.NaN;
        }

        /// <summary>
        ///读取
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static PeriodNumerialStorage Read(string path)
        {
            ObjectTableReader reader = new ObjectTableReader(path);
            var table = reader.Read();

            return Parse(table);
        }

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static PeriodNumerialStorage Parse(ObjectTableStorage table)
        {
            PeriodNumerialStorage storage = new PeriodNumerialStorage();
            foreach (var row in table.BufferedValues)
            {
                var paramName = row["Name"].ToString();
                var timePeriod = TimePeriod.Parse(row["TimePeriod"].ToString());
                double val = Double.NaN;
                if (row.ContainsKey("Value"))
                {
                    val = double.Parse(row["Value"].ToString());
                }
                storage.GetOrCreate(paramName)[timePeriod] = val;
            }
            return storage;
        }
        /// <summary>
        /// 转换为表格
        /// </summary>
        /// <returns></returns>
        public ObjectTableStorage ToTable()
        {
            ObjectTableStorage table = new ObjectTableStorage();
            int i = 0;
            foreach (var kv in this.KeyValues)
            {
                foreach (var item in kv.Value.KeyValues)
                {
                    table.NewRow();
                    table.AddItem("Index", i);
                    table.AddItem("Name", kv.Key);
                    table.AddItem("TimePeriod", item.Key.ToString());
                    table.AddItem("Value", item.Value);

                    i++;
                }
            }
            return table;
        }
    }

    /// <summary>
    /// 时段数据存储器
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class PeriodValueStorage<TKey, TValue> : GroupedValue<TKey, TimePeriod, TValue>
    {
        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="key"></param>
        /// <param name="epoch"></param>
        /// <returns></returns>
        public TValue GetValue(TKey key, Time epoch)
        {
            var vals = Get(key);
            foreach (var val in vals.KeyValues)
            {
                if (val.Key.Contains(epoch))
                {
                    return val.Value;
                }
            }
            return default(TValue);
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="timePeriod"></param>
        public void Remove(TKey key, TimePeriod timePeriod)
        {
            var data = this.GetOrCreate(key);
            if (data.Contains(timePeriod))
            {
                data.Remove(timePeriod);
            }
        }
        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="key"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool Contains(TKey key, Time time)
        {
            if (this.Contains(key))
            {
                var pVals = this[key];
                foreach (var kv in pVals.KeyValues)
                {
                    if (kv.Key.Contains(time))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 获取值，失败返回默认值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public TValue Get(TKey key, Time time)
        {
            if (this.Contains(key))
            {
                var pVals = this[key];
                foreach (var kv in pVals.KeyValues)
                {
                    if (kv.Key.Contains(time))
                    {
                        return kv.Value;
                    }
                }
            }
            return GetDefaultValue();
        }

        /// <summary>
        /// 获取失败后的默认值
        /// </summary>
        /// <returns></returns>
        public virtual TValue GetDefaultValue()
        {
            return default(TValue);
        }
    } 
   
    /// <summary>
    /// 以时间作为分组的存储对象
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class PeriodRmsNumeralStorage<TKey> : GroupedRmsValue<TKey, TimePeriod>
    {
        /// <summary>
        /// 移除RMS比此大的数据
        /// </summary>
        /// <param name="maxRms"></param>
        public void RemoveRmsGreaterThan(double maxRms)
        {
            foreach (var kv in this.KeyValues)
            {
                var vals = kv.Value;
                RemoveRmsGreaterThan(vals, maxRms);
            }
        }

        private static void RemoveRmsGreaterThan(BaseDictionary<TimePeriod, RmsedNumeral> vals, double maxRms)
        {
            List<TimePeriod> toRemoved = new List<TimePeriod>();
            foreach (var item in vals.KeyValues)
            {
                if (item.Value.Rms > maxRms)
                {
                    toRemoved.Add(item.Key);
                }
            }
            vals.Remove(toRemoved);
        }

        /// <summary>
        /// 总共的观测时段
        /// </summary>
        public TimePeriod TimePeriod
        {
            get
            {
                TimePeriod timePeriod = new TimePeriod(this.First.FirstKey);
                foreach (var item in this)
                {
                    foreach (var period in item.Keys)
                    {
                        timePeriod = (timePeriod.Exppand(period));
                    }
                }
                return timePeriod;
            }
        }
        /// <summary>
        /// 获取历元信息
        /// </summary>
        /// <param name="epoch"></param>
        /// <returns></returns>
        public BaseDictionary<TKey, RmsedNumeral> GetEpochValues(Time epoch)
        {
            var result = new BaseDictionary<TKey, RmsedNumeral>();
            foreach (var kv in this.KeyValues)
            {
                foreach (var item in kv.Value.KeyValues)
                {
                    if (item.Key.Contains(epoch))
                    {
                        result[kv.Key] = item.Value;
                        break;//一个卫星获取一个数值即可
                    }
                }
            }
            return result;
        }
    }

}
