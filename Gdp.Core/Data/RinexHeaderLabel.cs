
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace Gdp.Data.Rinex
{
    /// <summary>
    /// Rinex 头文件标签。
    /// </summary>
    public class RinexHeaderLabel
    {
        public RinexHeaderLabel()
        {
        }

        #region iono
        public const string IONEX_VERSION_TYPE = "IONEX VERSION / TYPE";
        public const string DESCRIPTION = "DESCRIPTION";
        public const string EPOCH_OF_FIRST_MAP = "EPOCH OF FIRST MAP";
        public const string EPOCH_OF_LAST_MAP = "EPOCH OF LAST MAP";
        public const string OF_MAPS_IN_FILE = "# OF MAPS IN FILE";
        public const string MAPPING_FUNCTION = "MAPPING FUNCTION";
        public const string ELEVATION_CUTOFF = "ELEVATION CUTOFF";
        public const string OBSERVABLES_USED = "OBSERVABLES_USED";
        public const string OF_STATIONS = "# OF STATIONS";
        public const string OF_SATELLITES = "# OF SATELLITES";
        public const string BASE_RADIUS = "BASE RADIUS";
        public const string MAP_DIMENSION = "MAP DIMENSION";
        public const string HGT1_HGT2_DHGT = "HGT1 / HGT2 / DHGT";
        public const string LAT1_LAT2_DLAT = "LAT1 / LAT2 / DLAT";
        public const string LON1_LON2_DLON = "LON1 / LON2 / DLON";
        public const string EXPONENT = "EXPONENT";
        public const string START_OF_AUX_DATA = "START OF AUX DATA";
        public const string PRN_BIAS_RMS = "PRN / BIAS / RMS";
        public const string STATION_BIAS_RMS = "STATION / BIAS / RMS";
        public const string END_OF_AUX_DATA = "END OF AUX DATA";
        public const string END_OF_TEC_MAP = "END OF TEC MAP";
        public const string START_OF_TEC_MAP = "START OF TEC MAP";
        public const string EPOCH_OF_CURRENT_MAP = "EPOCH OF CURRENT MAP";
        public const string LAT_LON1_LON2_DLON_H = "LAT/LON1/LON2/DLON/H";
        public const string END_OF_RMS_MAP = "END OF RMS MAP";
        public const string END_OF_FILE = "END OF FILE";
        public const string START_OF_RMS_MAP = "START OF RMS MAP";
        #endregion

        // GNSS OBSERVATION DATA FILE - HEADER SECTION DESCRIPTION  
        /// <summary>
        /// The "RINEX VERSION / TYPE" clk must be the prevObj clk in coefficient fileB
        /// </summary>
        public const string RINEX_VERSION_TYPE = "RINEX VERSION / TYPE";
        public const string PGM_RUN_BY_DATE = "PGM / RUN BY / DATE";
        public const string COMMENT = "COMMENT";
        //Name of antenna marker 
        public const string MARKER_NAME = "MARKER NAME";//MARKER NAME
        //Number of antenna marker 
        public const string MARKER_NUMBER = "MARKER NUMBER";
        public const string OBSERVER_AGENCY = "OBSERVER / AGENCY";
        public const string REC_NUM_TYPE_VERS = "REC # / TYPE / VERS";
        //Antenna number and type  
        public const string ANT_NUM_TYPE = "ANT # / TYPE";
        //APPROX POSITION XyzCos
        public const string APPROX_POSITION_XYZ = "APPROX POSITION XYZ";
        public const string ANTENNA_DELTA_H_E_N = "ANTENNA: DELTA H/E/N";
        public const string ANTENNA_DELTA_XYZ = "ANTENNA: DELTA X/Y/Z";
        public const string WAVELENGTH_FACT_L1_2 = "WAVELENGTH FACT L1/2";
        public const string TYPES_OF_OBSERV = "# / TYPES OF OBSERV";
        public const string INTERVAL = "INTERVAL";
        public const string TIME_OF_FIRST_OBS = "TIME OF FIRST OBS";
        public const string TIME_OF_LAST_OBS = "TIME OF LAST OBS";
        public const string RCV_CLOCK_OFFS_APPL = "RCV CLOCK OFFS APPL";
        public const string LEAP_SECONDS = "LEAP SECONDS";
        public const string NUM_OF_SATELLITES = "# OF SATELLITES";
        public const string PRN_NUM_OF_OBS = "PRN / # OF OBS";
        public const string END_OF_HEADER = "END OF HEADER";

        public const string SYS_PHASE_SHIFT = "SYS / PHASE SHIFT";

        /// <summary>
        /// GLONASS SLOT / FRQ # RINEX 3.03
        /// </summary>
        public const string GLONASS_SLOT_FRQ = "GLONASS SLOT / FRQ #";
        /// <summary>
        /// GLONASS COD/PHS/BIS  RINEX 3.03
        /// </summary>
        public const string GLONASS_COD_PHS_BIS = "GLONASS COD/PHS/BIS";

        //导航文件
        public const string ION_ALPHA = "ION ALPHA";
        public const string ION_BETA = "ION BETA";
        public const string DELTA_UTC_A0_A1_T_W = "DELTA-UTC: A0,A1,T,W";
        public const string PRN_LIST = "PRN LIST";
        //GNSS OBSERVATION DATA FILE - DATA RECORD DESCRIPTION 

        //-----------------rinex 3.0 obs file
        /// <summary>
        /// Rinex 3.0
        /// </summary>
        public const string SYS_OBS_TYPES = "SYS / # / OBS TYPES";
        /// <summary>
        /// rinex 3.0
        /// </summary>
        public const string MARKER_TYPE = "MARKER TYPE";

        //----------------rinex 3.0 nav file
        /// <summary>
        /// Ionospheric correction parameters
        /// - Correction type
        /// GAL = Galileo ai0 – ai2
        /// GPSA = GPS alpha0 - alpha3
        /// GPSB = GPS beta0 - beta3
        /// - Parameters
        /// GPS: alpha0-alpha3 or beta0-beta3 |
        /// GAL: ai0, ai1, ai2, zero
        /// </summary>
        public const string IONOSPHERIC_CORR = "IONOSPHERIC CORR";
        /// <summary>
        /// Corrections to transform the system time | |*
        ///to UTC or other time systems | |
        ///- Correction type | A4,1X, |
        ///GAUT = GAL to UTC a0, a1 | |
        ///GPUT = GPS to UTC a0, a1 | |
        ///SBUT = SBAS to UTC a0, a1 | |
        ///GLUT = GLO to UTC a0=TauC, a1=zero | |
        ///GPGA = GPS to GAL a0=A0G, a1=A1G | |
        ///GLGP = GLO to GPS a0=TauGPS, a1=zero | |
        ///- a0,a1 Coefficients of 1-deg polynomial | D17.10, |
        ///(a0 sec, a1 sec/sec) | D16.9, |
        ///CORR(s) = a0 + a1*DELTAT | |
        ///- TProduct Reference time for polynomial | I7, |
        ///(Fraction into GPS/GAL week) | |
        ///- W Reference week number | I5, |
        ///(GPS/GAL week, continuous number) | |
        ///TProduct and W zero for GLONASS. | |
        ///- S EGNOS, WAAS, or MSAS ... | 1X,A5,1X |
        ///(left-justified) | |
        ///Derived from MT17 service provider. | |
        ///If not known: Use Snn with | |
        ///nn = PRN-100 of satellite | |
        ///broadcasting the MT12 | |
        ///- U UTC Identifier (0 if unknown) | I2,1X |
        ///1=UTC(NIST), 2=UTC(USNO), 3=UTC(SU), | |
        ///4=UTC(BIPM), 5=UTC(Europe Lab), | |
        ///6=UTC(CRL), >6 = not assigned yet | |
        ///S and U for SBAS only. |
        /// </summary>
        public const string TIME_SYSTEM_CORR = "TIME SYSTEM CORR";
        /// <summary>
        /// 是否包含头部标签
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        static public bool ContainLabel(string line)
        {
            var labels = HeaderLabels();
            foreach (var item in labels)
            {
                if (line.Contains(item))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 头部标签
        /// </summary>
        /// <returns></returns>
        public static List<string> HeaderLabels()
        {
            List<String> labels = new List<string>()
            {
                RINEX_VERSION_TYPE,
               PGM_RUN_BY_DATE ,
               COMMENT,         
          
                //Name of antenna marker 
               MARKER_NAME ,
          
                //Number of antenna marker 
               MARKER_NUMBER ,

               OBSERVER_AGENCY,

               REC_NUM_TYPE_VERS,
          
                //Antenna number and type  
               ANT_NUM_TYPE,
                //APPROX POSITION XyzCos
               APPROX_POSITION_XYZ,
               ANTENNA_DELTA_H_E_N ,
               ANTENNA_DELTA_XYZ ,
               WAVELENGTH_FACT_L1_2 ,
               TYPES_OF_OBSERV ,
               INTERVAL,
               TIME_OF_FIRST_OBS,
               TIME_OF_LAST_OBS ,
               RCV_CLOCK_OFFS_APPL ,
               LEAP_SECONDS ,
               NUM_OF_SATELLITES ,
               PRN_NUM_OF_OBS,
               END_OF_HEADER ,
               ION_ALPHA ,
               ION_BETA ,
               DELTA_UTC_A0_A1_T_W,
               PRN_LIST,
               SYS_OBS_TYPES ,
               MARKER_TYPE,IONOSPHERIC_CORR ,TIME_SYSTEM_CORR ,
            };
            return labels;
        }
    }
}