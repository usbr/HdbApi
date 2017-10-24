using System.Collections.Generic;
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
        /// <param name="table">Optional - M for Modeled Data</param>
        /// <param name="mrid">Required if table=M</param>
        /// <returns></returns>
        /// <response code="200"></response>
        [HttpGet, Route("series/")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Models.SeriesModel.TimeSeries))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(TimeSeriesExample))]
        public IHttpActionResult Get([FromUri] int sdi, [FromUri] string interval, [FromUri] DateTime t1, [FromUri] DateTime t2, [FromUri] string table = "R", [FromUri] int mrid = 0)
        {
            IDbConnection db = HdbController.Connect(this.Request.Headers);
            var seriesProcessor = new HdbApi.DataAccessLayer.SeriesRepository();
            return Ok(seriesProcessor.GetSeries(db, sdi, interval, t1, t2, table, mrid)); ;
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
        [HttpPost, Route("point/r-write/{input=input}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Models.PointModel.ObservedPoint))]
        public IHttpActionResult Post([FromBody] Models.PointModel.ObservedPoint input)
        {
            IDbConnection db = HdbController.Connect(this.Request.Headers);
            return Ok(new Models.PointModel.ObservedPoint());
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
        [HttpPost, Route("point/m-write/{input=input}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Models.PointModel.ModeledPoint))]
        public IHttpActionResult Post([FromBody] Models.PointModel.ModeledPoint input)
        {
            IDbConnection db = HdbController.Connect(this.Request.Headers);
            return Ok(new Models.PointModel.ModeledPoint());
        }



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
                    value = 3.1416,
                    flag = ""
                };
                var tsPoint2 = new Models.SeriesModel.TimeSeriesPoint
                {
                    datetime = new DateTime(2000, 1, 2, 0, 0, 0),
                    value = 2.7183,
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

    }
}
