using Gdp.Data.Rinex;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Gdp.Winform
{
    public partial class ChartForm : Form
    {
        public ChartForm()
        {
            InitializeComponent();
        }

        public void DrawVisibility(RinexObsFile ObsFile, float fontSize = 10)
        {
            this.Text = "Visibility of " +  ObsFile.Header.SiteName;
            chart1.Series.Clear();
            double interval = ObsFile.Header.Interval;
            var prns = ObsFile.GetPrns();
            int prnIndex = 1;

            foreach (var prn in prns)
            {
                List<DateTime> txData = new List<DateTime>();
                List<int> tyData = new List<int>();
                foreach (var epoch in ObsFile)
                {
                    if (epoch.Contains(prn))
                    {
                        txData.Add(epoch.ReceiverTime.DateTime);
                        tyData.Add(prnIndex);
                    }
                }

                var series =  chart1.Series.Add(prn.ToString());  

                var color =  Color.DarkBlue;
         
                switch (prn.SatelliteType)
                {
                    case SatelliteType.U:
                        break;
                    case SatelliteType.G:
                        color = Color.Blue;
                        break;
                    case SatelliteType.R:
                        color = Color.DimGray;
                        break;
                    case SatelliteType.S:
                        break;
                    case SatelliteType.E:
                        color = Color.DarkSeaGreen;
                        break;
                    case SatelliteType.C:
                        color = Color.OrangeRed;
                        break;
                    case SatelliteType.M:
                        break;
                    case SatelliteType.J:
                        break;
                    case SatelliteType.D:
                        break;
                    case SatelliteType.I:
                        break;
                    default:
                        break;
                }
                series.Color = color;
           //    series.IsValueShownAsLabel = true;
           series.IsVisibleInLegend = false;
                series.ChartType = SeriesChartType.Point;
                series.Points.DataBindXY(txData, tyData);
                series.BorderWidth = 1;

                prnIndex++;
            }

            ChartArea chartArea = chart1.ChartAreas[0];
            //    chartArea.AxisX.Minimum = 0;
            ////      chartArea.AxisX.Maximum = 24 * 60 * 60;
            chartArea.AxisY.Minimum = 0;
            chartArea.AxisY.Maximum = prns.Count + 1;

            //chartArea.AxisX.ScrollBar.IsPositionedInside = true; //滚动条
            //chartArea.AxisX.ScrollBar.Enabled = true;

            chartArea.CursorX.IsUserEnabled = true;//设置坐标轴可以用鼠标点击放大
            chartArea.CursorX.IsUserSelectionEnabled = true; //用户可以选择从那里放大
                                                             //chartArea.AxisX.Title = "Time/s";  //设置下方横坐标名称，当然AxisX2为上方的横坐标。

            //string Ytitle = null;
            //for (int i = prns.Count - 1; i >= 0; i--)
            //{
            //    Ytitle += prns[i].ToString() + "\r\n";
            //}
            //   chartArea.AxisY.Title = "PRN";// Ytitle; //Y轴的标题名字是对应的卫星编号
            //    chartArea.AxisY.TextOrientation = TextOrientation.Horizontal;
            chartArea.AxisX.IsLabelAutoFit = true;
         //   chartArea.AxisX.LabelStyle.Angle = 90;
            chartArea.AxisX.LabelStyle.Format = "HH:mm";
            chartArea.AxisX.LabelStyle.Font = new Font("Times New Roman", fontSize);
            //此处无用
            chartArea.AxisY.LabelStyle.Font = new Font("Times New Roman", fontSize);
            chartArea.AxisX.Minimum = ObsFile.Header.StartTime.DateTime.ToOADate();
            chartArea.AxisX.Maximum = ObsFile.Header.EndTime.DateTime.ToOADate();

            var totalHours = (int)(Math.Round(ObsFile.Header.TimePeriod.Span / 3600));//hour
            double xInterval =  4 / 24.0;// 0.0416666667;
            if (totalHours < 1)//10m
            {
                xInterval = 4 / 24.0/6;
            }if(totalHours < 10)//30m
            {
                xInterval = 4 / 24.0 / 2;
            } 
            chartArea.AxisX.Interval = xInterval;
            chartArea.AxisY.IsLabelAutoFit = true;
            chartArea.AxisY.Interval = 1;
            for (int i = 0; i < prns.Count; i++)
            {
                var p = prns[i];
                 
                chartArea.AxisY.CustomLabels.Add(i+0.5, i+1.5, p.ToString());//
            } 

            chartArea.CursorY.IsUserEnabled = true;//设置坐标轴可以用鼠标点击放大
            chartArea.CursorY.IsUserSelectionEnabled = true; //用户可以选择从那里放大
        }

        private void 调整字体大小FToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double fontSize = chart1.ChartAreas[0].AxisY.LabelStyle.Font.Size;
           if (   Gdp.Utils.FormUtil.ShowInputNumeralForm("font size",out fontSize, fontSize))
            {
                chart1.ChartAreas[0].AxisY.LabelStyle.Font = new Font(chart1.ChartAreas[0].AxisY.LabelStyle.Font.Name, (float)fontSize);
                chart1.ChartAreas[0].AxisY.LabelStyle.Font = new Font(chart1.ChartAreas[0].AxisX.LabelStyle.Font.Name, (float)fontSize);
            }
        }
    }
}
