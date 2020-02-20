//2016.10.28, czs, create  in hongqing, 观测文件选择选项。
//2016.10.29, czs, edit  in hongqing, 观测文件格式化选项。

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
    public partial class ObsFileConvertOptionForm : Form
    {
        public ObsFileConvertOptionForm()
        {
            InitializeComponent();
            enumRadioControl_nameType.Init<RinexNameType>();
            enumCheckBoxControlObsTypes.Init<ObsTypes>();
        }
        public ObsFileConvertOptionForm(ObsFileConvertOption option)
        {
            InitializeComponent();
            enumRadioControl_nameType.Init<RinexNameType>();
            enumCheckBoxControlObsTypes.Init<ObsTypes>();
            this.SetOption(option);


        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            UiToEntity();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void SetOption(ObsFileConvertOption Option)
        {
            this.Option = Option;
            EntityToUi();
        }

        /// <summary>
        /// 选项
        /// </summary>
        public ObsFileConvertOption Option { get; set; }


        private void EntityToUi()
        {
            if (Option == null) { Option = CreateNewModel(); } 
            this.fileOpenControl1IndicatedEphemeris.FilePath = Option.IndicatedEphemeris.Value;
            this.checkBox1IndicatedEphemeris.Checked = Option.IndicatedEphemeris.Enabled;
            checkBox_isConsice.Checked = Option.IsConciseOutput;
            this.checkBox_IsRoundFractionSecondToInt.Checked = Option.IsRoundFractionSecondToInt;
            this.checkBox_IsUpperSiteName.Checked = Option.IsUpperSiteName;
            this.checkBox_IsUpperFileName.Checked = Option.IsUpperFileName;

            checkBox1IsConvertPhaseToLength.Checked = Option.IsConvertPhaseToLength;
            checkBox1IsRemoveRedundantObsForIonoFree.Checked = Option.IsRemoveRedundantObsForIonoFree;
            checkBox1IsRemoveIonoFreeUnavaliable.Checked = Option.IsRemoveIonoFreeUnavaliable;
            this.checkBox_IsUseFileNameAsSiteName.Checked = Option.IsUseFileNameAsSiteName;

            this.enabledIntControl_SiteNameLength.SetEnabledValue(Option.SiteNameLength);
            var prnStr = SatelliteNumberUtils.GetString(Option.SatsToBeRemoved);
            enabledStringControl_RemoveSats.SetEnabledValue(new EnableString() { Enabled = Option.IsEnableRemoveSats, Value = prnStr });
            this.checkBox_enableCode.Checked = Option.IsEnableObsCodes;
            this.checkBox_interval.Checked = Option.IsEnableInterval;
            this.checkBox_enableTimePeriod.Checked = Option.IsEnableTimePeriod;
            this.timePeriodControl1.SetTimePerid(Option.TimePeriod);
            this.comboBox_version.Text = Option.Version.ToString();
            this.textBox_interval.Text = Option.Interval.ToString();
            this.checkBoxMinObsCodeAppearRatio.Checked = Option.IsEnableMinObsCodeAppearRatio;
            this.textBoxMinObsCodeAppearRatio.Text = Option.MinObsCodeAppearRatio.ToString();

            this.checkBoxSatelliteTypes.Checked = Option.IsEnableSatelliteTypes;
            this.multiGnssSystemSelectControl1.SetSatelliteTypes(Option.SatelliteTypes);

            textBoxNotVacantCodes.Text = Option.NotVacantCodes;
            checkBox_deleVacantSat.Checked = Option.IsDeleteVacantSat;
            Option.IsEnableObsTypes = checkBox_enableCode.Checked;
            enumCheckBoxControlObsTypes.Select<ObsTypes>(Option.ObsTypes);

            enabledFloatControl1Section.SetEnabledValue(   Option.EnabledSection);            
            checkBoxIsRemoveZeroRangeSat.Checked = Option.IsRemoveZeroRangeSat;
            this.checkBoxIsRemoveZeroPhaseSat.Checked = Option.IsRemoveZeroPhaseSat;

            this.checkBoxIsEnableAlignPhase.Checked = Option.IsEnableAlignPhase;
            this.checkBox1IsAmendBigCs.Checked = Option.IsAmendBigCs;
           
            this.checkBox1IsReomveOtherCodes.Checked = Option.IsReomveOtherCodes;
            this.textBox1OnlyCodes.Text = Option.OnlyCodesString;
            this.enabledIntControl_removeEpochCount.SetEnabledValue(new EnableInteger() { Enabled = Option.IsEnableMinEpochCount, Value = Option.MinEpochCount });
            this.namedIntControl1MaxBreakCount.SetValue(this.Option.MaxBreakCount);
            this.checkBox_IsEnableRinexVertion.Checked = Option.IsEnableRinexVertion;
            this.enumRadioControl_nameType.SetCurrent<RinexNameType>(Option.RinexNameType);
            enabledFloatControl_SatCutOffAngle.SetEnabledValue(this.Option.SatCutOffAngle);


            this.checkBox_StrOfFrequenceNumToBeRemoved.Checked = Option.IsEnableRemoveIndicatedFrequence;
            namedStringControl_StrOfFrequenceNumToBeRemoved.SetValue(Option.StrOfFrequenceNumToBeRemoved);

             checkBox_XToP2Enabled.Checked  = Option.IsUseXCodeAsPLWhenEmpty;
        }

        public void UiToEntity()
        {
            if (Option == null) { Option = CreateNewModel(); }

            Option.IsRoundFractionSecondToInt = this.checkBox_IsRoundFractionSecondToInt.Checked;
            Option.IsUpperFileName = this.checkBox_IsUpperFileName.Checked;
            Option.IsUpperSiteName = this.checkBox_IsUpperSiteName.Checked;
            Option.SiteNameLength = this.enabledIntControl_SiteNameLength.GetEnabledValue();

            Option.IsConvertPhaseToLength = checkBox1IsConvertPhaseToLength.Checked;
            Option.IndicatedEphemeris = new EnableString(this.fileOpenControl1IndicatedEphemeris.FilePath, this.checkBox1IndicatedEphemeris.Checked);

            Option.IsRemoveRedundantObsForIonoFree = checkBox1IsRemoveRedundantObsForIonoFree.Checked;
            Option.IsRemoveIonoFreeUnavaliable = checkBox1IsRemoveIonoFreeUnavaliable.Checked;
            Option.IsUseFileNameAsSiteName = this.checkBox_IsUseFileNameAsSiteName.Checked;
            Option.IsConciseOutput = checkBox_isConsice.Checked;

            Option.IsEnableRinexVertion =   this.checkBox_IsEnableRinexVertion.Checked;
            var val = enabledStringControl_RemoveSats.GetEnabledValue();
            Option.IsEnableRemoveSats = val.Enabled;
            Option.SatsToBeRemoved = SatelliteNumberUtils.ParseString(val.Value);  

            Option.IsEnableObsCodes = this.checkBox_enableCode.Checked;
            Option.IsEnableInterval = this.checkBox_interval.Checked;

            Option.IsEnableTimePeriod = this.checkBox_enableTimePeriod.Checked;
            Option.TimePeriod = this.timePeriodControl1.TimePeriod;
            Option.Version = double.Parse(this.comboBox_version.Text);
            Option.Interval = double.Parse(this.textBox_interval.Text);

            Option.IsEnableMinObsCodeAppearRatio = this.checkBoxMinObsCodeAppearRatio.Checked;
            Option.MinObsCodeAppearRatio = double.Parse(this.textBoxMinObsCodeAppearRatio.Text);

            Option.IsEnableSatelliteTypes = this.checkBoxSatelliteTypes.Checked;
            Option.SatelliteTypes =  this.multiGnssSystemSelectControl1.SatelliteTypes;

            Option.IsEnableObsTypes = checkBox_enableCode.Checked;
            Option.ObsTypes = enumCheckBoxControlObsTypes.GetSelected<ObsTypes>();

            Option.IsRemoveZeroRangeSat = checkBoxIsRemoveZeroRangeSat.Checked;
            Option.IsRemoveZeroPhaseSat = this.checkBoxIsRemoveZeroPhaseSat.Checked;

            Option.NotVacantCodes = textBoxNotVacantCodes.Text.Trim();
            Option.IsDeleteVacantSat = checkBox_deleVacantSat.Checked;
            Option.EnabledSection = enabledFloatControl1Section.GetEnabledValue();
            
            Option.IsReomveOtherCodes = this.checkBox1IsReomveOtherCodes.Checked;
            Option.OnlyCodesString = this.textBox1OnlyCodes.Text;
            Option.IsEnableAlignPhase = this.checkBoxIsEnableAlignPhase.Checked;
            Option.IsAmendBigCs = this.checkBox1IsAmendBigCs.Checked;

            Option.IsEnableMinEpochCount = this.enabledIntControl_removeEpochCount.GetEnabledValue().Enabled;
            Option.MinEpochCount = this.enabledIntControl_removeEpochCount.GetValue();
            this.Option.MaxBreakCount =  this.namedIntControl1MaxBreakCount.GetValue();
            this.Option.SatCutOffAngle = enabledFloatControl_SatCutOffAngle.GetEnabledValue();
            Option.RinexNameType = this.enumRadioControl_nameType.GetCurrent<RinexNameType>();

            Option.IsEnableRemoveIndicatedFrequence = this.checkBox_StrOfFrequenceNumToBeRemoved.Checked;
            Option.StrOfFrequenceNumToBeRemoved = this.namedStringControl_StrOfFrequenceNumToBeRemoved.GetValue();
            Option.IsUseXCodeAsPLWhenEmpty = checkBox_XToP2Enabled.Checked;
        }

        private ObsFileConvertOption CreateNewModel()
        {
            return new ObsFileConvertOption();
        }

        private void button_reset_Click(object sender, EventArgs e)
        {
            this.EntityToUi();
        }

        private void ObsFileConvertOptionForm_Load(object sender, EventArgs e)
        {
            fileOpenControl1IndicatedEphemeris.Filter = Setting.RinexEphFileFilter;
        }

        private void NamedIntControl1MaxBreakCount_Load(object sender, EventArgs e)
        {

        }
    }


}
