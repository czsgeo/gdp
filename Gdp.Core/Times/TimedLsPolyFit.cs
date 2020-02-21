//2018.06.20, czs, create in hmx, 时间为关键字的最小二乘多项式拟合

using System;
using System.Collections.Generic;
using System.Collections;
using System.Text; 
using System.Linq; 


namespace Gdp
{ 
    /// <summary>
    ///时间为关键字的最小二乘多项式拟合 
    /// </summary>
    public class TimedLsPolyFit// : ITimeXGetY
    {

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="dics"></param>
        /// <param name="order"></param>
        public TimedLsPolyFit(IDictionary<Time, double> dics, int order)
        {
            this.Order = order;

            DicData = new Dictionary<double, double>();
            var keys = dics.Keys;
            keys.OrderBy(m => m.DateTime);

            this.FirstTime = keys.First();

            foreach (var time in keys)
            {
                var data = dics[time];
                double x = (time - FirstTime);
                double y = data;
                DicData.Add(x, y);
            } 
        }
        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            LsPolyFit = new LsPolyFit(this.Order);
            LsPolyFit.InitAndFitParams<double>(DicData, m => m);
        }
        /// <summary>
        /// 阶次
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// 第一个时间，历元
        /// </summary>
        public Time FirstTime { get; set; }
        LsPolyFit LsPolyFit { get; set; }
        Dictionary<double, double> DicData { get; set; }
        /// <summary>
        /// 字符串显示
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        { 
            return LsPolyFit.ToString();
        }
        /// <summary>
        /// 结束减去起始
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public double GetDifferOf(Time end,Time start)
        {
            return GetY(end) - GetY(start);
        }
        /// <summary>
        /// 结束减去起始
        /// </summary>
        /// <param name="end"></param> 
        /// <returns></returns>
        public double GetDifferFromStart(Time end)
        {
            return GetY(end) - GetY(FirstTime);
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public double GetY(Time time)
        {
            if (LsPolyFit == null)
            {
                throw new Exception("请Init初始化先！");
            }
            double xValue = time - FirstTime;

            return LsPolyFit.GetY(xValue);
        }
        /// <summary>
        /// 获取结果
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public RmsedNumeral GetRmsedY(Time time)
        {
            var y = GetY(time);
            return new RmsedNumeral(y, this.LsPolyFit.StdDev);
        }
    }
}
