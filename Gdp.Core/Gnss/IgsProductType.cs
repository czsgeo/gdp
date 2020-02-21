//2015.12.09, czs, create in 达州到成都火车 D5181, IGS 网络产品服务数据源类型
//2018.03.15, czs, edit in hmx, 提取到单独文件，增加快速和超快SP3，ERP等
//2018.05.26, czs, create in HMX, CODE电离层球谐函数

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gdp
{  
    /// <summary>
    /// IGS 网络产品服务数据源类型
    /// </summary>
    public enum IgsProductType
    {
        /// <summary>
        /// 快速产品
        /// </summary>
        igr_Sp3,
        /// <summary>
        /// 超快星历，每6小时播发一次
        /// </summary>
        igu_Sp3,
        /// <summary>
        /// 超快星历，含GLONASS
        /// </summary>
        igv_Sp3,
        /// <summary>
        /// 快速
        /// </summary>
        igr_Erp,
        /// <summary>
        /// 超快
        /// </summary>
        igu_Erp,
        /// <summary>
        /// 超快，含GLONASS
        /// </summary>
        igv_Erp,
        /// <summary>
        /// 快速钟差，延迟一天,而超快在SP3内
        /// </summary>
        igr_Clk,
        Erp,
        Sp3,
        Clk,
        Clk_05s,
        Clk_30s,
        Snx,
        Ssc,
        Eph,
        Dcb,
        Bia,
        Sum,
        /// <summary>
        /// IGS 格网电离层
        /// </summary>
        I,
        /// <summary>
        /// CODE 电离层
        /// </summary>
        ION,
        /// <summary>
        /// 观测文件
        /// </summary>
        O,
        /// <summary>
        /// 导航星历
        /// </summary>
        N

    }
}
