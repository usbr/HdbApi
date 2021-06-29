using System.Collections.Generic;
using System.Web.Http;
using System;
using System.Net;
using System.Net.Http;
using Swashbuckle.Swagger.Annotations;
using Swashbuckle.Examples;
using System.Data;

namespace HdbApi.Controllers
{
    public class SeriesController : ApiController
    {
        /// <summary>
        /// Query Time-Series Data
        /// </summary>
        /// <remarks>
        /// Gets Time-Series Data given certain input filters
        /// </remarks>
        /// <param name="sdi">Site Datatype ID</param>
        /// <param name="interval">Time-Series Interval</param>
        /// <param name="t1">Start Date in MM-DD-YYYY HH:MM</param>
        /// <param name="t2">End Date in MM-DD-YYYY HH:MM</param>
        /// <param name="rbase">Optional: TRUE to query r-base tables, FALSE to query interval tables (default)</param>
        /// <param name="table">Optional: R for Observed Data (default), M for Modeled Projections</param>
        /// <param name="mrid">Model Run ID required if table=M</param>
        /// <returns></returns>
        /// <response code="200"></response>
        [HttpGet, Route("series/")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Models.SeriesModel.TimeSeries))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(TimeSeriesExample))]
        [SwaggerOperation(Tags = new[] { "HDB TimeSeries Data" })]
        public IHttpActionResult Get([FromUri] string sdi, [FromUri] DateTime t1, [FromUri] DateTime t2, [FromUri] IntervalType interval = new IntervalType(), [FromUri] bool rbase = false, [FromUri] TableType table = new TableType(), [FromUri] int mrid = 0, [FromUri] string instantMinutes = "60")
        {
            IDbConnection db = HdbController.Connect(this.Request.Headers);
            var seriesProcessor = new HdbApi.DataAccessLayer.SeriesRepository();
            var result = seriesProcessor.GetSeries(db, sdi, interval.ToString(), t1, t2, table.ToString(), mrid, rbase, instantMinutes);

            try
            {
                db.Close();
                db.Dispose();
            }
            catch
            {

            }

            return Ok(result); 
        }


        public enum TableType
        {
            R = 0,
            M = 1,
            B = 3
        }


        public enum IntervalType
        {
            month = 0,
            day = 1,
            hour = 2,
            instant = 3,
            year = 4
        }


        /// <summary>
        /// Write Observed Data
        /// </summary>
        /// <remarks>
        /// Write Time-Series Data points for Observed Data
        /// </remarks>
        /// <param name="input">HDB Observed Data Writer Object</param>
        /// <returns></returns>
        /// <response code="200"></response>
        [HttpPost, Route("series/r-write/{input=input}")]
        [SwaggerOperation(Tags = new[] { "HDB TimeSeries Data" })]
        public IHttpActionResult Post([FromBody] List<Models.PointModel.ObservedPoint> input)
        {
            IDbConnection db = HdbController.Connect(this.Request.Headers);

            var hdbProcessor = new HdbApi.App_Code.HdbCommands();
            
            foreach (Models.PointModel.ObservedPoint point in input)
            {
                if (point.loading_application_id < 1)
                {
                    point.loading_application_id = -99;
                }
                if (point.computation_id < 1)
                {
                    point.computation_id = -99;
                }
                if (point.data_flags == null)
                {
                    point.data_flags = "";
                }
                var result = hdbProcessor.modify_r_base_raw(db, point.site_datatype_id, point.interval, point.start_date_time, point.value, point.overwrite_flag, point.validation, point.do_update_y_or_n, point.loading_application_id, point.computation_id, point.data_flags);
            }

            try
            {
                db.Close();
                db.Dispose();
            }
            catch
            {

            }

            return Ok(input);
        }


        /// <summary>
        /// Delete Observed Data
        /// </summary>
        /// <remarks>
        /// Delete Time-Series Data points for Observed Data
        /// </remarks>
        /// <param name="input">HDB Observed Data Writer Object</param>
        /// <returns></returns>
        /// <response code="200"></response>
        [HttpDelete, Route("series/r-delete/{input=input}")]
        [SwaggerOperation(Tags = new[] { "HDB TimeSeries Data" })]
        public IHttpActionResult Delete([FromBody] List<Models.PointModel.ObservedPoint> input)
        {
            IDbConnection db = HdbController.Connect(this.Request.Headers);

            var hdbProcessor = new HdbApi.App_Code.HdbCommands();

            foreach (Models.PointModel.ObservedPoint point in input)
            {
                var result = hdbProcessor.delete_from_hdb(db, point.site_datatype_id, point.start_date_time, point.interval);
            }

            try
            {
                db.Close();
                db.Dispose();
            }
            catch
            {

            }

            return Ok(input);
        }


        /// <summary>
        /// Write Modeled Data
        /// </summary>
        /// <remarks>
        /// Write Time-Series Data points for Modeled Data
        /// </remarks>
        /// <param name="input">HDB Modeled Data Writer Object</param>
        /// <returns></returns>
        /// <response code="200"></response>
        [HttpPost, Route("series/m-write/{input=input}")]
        [SwaggerOperation(Tags = new[] { "HDB TimeSeries Data" })]
        public IHttpActionResult Post([FromBody] List<Models.PointModel.ModeledPoint> input)
        {
            IDbConnection db = HdbController.Connect(this.Request.Headers);

            var hdbProcessor = new HdbApi.App_Code.HdbCommands();

            foreach (Models.PointModel.ModeledPoint point in input)
            {
                var result = hdbProcessor.modify_m_table_raw(db, point.model_run_id, point.site_datatype_id, point.start_date_time, point.value, point.interval, point.do_update_y_or_n);
            }

            try
            {
                db.Close();
                db.Dispose();
            }
            catch
            {

            }

            return Ok(input);
        }
        

        /// <summary>
        /// Delete Modeled Data
        /// </summary>
        /// <remarks>
        /// Delete Time-Series Data points for Modeled Data
        /// </remarks>
        /// <param name="input">HDB Modeled Data Writer Object</param>
        /// <returns></returns>
        /// <response code="200"></response>
        [HttpDelete, Route("series/m-delete/{input=input}")]
        [SwaggerOperation(Tags = new[] { "HDB TimeSeries Data" })]
        public IHttpActionResult Delete([FromBody] List<Models.PointModel.ModeledPoint> input)
        {
            IDbConnection db = HdbController.Connect(this.Request.Headers);

            var hdbProcessor = new HdbApi.App_Code.HdbCommands();

            foreach (Models.PointModel.ModeledPoint point in input)
            {
                var result = hdbProcessor.delete_from_hdb(db, point.site_datatype_id, point.start_date_time, point.interval, point.model_run_id);
            }

            try
            {
                db.Close();
                db.Dispose();
            }
            catch
            {

            }

            return Ok(input);
        }
        
        
        /// <summary>
         /// Example provider for the Get TimeSeries method 
         /// </summary>
        public class TimeSeriesExample : IExamplesProvider
        {
            public object GetExamples()
            {
                var tsQuery = new Models.SeriesModel.TimeSeriesQuery
                {
                    hdb = "lchdb2",
                    sdi = "1980",
                    t1 = new DateTime(2000, 1, 1, 0, 0, 0),
                    t2 = new DateTime(2000, 1, 2, 0, 0, 0),
                    interval = "day",
                    table = "r",
                    mrid = 0
                };
                var tsMeta =new Models.SiteDatatypeModel.SiteDataTypeMetadata
                {
                    site_metadata = new Models.SiteModel.HdbSite
                    {
                        site_id = "919",
                        site_name = "LAKE POWELL",
                        site_common_name = "LAKE POWELL",
                        description = DBNull.Value.ToString(),
                        elevation = "3700",
                        lat = "37.05778",
                        longi = "-111.30332",
                        db_site_code = "UC",
                        objecttype_id = 7,
                        objecttype_name = "reservoir",
                        basin_id = 2029,
                        hydrologic_unit = null,
                        river_mile = float.NaN,
                        segment_no = 0,
                        state_id = 3,
                        state_code = "UT",
                        usgs_id = DBNull.Value.ToString(),
                        nws_code = DBNull.Value.ToString(),
                        shef_code = DBNull.Value.ToString(),
                        scs_id = DBNull.Value.ToString(),
                        parent_objecttype_id = 0,
                        parent_site_id = 0
                    },
                    datatype_metadata = new Models.DatatypeModel.HdbDatatype
                    {
                        datatype_id = "1393",
                        datatype_name = "average reservoir elevation",
                        datatype_common_name = "ave reservoir elevation",
                        physical_quantity_name = "water surface elevation",
                        unit_id = 4,
                        unit_name = "feet",
                        unit_common_name = "feet",
                        allowable_intervals = "non-instant",
                        agen_id = 0,
                        cmmnt = ""
                    }
                };
                var tsPoint1 = new Models.SeriesModel.TimeSeriesPoint
                {
                    datetime = new DateTime(2000, 1, 1, 0, 0, 0),
                    value = 3.1416.ToString(),
                    flag = ""
                };
                var tsPoint2 = new Models.SeriesModel.TimeSeriesPoint
                {
                    datetime = new DateTime(2000, 1, 2, 0, 0, 0),
                    value = 2.7183.ToString(),
                    flag = ""
                };
                var tsData = new List<Models.SeriesModel.TimeSeriesPoint>
                {
                    tsPoint1, tsPoint2
                };
                var ts = new Models.SeriesModel.TimeSeries
                {
                    query = tsQuery,
                    metadata = tsMeta,
                    data = tsData
                };
                return ts;
            }
        }


        /// <summary>
        /// Query Time-Series Data (Legacy CGI Program)
        /// </summary>
        /// <remarks>
        /// Calls the stored procedure used by the legacy CGI program for backwards compatibility
        /// </remarks>
        /// <returns></returns>
        /// <param name="svr">HDB instance name</param>
        /// <param name="sdi">Comma delimited list of SDIs</param>
        /// <param name="tstp">Interval table {INSTANT, HOUR, DAY, MONTH, YEAR}</param>
        /// <param name="t1">Start date</param>
        /// <param name="t2">End date</param>
        /// <param name="table">Optional - HDB Table {R, M, B}</param>
        /// <param name="mrid">Optional - Model Run ID if table=M</param>
        /// <param name="format">Output format</param>
        /// <returns></returns>
        [HttpGet, Route("cgi")]
        [SwaggerOperation(Tags = new[] { "HDB TimeSeries Data" })]
        public HttpResponseMessage Get([FromUri] string svr, [FromUri] string sdi, [FromUri] string t1, [FromUri] string t2, [FromUri] string tstp = "DY", 
            [FromUri] TableType table = new TableType(), [FromUri] string mrid = "0", [FromUri] string format = "1")
        {
            var cgiProcessor = new HdbApi.DataAccessLayer.CgiRepository();

            IDbConnection db;
            if (svr.ToLower() == "pnhyd" || svr.ToLower() == "gphyd")
            {
                db = null;
            }
            else //HDB
            {
                // Connect to HDB
                bool hostFound = false;
                List<string[]> hostList = cgiProcessor.get_host_list();
                foreach (string[] hostInfo in hostList)
                {
                    if (svr == hostInfo[0].ToString())
                    {
                        hostFound = true;
                        this.Request.Headers.Add("api_hdb", hostInfo[0]);
                        this.Request.Headers.Add("api_user", hostInfo[4]);
                        this.Request.Headers.Add("api_pass", hostInfo[5]);
                    }
                }
                if (!hostFound)
                {
                    throw new Exception("HDB Database not recognized.");
                }
                db = HdbController.Connect(this.Request.Headers);
            }
            // Build CGI query URL
            //      ?svr=lchdb2&sdi=25401&tstp=IN&t1=2018-05-07T05:00&t2=2018-05-07T08:00&table=R&mrid=&format=2            
            var tstpString = "";
            switch (tstp.ToString().ToLower())
            {
                case "instant":
                case "in":
                    tstpString = "IN";
                    break;
                case "hour":
                case "hr":
                    tstpString = "HR";
                    break;
                case "month":
                case "mn":
                    tstpString = "MN";
                    break;
                case "year":
                case "yr":
                    tstpString = "YR";
                    break;
                case "wy":
                    tstpString = "WY";
                    break;
                default:
                    tstpString = "DY";
                    break;
            }
            tstp = tstpString;

            int t1Int, t2Int;
            DateTime t1Input, t2Input;

            if (DateTime.TryParse(t1, out t1Input) && DateTime.TryParse(t2, out t2Input))
            {
                // Snap date-times to the time-step-specific date format
                switch (tstp.ToString().ToLower())
                {
                    case "in":
                    case "hr":
                        t1Input = t1Input;
                        t2Input = t2Input;
                        break;
                    case "dy":
                        t1Input = new DateTime(t1Input.Year, t1Input.Month, t1Input.Day, 0, 0, 0);
                        t2Input = new DateTime(t2Input.Year, t2Input.Month, t2Input.Day, 0, 0, 0);
                        break;
                    case "mn":
                        t1Input = new DateTime(t1Input.Year, t1Input.Month, 1, 0, 0, 0);
                        t2Input = new DateTime(t2Input.Year, t2Input.Month, 1, 0, 0, 0);
                        break;
                    case "yr":
                    case "wy":
                        t1Input = new DateTime(t1Input.Year, 1, 1, 0, 0, 0);
                        t2Input = new DateTime(t2Input.Year, 1, 1, 0, 0, 0);
                        break;
                    default:
                        throw new Exception("Error: Invalid Query Time-Step.");
                }
            }
            // Special case for T1 and T2 - If integers, query last X-timestep's worth of data and snap dates
            else if (int.TryParse(t1, out t1Int) && int.TryParse(t2, out t2Int))
            {
                switch (tstp.ToString().ToLower())
                {
                    case "in":
                    case "hr":
                        t1Input = DateTime.Now.AddHours(t1Int);
                        t1Input = new DateTime(t1Input.Year, t1Input.Month, t1Input.Day, t1Input.Hour, 0, 0);
                        t2Input = DateTime.Now.AddHours(t2Int);
                        t2Input = new DateTime(t2Input.Year, t2Input.Month, t2Input.Day, t2Input.Hour, 0, 0);
                        break;
                    case "dy":
                        t1Input = DateTime.Now.AddDays(t1Int);
                        t1Input = new DateTime(t1Input.Year, t1Input.Month, t1Input.Day, 0, 0, 0);
                        t2Input = DateTime.Now.AddDays(t2Int);
                        t2Input = new DateTime(t2Input.Year, t2Input.Month, t2Input.Day, 0, 0, 0);
                        break;
                    case "mn":
                        t1Input = DateTime.Now.AddMonths(t1Int);
                        t1Input = new DateTime(t1Input.Year, t1Input.Month, 1, 0, 0, 0);
                        t2Input = DateTime.Now.AddMonths(t2Int);
                        t2Input = new DateTime(t2Input.Year, t2Input.Month, 1, 0, 0, 0);
                        break;
                    case "yr":
                    case "wy":
                        t1Input = DateTime.Now.AddYears(t1Int);
                        t1Input = new DateTime(t1Input.Year, 1, 1, 0, 0, 0);
                        t2Input = DateTime.Now.AddYears(t2Int);
                        t2Input = new DateTime(t2Input.Year, 1, 1, 0, 0, 0);
                        break;
                    default:
                        throw new Exception("Error: Invalid Query Time-Step.");
                }
            }
            else
            {
                throw new Exception("Error: Invalid Query Dates.");
            }            

            var urlString = "?svr=" + svr
                + "&sdi=" + sdi
                + "&tstp=" + tstpString
                + "&t1=" + t1Input.ToString("yyyy-MM-ddTHH\\:mm")
                + "&t2=" + t2Input.ToString("yyyy-MM-ddTHH\\:mm")
                + "&table=" + table.ToString()
                + "&mrid=" + mrid
                + "&format=" + format;
            List<string> result = cgiProcessor.get_cgi_data(db, urlString);

            var output = String.Join<string>(String.Empty, result);
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(output, System.Text.Encoding.UTF8, "text/html");

            try
            {
                db.Close();
                db.Dispose();
            }
            catch
            {

            }

            return response;
        }



    }
}
