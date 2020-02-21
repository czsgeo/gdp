//2016.10.28, czs, create in hongqing, 中心区域

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics; 
using Gdp.Utils;


namespace Gdp
{
    /// <summary>
    /// 中心区域.二维或三维 
    public abstract class CenterRegion<TCoord>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        public CenterRegion(TCoord center, double radius)
        {
            this.Center = center;
            this.Radius = radius;
        }
        /// <summary>
        /// 中心坐标
        /// </summary>
        public TCoord Center { get; set; }
        /// <summary>
        /// 半径
        /// </summary>
        public double Radius { get; set; }
        /// <summary>
        /// 是否包含
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        public abstract bool Contains(TCoord coord);
    }

    /// <summary>
    /// 三维空间直角坐标的中心区域
    /// </summary>
    public class XyzCenterRegion : CenterRegion<XYZ>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        public XyzCenterRegion(XYZ center, double radius) : base(center, radius)
        {
        }

        /// <summary>
        /// 是否包含
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        public override bool Contains(XYZ coord)
        {
            return (this.Center - coord).Length <= Radius;
        }
    }
}
