//2016.10.28, czs, create  in hongqing, 观测文件选择选项。

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace Gdp.Winform
{
    public partial class ObsFileSelectOptionForm : Form
    {
        public ObsFileSelectOptionForm()
        {
            InitializeComponent();
            Init();
        }

        public ObsFileSelectOptionForm(ObsFileSelectOption option)
        {
            InitializeComponent();
            Init();

            this.SetOption(option);
        }

        private void Init()
        {
            enabledFloatControl_mp1Factor.Init("Multipath effect MP1 value filtering:", 0.6);
            enabledFloatControl_mp2Factor.Init("Multipath effect MP2 value filtering:", 0.6);
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            UiToEntity();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void SetOption(ObsFileSelectOption Option)
        {
            this.Option = Option;
            EntityToUi();
        }

        /// <summary>
        /// 选项
        /// </summary>
        public ObsFileSelectOption Option { get; set; }


        private void EntityToUi()
        {
            if (Option == null) { Option = CreateNewModel(); }

            this.checkBox_enableCode.Checked = Option.IsEnableObsCodes;
            this.checkBox_enableExcludeSiteNames.Checked = Option.IsEnableExcludeSiteNames;
            this.checkBox_enableincludeSiteNames.Checked = Option.IsEnableIncludeSiteNames;
            this.checkBox_enableRegionFilter.Checked = Option.IsEnableCenterRegion;
            this.checkBox_enableTimePeriod.Checked = Option.IsEnableTimePeriod;
            this.timePeriodControl1.SetTimePerid(Option.TimePeriod);

            this.textBox_centerXyz.Text = Option.CenterRegion.Center + "";
            this.textBox_radiusFromCenter.Text = Option.CenterRegion.Radius + "";
            this.textBox_obsCodes.Text = Gdp.Utils.StringUtil.GetFormatedString<string>(Option.ObsCodes, ",");
            this.textBox_includeSiteNames.Text = Gdp.Utils.StringUtil.GetFormatedString<string>(Option.IncludeSiteNames, ",");
            this.textBox_excludeSites.Text = Gdp.Utils.StringUtil.GetFormatedString<string>(Option.ExcludeSiteNames, ",");
            this.textBox1MinEpochCount.Text = Option.MinEpochCount + "";
            this.checkBox1MinEpochCount.Checked = Option.IsEnableMinEpochCount;

            this.textBox1MinFileSizeMB.Text = Option.MinFileSizeMB + "";
            this.checkBox1MinFileSizeMB.Checked = Option.IsEnableMinFileSizeMB;


            this.textBox1MinFrequencyCount.Text = Option.MinFrequencyCount + "";
            this.checkBox1MinFrequencyCount.Checked = Option.IsEnableMinFrequencyCount;

            checkBox_IsNavCopy.Checked = Option.IsNavCopy;

            this.textBox1MinSatCount.Text = Option.MinSatCount + "";
            this.textBox2MinSatCountRatio.Text = Option.MinRatioOfSatCount + "";
            this.checkBox1MinSatCount.Checked = Option.IsEnableMinRatioOfSatCount;

            enabledFloatControl_mp1Factor.SetEnabledValue(this.Option.MultipathMp1);
            enabledFloatControl_mp2Factor.SetEnabledValue(this.Option.MultipathMp2);

            this.checkBox_enableSatelliteTypes.Checked = Option.IsEnableSatelliteTypes;
            this.multiGnssSystemSelectControl1.SetSatelliteTypes(Option.SatelliteTypes);
        }

        public void UiToEntity()
        {
            if (Option == null) { Option = CreateNewModel(); }

            var spliter = new char[] { ',', ' ', '\n', '\t', '\r', ';' };

            Option.IsEnableObsCodes = this.checkBox_enableCode.Checked;
            Option.IsEnableExcludeSiteNames = this.checkBox_enableExcludeSiteNames.Checked;
            Option.IsEnableIncludeSiteNames = this.checkBox_enableincludeSiteNames.Checked;
            Option.IsEnableCenterRegion = this.checkBox_enableRegionFilter.Checked;

            Option.IsEnableTimePeriod = this.checkBox_enableTimePeriod.Checked;
            Option.TimePeriod = this.timePeriodControl1.TimePeriod;

            Option.CenterRegion.Center = XYZ.Parse(this.textBox_centerXyz.Text);
            Option.CenterRegion.Radius = Double.Parse(this.textBox_radiusFromCenter.Text);
            Option.ObsCodes = new List<string>(this.textBox_obsCodes.Text.Split(spliter, StringSplitOptions.RemoveEmptyEntries));
            Option.IncludeSiteNames = new List<string>(this.textBox_includeSiteNames.Text.Split(spliter, StringSplitOptions.RemoveEmptyEntries));
            Option.ExcludeSiteNames = new List<string>(this.textBox_excludeSites.Text.Split(spliter, StringSplitOptions.RemoveEmptyEntries));

            Option.IsNavCopy = checkBox_IsNavCopy.Checked;
            Option.MinEpochCount = int.Parse(this.textBox1MinEpochCount.Text);
            Option.IsEnableMinEpochCount = this.checkBox1MinEpochCount.Checked;

            Option.MinFileSizeMB = Double.Parse(this.textBox1MinFileSizeMB.Text);
            Option.IsEnableMinFileSizeMB = this.checkBox1MinFileSizeMB.Checked;

            Option.MinFrequencyCount = int.Parse(this.textBox1MinFrequencyCount.Text);
            Option.IsEnableMinFrequencyCount = this.checkBox1MinFrequencyCount.Checked;

            Option.MinSatCount = int.Parse(this.textBox1MinSatCount.Text);
            Option.MinRatioOfSatCount = double.Parse(this.textBox2MinSatCountRatio.Text);
            Option.IsEnableMinRatioOfSatCount = (this.checkBox1MinSatCount.Checked);

            this.Option.MultipathMp1 = enabledFloatControl_mp1Factor.GetEnabledValue();
            this.Option.MultipathMp2 = enabledFloatControl_mp2Factor.GetEnabledValue();

            Option.IsEnableSatelliteTypes = this.checkBox_enableSatelliteTypes.Checked;
            Option.SatelliteTypes = this.multiGnssSystemSelectControl1.SatelliteTypes;
        }

        private ObsFileSelectOption CreateNewModel()
        {
            return new ObsFileSelectOption();
        }

        private void button_reset_Click(object sender, EventArgs e)
        {
            this.EntityToUi();
        }

        private void TextBox1MinFileSizeMB_TextChanged(object sender, EventArgs e)
        {

        }

        private void Label10_Click(object sender, EventArgs e)
        {

        }
    }

}