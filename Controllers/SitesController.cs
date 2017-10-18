using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Dapper;
using Swashbuckle.Swagger.Annotations;
using Swashbuckle.Examples;

namespace HdbApi.Controllers
{
    public class SitesController : ApiController
    {
        /// <summary>
        /// Get Site(s)
        /// </summary>
        /// <remarks>
        /// Get metadata for available Site(s) 
        /// </remarks>
        /// <param name="id">(Optional) HDB Site ID(s) of interest. Blank for all Sites</param>
        /// <returns></returns>
        [HttpGet, Route("sites/")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Models.SiteModel.HdbSite))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(SiteExample))]
        public IHttpActionResult Get([FromUri] int[] id = null)
        {
            IDbConnection db = new OracleConnection(System.Configuration.ConfigurationManager.ConnectionStrings["AppUserConnection"].ConnectionString);
            string sqlString = "select * from HDB_SITE order by SITE_ID";
            var sites = (List<Models.SiteModel.HdbSite>)db.Query<Models.SiteModel.HdbSite>(sqlString);
            return Ok(sites);
        }


        public class SiteExample : IExamplesProvider
        {
            public object GetExamples()
            {
                var site = new Models.SiteModel.HdbSite
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
                    basin_id = 2029,
                    hydrologic_unit = null,
                    river_mile = float.NaN,
                    segment_no = 0,
                    state_id = 3,
                    usgs_id = DBNull.Value.ToString(),
                    nws_code = DBNull.Value.ToString(),
                    shef_code = DBNull.Value.ToString(),
                    scs_id = DBNull.Value.ToString(),
                    parent_objecttype_id = 0,
                    parent_site_id = 0
                };
                return site;
            }
        }

    }
}
