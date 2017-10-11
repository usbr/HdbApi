using System.Collections.Generic;
using System.Web.Http;
using System;

namespace HdbApi.Controllers
{
    /// <summary>
    /// Represents test controller that should be removed.
    /// </summary>
    public class SeriesController : ApiController
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpGet, Route("series/{input=input}")]
        public IHttpActionResult Get(TimeSeriesQuery input)
        {
            return Ok(new TimeSeriesList());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost, Route("series/{args=args}")]
        public IHttpActionResult Post(TimeSeriesQuery args)
        {
            return Ok(new TimeSeriesList());
        }

        public class TimeSeriesList
        {
            public List<TimeSeries> timeseries { get; set; }
        }

        /// <summary>
        /// Model for querying TS data
        /// </summary>
        public class TimeSeriesQuery
        {
            public string hdb { get; set; }
            public int sdi { get; set; }
            public string interval { get; set; }
            public DateTime t1 { get; set; }
            public DateTime t2 { get; set; }
            public string format { get; set; }
            public string table { get; set; } = "r";
            public int mrid { get; set; } = 0;
        }

        /// <summary>
        /// Model for querying TS data
        /// </summary>
        public class TimeSeries
        {
            public TimeSeriesQuery metadata { get; set; }
            public TimeSeriesData data { get; set; }
        }

        /// <summary>
        /// Model for TS data
        /// </summary>
        public class TimeSeriesData
        {
            public DateTime datetime { get; set; }
            public double value { get; set; }
            public string flag { get; set; }
        }
    }
}
