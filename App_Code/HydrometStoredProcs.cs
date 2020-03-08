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
        //https://www.usbr.gov/pn-bin/daily.pl?list=sco%20fb&start=2020-02-23&end=2020-02-26&format=html&flags=false&description=false
        private static string pnDailyURL = "https://www.usbr.gov/pn-bin/daily.pl?list=$CBTTPCODE$&start=$T1$&end=$T2$&format=csv&flags=false&description=false";
        //https://www.usbr.gov/pn-bin/instant.pl?list=sco%20fb&start=2020-02-23&end=2020-02-26&format=html&flags=false&description=false
        private static string pnInstantURL = "https://www.usbr.gov/pn-bin/instant.pl?list=$CBTTPCODE$&start=$T1$&end=$T2$&format=csv&flags=false&description=false";
        private static string gpDailyURL = "";
        private static string gpInstantURL = "";
        private static DataTable pcodeTable = GetDataTableFromCsv(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory.ToString(), "pnHydrometParameterCatalog.csv"), true);
        private static DataTable siteTable = GetDataTableFromCsv(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory.ToString(), "pnHydrometSiteCatalog.csv"), true);


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
            
            var sitePcodeArray = cbttPcode.Trim().Split(',');
            // Get info table
            DataTable infoTable = GetInfoTable(sitePcodeArray);
            // Get data table
            DataTable dataTable = new DataTable();

            string url = baseUrl.Replace("$CBTTPCODE$", cbttPcode);
            url = url.Replace("$T1$", t1.ToString("yyyy-MM-dd"));
            url = url.Replace("$T2$", t2.ToString("yyyy-MM-dd"));

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            StreamReader sr = new StreamReader(resp.GetResponseStream());
            string urlData = sr.ReadToEnd();
            sr.Close();

            string[] tableData = urlData.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            // define datatable columns
            dataTable.Columns.Add("HDB_DATETIME", typeof(string));
            for (int i = 1; i < tableData[0].Split(',').Length; i++)
            {
                dataTable.Columns.Add("SDI_" + tableData[0].Split(',')[i].ToString().Trim().Replace(" ", "_").ToUpper(), typeof(string));
            }
            // populate datatable
            for (int i = 1; i < tableData.Length; i++)
            {
                var tableDataRowVals = tableData[i].Split(',');
                var newDataRow = dataTable.NewRow();
                for (int j = 0; j < tableDataRowVals.Length; j++)
                {
                    newDataRow[j] = tableDataRowVals[j].ToString();
                }
                dataTable.Rows.Add(newDataRow);
            }

            return new DataTable[] { dataTable, infoTable };
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
            //var pcodeTable = GetDataTableFromCsv(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory.ToString(), "pnHydrometParameterCatalog.csv"), true);
            //var siteTable = GetDataTableFromCsv(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory.ToString(), "pnHydrometSiteCatalog.csv"), true);

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
                infoRow["dbsitecode"] = "pnhyd";

                infoTable.Rows.Add(infoRow);
            }

            return infoTable;
        }


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
            var sitePcodeList = GetSitePcodeList(sitePcodeArray, false);
            if (sitePcodeList == null)
            {
                List<string[]> allSites = new List<string[]>();
                for (int i = 0; i < siteTable.Rows.Count; i++)
                {
                    allSites.Add(new string[] { siteTable.Rows[i]["siteid"].ToString() });
                }
                sitePcodeList = allSites;
            }
            foreach (string[] siteCode in sitePcodeList)
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
                ithSite.db_site_code = "pnhyd";
                siteReults.Add(ithSite);
            }
            return siteReults;
        }


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
            var sitePcodeList = GetSitePcodeList(sitePcodeArray, false);
            if (sitePcodeList == null)
            {
                List<string[]> allSites = new List<string[]>();
                for (int i = 0; i < pcodeTable.Rows.Count; i++)
                {
                    allSites.Add(new string[] { pcodeTable.Rows[i]["pcode"].ToString() });
                }
                sitePcodeList = allSites;
            }
            foreach (string[] pCode in sitePcodeList)
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

    }
}