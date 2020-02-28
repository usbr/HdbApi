using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Data;

namespace HdbApi.App_Code
{
    internal interface IHydrometCommands
    {
        DataTable[] get_hdyromet_data(string region, string tstep, string cbttPcode, System.DateTime t1, System.DateTime t2);
    }

    public class HydrometCommands : IHydrometCommands
    {
        //https://www.usbr.gov/pn-bin/daily.pl?list=sco%20fb&start=2020-02-23&end=2020-02-26&format=html&flags=false&description=true
        private static string pnDailyURL = "https://www.usbr.gov/pn-bin/daily.pl?list=$CBTTPCODE$&start=$T1$&end=$T2$&format=html&flags=false&description=true";
        //https://www.usbr.gov/pn-bin/instant.pl?list=sco%20fb&start=2020-02-23&end=2020-02-26&format=html&flags=false&description=true
        private static string pnInstantURL = "https://www.usbr.gov/pn-bin/instant.pl?list=$CBTTPCODE$&start=$T1$&end=$T2$&format=html&flags=false&description=true";
        private static string gpDailyURL = "";
        private static string gpInstantURL = "";

        public DataTable[] get_hdyromet_data(string region, string tstep, string cbttPcode, DateTime t1, DateTime t2)
        {
            string baseUrl;
            if (tstep == "INSTANT")
            {
                baseUrl = pnInstantURL;
            }
            else
            {
                baseUrl = pnDailyURL;
            }

            DataTable dataTable = new DataTable();
            DataTable infoTable = new DataTable();
            /*
             SELECT
                HDB_SITE_DATATYPE.SITE_DATATYPE_ID,
                HDB_SITE.SITE_NAME, 
                HDB_DATATYPE.DATATYPE_NAME, 
                HDB_UNIT.UNIT_COMMON_NAME, 
                HDB_SITE.LAT, 
                HDB_SITE.LONGI, 
                HDB_SITE.ELEVATION,
                HDB_SITE.DB_SITE_CODE 
             FROM 
                HDB_SITE 
                INNER JOIN HDB_SITE_DATATYPE ON HDB_SITE.SITE_ID=HDB_SITE_DATATYPE.SITE_ID 
                INNER JOIN HDB_DATATYPE ON HDB_SITE_DATATYPE.DATATYPE_ID=HDB_DATATYPE.DATATYPE_ID 
                INNER JOIN HDB_UNIT ON HDB_DATATYPE.UNIT_ID=HDB_UNIT.UNIT_ID 
             WHERE 
                HDB_SITE_DATATYPE.SITE_DATATYPE_ID IN (x,x,x,);

            sco fb = Scoggins Dam & Henry Hagg Lake nr Forest Grove, OR Reservoir Water Surface Elevation, feet
            */
            return new DataTable[] { dataTable, infoTable };
        }

    }
}