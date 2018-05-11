﻿using System.Collections.Generic;
using System.Web.Http;
using System;
using System.Net;
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
        public IHttpActionResult Get([FromUri] int sdi, [FromUri] DateTime t1, [FromUri] DateTime t2, [FromUri] IntervalType interval = new IntervalType(), [FromUri] bool rbase = false, [FromUri] TableType table = new TableType(), [FromUri] int mrid = 0, [FromUri] string instantMinutes = "60")
        {
            IDbConnection db = HdbController.Connect(this.Request.Headers);
            var seriesProcessor = new HdbApi.DataAccessLayer.SeriesRepository();
            return Ok(seriesProcessor.GetSeries(db, sdi, interval.ToString(), t1, t2, table.ToString(), mrid, rbase, instantMinutes)); ;
        }


        public enum TableType
        {
            R = 0,
            M = 1
        }


        public enum IntervalType
        {
            month = 0,
            day = 1,
            hour = 2,
            instant = 3
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
                var result = hdbProcessor.modify_r_base_raw(db,point.site_datatype_id,point.interval,point.start_date_time,point.value,point.overwrite_flag,point.validation,point.do_update_y_or_n);
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
                    sdi = 1980,
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
                        site_id = 919,
                        site_name = "LAKE POWELL",
                        site_common_name = "LAKE POWELL",
                        description = DBNull.Value.ToString(),
                        elevation = 3700,
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
                        datatype_id = 1393,
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
        /// <param name="table">Optional - HDB Table {R, M}</param>
        /// <param name="mrid">Optional - Model Run ID if table=M</param>
        /// <param name="format">Output format</param>
        /// <returns></returns>
        [HttpGet, Route("cgi")]
        [SwaggerOperation(Tags = new[] { "HDB TimeSeries Data" })]
        public System.Net.Http.HttpResponseMessage Get([FromUri] string svr, [FromUri] string sdi, [FromUri] System.DateTime t1, [FromUri] System.DateTime t2, [FromUri] string tstp = "DY", [FromUri] TableType table = new TableType(), [FromUri] int mrid = 0, [FromUri] string format = "1")
        {
            var cgiProcessor = new HdbApi.DataAccessLayer.CgiRepository();
            
            // Connect to HDB
            bool hostFound = false;
            string user, pass;
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
            IDbConnection db = HdbController.Connect(this.Request.Headers);

            // Build CGI query URL
            //      ?svr=lchdb2&sdi=25401&tstp=IN&t1=2018-05-07T05:00&t2=2018-05-07T08:00&table=R&mrid=&format=2            
            var tstpString = "";
            switch (tstp.ToString().ToLower())
            {
                case "instant":
                case "in":
                    tstpString = "IN";
                    break;
                case "month":
                case "mn":
                    tstpString = "MN";
                    break;
                case "hour":
                case "hr":
                    tstpString = "HR";
                    break;
                default:
                    tstpString = "DY";
                    break;
            }

            var urlString = "?svr=" + svr
                + "&sdi=" + sdi
                + "&tstp=" + tstpString
                + "&t1=" + t1.ToString("yyyy-MM-ddTHH\\:mm")
                + "&t2=" + t2.ToString("yyyy-MM-ddTHH\\:mm")
                + "&table=" + table.ToString()
                + "&mrid=" + mrid
                + "&format=" + format;
            var result = cgiProcessor.get_cgi_data(db, urlString);

            var output = String.Join<string>(String.Empty, result);
            var resp = new System.Net.Http.HttpResponseMessage(HttpStatusCode.OK);
            resp.Content = new System.Net.Http.StringContent(output, System.Text.Encoding.UTF8, "text/html");
            return resp;
        }



    }
}
