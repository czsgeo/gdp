// 2014.12.11, czs, create in jinxingliaomao shuangliao，卫星编号工具类
// 2016.03.17, czs, edit in hongqing, 增加 GetDifferentPrns

using System;
using System.Collections.Generic; 
using System.Text;
using Gdp.Utils;

namespace Gdp
{
   
    /// <summary>
    /// 卫星编号工具类
    /// </summary>
    public static class SatelliteNumberUtils
    {
        /// <summary>
        /// 如果有不同返回true， 相同则返回false。
        /// </summary>
        /// <param name="olds"></param>
        /// <param name="news"></param>
        /// <param name="addedPrns"></param>
        /// <param name="removedPrns"></param>
        /// <returns></returns>
        public static bool FindDifferentPrns(List<SatelliteNumber> olds, List<SatelliteNumber> news, out List<SatelliteNumber> addedPrns, out List<SatelliteNumber> removedPrns)
        {
            removedPrns = olds.FindAll(m => !news.Contains(m));
            addedPrns = news.FindAll(m => !olds.Contains(m));

            return addedPrns.Count != 0 || removedPrns.Count != 0;
        } 

        /// <summary>
        /// 两个卫星列表是否是相同的大小和顺序
        /// </summary>
        /// <param name="prnsA">待比较的卫星编号列表A</param>
        /// <param name="prnsB">待比较的卫星编号列表B</param>
        /// <returns></returns>
        public static bool IsEqual(List<SatelliteNumber> prnsA, List<SatelliteNumber> prnsB)
        {
            prnsA.Sort();
            prnsB.Sort();

            //判断卫星数量是否一致
            if (prnsA.Count != prnsB.Count) return false;
            int count = prnsA.Count;
            //卫星顺序是否一致
            for (int i = 0; i < count; i++)
            {
                if (!prnsA[i].Equals(prnsB[i]))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 获取两个列表中不同的部分。
        /// </summary>
        /// <param name="listA">卫星列表 A </param>
        /// <param name="listB">卫星列表 B </param>
        /// <returns></returns>
        public static List<SatelliteNumber> GetDiffers(List<SatelliteNumber> listA, List<SatelliteNumber> listB)
        {
            List<SatelliteNumber> result = new List<SatelliteNumber>();
            foreach (var item in listA)
            {
                if (!listB.Contains(item)) result.Add(item);
            }
            foreach (var item in listB)
            {
                if (!listA.Contains(item)) result.Add(item);
            }
            return result;
        }
        /// <summary>
        /// 获取新出现的新卫星编号。
        /// </summary>
        /// <param name="news">卫星列表 A </param>
        /// <param name="olds">卫星列表 B </param>
        /// <returns></returns>
        public static List<SatelliteNumber> GetNews(List<SatelliteNumber> news, List<SatelliteNumber> olds)
        {
            List<SatelliteNumber> result = new List<SatelliteNumber>();
            foreach (var item in news)
            {
                if (!olds.Contains(item)) result.Add(item);
            }
            return result;
        }

        /// <summary>
        /// 获取两个列表中相同的部分，都包含的卫星列表。
        /// </summary>
        /// <param name="listA">卫星列表 A </param>
        /// <param name="listB">卫星列表 B </param>
        /// <returns></returns>
        public static List<SatelliteNumber> GetCommons(List<SatelliteNumber> listA, List<SatelliteNumber> listB)
        {
            List<SatelliteNumber> result = new List<SatelliteNumber>();
            foreach (var item in listA)
            {
                if (listB.Contains(item)) result.Add(item);
            }
            return result;
        }
        /// <summary>
        /// 卫星编号转换成字符串
        /// </summary>
        /// <param name="listA"></param>
        /// <param name="splitter"></param>
        /// <returns></returns>
        public static string GetString(List<SatelliteNumber> listA, string splitter = ",")
        {
            if (listA == null) { return ""; }
            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach (var item in listA)
            {
                if (i != 0) { sb.Append(splitter); }
                sb.Append(item);
                i++;
            }

            return sb.ToString();
        }
        /// <summary>
        /// 解析字符串为卫星编号
        /// </summary>
        /// <param name="str"></param>
        /// <param name="splitter"></param>
        /// <returns></returns>
        public static List<SatelliteNumber>  ParseString( string str, char [] splitter =null)
        {
            if(splitter == null){
                splitter = new char[] { ',', ';', '\t', ' ' };
            }
           
            List<SatelliteNumber> prns = new List<SatelliteNumber>();
            var strs = str.Split( splitter  , StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in strs)
            {
                var prn = SatelliteNumber.Parse(item);
                if(prn.SatelliteType != SatelliteType.U)
                prns.Add(prn);
            }
            return prns;
        } 
    }
}
