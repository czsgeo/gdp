//2014.12.04, czs, create in jinxingliaomao husangliao, 多 GNSS 系统选择
//2018.04.27, czs, edit in hmx, 增加SBAS、QZSS、IRNSS系统的支持

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Gdp.Winform.Controls
{
    /// <summary>
    /// 多GNSS系统选择器
    /// </summary>
    public partial class MultiGnssSystemSelectControl : UserControl
    {
        public MultiGnssSystemSelectControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// GNSS 系统类型。
        /// </summary>
        public List<GnssType> GnssTypes
        {
            get
            {
                List<GnssType> gnssSystems = new List<GnssType>();
                foreach (var sat in SatelliteTypes)
                {
                    gnssSystems.Add(GnssSystem.GetGnssType(sat));
                }
                return gnssSystems;
            }
        }

        /// <summary>
        /// GNSS 系统。
        /// </summary>
        public List<GnssSystem> GnssSystems
        {
            get
            {
                List<GnssSystem> gnssSystems = new List<GnssSystem>();
                foreach (var sat in SatelliteTypes)
                {
                    gnssSystems.Add(GnssSystem.GetGnssSystem(sat));
                }
                return gnssSystems;
            }
        }

        public void SetSatelliteTypes(List<SatelliteType> SatelliteTypes)
        {
            this.SatelliteTypes = SatelliteTypes;
        }

        /// <summary>
        /// 卫星类型
        /// </summary>
        public List<SatelliteType> SatelliteTypes
        {
            get
            {
                List<SatelliteType> satelliteTypes = new List<SatelliteType>();
                if (checkBox_beidou.Checked) satelliteTypes.Add(SatelliteType.C);
                if (this.checkBox_galileo.Checked) satelliteTypes.Add(SatelliteType.E);
                if (this.checkBox_gps.Checked) satelliteTypes.Add(SatelliteType.G);
                if (this.checkBox_glonass.Checked) satelliteTypes.Add(SatelliteType.R);
                if (this.checkBox_SBAS.Checked) satelliteTypes.Add(SatelliteType.S);
                if (this.checkBox_NAVIC.Checked) satelliteTypes.Add(SatelliteType.I);
                if (this.checkBox_qzss.Checked) satelliteTypes.Add(SatelliteType.J);
                return satelliteTypes;
            }
           protected set
            { 
                if (value == null || value.Count ==0) return;
                this.checkBox_beidou.Checked = value.Contains(SatelliteType.C);
                this.checkBox_galileo.Checked = value.Contains(SatelliteType.E);
                (this.checkBox_gps.Checked) = value.Contains(SatelliteType.G);
                this.checkBox_glonass.Checked = value.Contains(SatelliteType.R);
                this.checkBox_NAVIC.Checked = value.Contains(SatelliteType.I);
                this.checkBox_SBAS.Checked = value.Contains(SatelliteType.S);
                this.checkBox_qzss.Checked = value.Contains(SatelliteType.J);
            }
        }
    }
}
