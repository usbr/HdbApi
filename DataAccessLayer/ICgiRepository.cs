using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HdbApi.Models;
using Dapper;
using System.Data;
using System.Text.RegularExpressions;
using System.IO;
using Newtonsoft.Json;

namespace HdbApi.DataAccessLayer
{
    /// <summary>
    /// Processors for CGI Series objects
    /// </summary>
    internal interface ICgiRepository
    {
        List<string> get_cgi_data(IDbConnection db, string urlArgs);

        List<string[]> get_host_list();
    }

    
    public class CgiRepository : ICgiRepository
    {
        //private System.Data.IDbConnection db = HdbApi.App_Code.DbConnect.Connect();

        public List<string> get_cgi_data(IDbConnection hDB, string srchStr)
        {
            var hdbProcessor = new HdbApi.App_Code.HdbCommands();

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Build date ranges for series lookup
            // Define HDB table time-step.
            Match outTstep = Regex.Match(srchStr, @"&tstp=([A-Za-z]+)&");
            string sourceTstep = "";
            switch (outTstep.Groups[1].Value.ToString().ToLower())
            {
                case "in":
                    sourceTstep = "INSTANT";
                    break;
                case "hr":
                    sourceTstep = "HOUR";
                    break;
                case "dy":
                    sourceTstep = "DAY";
                    break;
                case "mn":
                    sourceTstep = "MONTH";
                    break;
                case "yr":
                    sourceTstep = "YEAR";
                    break;
                case "wy":
                    sourceTstep = "WY";
                    break;
                default:
                    throw new Exception("Error: Invalid Query Time-Step.");
            }

            DateTime t1 = new DateTime();
            DateTime t2 = new DateTime();
            DateTime t1Input = new DateTime();
            DateTime t2Input = new DateTime();

            // support for iso8601/parse-able date formats
            Match t1Iso = Regex.Match(srchStr, @"&t1=(.+?)&");
            Match t2Iso = Regex.Match(srchStr, @"&t2=(.+?)&");

            if (DateTime.TryParse(t1Iso.Groups[1].Value, out t1) && DateTime.TryParse(t2Iso.Groups[1].Value, out t2))
            {
                t1Input = t1;
                t2Input = t2;
                t2 = t2.AddDays(1);//hack to grab all t2 data
            }
            else
            {
                throw new Exception("Error: Invalid Query Dates.");
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Get SDIs and query information for HDB lookup
            // [OPTIONAL INPUT] Define HDB table source. Default to the R-tables if not defined in input search string
            string sourceTable = "";
            string mridString = "0";
            int mridInt = 0;
            Match outSourceTable = Regex.Match(srchStr, @"&table=([A-Za-z])");
            if (!outSourceTable.Success)
            {
                sourceTable = "R";
                mridString = null;
            }
            else
            {
                sourceTable = outSourceTable.Groups[1].Value.ToString();
                if (sourceTable == "M")
                {
                    mridString = Regex.Match(srchStr, @"&mrid=([\s\S]*?)&").Groups[1].Value.ToString();
                    mridInt = Convert.ToInt32(mridString);
                }
            }
            // Get SDIs and check for duplicates
            var sdiString = Regex.Match(srchStr, @"sdi=([\s\S]*?)&").Groups[1].Value.ToString();
            List<string> sdiList = new List<string>();
            sdiList.AddRange(sdiString.Split(','));
            sdiList = sdiList.Distinct().ToList();
            List<string> invalidSdiList = new List<string>();
            invalidSdiList = sdiList.Where(w => w.Any(c => !Char.IsDigit(c))).ToList();
            sdiString = "";
            if (invalidSdiList.Count() > 0 && sourceTable == "M") //no sdi passed in so get a list of sdis
            {
                sdiString = hdbProcessor.get_sdis_from_mrid(hDB, mridString, sourceTstep).ToString();
            }
            else if (invalidSdiList.Count() > 0) //querying r tables with no sdi? nope!
            {
                Console.WriteLine("Error: Invalid SDI in query.");
                return new List<string> { };
            }
            else //build list of sdis
            {
                foreach (var item in sdiList)
                { sdiString = sdiString + item + ","; }
            }
            sdiString = sdiString.Remove(sdiString.Count() - 1);

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Get HDB data
            // Main data query. Uses Stored HDB Procedure "GET_HDB_CGI_DATA" & "GET_HDB_CGI_INFO"
            var downloadTable = hdbProcessor.get_hdb_cgi_data(hDB, sdiString, sourceTstep, t1, t2, sourceTable, mridInt);
            downloadTable = downloadTable.Select("HDB_DATETIME >= #" + t1Input + "# AND HDB_DATETIME <= #" + t2Input + "#").CopyToDataTable();
            // SDI info query
            var sdiInfo = hdbProcessor.get_hdb_cgi_info(hDB, sdiString);

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Generate output
            DataTable table = new DataTable();
            var outFile = new List<string>();
            string outFormat = Regex.Match(srchStr, @"&format=([A-Za-z0-9]+)").Groups[1].Value.ToString();
            if (outFormat == "json")
            {
                var jsonOut = buildOutputJson(sdiInfo, downloadTable, t1, t2, sourceTstep, sourceTable, mridString);
                outFile.Add(JsonConvert.SerializeObject(jsonOut));
            }
            else
            {
                outFile = buildOutputText(sdiInfo, downloadTable, srchStr, outFormat);
            }
            return outFile;
        }


        /// <summary>
        /// Container for the HDB host and log-on information -- populated by hostlist.txt file
        /// </summary>
        private static List<string[]> hostList = new List<string[]>
        {
            // new string[] {hdbname, host, service, port, user, pass}
        };


        /// <summary>
        /// Get hostlist.txt file which contains the specifics for the HDBs that are available for active connections
        /// </summary>
        public List<string[]> get_host_list()
        {
            try
            {
                var path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory.ToString(), "hostlist.txt");
                string[] hostText = File.ReadAllLines(path);
                foreach (var host in hostText)
                {
                    List<string> hostItems = new List<string>();
                    foreach (var item in host.Split(','))
                    {
                        hostItems.Add(item.Trim());
                    }
                    hostList.Add(hostItems.ToArray());
                }
                return hostList;
            }
            catch
            {
                var exceptionString = "";
                exceptionString += "Textfile containing HDB hosts information not found...";
                exceptionString += "\thostlist.txt has to be in the same folder as the executable ";
                exceptionString += "\ttext should contain:";
                exceptionString += "\thdb-server-name, hdb-service-name, hdb-port-number, hdb-user-name, hdb-user-password";
                exceptionString += "";

                throw new FileNotFoundException(exceptionString);
            }
        }


        /// <summary>
        /// Builds a custom text output using queried data
        /// </summary>
        /// <param name="sdiInfo"></param>
        /// <param name="downloadTable"></param>
        /// <param name="srchStr"></param>
        /// <param name="outFormat"></param>
        /// <returns></returns>
        private static List<string> buildOutputText(DataTable sdiInfo, DataTable downloadTable,
            string srchStr, string outFormat)
        {
            var outText = new List<string>();

            // Generate header
            var txt = new List<string>();
            txt.Add("USBR Hydrologic Database (HDB) System Data Access");
            txt.Add(" ");
            txt.Add("The Bureau of Reclamation makes efforts to maintain the accuracy of data found ");
            txt.Add("in the HDB system databases but the data is largely unverified and should be ");
            txt.Add("considered preliminary and subject to change.  Data and services are provided ");
            txt.Add("with the express understanding that the United States Government makes no ");
            txt.Add("warranties, expressed or implied, concerning the accuracy, completeness, ");
            txt.Add("usability, or suitability for any particular purpose of the information or data ");
            txt.Add("obtained by access to this computer system. The United States shall be under no ");
            txt.Add("liability whatsoever to any individual or group entity by reason of any use made ");
            txt.Add("thereof. ");
            txt.Add(" ");
            for (int i = 0; i < sdiInfo.Rows.Count; i++)
            {
                txt.Add("SDI " + sdiInfo.Rows[i][0] + ": " + sdiInfo.Rows[i][1].ToString().ToUpper() + " - " +
                    sdiInfo.Rows[i][2].ToString().ToUpper() + " in " + sdiInfo.Rows[i][3].ToString().ToUpper());
            }
            txt.Add("BEGIN DATA");
            string headLine = "";
            for (int i = 0; i < downloadTable.Columns.Count; i++)
            {
                if (i == 0)
                { headLine = headLine + "DATETIME".PadLeft(16) + ", "; }
                else
                { headLine = headLine + downloadTable.Columns[i].ColumnName.PadLeft(12) + ", "; }
            }
            txt.Add(headLine.Remove(headLine.Length - 2));
            // Generate body
            foreach (DataRow row in downloadTable.Rows)
            {
                string[] fields = row.ItemArray.Select(field => field.ToString()).ToArray();
                string newRow = "";
                for (int i = 0; i < fields.Count(); i++)
                {
                    if (i == 0)
                    { newRow = newRow + DateTime.Parse(fields[i]).ToString("MM/dd/yyyy HH:mm") + ", "; }
                    else
                    {
                        string fieldVal = fields[i];
                        if (fieldVal == "")
                        { fieldVal = "NaN"; }
                        newRow = newRow + fieldVal.PadLeft(12) + ", ";
                    }
                }
                txt.Add(newRow.Remove(newRow.Length - 2));
                //txt.Add(string.Join(",", fields));
            }
            txt.Add("END DATA");

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Return output
            string[] txtArray = txt.ToArray();
            List<string> outFile = writeHTML(txtArray, outFormat, srchStr);
            return outFile;
        }


        /// <summary>
        /// Builds a JSON array using queried data
        /// </summary>
        /// <param name="sdiInfo"></param>
        /// <param name="downloadTable"></param>
        /// <param name="srchStr"></param>
        /// <param name="outFormat"></param>
        /// <returns></returns>
        private static CgiModel.HdbCgiJson buildOutputJson(DataTable sdiInfo, DataTable downloadTable,
             DateTime t1, DateTime t2, string sourceTstep, string sourceTable, string mridString)
        {
            var outText = new List<string>();

            // Generate header
            var txt = new List<string>();

            // Populate top level JSON
            var jsonOut = new CgiModel.HdbCgiJson();
            jsonOut.QueryDate = DateTime.Now.ToString("G");
            jsonOut.StartDate = t1.ToString("G");
            jsonOut.EndDate = t2.ToString("G");
            jsonOut.TimeStep = sourceTstep;
            if (sourceTable.ToLower() == "m") { jsonOut.DataSource = "Modeled"; }
            else { jsonOut.DataSource = "Observed"; }

            // Resolve MRIDs
            var mridList = new List<string>();
            if (mridString == "" || mridString == null || mridString == "0") { mridList.Add(null); }
            else { mridList.AddRange(mridString.Split(',').ToArray<string>()); }

            // Initialize JSON SDI/Site container
            var jsonSites = new List<CgiModel.Sites>();

            // Build JSON SDI/Site Objects
            foreach (DataRow series in sdiInfo.Rows)//loop through each SDI
            {
                foreach (var mrid in mridList)//loop through each mrid
                {
                    // Populate SDI/Site level metadata
                    var jsonSite = new CgiModel.Sites();
                    jsonSite.SDI = series[0].ToString();
                    jsonSite.MRID = mrid;
                    jsonSite.SiteName = series[1].ToString();
                    jsonSite.DataTypeName = series[2].ToString();
                    jsonSite.DataTypeUnit = series[3].ToString();
                    jsonSite.Latitude = series[4].ToString();
                    jsonSite.Longitude = series[5].ToString();
                    jsonSite.Elevation = series[6].ToString();
                    jsonSite.DB = series[7].ToString();

                    // Select TS data for the SDI - MRID
                    DataView view = new DataView(downloadTable);
                    DataTable dtQueryTable;
                    DataRow[] rows;
                    if (mrid == null)
                    {
                        dtQueryTable = view.ToTable(false, new string[] { "HDB_DATETIME", "SDI_" + jsonSite.SDI });
                        rows = dtQueryTable.Select();
                    }
                    else
                    {
                        dtQueryTable = view.ToTable(false, new string[] { "HDB_DATETIME", "MODEL_RUN_ID", "SDI_" + jsonSite.SDI });
                        rows = dtQueryTable.Select("MODEL_RUN_ID = '" + mrid + "'");
                    }

                    // Build the TS Data JSON container
                    var jsonSiteData = new List<CgiModel.Data>();
                    foreach (DataRow row in rows)
                    {
                        var jsonSiteDataPoint = new CgiModel.Data();
                        jsonSiteDataPoint.t = DateTime.Parse(row["HDB_DATETIME"].ToString()).ToString("G");
                        jsonSiteDataPoint.v = row["SDI_" + jsonSite.SDI].ToString();
                        jsonSiteData.Add(jsonSiteDataPoint);
                    }

                    // Add Site Data to JSON Site Object
                    jsonSite.Data = jsonSiteData;

                    // Add JSON Site Object to JSON Sites Container
                    jsonSites.Add(jsonSite);
                }
            }

            // Add JSON Site Container Array to JSON out
            jsonOut.Series = jsonSites;

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Return output
            return jsonOut;
        }


        /// <summary>
        /// Writes a HTML tagged C# List for output
        /// </summary>
        /// <param name="outFile"></param>
        /// <param name="outFormat"></param>
        /// <param name="srchStr"></param>
        /// <returns></returns>
        private static List<string> writeHTML(string[] outFile, string outFormat, string srchStr)
        {
            /* FORMAT
                 * 1 - CSV with preamble and headers
                 * 2 - Table with preamble and headers
                 * 3 - CSV no preamble with headers
                 * 4 - Table no preamble with headers
                 * 5 - 
                 * 6 - 
                 * 7 - 
                 * 8 - Pisces HdbWebSeries Query
                 * 9 - DyGraphs in Graphing.cs 
                 * 88 - Pure CSV for DyGraph generation
                 * 99 - AM Chart in Graphing.cs
                 * */

            var htmlOut = new List<string>();
            //string format = "3";
            bool isCSV = false, hasPreamble = false, isAmChart = false, isDyGraph = false, isHdbWebSeriesQuery = false, isJson = false;
            if (outFormat == "1" || outFormat == "3" || outFormat == "5")
            { isCSV = true; }
            if (outFormat == "1" || outFormat == "2")
            { hasPreamble = true; }
            if (outFormat == "8" || outFormat == "88")
            { isHdbWebSeriesQuery = true; }
            if (outFormat == "9" || outFormat.ToLower() == "graph")
            { isDyGraph = true; }
            if (outFormat == "99")
            { isAmChart = true; }
            if (outFormat.ToLower() == "csv")
            {
                hasPreamble = false;
                isCSV = true;
            }
            if (outFormat.ToLower() == "html")
            {
                hasPreamble = false;
            }
            if (outFormat.ToLower() == "json")
            {
                isJson = true;
            }

            int startOfDataRow = Array.IndexOf(outFile, "BEGIN DATA");

            // format == 99 or format == graph
            if (isAmChart || isDyGraph)
            { htmlOut = writeHTML_dyGraphs(outFile, srchStr); }
            // format == 8
            else if (isHdbWebSeriesQuery)
            {
                if (outFormat == "8")
                {
                    htmlOut.Add("<PRE>");
                    for (int i = startOfDataRow + 2; i < outFile.Count() - 1; i++)
                    { htmlOut.Add(outFile[i] + "\r\n"); }
                }
                else
                {
                    var headerString = "Date,";
                    for (int i = 12; i < Array.IndexOf(outFile, "BEGIN DATA"); i++)
                    { headerString = headerString + outFile[i].Replace(",", " ") + ","; }
                    htmlOut.Add(headerString.Remove(headerString.Length - 1) + "\n");
                    for (int i = startOfDataRow + 2; i < outFile.Count() - 1; i++)
                    { htmlOut.Add(outFile[i] + "\n"); }
                }
                return htmlOut;
            }
            // format == json
            else if (isJson)
            {
                //[JR] build JSON output
            }
            // format == 1, 2, 3, 4
            else
            {
                htmlOut.Add("<HTML>");
                htmlOut.Add("<HEAD>");
                htmlOut.Add("<TITLE>Bureau of Reclamation HDB Data</TITLE>");
                htmlOut.Add("</HEAD>");
                htmlOut.Add("<BODY>");

                // Add preamble
                if (hasPreamble)
                {
                    htmlOut.Add("<PRE>");

                    htmlOut.Add("<B>" + outFile[0] + "</B>");
                    for (int i = 1; i <= startOfDataRow - 1; i++)
                    {
                        if (false)//!HDB_CGI.cgi.jrDebug)
                        { htmlOut.Add("<BR>" + outFile[i]); }
                        else
                        { htmlOut.Add(outFile[i]); }
                    }
                    htmlOut.Add("</PRE>");
                    htmlOut.Add("<p>");
                }
                // Add data
                for (int i = startOfDataRow; i < outFile.Count(); i++)
                {
                    if (isCSV && hasPreamble)
                    {
                        if (i == startOfDataRow)
                        { htmlOut.Add("<PRE>"); }
                        htmlOut.Add("<BR>" + outFile[i]);
                    }
                    else if (isCSV && !hasPreamble)
                    {
                        if (i == startOfDataRow)
                        {
                            htmlOut.Add("<PRE>");
                            i++;
                            htmlOut.Add(outFile[i]);
                        }
                        else if (i == outFile.Count() - 1)
                        { }
                        else
                        { htmlOut.Add("<BR>" + outFile[i]); }
                    }
                    else
                    {
                        if (i == startOfDataRow)
                        {
                            htmlOut.Add("<TABLE BORDER=1>");
                            i++;
                            htmlOut.Add("<TR><TH>" + outFile[i].Replace(",", "</TH><TH>") + "</TH></TR>");
                        }
                        else if (i == outFile.Count() - 1)
                        { }
                        else
                        { htmlOut.Add("<TR><TD>" + outFile[i].Replace(",", "</TD><TD>") + "</TD></TR>"); }
                    }
                }
                // Add final lines
                if (isCSV)
                { htmlOut.Add("</PRE>"); }
                else
                { htmlOut.Add("</TABLE>"); }
                htmlOut.Add("</BODY></HTML>");
            }
            // Output
            return htmlOut;
        }


        /// <summary>
        /// Writes a HTML tagged C# List for dyGraphs output
        /// </summary>
        /// <param name="outFile"></param>
        /// <param name="query"></param>
        /// <param name="dygraphsUrlData"></param>
        /// <returns></returns>
        private static List<string> writeHTML_dyGraphs(string[] outFile, string query, bool dygraphsUrlData = true)
        {
            // The data in the outFile has to be preceded by a line that says "BEGIN DATA"
            //      and followed by a line that says "END DATA"
            // The series info has to be in outFile[12]

            List<string> htmlOut = new List<string>();

            // Populate chart HTML requirements
            #region
            htmlOut.Add(@"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01//EN"" ""http://www.w3.org/TR/html4/strict.dtd"">");
            htmlOut.Add("<html>");
            htmlOut.Add("<head>");
            htmlOut.Add(@"<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"">");
            htmlOut.Add("<title>HDB CGI Data Query Graph</title>");
            htmlOut.Add("<!-- Call DyGraphs JavaScript Reference -->");
            htmlOut.Add(@"<script type=""text/javascript""  src=""https://www.usbr.gov/js/waterops/dygraph.min.js""></script>");
            htmlOut.Add(@"<link rel=""stylesheet"" href=""https://www.usbr.gov/js/waterops/dygraph.css"">");
            htmlOut.Add(@"<style type=""text/css"">");
            htmlOut.Add("#graphdiv {position: absolute; left: 50px; right: 50px; top: 75px; bottom: 50px;}");
            htmlOut.Add("#graphdiv .dygraph-legend {width: 300px !important; background-color: transparent !important; left: 75px !important;}");
            htmlOut.Add("</style></head>");
            htmlOut.Add("<body>");
            htmlOut.Add("<!-- Place DyGraphs Chart -->");
            htmlOut.Add(@"<div id=""status"" style=""width:1000px; font-size:0.8em; padding-top:5px;""></div>");
            htmlOut.Add("<br>");
            htmlOut.Add(@"<div id=""graphdiv""></div>");
            htmlOut.Add("");
            htmlOut.Add("<!-- Build DyGraphs Chart -->");
            htmlOut.Add(@"<script type=""text/javascript"">");
            htmlOut.Add("g = new Dygraph(");
            htmlOut.Add(@"document.getElementById(""graphdiv""),");
            htmlOut.Add("");
            #endregion

            // Populate html data
            #region
            int startOfDataRow = Array.IndexOf(outFile, "BEGIN DATA") + 2;
            int endOfDataRow = Array.IndexOf(outFile, "END DATA") - 1;
            string headerString = @"""Date,";
            var units = new List<string>();
            for (int i = 12; i < Array.IndexOf(outFile, "BEGIN DATA"); i++)
            {
                headerString = headerString + outFile[i].Replace(",", " ") + ",";
                var unit = Regex.Matches(outFile[i], @"(in ).*");
                units.Add(unit[0].ToString().Replace("in ", ""));
            }
            if (!dygraphsUrlData)
            {
                htmlOut.Add(headerString.Remove(headerString.Length - 1) + @"\n "" +");
                // POPULATE DATA
                for (int i = startOfDataRow; i < endOfDataRow; i++)
                {
                    var val = outFile[i].Split(',');
                    var t = DateTime.Parse(outFile[i].Split(',')[0].ToString()).ToString("yyyy-MM-dd HH:mm");
                    string dataRow = "$" + t + ", ";
                    for (int j = 1; j < val.Count(); j++)
                    {
                        var jthVal = val[j].ToString().Trim();
                        if (jthVal == double.NaN.ToString())
                        { jthVal = "NaN"; }
                        dataRow = dataRow + jthVal + ", ";
                    }
                    if (i + 1 == endOfDataRow)
                    { htmlOut.Add((dataRow.Remove(dataRow.Length - 2) + @"\n$").Replace('$', '"')); }
                    else
                    { htmlOut.Add((dataRow.Remove(dataRow.Length - 2) + @"\n$ +").Replace('$', '"')); }
                }
            }
            else
            {
                //string query = @"localhost:8080/HDB_CGI.com?sdi=1930,1863&tstp=HR&syer=2015&smon=1&sday=1&eyer=2015&emon=1&eday=10&format=88";
                var tempQuery = query;
                tempQuery = tempQuery.ToLower().Replace("format=9", "format=88");
                tempQuery = tempQuery.ToLower().Replace("format=graph", "format=88");
                htmlOut.Add("'" + tempQuery + "'");
            }
            #endregion

            // Populate chart HTML requirements
            #region
            htmlOut.Add(", {fillGraph: true, showRangeSelector: true, legend: 'always'");
            //htmlOut.Add(", rangeSelectorPlotStrokeColor: '#0000ff', rangeSelectorPlotFillColor: '#0000ff'");
            htmlOut.Add(", xlabel: 'Date', ylabel: '" + units[0] + "', labelsSeparateLines: true");
            htmlOut.Add(", labelsDiv: document.getElementById('status'), axisLabelWidth: 75");
            htmlOut.Add(", highlightCircleSize: 5, pointSize: 1.5, strokeWidth: 1.5");
            if (units.Distinct().Count() > 1)
            { htmlOut.Add(", y2label: '" + units[1] + "', '" + headerString.Split(',')[2] + "' : { axis : { } } }"); }
            else
            { htmlOut.Add("}"); }
            htmlOut.Add(");");
            htmlOut.Add("");
            htmlOut.Add("</script>");
            htmlOut.Add("</body>");
            htmlOut.Add("</html>");
            #endregion

            return htmlOut;
        }

    }
}