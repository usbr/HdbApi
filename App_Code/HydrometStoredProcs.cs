using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Globalization;
using System.Net;
using System.Linq;
using HdbApi.Models;

namespace HdbApi.App_Code
{
    internal interface IHydrometCommands
    {
        DataTable[] get_hdyromet_data(string region, string tstep, string cbttPcode, System.DateTime t1, System.DateTime t2);
    }
    
    public class HydrometCommands : IHydrometCommands
    {
        //https://www.usbr.gov/pn-bin/daily.pl?list=sco%20fb&start=2020-02-23&end=2020-02-26&format=csv&flags=false&description=false
        private static string pnDailyURL = "https://www.usbr.gov/pn-bin/daily.pl?list=$CBTTPCODE$&start=$T1$&end=$T2$&format=csv&flags=false&description=false";
        //https://www.usbr.gov/pn-bin/instant.pl?list=sco%20fb&start=2020-02-23&end=2020-02-26&format=csv&flags=false&description=false
        private static string pnInstantURL = "https://www.usbr.gov/pn-bin/instant.pl?list=$CBTTPCODE$&start=$T1$&end=$T2$&format=csv&flags=false&description=false";
        //https://www.usbr.gov/gp-bin/webarccsv.pl?parameter=PUER%20FB&syer=2018&smnth=1&sdy=1&eyer=2018&emnth=1&edy=24&format=4
        private static string gpDailyURL = "https://www.usbr.gov/gp-bin/webarccsv.pl?parameter=$CBTTPCODE$&syer=$Y1$&smnth=$M1$&sdy=$D1$&eyer=$Y2$&emnth=$M2$&edy=$D2$&format=4";
        //https://www.usbr.gov/gp-bin/webdaycsv.pl?parameter=PUER%20FB&syer=2018&smnth=1&sdy=1&eyer=2018&emnth=1&edy=24&format=4
        private static string gpInstantURL = "https://www.usbr.gov/gp-bin/webdaycsv.pl?parameter=$CBTTPCODE$&syer=$Y1$&smnth=$M1$&sdy=$D1$&eyer=$Y2$&emnth=$M2$&edy=$D2$&format=4";
        private static DataTable pcodeTable = GetDataTableFromCsv(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory.ToString(), "hydrometParameterCatalog.csv"), true);
        private static DataTable siteTable = GetDataTableFromCsv(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory.ToString(), "hydrometSiteCatalog.csv"), true);
        private static DataTable seriesTable = GetDataTableFromCsv(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory.ToString(), "hydrometSeriesCatalog.csv"), true);
        private static string instantURL = "";
        private static string dailyURL = "";


        private static DataTable GetDataTableFromCsv(string path, bool isFirstRowHeader)
        {
            string header = isFirstRowHeader ? "Yes" : "No";

            string pathOnly = Path.GetDirectoryName(path);
            string fileName = Path.GetFileName(path);

            string sql = @"SELECT * FROM [" + fileName + "]";

            using (OleDbConnection connection = new OleDbConnection(
                      @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pathOnly +
                      ";Mode=Read;Extended Properties=\"Text;HDR=" + header + "\""))
            using (OleDbCommand command = new OleDbCommand(sql, connection))
            using (OleDbDataAdapter adapter = new OleDbDataAdapter(command))
            {
                DataTable dataTable = new DataTable();
                dataTable.Locale = CultureInfo.CurrentCulture;
                adapter.Fill(dataTable);
                return dataTable;
            }
        }


        private static string GetHydrometDB(string site)
        {
            DataRow[] siteRow = siteTable.Select("siteid = '" + site.ToLower() + "'");
            string regionCode = siteRow[0]["agency_region"].ToString();
            string dbString;
            if (regionCode.ToLower() == "pn")
            {
                dbString = "pnhyd";
                dailyURL = pnDailyURL;
                instantURL = pnInstantURL;
            }
            else if (regionCode.ToLower() == "gp")
            {
                dbString = "gphyd";
                dailyURL = gpDailyURL;
                instantURL = gpInstantURL;
            }
            else
            {
                throw new Exception("Hydromet regional DB not defined... ");
            }
            return dbString;
        }


        private static List<string[]> GetSitePcodeList(string[] sitePcodeArray, bool parseBothCodes = true)
        {
            var sitePcodeList = new List<string[]>();
            if (sitePcodeArray == null)
            {                
                return null;
            }
            foreach (var item in sitePcodeArray)
            {
                var sitePcodeArrayItems = item.Trim().Split(' ');
                if (parseBothCodes)
                {
                    if (sitePcodeArrayItems.Length != 2)
                    {
                        throw new Exception("Invalid site-pcode combination '" + item.ToString().Trim().ToLower() + "'");
                    }
                    string[] items = new string[] { sitePcodeArrayItems[0].ToString().ToLower(), sitePcodeArrayItems[1].ToString().ToLower() };
                    sitePcodeList.Add(items);
                }
                else
                {
                    sitePcodeList.Add(new string[] { sitePcodeArrayItems[0].ToString().ToLower() });
                }
            }
            return sitePcodeList;
        }


        private static DataTable GetInfoTable(string[] sitePcodeArray)
        {
            var sitePcodeList = GetSitePcodeList(sitePcodeArray);

            // infoTable Columns { SDI, SITENAME, DATATYPENAME, UNITNAME, LAT, LON, ELEV, DBSITECODE}            
            DataTable infoTable = new DataTable();
            infoTable.Columns.Add("cbtt", typeof(string));
            infoTable.Columns.Add("sitename", typeof(string));
            infoTable.Columns.Add("datatype", typeof(string));
            infoTable.Columns.Add("units", typeof(string));
            infoTable.Columns.Add("lat", typeof(string));
            infoTable.Columns.Add("lon", typeof(string));
            infoTable.Columns.Add("elev", typeof(string));
            infoTable.Columns.Add("dbsitecode", typeof(string));

            foreach (string[] sitePcode in sitePcodeList)
            {
                DataRow[] siteRow = siteTable.Select("siteid = '" + sitePcode[0].ToString().ToLower() + "'");
                DataRow[] pcodeRow = pcodeTable.Select("pcode = '" + sitePcode[1].ToString().ToLower() + "'");
                if (siteRow.Length < 1 || pcodeRow.Length < 1)
                {
                    throw new Exception("site/pcode not found '" + sitePcode[0].ToString().ToLower() + " " + sitePcode[1].ToString().ToLower() + "'");
                }
                DataRow infoRow = infoTable.NewRow();
                infoRow["cbtt"] = "" + siteRow[0]["siteid"].ToString().ToUpper() + "_" + pcodeRow[0]["pcode"].ToString().ToUpper();
                infoRow["sitename"] = siteRow[0]["description"].ToString() + " -- " + siteRow[0]["state"].ToString();
                infoRow["datatype"] = pcodeRow[0]["name"].ToString();
                infoRow["units"] = pcodeRow[0]["units"].ToString();
                infoRow["lat"] = siteRow[0]["latitude"].ToString();
                infoRow["lon"] = siteRow[0]["longitude"].ToString();
                infoRow["elev"] = siteRow[0]["elevation"].ToString();
                infoRow["dbsitecode"] = siteRow[0]["agency_region"].ToString().ToLower() + "hyd";

                infoTable.Rows.Add(infoRow);
            }

            return infoTable;
        }


        public static DataTable GetTsDataTable(string tStep, string cbttPcode, DateTime t1, DateTime t2)
        {
            DataTable dataTable = new DataTable();

            // Check that users are querying TS data from 1 specific Hydromet DB
            List<string[]> sitePcodeList = GetSitePcodeList(cbttPcode.Split(',').ToArray(), true);
            string dbString = "";
            foreach (string[] sitePcode in sitePcodeList)
            {
                string ithDbString = GetHydrometDB(sitePcode[0].ToLower());
                if (dbString == "")
                {
                    dbString = ithDbString; 
                }
                else
                {
                    if (dbString != ithDbString)
                    {
                        throw new Exception("Querying data from 2 different Hydromet DBs is not supported. Perform separate queries for each Hydromet DB... ");
                    }
                }
            }

            // Check which timestep to query
            string baseUrl;
            if (tStep.ToLower() == "instant")
            {
                baseUrl = instantURL;
            }
            else
            {
                baseUrl = dailyURL;
            }

            // Account for the different Hydromet data service date formats
            string url = baseUrl.Replace("$CBTTPCODE$", cbttPcode);
            if (dbString == "pnhyd")
            {
                //&start=$T1$&end=$T2$&
                url = url.Replace("$T1$", t1.ToString("yyyy-MM-dd"));
                url = url.Replace("$T2$", t2.ToString("yyyy-MM-dd"));
            }
            if (dbString == "gphyd")
            {
                //&syer=$Y1$&smnth=$M1$&sdy=$D1$&eyer=$Y2$&emnth=$M2$&edy=$D2$&
                url = url.Replace("$Y1$", t1.Year.ToString("F0"));
                url = url.Replace("$Y2$", t2.Year.ToString("F0"));
                url = url.Replace("$M1$", t1.Month.ToString("F0"));
                url = url.Replace("$M2$", t2.Month.ToString("F0"));
                url = url.Replace("$D1$", t1.Day.ToString("F0"));
                url = url.Replace("$D2$", t2.Day.ToString("F0"));
            }

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            StreamReader sr = new StreamReader(resp.GetResponseStream());
            string urlData = sr.ReadToEnd();
            sr.Close();

            urlData = urlData.Replace("NO RECORD", ",NaN");
            string[] tableData = urlData.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            // define datatable columns
            dataTable.Columns.Add("HDB_DATETIME", typeof(string));
            // catch special case for gp-instant data service...
            int headerIdx = 0;
            int startIdx = 1;
            int endIdx = tableData.Length;
            if (tStep.ToLower() == "instant" && dbString == "gphyd")
            {
                headerIdx = 3;
                startIdx = 4;
                endIdx = endIdx - 5;
            }
            for (int i = 1; i < tableData[headerIdx].Split(',').Length; i++)
            {
                dataTable.Columns.Add("SDI_" + tableData[headerIdx].Split(',')[i].ToString().Trim().Replace(" ", "_").ToUpper(), typeof(string));
            }
            // populate datatable
            for (int i = startIdx; i < endIdx; i++)
            {
                var tableDataRowVals = tableData[i].Split(',');
                var newDataRow = dataTable.NewRow();
                for (int j = 0; j < tableDataRowVals.Length; j++)
                {
                    newDataRow[j] = tableDataRowVals[j].ToString().Trim();
                }
                dataTable.Rows.Add(newDataRow);
            }
            return dataTable;
        }


        /// <summary>
        /// Hydromet wrapper for the HDB CGI data query
        /// </summary>
        /// <param name="region"></param>
        /// <param name="tstep"></param>
        /// <param name="cbttPcode"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public DataTable[] get_hdyromet_data(string region, string tstep, string cbttPcode, DateTime t1, DateTime t2)
        {
            var sitePcodeArray = cbttPcode.Trim().Split(',');
            // Get info table
            DataTable infoTable = GetInfoTable(sitePcodeArray);
            // Get data table
            DataTable dataTable = GetTsDataTable(tstep, cbttPcode, t1, t2);

            return new DataTable[] { dataTable, infoTable };
        }


        /// <summary>
        /// Method to emulate the HDB_SITE table
        /// </summary>
        /// <param name="sitePcodeArray"></param>
        /// <returns></returns>
        public static List<SiteModel.HdbSite> GetSiteInfo(string[] sitePcodeArray)
        {
            /*
            {
              "site_id": 919,
              "site_name": "LAKE POWELL",
              "site_common_name": "LAKE POWELL",
              "description": "",
              "elevation": 3700,
              "lat": "37.05778",
              "longi": "-111.30332",
              "db_site_code": "UC",
              "objecttype_id": 7,
              "objecttype_name": "reservoir",
              "basin_id": 2029,
              "river_mile": "NaN",
              "segment_no": 0,
              "state_id": 3,
              "state_code": "UT",
              "usgs_id": "",
              "nws_code": "",
              "shef_code": "",
              "scs_id": "",
              "parent_objecttype_id": 0,
              "parent_site_id": 0
            }
            */
            List<Models.SiteModel.HdbSite> siteReults = new List<SiteModel.HdbSite>();
            var siteList = GetSitePcodeList(sitePcodeArray, false);
            if (siteList == null)
            {
                List<string[]> allSites = new List<string[]>();
                for (int i = 0; i < siteTable.Rows.Count; i++)
                {
                    allSites.Add(new string[] { siteTable.Rows[i]["siteid"].ToString() });
                }
                siteList = allSites;
            }
            foreach (string[] siteCode in siteList)
            {
                DataRow[] siteRow = siteTable.Select("siteid = '" + siteCode[0].ToString().ToLower() + "'");
                if (siteRow.Length < 1)
                {
                    throw new Exception("site not found '" + siteCode[0].ToString().ToLower() + "'");
                }
                var ithSite = new SiteModel.HdbSite();
                ithSite.site_id = siteRow[0]["siteid"].ToString().ToUpper();
                ithSite.lat = siteRow[0]["latitude"].ToString();
                ithSite.longi = siteRow[0]["longitude"].ToString();
                ithSite.elevation = siteRow[0]["elevation"].ToString();
                ithSite.state_code = siteRow[0]["state"].ToString();
                ithSite.site_name = siteRow[0]["description"].ToString();
                ithSite.site_common_name = siteRow[0]["description"].ToString();
                ithSite.hydrologic_unit = siteRow[0]["huc12"].ToString();
                ithSite.db_site_code = siteRow[0]["agency_region"].ToString().ToLower() + "hyd";
                siteReults.Add(ithSite);
            }
            return siteReults;
        }


        /// <summary>
        /// Method to emulate the HDB_DATATYPE table
        /// </summary>
        /// <param name="sitePcodeArray"></param>
        /// <returns></returns>
        public static List<DatatypeModel.HdbDatatype> GetPcodeInfo(string[] sitePcodeArray)
        {
            /*
            {
              "datatype_id": 1393,
              "datatype_name": "average reservoir elevation",
              "datatype_common_name": "ave reservoir elevation",
              "physical_quantity_name": "water surface elevation",
              "unit_id": 4,
              "unit_name": "feet",
              "unit_common_name": "feet",
              "allowable_intervals": "non-instant",
              "agen_id": 0,
              "cmmnt": ""
            }
            */
            List<Models.DatatypeModel.HdbDatatype> pCodeResults = new List<DatatypeModel.HdbDatatype>();
            var pcodeList = GetSitePcodeList(sitePcodeArray, false);
            if (pcodeList == null)
            {
                List<string[]> allSites = new List<string[]>();
                for (int i = 0; i < pcodeTable.Rows.Count; i++)
                {
                    allSites.Add(new string[] { pcodeTable.Rows[i]["pcode"].ToString() });
                }
                pcodeList = allSites;
            }
            foreach (string[] pCode in pcodeList)
            {
                DataRow[] pCodeRow = pcodeTable.Select("pcode = '" + pCode[0].ToString().ToLower() + "'");
                if (pCodeRow.Length < 1)
                {
                    throw new Exception("pcode not found '" + pCode[0].ToString().ToLower() + "'");
                }
                var ithPCode = new DatatypeModel.HdbDatatype();
                ithPCode.datatype_id = pCodeRow[0]["pcode"].ToString().ToUpper();
                ithPCode.datatype_name = pCodeRow[0]["name"].ToString();
                ithPCode.datatype_common_name = pCodeRow[0]["name"].ToString();
                ithPCode.unit_name = pCodeRow[0]["units"].ToString();
                ithPCode.unit_common_name = pCodeRow[0]["units"].ToString();
                pCodeResults.Add(ithPCode);
            }
            return pCodeResults;
        }
        

        /// <summary>
        /// Method to emulate the HDB_SITEDATATYPE table
        /// </summary>
        /// <param name="cbttPcode"></param>
        /// <param name="cbtt"></param>
        /// <param name="pcode"></param>
        /// <returns></returns>
        public static List<SiteDatatypeModel.HdbSiteDatatype> GetCbttPcodeInfo(string[] cbttPcode, string[] cbtt, string[] pcode)
        {
            List<Models.SiteDatatypeModel.HdbSiteDatatype> cbttPCodeResults = new List<SiteDatatypeModel.HdbSiteDatatype>();
            var sitePcodeList = GetSitePcodeList(cbttPcode);
            var siteList = GetSitePcodeList(cbtt, false);
            var pcodeList = GetSitePcodeList(pcode, false);

            var sqlString = "isfolder=0 ";
            if (sitePcodeList != null && sitePcodeList.Count() != 0)
            {
                string ids = "";
                foreach (string[] ithId in sitePcodeList)
                {
                    ids += "'" + ithId[0] + "_" + ithId[1] + "',";
                }
                sqlString += "and name in (" + ids.TrimEnd(',') + ") ";
            }
            if (siteList != null && siteList.Count() != 0)
            {
                string ids = "";
                foreach (string[] ithId in siteList)
                {
                    ids += "'" + ithId[0] + "',";
                }
                sqlString += "and siteid in (" + ids.TrimEnd(',') + ") ";
            }
            if (pcodeList != null && pcodeList.Count() != 0)
            {
                string ids = "";
                foreach (string[] ithId in pcodeList)
                {
                    ids += "'" + ithId[0] + "',";
                }
                sqlString += "and parameter in (" + ids.TrimEnd(',') + ") ";
            }

            var searchTable = seriesTable.Select(sqlString).CopyToDataTable();
            DataRow[] sitePcodeRow = new DataView(searchTable).ToTable(true, "siteid", "parameter").Select();

            foreach (DataRow sitePcode in sitePcodeRow)
            {
                try
                {
                    var ithSitePCode = new SiteDatatypeModel.HdbSiteDatatype();
                    ithSitePCode.site_datatype_id = sitePcode["siteid"].ToString().ToUpper() + " " + sitePcode["parameter"].ToString().ToUpper();
                    ithSitePCode.site_id = sitePcode["siteid"].ToString().ToUpper();
                    ithSitePCode.datatype_id = sitePcode["parameter"].ToString().ToUpper();
                    var siteMetadata = GetSiteInfo(new string[] { ithSitePCode.site_id });
                    var pcodeMetadata = GetPcodeInfo(new string[] { ithSitePCode.datatype_id });
                    ithSitePCode.metadata = new SiteDatatypeModel.SiteDataTypeMetadata();
                    ithSitePCode.metadata.site_metadata = siteMetadata[0];
                    ithSitePCode.metadata.datatype_metadata = pcodeMetadata[0];
                    cbttPCodeResults.Add(ithSitePCode);
                }
                catch
                {

                }
            }
            return cbttPCodeResults;
        }


    }
}