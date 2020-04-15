//2017.08.18, czs, create in hongqing, IGS产品名称生成器
//2017.05.27, czs,  edit in hmx, 可以直接以ftp开头，将自动切断前面的目录

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
using System.IO; 
using Gdp.Data; 

namespace Gdp.Data
{ 
    /// <summary>
    /// IGS 产品目录模板，即存储文件的位置
    /// </summary>
    public class IgsProductDirectoryModel : BaseDictionary<IgsProductType, string>
    {
        /// <summary>
        /// 单例模式
        /// </summary>
        public static IgsProductDirectoryModel Instance = new IgsProductDirectoryModel();

        /// <summary>
        /// 构造函数
        /// </summary>
        private IgsProductDirectoryModel()
        {
            this.Add(IgsProductType.igu_Sp3, ELMarker.Week);
            this.Add(IgsProductType.igv_Sp3, ELMarker.Week);
            this.Add(IgsProductType.igr_Sp3, ELMarker.Week);
            this.Add(IgsProductType.igu_Erp, ELMarker.Week);
            this.Add(IgsProductType.igv_Erp, ELMarker.Week);

            this.Add(IgsProductType.Sp3, ELMarker.Week);
            this.Add(IgsProductType.Clk, ELMarker.Week);
            this.Add(IgsProductType.igr_Clk, ELMarker.Week);
            this.Add(IgsProductType.Clk_05s, ELMarker.Week);
            this.Add(IgsProductType.Clk_30s, ELMarker.Week);
            this.Add(IgsProductType.Bia, ELMarker.Week);
            this.Add(IgsProductType.Dcb, ELMarker.Week);
            this.Add(IgsProductType.Eph, ELMarker.Week);
            this.Add(IgsProductType.Erp, ELMarker.Week);
            this.Add(IgsProductType.ION, "ftp://ftp.aiub.unibe.ch/CODE/{Year}");//可以直接以ftp开头，将自动切断前面的目录
            this.Add(IgsProductType.I, "ionex/{Year}/{DayOfYear}");
            this.Add(IgsProductType.O, "../data/daily/{Year}/{DayOfYear}/{SubYear}o");
            this.Add(IgsProductType.N, "../data/daily/{Year}/{DayOfYear}/{SubYear}n");
            this.Add(IgsProductType.Snx, ELMarker.Week);
            this.Add(IgsProductType.Sum, ELMarker.Week);
            this.Add(IgsProductType.Ssc, ELMarker.Week);
        }
    }
    
    
}