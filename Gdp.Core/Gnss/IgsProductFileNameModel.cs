//2017.08.18, czs, create in hongqing, IGS产品名称生成器
//2018.03.15, czs, edit in hmx, 增加快速和超快SP3，ERP等的配置

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
using System.IO;
using Gdp.Data; 

namespace Gdp.Data
{ 
    /// <summary>
    /// IGS产品名称生成器
    /// </summary>
    public class IgsProductFileNameModel : BaseDictionary<IgsProductType, string>
    {
        /// <summary>
        /// 单例模式
        /// </summary>
        public static IgsProductFileNameModel Instance = new IgsProductFileNameModel();

        /// <summary>
        /// 构造函数
        /// </summary>
        private IgsProductFileNameModel()
        {
            //不同产品，对应的名称EL组成-------注意：有的FTP文件大小写不一样认为是不同的文件，会导致下载失败！！
            this.Add(IgsProductType.igu_Sp3, "igu" + ELMarker.Week + ELMarker.DayOfWeek + "_" + ELMarker.Hour + ".sp3.Z");
            this.Add(IgsProductType.igv_Sp3, "igu" + ELMarker.Week + ELMarker.DayOfWeek + "_" + ELMarker.Hour + ".sp3.Z");
            this.Add(IgsProductType.igr_Sp3, ELMarker.SourceName + ELMarker.Week + ELMarker.DayOfWeek + ".sp3.Z");
            this.Add(IgsProductType.Sp3, ELMarker.SourceName+ELMarker.Week+ELMarker.DayOfWeek+ ".sp3.Z");
            this.Add(IgsProductType.Bia, ELMarker.SourceName+ELMarker.Week+ELMarker.DayOfWeek+ ".bia.Z");
            this.Add(IgsProductType.igr_Clk, ELMarker.SourceName + ELMarker.Week + ELMarker.DayOfWeek + ".igr_clk.Z");
            this.Add(IgsProductType.Clk, ELMarker.SourceName + ELMarker.Week + ELMarker.DayOfWeek + ".clk.Z");
            this.Add(IgsProductType.Clk_05s, ELMarker.SourceName + ELMarker.Week + ELMarker.DayOfWeek + ".clk_05s.Z");
            this.Add(IgsProductType.Clk_30s, ELMarker.SourceName + ELMarker.Week + ELMarker.DayOfWeek + ".clk_30s.Z");
            this.Add(IgsProductType.Dcb, ELMarker.SourceName+ELMarker.Week+ELMarker.DayOfWeek+".DCB.Z");
            this.Add(IgsProductType.Eph, ELMarker.SourceName + ELMarker.Week + ELMarker.DayOfWeek + ".eph.Z");
            this.Add(IgsProductType.igu_Erp, "igu" + ELMarker.Week + ELMarker.DayOfWeek + "_" + ELMarker.Hour + ".erp.Z");
            this.Add(IgsProductType.igv_Erp, "igv" + ELMarker.Week + ELMarker.DayOfWeek + "_" + ELMarker.Hour + ".erp.Z");
            this.Add(IgsProductType.igr_Erp, ELMarker.SourceName + ELMarker.Week + ELMarker.DayOfWeek + "_" + ELMarker.Hour + ".erp.Z");
            this.Add(IgsProductType.Erp, ELMarker.SourceName + ELMarker.Week + "7.erp.Z"); //最终版ERP为末尾带7，全周的。
            this.Add(IgsProductType.I, ELMarker.SourceName + "g" + ELMarker.DayOfYear + "0." + ELMarker.SubYear +  "i.Z");
            this.Add(IgsProductType.O, ELMarker.SiteName + ELMarker.DayOfYear + "0." + ELMarker.SubYear + "o.Z");
           // this.Add(IgsProductType.N, ELMarker.SiteName + ELMarker.DayOfYear + "0." + ELMarker.SubYear + ELMarker.ProductType + ".Z");
            this.Add(IgsProductType.N, "brdc"+ ELMarker.DayOfYear + "0." + ELMarker.SubYear + "n.Z");
            this.Add(IgsProductType.Snx, ELMarker.SourceName+ELMarker.Week+ELMarker.DayOfWeek+".snx.Z");
            this.Add(IgsProductType.Sum, ELMarker.SourceName+ELMarker.Week+ELMarker.DayOfWeek+".sum.Z");
            this.Add(IgsProductType.Ssc, ELMarker.SourceName+ELMarker.Week+ELMarker.DayOfWeek+".ssc.Z");
            this.Add(IgsProductType.ION, "COD"+ELMarker.Week+ELMarker.DayOfWeek+"."+ELMarker.ProductType+".Z");

            //不同产品对应的扩展名称
            extensions = new Dictionary<IgsProductType, string>();
            extensions.Add(IgsProductType.igu_Sp3, ".sp3.Z");
            extensions.Add(IgsProductType.igv_Sp3, ".sp3.Z");
            extensions.Add(IgsProductType.igr_Sp3, ".sp3.Z");
            extensions.Add(IgsProductType.Sp3,  ".sp3.Z");
            extensions.Add(IgsProductType.Bia,  ".bia.Z");
            extensions.Add(IgsProductType.igr_Clk,  ".clk.Z");
            extensions.Add(IgsProductType.Clk,  ".clk.Z");
            extensions.Add(IgsProductType.Clk_05s, ".clk_05s.Z");
            extensions.Add(IgsProductType.Clk_30s, ".clk_30s.Z");
            extensions.Add(IgsProductType.Dcb,  ".DCB.Z");
            extensions.Add(IgsProductType.Eph,  ".eph.Z");
            extensions.Add(IgsProductType.Erp, ".erp.Z");
            extensions.Add(IgsProductType.igu_Erp, ".erp.Z");
            extensions.Add(IgsProductType.igv_Erp, ".erp.Z");
            extensions.Add(IgsProductType.igr_Erp, ".erp.Z");
            extensions.Add(IgsProductType.I, "." + ELMarker.SubYear + "i.Z");
            extensions.Add(IgsProductType.O, "." + ELMarker.SubYear + ELMarker.ProductType + ".Z");
            extensions.Add(IgsProductType.N, "." + ELMarker.SubYear + ELMarker.ProductType + ".Z");
            extensions.Add(IgsProductType.Snx,  "." + ELMarker.ProductType + ".Z");
            extensions.Add(IgsProductType.Sum,  "." + ELMarker.ProductType + ".Z");
            extensions.Add(IgsProductType.Ssc,  "." + ELMarker.ProductType + ".Z");
            extensions.Add(IgsProductType.ION,  "." + ELMarker.ProductType + ".Z");
        }
        /// <summary>
        /// 扩展名数据
        /// </summary>
        private Dictionary<IgsProductType, string> extensions { get; set; }
        /// <summary>
        /// 获取扩展名
        /// </summary>
        /// <param name="IgsProductType"></param>
        /// <returns></returns>
        public string GetExtensionModel(IgsProductType IgsProductType) { return extensions[IgsProductType]; }

    } 
    
    
}