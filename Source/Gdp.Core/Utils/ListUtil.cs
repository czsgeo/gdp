//2015.06.06, czs, create in namu, 

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Linq;


namespace Gdp.Utils
{
    /// <summary>
    /// 列表
    /// </summary>
    public static class ListUtil
    {

        /// <summary>
        /// 快速设置列表维数。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="dimension"></param>
        public static List<T> SetDimension<T>(List<T> list, int dimension)
        {
            if (list == null)
            {
                var array = new T[dimension];
                list = new List<T>(array);
            }
            if (list.Count < dimension)
            {
                var adding = new T[dimension - list.Count];
                list.AddRange(adding);
            }
            return list;
        }
        /// <summary>
        /// 不重复的添加，添加前判断一次。
        /// </summary>
        /// <param name="siteNames"></param>
        /// <param name="siteName"></param>
        public static void AddNoRepeat<T>(List<T> siteNames, T siteName)
        {
            if (!siteNames.Contains(siteName))
            {
                siteNames.Add(siteName);
            }
        }

        /// <summary>
        /// 是否相等
        /// </summary>
        /// <param name="listA"></param>
        /// <param name="listB"></param>
        /// <returns></returns>
        public static bool IsEqual(List<string> listA, List<string> listB)
        {
            //判断未知数量是否一致
            if (listA.Count != listB.Count) return false;
            return GetDifferences(listA, listB).Count == 0; 
        }
        /// <summary>
        /// 获取不同的部分
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listA"></param>
        /// <param name="listB"></param>
        /// <returns></returns>
        static public List<T> GetDifferences<T>(IEnumerable<T> listA, IEnumerable<T> listB)
        {
            List<T> list = new List<T>();
            foreach (var item in listA)
            {
                if (!listB.Contains(item)) list.Add(item);
            }

            foreach (var item in listB)
            {
                if (!listA.Contains(item)) list.Add(item);
            }
            return list;
        }
        /// <summary>
        /// 查找在列表中的未知，最为接近即可。从小到大的排列。
        /// 采用递归二分法实现。
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="orderedKeys"></param>
        /// <param name="key"></param>
        /// <param name="keyToDouble">计算二者的距离，以判断远近</param>
        /// <param name="lowIndex"></param>
        /// <param name="highIndex"></param>
        /// <param name="prevIndex"></param>
        /// <param name="prevDistance"></param>
        /// <returns></returns>
        internal static int ClosestToIndexOf<TKey>(List<TKey> orderedKeys, TKey key, Func<TKey,double> keyToDouble = null, int lowIndex = 0, int highIndex = int.MaxValue, int prevIndex = int.MaxValue, double prevDistance = int.MaxValue)
            where TKey : IComparable<TKey>
        {
            if(keyToDouble == null)
            {
                Gdp.Utils.DoubleUtil.AutoSetKeyToDouble(out keyToDouble);//= new Func<TKey, double>((m) =>  Convert.ToDouble(m));
            }

            //orderedKeys.Min(new Func<TKey, double>(m => m.CompareTo(key)));
            if(lowIndex == highIndex) { //查找到了边界
                return lowIndex;
            }

            if (highIndex == int.MaxValue || highIndex == -1 )  {   highIndex = orderedKeys.Count - 1;  }

            int midIndex = lowIndex + (highIndex - lowIndex) / 2;
            if(midIndex == lowIndex || midIndex == highIndex) { return midIndex; }

            double differ = keyToDouble(key)- keyToDouble(orderedKeys[midIndex]);  // orderedKeys[mid]  .CompareTo(key);
            double currentDistance = Math.Abs(differ);

            //if (prevDistance == int.MaxValue)//初始赋值
            //{
            //    prevDistance = currentDistance;
            //}
            //else if (currentDistance > prevDistance)//如果比上次大，则返回上次结果，否则比上次小，继续查找
            //{
            //    return prevIndex;
            //}

            //判断当前和中间的位置
            if (differ == 0)
            {
                return midIndex;
            }
            else  if (differ > 0)//当前值比中间的大，则在大的区域查找
            {
                return ClosestToIndexOf(orderedKeys, key, keyToDouble, midIndex + 1, highIndex, midIndex, currentDistance);
            }
            else //否则在小的区域查找
            {
                return ClosestToIndexOf(orderedKeys, key, keyToDouble, lowIndex, midIndex - 1, midIndex, currentDistance);
              
            }
        }
        /// <summary>
        /// 返回所有，不重复
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys"></param>
        /// <param name="prns"></param>
        /// <returns></returns>
        public static List<T> GetAll<T>(IEnumerable<T> keys, IEnumerable<T> prns)
        {
            List<T> all = new List<T>(keys);
            foreach (var item in prns)
            {
                if (!all.Contains(item))
                {
                    all.Add(item);
                }
            }

            return all;
        }

        /// <summary>
        /// 获取相同的部分
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listA"></param>
        /// <param name="listB"></param>
        /// <returns></returns>
        static public List<T> GetCommonsOfTwo<T>(IEnumerable<T> listA, IEnumerable<T> listB)
        {
            List<T> list = new List<T>();
            foreach (var item in listA)
            {
                if (listB.Contains(item)) list.Add(item);
            }
            return list;
        }
        /// <summary>
        /// 获取相同的部分
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lists"></param> 
        /// <returns></returns>
        static public List<T> GetCommons<T>( params  IEnumerable<T> [] lists)
        {
            int length = lists.Length;
            var first = lists[0];

            List<T> commons =new List<T>( first);
            for (int i = 1; i < length; i++)
            {
                commons = GetCommonsOfTwo<T>(commons, lists[i]); 
            } 
            return commons;
        }

        /// <summary>
        /// 获取前面有，后面没有的部分
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listhas"></param>
        /// <param name="listNot"></param>
        /// <returns></returns>
        static public List<T> GetExcept<T>(IEnumerable<T> listhas, IEnumerable<T> listNot)
        {
            List<T> list = new List<T>();
            foreach (var item in listhas)
            {
                if (!listNot.Contains(item)) list.Add(item);
            }
            return list;
        }


        /// <summary>
        /// 合并
        /// </summary> 
        /// <param name="indexes"></param>
        /// <param name="oneIndexe"></param>
        /// <returns></returns>
        public static List<T> Emerge<T>(IEnumerable<T> indexes, IEnumerable<T> oneIndexe)
        {
            var differ = GetExcept<T>(oneIndexe, indexes);
            List<T> result = new List<T>();
            result.AddRange(indexes);
            result.AddRange(differ);
            return result;

        }

        /// <summary>
        /// 获取当前外的下一个
        /// </summary>
        /// <param name="colName"></param>
        /// <param name="current"></param>
        /// <returns></returns>
        public static T GetNext<T>(List<T> list, T current)
        {
            var index = list.LastIndexOf(current);
            if (index < list.Count - 1)
            {
                return list[index + 1];
            }
            return default(T);
        }

        /// <summary>
        /// 返回无重复元素的列表。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<T> GetNoRepeatList<T>(IEnumerable<T> list)
        {
            List<T> result = new List<T>();
            foreach (var item in list)
            {
                if (result.Contains(item)) { continue; }
                result.Add(item);                
            }

            return result;
        }
        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="list"></param>
        /// <param name="spliters"></param>
        /// <returns></returns>
        public static List<string> Parse(string list, params char[] spliters)
        {
            if(spliters == null || spliters.Length == 0)
            {
                spliters = new char[] {',',';','\t','\n','，','；' };

            }
            return new List<string>(list.Split(spliters, StringSplitOptions.RemoveEmptyEntries));
        }
        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="list"></param> 
        /// <returns></returns>
        public static List<int> ParseToInts(List<string> list)
        {
            var ints = new List<int>();
            foreach (var item in list)
            {
                ints.Add(int.Parse(item));
            }
            return ints;
        }
        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="list"></param>
        /// <param name="spliters"></param>
        /// <returns></returns>
        public static List<int> ParseToInts(string list, params char[] spliters)
        {
            return ParseToInts(Parse(list, spliters));
        }

        /// <summary>
        /// 批量移除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <param name="tobeRemoves"></param>
        public static void RemoveAt<T>(List<T> values, List<int> tobeRemoves)
        {
            if(tobeRemoves.Count == 0) { return; }

            tobeRemoves.Sort();
            tobeRemoves.Reverse();//逆序删除
            foreach (var item in tobeRemoves)
            {
                values.RemoveAt(item);
            }
        }
    }
}
