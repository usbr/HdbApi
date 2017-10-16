using System.Collections.Generic;
using System.Web.Http;
using System;
using System.Net;
using Swashbuckle.Swagger.Annotations;
using Swashbuckle.Examples;

namespace HdbApi.Controllers
{
    /// <summary>
    /// Represents test controller that should be removed.
    /// </summary>
    public class SeriesController : ApiController
    {

        /// <summary>
        /// Query Time-Series Data
        /// </summary>
        /// <remarks>
        /// Gets Time-Series Data given certain input filters
        /// </remarks>
        /// <param name="hdb">HDB DB Name</param>
        /// <param name="sdi">Integer Array</param>
        /// <param name="interval">Time-Series Interval</param>
        /// <param name="t1">Start Date in MM-DD-YYYY HH:MM</param>
        /// <param name="t2">End Date in MM-DD-YYYY HH:MM</param>
        /// <param name="format">Optional</param>
        /// <param name="table">Optional - M for Modeled Data</param>
        /// <param name="mrid">Required if table=M</param>
        /// <returns></returns>
        /// <response code="200"></response>
        [HttpGet, Route("series/")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(TimeSeriesList))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(TimeSeriesListExample))]
        public IHttpActionResult Get([FromUri] string hdb, [FromUri] int[] sdi, [FromUri] string interval, [FromUri] DateTime t1, [FromUri] DateTime t2, [FromUri] string format = "json", [FromUri] string table = "R", [FromUri] int mrid = 0)
        {
            return Ok(new TimeSeriesList());
        }

        /// <summary>
        /// Query Time-Series Data
        /// </summary>
        /// <remarks>
        /// Gets Time-Series Data given certain input filters
        /// </remarks>
        /// <param name="input">HDB Time-Series Query Input Array</param>
        /// <returns></returns>
        /// <response code="200"></response>
        [HttpPost, Route("series/{input=input}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<TimeSeriesList>))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(TimeSeriesListExample))]
        public IHttpActionResult Post(TimeSeriesQuery input)
        {
            return Ok(new TimeSeriesList());
        }

        /// <summary>
        /// Array of Time-Series Objects
        /// </summary>
        public class TimeSeriesList
        {
            /// <summary>
            /// Time-Series Object
            /// </summary>
            public List<TimeSeries> timeseries { get; set; }
        }

        /// <summary>
        /// Time-Series Object
        /// </summary>
        public class TimeSeries
        {
            /// <summary>
            /// Object Metadata
            /// </summary>
            public TimeSeriesQuery metadata { get; set; }
            /// <summary>
            /// Object Time-Series Data
            /// </summary>
            public List<TimeSeriesPoint> data { get; set; }
        }

        /// <summary>
        /// Model for querying TS data
        /// </summary>
        public class TimeSeriesQuery
        {
            /// <summary>
            /// HDB Instance Name
            /// </summary>
            public string hdb { get; set; }
            /// <summary>
            /// Array of SDIs to query
            /// </summary>
            public int[] sdi { get; set; }
            /// <summary>
            /// Time-Series Time-Interval 
            /// </summary>
            public string interval { get; set; }
            /// <summary>
            /// Start Date
            /// </summary>
            public DateTime t1 { get; set; }
            /// <summary>
            /// End Date
            /// </summary>
            public DateTime t2 { get; set; }
            /// <summary>
            /// Output Format
            /// </summary>
            public string format { get; set; } = "json";
            /// <summary>
            /// HDB Table
            /// </summary>
            public string table { get; set; } = "r";
            /// <summary>
            /// Model Run ID
            /// </summary>
            public int mrid { get; set; } = 0;
        }

        /// <summary>
        /// Object Time-Series Data
        /// </summary>
        public class TimeSeriesData
        {
            /// <summary>
            /// Date-Time
            /// </summary>
            public List<TimeSeriesPoint> data{ get; set; }
        }

        /// <summary>
        /// Object Time-Series Point
        /// </summary>
        public class TimeSeriesPoint
        {
            /// <summary>
            /// Date-Time
            /// </summary>
            public DateTime datetime { get; set; }
            /// <summary>
            /// Value
            /// </summary>
            public double value { get; set; }
            /// <summary>
            /// Flag
            /// </summary>
            public string flag { get; set; }
        }

        public class TimeSeriesListExample : IExamplesProvider
        {
            public object GetExamples()
            {
                var tsMeta = new TimeSeriesQuery
                {
                    hdb = "lchdb2",
                    sdi = (new List<int> { 1980 }).ToArray(),
                    t1 = new DateTime(2000, 1, 1, 0, 0, 0),
                    t2 = new DateTime(2000, 1, 2, 0, 0, 0),
                    interval = "day",
                    format = "json",
                    table = "r",
                    mrid = 0
                };
                var tsPoint1 = new TimeSeriesPoint
                {
                    datetime = new DateTime(2000, 1, 1, 0, 0, 0),
                    value = 3.1416,
                    flag = ""
                };
                var tsPoint2 = new TimeSeriesPoint
                {
                    datetime = new DateTime(2000, 1, 2, 0, 0, 0),
                    value = 2.7183,
                    flag = ""
                };
                var ts = new TimeSeries
                {
                    metadata = tsMeta
                };
                var tsData = new List<TimeSeriesPoint>
                {
                    tsPoint1, tsPoint2
                };
                ts.data = tsData;
                var tsList = new TimeSeriesList();
                var tsItems = new List<TimeSeries>
                {
                    ts
                };
                tsList.timeseries = tsItems;
                return tsList;
            }
        }
    }
}
