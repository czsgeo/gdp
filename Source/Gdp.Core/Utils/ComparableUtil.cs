//2018.06.25, czs, create in HMX, 可比较的工具

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gdp.Utils
{
    /// <summary>
    /// 可比较的工具
    /// </summary>
    public class ComparableUtil
    {
        /// <summary>
      /// 获取距离最接近的列表
      /// </summary>
      /// <param name="XArray">待选</param>
      /// <param name="refKey">指定值</param>
      /// <param name="count">选择数量</param>
      /// <param name="keyToDouble">转换为浮点数计算</param>
      /// <returns></returns>
        private static List<TKey> GetNearstList<TKey>(List<TKey> XArray, TKey refKey, int count = 9, Func<TKey, double> keyToDouble = null)
            where TKey : IComparable<TKey>
        {
            List<TKey> indexes = new List<TKey>();
            //如果数量大于数组数量，则返回全部
            if (count >= XArray.Count)
            {
                return XArray;
            }
            double refValu = 0;
            if (keyToDouble != null)
            {
                refValu = keyToDouble(refKey);
            }

            //找到离X最小的编号
            int halfCount = count / 2;
            int index = 0; 
            Dictionary<double, TKey> smaller = new Dictionary<double, TKey>();
            Dictionary<double, TKey> larger = new Dictionary<double, TKey>();
            for (int i = 0; i < XArray.Count; i++)
            {
                var key = XArray[i];
                double diff = 0;

                if (keyToDouble != null)
                {
                    diff =  keyToDouble(key) - (refValu);
                }
                else
                {
                    diff = key.CompareTo(refKey);
                }


                if(diff >= 0)
                {
                    larger.Add(diff, key);
                }
                else
                {
                    smaller.Add(diff, key);
                } 
            }

         //   larger.Keys.Sort(); 





            //在数组的头部 
            int startIndex = 0;
            if (index - halfCount <= 0) { startIndex = 0; }//for (int i = 0; i < count; i++) indexes.Add(XArray[i]);
            //尾部
            else if (index + halfCount >= XArray.Count - 1) { startIndex = XArray.Count - count; }//for (int i = 0, j = XArray.Count - 1; i < count; i++, j--) indexes.Insert(0, XArray[j]);
            //中间
            else for (int i = 0; i < count; i++) { startIndex = index - halfCount; }// indexes.Add(XArray[index - halfCount + i]);

            //if (indexes[0] < 0) throw new Exception("出错了");

            indexes = XArray.GetRange(startIndex, count);

            return indexes;
        }

        /// <summary>
        /// 获取最接近的列表，注意这种需要无断裂，连续的才行
        /// </summary>
        /// <param name="XArray">待选</param>
        /// <param name="tobeNear">指定值</param>
        /// <param name="count">选择数量</param>
        /// <param name="keyToDouble">转换为浮点数计算</param>
        /// <returns></returns>
        public static List<TKey> GetNearst<TKey>(List<TKey> XArray, TKey tobeNear, int count = 9, Func<TKey, double> keyToDouble = null)
            where TKey : IComparable<TKey>
        {
            List<TKey> indexes = new List<TKey>();
            //如果数量大于数组数量，则返回全部
            if (count >= XArray.Count)
            {
                return XArray;
            }
            double doubleKey = 0;
            if (keyToDouble != null)
            {
                doubleKey = keyToDouble(tobeNear);
            }

            //找到离X最小的编号
            int halfCount = count / 2;
            int index = 0;
            double min = double.MaxValue;
            for (int i = 0; i < XArray.Count; i++)
            {
                double diff = 0;

                if(keyToDouble != null)
                {
                    diff = Math.Abs(keyToDouble(XArray[i]) - (doubleKey));
                }
                else
                {
                    diff = Math.Abs(XArray[i].CompareTo(tobeNear));
                }


                // if (diff == 0) return YArray[time];
                if (diff < min)
                {
                    min = diff;
                    index = i;
                }
            }
            //在数组的头部 
            int startIndex = 0;
            if (index - halfCount <= 0) { startIndex = 0; }//for (int i = 0; i < count; i++) indexes.Add(XArray[i]);
            //尾部
            else if (index + halfCount >= XArray.Count - 1) { startIndex = XArray.Count - count; }//for (int i = 0, j = XArray.Count - 1; i < count; i++, j--) indexes.Insert(0, XArray[j]);
            //中间
            else for (int i = 0; i < count; i++) { startIndex = index - halfCount; }// indexes.Add(XArray[index - halfCount + i]);

            //if (indexes[0] < 0) throw new Exception("出错了");

            indexes = XArray.GetRange(startIndex, count);

            return indexes;
        }
         
    }
}
