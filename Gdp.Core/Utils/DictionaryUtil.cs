
//2016.02.11, czs, create in hongqing, 字典工具

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gdp.Utils
{

    /// <summary>
    /// 字典工具
    /// </summary>

    public class DictionaryUtil
    { 
        /// <summary>
        /// 两列
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dicA"></param>
        /// <returns></returns>
        static public string ToStringLines<TKey, TValue>(IDictionary<TKey, TValue> dicA)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in dicA)
            {
                sb.AppendLine(item.Key + "\t" + item.Value);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 比较是否相等
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dicA"></param>
        /// <param name="dicB"></param>
        /// <returns></returns>
        public static bool IsEqual<TKey, TValue>(IDictionary<TKey, TValue> dicA, IDictionary<TKey, TValue> dicB)
        {
            if (dicA.Count != dicB.Count) return false;

            foreach (var item in dicA)
            {
                if (dicB.ContainsKey(item.Key))
                {
                    if (!item.Equals(dicA[item.Key]))
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 减法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rovMw"></param>
        /// <param name="refMw"></param>
        /// <returns></returns>
        public static Dictionary<T, RmsedNumeral> Minus<T>(Dictionary<T, RmsedNumeral> rovMw, Dictionary<T, RmsedNumeral> refMw)
        {
            Dictionary<T, RmsedNumeral> result = new Dictionary<T, RmsedNumeral>();
            foreach (var item in rovMw)
            {
                if (!refMw.ContainsKey(item.Key))
                {
                    continue;
                }
                result[item.Key] = item.Value - refMw[item.Key];
            }

            return result;
        }


        /// <summary>
        /// 返回第一个值。
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static TValue GetFirstValue<TKey, TValue>(Dictionary<TKey, TValue> dic)
        {
            foreach (var item in dic)
            {
                return item.Value;
            }
            return default(TValue);
        }
        /// <summary>
        /// 返回最后一个值。
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static TValue GetLastValue<TKey, TValue>(Dictionary<TKey, TValue> dic)
        {
            TValue val = default(TValue);
            foreach (var item in dic)
            {
                val = item.Value;
            }
            return val;
        }
        /// <summary>
        /// 返回最后一个值。
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static TKey GetLastKey<TKey, TValue>(Dictionary<TKey, TValue> dic)
        {
            TKey val = default(TKey);
            foreach (var item in dic)
            {
                val = item.Key;
            }
            return val;
        }
        /// <summary>
        /// 返回第一个值。
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static TKey GetFirstKey<TKey, TValue>(Dictionary<TKey, TValue> dic)
        {
            foreach (var item in dic)
            {
                return item.Key;
            }
            return default(TKey);
        }
        /// <summary>
        ///  获取浮点数。错误则返回NaN
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static Dictionary<string, double> GetNumeralRow(Dictionary<string, object> row)
        {
            Dictionary<string, double> dic = new Dictionary<string, double>();
            Double defaultVal = Double.NaN;
            foreach (var item in row)
            {
                var val = Gdp.Utils.ObjectUtil.GetNumeral(item.Value, defaultVal);
                if (!Double.IsNaN(val))
                {
                    dic[item.Key] = val;
                }
            }
            return dic;
        }

        /// <summary>
        ///  获取浮点数。错误则返回NaN
        /// </summary>
        /// <param name="colName"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public static double GetNumeral(Dictionary<string, object> row, string colName)
        {
            if (row == null) { return double.NaN; }
            if (row.ContainsKey(colName))
            {
                var valObj = row[colName];
                return Gdp.Utils.ObjectUtil.GetNumeral(valObj);
            }
            return double.NaN;
        }

        /// <summary>
        /// 获取有效数据的数量，非null或空
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static int GetValidDataCount(Dictionary<string, object> dic)
        {
            int i = 0;
            foreach (var item in dic)
            {
                if (!Gdp.Utils.ObjectUtil.IsEmpty(item.Value))
                {
                    i++;
                }
            }
            return i;
        }
        /// <summary>
        /// 获取列表共同key的值，返回数组
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic1"></param>
        /// <param name="intValOfNL"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue[]> GetCommonValues<TKey, TValue>(Dictionary<TKey, TValue> dic1, params  Dictionary<TKey, TValue> [] dic2toN)
        {
            var dic =  new Dictionary<TKey, TValue[]>();
            foreach (var item in dic1)
            { 
                foreach (var dicn in dic2toN)
                {
                    if (!dicn.ContainsKey(item.Key)) {  break; }
                }

                List<TValue> list = new List<TValue>();
                foreach (var dicn in dic2toN)
                {
                    list.Add(dicn[item.Key]);
                }
                dic[item.Key] = list.ToArray(); 
            }
            return dic;
        }
        /// <summary>
        /// 过滤转换
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="intValOfWL"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> FilterOtherValueType<TKey, TValue>(Dictionary<TKey, object> intValOfWL)
        {

            var dic = new Dictionary<TKey, TValue>();
            foreach (var item in intValOfWL)
            {
                if (item.Value is TValue)
                {
                    dic.Add(item.Key, (TValue)item.Value);
                }
            }

            return dic;
        }
        /// <summary>
        /// 解析。
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dicStr"></param>
        /// <param name="strToKey"></param>
        /// <param name="strToVal"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> Parse<TKey, TValue>(string dicStr, Func<string, TKey> strToKey, Func<string, TValue> strToVal) 
        { 
            var result = new Dictionary<TKey, TValue>();
            if (String.IsNullOrEmpty(dicStr)) { return result; }
            var dic = dicStr.Split(new char[] { ';', '，', '；', ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in dic)
            {
                if (String.IsNullOrEmpty(item)) { continue; }
                var kv = item.Split(new char[] { ':', '：' }, StringSplitOptions.RemoveEmptyEntries);
                var satelliteType = strToKey(kv[0]);
                var val = strToVal(kv[1]);
                result[satelliteType] = val;
            }
            return result;
        }

        /// <summary>
        /// 转换为可读的字符串
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <param name="itemSpliter"></param>
        /// <param name="kvSpliter"></param>
        /// <returns></returns>
        public static string ToString<TKey, TValue>(Dictionary<TKey, TValue> dic, string itemSpliter = ",", string kvSpliter = ":")
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach (var item in dic)
            {
                if (i != 0)
                {
                    sb.Append(itemSpliter);
                }
                sb.Append(item.Key + kvSpliter + item.Value);
                i++;
            }

            return sb.ToString(); 
        }
    }
}

