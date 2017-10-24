using System;
using System.Net;
using System.Web.Http;
using Swashbuckle.Swagger.Annotations;
using Swashbuckle.Examples;
using System.Data;

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
        [SwaggerOperation(Tags = new[] { "HDB Tables" })]
        public IHttpActionResult Get([FromUri] int[] id = null)
        {
            IDbConnection db = HdbController.Connect(this.Request.Headers);
            var siteProcessor = new HdbApi.DataAccessLayer.SiteRepository();
            return Ok(siteProcessor.GetSites(db, id));
        }

        /// <summary>
        /// Delete Site
        /// </summary>
        /// <remarks>
        /// Delete specified Site 
        /// </remarks>
        /// <param name="id">HDB Site ID</param>
        /// <returns></returns>
        [HttpDelete, Route("sites/")]
        [SwaggerOperation(Tags = new[] { "HDB Tables" })]
        public IHttpActionResult Delete([FromUri] int id)
        {
            IDbConnection db = HdbController.Connect(this.Request.Headers);
            var siteProcessor = new HdbApi.DataAccessLayer.SiteRepository();
            return Ok(siteProcessor.DeleteSite(db, id));
        }

        /// <summary>
        /// Update Site
        /// </summary>
        /// <remarks>
        /// Update a fully defined Site 
        /// </remarks>
        /// <param name="site">HDB Site</param>
        /// <returns></returns>
        [HttpPatch, Route("sites/")]
        [SwaggerOperation(Tags = new[] { "HDB Tables" })]
        public IHttpActionResult Patch([FromBody] Models.SiteModel.HdbSite site)
        {
            IDbConnection db = HdbController.Connect(this.Request.Headers);
            var siteProcessor = new HdbApi.DataAccessLayer.SiteRepository();
            return Ok(siteProcessor.UpdateSite(db, site));
        }

        /// <summary>
        /// Add Site
        /// </summary>
        /// <remarks>
        /// Add a fully defined Site 
        /// </remarks>
        /// <param name="site">HDB Site</param>
        /// <returns></returns>
        [HttpPut, Route("sites/")]
        [SwaggerOperation(Tags = new[] { "HDB Tables" })]
        public IHttpActionResult Put([FromBody] Models.SiteModel.HdbSite site)
        {
            IDbConnection db = HdbController.Connect(this.Request.Headers);
            var siteProcessor = new HdbApi.DataAccessLayer.SiteRepository();
            return Ok(siteProcessor.InsertSite(db, site));
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
                };
                return site;
            }
        }

    }
}
