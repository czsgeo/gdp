using Gdp.Data.Rinex;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gdp
{


    public class ObsFileAnalyzer
    {
        public static ObjectTableStorage GetSatVisiblityTable(RinexObsFile ObsFile)
        {
            var table = new ObjectTableStorage("Visiblity Of Sats of " + ObsFile.SiteInfo.SiteName);
            double interval = ObsFile.Header.Interval;
            var lastTimes = new BaseDictionary<SatelliteNumber, Time>();

            var prns = ObsFile.GetPrns();
            foreach (var prn in prns)
            {
                Time lastTime = null;
                Time firstTime = null;
                int periodIndex = 1;
                table.NewRow();
                table.AddItem("Prn", prn.ToString());
                foreach (var epoch in ObsFile)
                {
                    if (epoch.Contains(prn))
                    {
                        if (firstTime == null)
                        {
                            lastTime = epoch.ReceiverTime;
                            firstTime = epoch.ReceiverTime;
                        }

                        if (epoch.ReceiverTime - lastTime > interval + 0.001)
                        {
                            TimePeriod timePeriod = new TimePeriod(firstTime, lastTime);
                            table.AddItem("Period" + (periodIndex++), timePeriod.ToString());

                            firstTime = null;
                        }

                        lastTime = epoch.ReceiverTime;
                    }
                }
                if (firstTime != null)
                {
                    TimePeriod timePeriod = new TimePeriod(firstTime, lastTime);
                    table.AddItem("Period" + (periodIndex++), timePeriod.ToString());
                    firstTime = null;
                }

            }

            return table;
        }

        static public ObjectTableStorage GetMwCycleTable(RinexObsFile ObsFile, FileEphemerisService FileEphemerisService)
        {
            var table = new ObjectTableStorage("MW values of " + ObsFile.SiteInfo.SiteName);
            foreach (var epoch in ObsFile)
            {
                table.NewRow();
                table.AddItem("Epoch", epoch.ReceiverTime);
                foreach (var sat in epoch)
                {
                    table.AddItem(sat.Prn + "_Mw", sat.MwCycle);
                    if (FileEphemerisService != null)
                    {
                        var eph = FileEphemerisService.Get(sat.Prn, epoch.ReceiverTime);
                        if (eph != null)
                        {
                            var polar = CoordTransformer.XyzToGeoPolar(eph.XYZ, ObsFile.Header.ApproxXyz);
                            table.AddItem(sat.Prn + "_Ele", polar.Elevation);
                        }
                    }
                }
            }
            return table;
        }
        static public ObjectTableStorage GetLiTable(RinexObsFile ObsFile, FileEphemerisService FileEphemerisService)
        {
            var table = new ObjectTableStorage("LI values of " + ObsFile.SiteInfo.SiteName);
            foreach (var epoch in ObsFile)
            {
                table.NewRow();
                table.AddItem("Epoch", epoch.ReceiverTime);
                foreach (var sat in epoch)
                {
                    table.AddItem(sat.Prn + "_Li", sat.GfValue);
                    if (FileEphemerisService != null)
                    {
                        var eph = FileEphemerisService.Get(sat.Prn, epoch.ReceiverTime);
                        if (eph != null)
                        {
                            var polar = CoordTransformer.XyzToGeoPolar(eph.XYZ, ObsFile.Header.ApproxXyz);
                            table.AddItem(sat.Prn + "_Ele", polar.Elevation);
                        }
                    }
                }
            }
            return table;
        }
        static public ObjectTableStorage GetRangeErrorTable(RinexObsFile ObsFile, double k1, double k2, FileEphemerisService FileEphemerisService)
        {
            var table = new ObjectTableStorage("Range Error of " + ObsFile.SiteInfo.SiteName);
            double interval = ObsFile.Header.Interval;
            var lastTimes = new BaseDictionary<SatelliteNumber, Time>();

            var prns = ObsFile.GetPrns();
            foreach (var prn in prns)
            {
                int index = 1;
                foreach (var epoch in ObsFile)
                {
                    if (epoch.Contains(prn))
                    {
                        var sat = epoch[prn];
                        var isError = sat.IsRangeGrossError(k1, k2);
                        if (isError)
                        {
                            if (index == 1)
                            {
                                table.NewRow();
                                table.AddItem("Prn", prn.ToString());
                            }
                            table.AddItem("Error" + (index++), epoch.ReceiverTime); 
                        }
                    }
                }
            }
            return table;
        }

        static public ObjectTableStorage GetMwCycleSlipTable(RinexObsFile ObsFile, double maxDiffer = 1)
        {
            var table = new ObjectTableStorage("Mw Cycle Slips Of Sats of " + ObsFile.SiteInfo.SiteName);
            double interval = ObsFile.Header.Interval;
            var lastTimes = new BaseDictionary<SatelliteNumber, Time>();

            var prns = ObsFile.GetPrns();
            foreach (var prn in prns)
            {
                int index = 1;
                double lastVal = 0;
                foreach (var epoch in ObsFile)
                {
                    if (epoch.Contains(prn))
                    {
                        var sat = epoch[prn];
                        var MwValue = sat.MwCycle;
                        if (lastVal == 0)
                        {
                            lastVal = MwValue;
                            continue;
                        }

                        var differ = Math.Abs(MwValue - lastVal);
                        if (differ > maxDiffer)
                        {
                            if (index == 1)
                            {
                                table.NewRow();
                                table.AddItem("Prn", prn.ToString());
                            }
                            //record and reset
                            table.AddItem("Cycle" + (index++), epoch.ReceiverTime);
                        }

                        lastVal = MwValue;
                    }
                }
            }
            return table;
        } 
        
        

        static public ObjectTableStorage GetLiCycleSlipTable(RinexObsFile ObsFile, double maxDiffer = 1)
        {
            var table = new ObjectTableStorage("Li Cycle Slips Of Sats of " + ObsFile.SiteInfo.SiteName);
            double interval = ObsFile.Header.Interval;
            var lastTimes = new BaseDictionary<SatelliteNumber, Time>();

            var prns = ObsFile.GetPrns();
            foreach (var prn in prns)
            {
                int index = 1;
                double lastVal = 0;
                foreach (var epoch in ObsFile)
                {
                    if (epoch.Contains(prn))
                    {
                        var sat = epoch[prn];

                        if (lastVal == 0)
                        {
                            lastVal = sat.GfValue;
                            continue;
                        }

                        var differ = Math.Abs(sat.GfValue - lastVal);
                        if (differ > maxDiffer)
                        {
                            if (index == 1)
                            {
                                table.NewRow();
                                table.AddItem("Prn", prn.ToString());
                            }
                            //record and reset
                            table.AddItem("Cycle" + (index++), epoch.ReceiverTime);
                        }

                        lastVal = sat.GfValue;
                    }
                }

            }

            return table;
        }
    }

}
