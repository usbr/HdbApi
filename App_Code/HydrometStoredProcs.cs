using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Globalization;
using System.Net;

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

        private static DataTable GetInfoTable(string[] sitePcodeArray)
        {
            var pcodeTable = GetDataTableFromCsv(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory.ToString(), "pnHydrometParameterCatalog.csv"), true);
            var siteTable = GetDataTableFromCsv(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory.ToString(), "pnHydrometSiteCatalog.csv"), true);

            var sitePcodeList = new List<string[]>();
            foreach (var item in sitePcodeArray)
            {
                var sitePcodeArrayItems = item.Trim().Split(' ');
                if (sitePcodeArrayItems.Length != 2)
                {
                    throw new Exception("Invalid site-pcode combination '" + item.ToString().Trim().ToLower() + "'");
                }
                string[] items = new string[] { sitePcodeArrayItems[0].ToString().ToLower(), sitePcodeArrayItems[1].ToString().ToLower() };
                sitePcodeList.Add(items);
            }

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