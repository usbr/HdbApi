using System.Collections.Generic;
using System.Web.Http;
using System;
using System.Net;
using Swashbuckle.Swagger.Annotations;
using Swashbuckle.Examples;

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
        /// <param name="hdb">HDB DB Name</param>
        /// <param name="sdi">Site Datatype ID</param>
        /// <param name="interval">Time-Series Interval</param>
        /// <param name="t1">Start Date in MM-DD-YYYY HH:MM</param>
        /// <param name="t2">End Date in MM-DD-YYYY HH:MM</param>
        /// <param name="format">Optional</param>
        /// <param name="table">Optional - M for Modeled Data</param>
        /// <param name="mrid">Required if table=M</param>
        /// <returns></returns>
        /// <response code="200"></response>
        [HttpGet, Route("series/")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Models.SeriesModel.TimeSeries))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(TimeSeriesExample))]
        public IHttpActionResult Get([FromUri] string hdb, [FromUri] int sdi, [FromUri] string interval, [FromUri] DateTime t1, [FromUri] DateTime t2, [FromUri] string format = "json", [FromUri] string table = "R", [FromUri] int mrid = 0)
        {
            return Ok(new Models.SeriesModel.TimeSeries());
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
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Models.SeriesModel.TimeSeries))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(TimeSeriesExample))]
        public IHttpActionResult Post(Models.SeriesModel.TimeSeriesQuery input)
        {
            return Ok(new Models.SeriesModel.TimeSeries());
        }

        

        public class TimeSeriesExample : IExamplesProvider
        {
            public object GetExamples()
            {
                var tsMeta = new Models.SeriesModel.TimeSeriesQuery
                {
                    hdb = "lchdb2",
                    sdi = 1980,
                    t1 = new DateTime(2000, 1, 1, 0, 0, 0),
                    t2 = new DateTime(2000, 1, 2, 0, 0, 0),
                    interval = "day",
                    format = "json",
                    table = "r",
                    mrid = 0
                };
                var tsPoint1 = new Models.SeriesModel.TimeSeriesPoint
                {
                    datetime = new DateTime(2000, 1, 1, 0, 0, 0),
                    value = 3.1416,
                    flag = ""
                };
                var tsPoint2 = new Models.SeriesModel.TimeSeriesPoint
                {
                    datetime = new DateTime(2000, 1, 2, 0, 0, 0),
                    value = 2.7183,
                    flag = ""
                };
                var ts = new Models.SeriesModel.TimeSeries
                {
                    metadata = tsMeta
                };
                var tsData = new List<Models.SeriesModel.TimeSeriesPoint>
                {
                    tsPoint1, tsPoint2
                };
                ts.data = tsData;
                var tsList = new Models.SeriesModel.TimeSeries();
                var tsItems = new List<Models.SeriesModel.TimeSeries>
                {
                    ts
                };
                return ts;
            }
        }

    }
}
