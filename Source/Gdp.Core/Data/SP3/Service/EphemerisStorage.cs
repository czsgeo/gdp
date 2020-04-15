//2013.11.04, czs, edit, 采用二级缓存加快计算速度
//2014.06.24, 崔阳, edit, 修改了IsHealth(),首先判断是否满足插值的条件。判断是否满足插值的限差   Cui Yang 2014.06.24
//2014.12.29,czs, edit in namu, 缓冲不易设置太大，如果设置2倍的话，会造成在星历拼接的时段结果不稳定。
//2015.05.10, czs, edit in namu, 分离数据与服务
//2016.03.06, czs, edit in hongqing, 优化
//2016.09.27, czs, edit in hongqqing, 优化，可用性判断

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO; 


namespace Gdp.Data.Rinex
{
     
    /// <summary>
    /// 单个卫星星历（XYZ和钟差）存储器，以历元周内秒为索引。
    /// </summary>
    public class EphemerisStorage : BaseDictionary<double, Gdp.Ephemeris>
    {
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="indexes"></param>
        /// <returns></returns>
        public List<Gdp.Ephemeris> GetEphemerises(List<double> indexes)
        {
            List<Gdp.Ephemeris> entities = new List<Gdp.Ephemeris>();
            foreach (var item in indexes)
            {
                entities.Add(this[item]);
            }
            return entities;
        }
        /// <summary>
        /// 获取指定
        /// </summary>
        /// <param name="indexes"></param>
        /// <returns></returns>
        public EphemerisStorage GetSubEphemerises(List<double> indexes)
        {
            EphemerisStorage entities = new EphemerisStorage();
            foreach (var item in indexes)
            {
                entities.Add(item, this[item]);
            }
            return entities;
        }
        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="Ephemerises"></param>
        public void Add(IEnumerable<Gdp.Ephemeris> Ephemerises)
        {
            foreach (var item in Ephemerises)
            {
                this.Set(item.Time.SecondsOfWeek, item);
            }
        }
        /// <summary>
        /// 字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Count + "";// base.ToString();
        }
        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="time"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public EphemerisStorage GetGetNearst(Time time, int order)
        {
            var storege = new EphemerisStorage(); 

            double minAbs = double.MaxValue;
            var nearTime = this.First.Time;
            foreach (var item in this)
            {
                var differ = item.Time - time;
                var absdiffer = Math.Abs(differ);
                if (minAbs > absdiffer)
                {
                    nearTime = item.Time;
                    minAbs = absdiffer;
                } 
                storege.Add(differ, item);
            }

            var indexes = Utils.DoubleUtil.GetNearstValues(storege.Keys, minAbs, order);

            return storege.GetSubEphemerises(indexes);
        }
    }
}