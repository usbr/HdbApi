using System.Net;
using System.Web.Http;
using Swashbuckle.Swagger.Annotations;
using Swashbuckle.Examples;

namespace HdbApi.Controllers
{
    public class SiteDataTypesController : ApiController
    {
        /// <summary>
        /// Get SiteDataType(s)
        /// </summary>
        /// <remarks>
        /// Get metadata for available SiteDataType(s) 
        /// </remarks>
        /// <param name="sdi">(Optional) HDB DataType ID(s) of interest. Blank for all DataTypes</param>
        /// <returns></returns>
        [HttpGet, Route("sitedatatypes/")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Models.SiteDatatypeModel.HdbSiteDatatype))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(SiteDatatypeExample))]
        public IHttpActionResult Get([FromUri] int[] sdi = null)
        {
            var sdiProcessor = new HdbApi.DataAccessLayer.SiteDataTypeRepository();
            return Ok(sdiProcessor.GetSiteDataTypes(sdi));
        }

        /// <summary>
        /// Delete SiteDataType
        /// </summary>
        /// <remarks>
        /// Delete specified SiteDataType 
        /// </remarks>
        /// <param name="sdi">HDB SiteDataType ID</param>
        /// <returns></returns>
        [HttpDelete, Route("sitedatatypes/")]
        public IHttpActionResult Delete([FromUri] int sdi)
        {
            var sdiProcessor = new HdbApi.DataAccessLayer.SiteDataTypeRepository();
            return Ok(sdiProcessor.DeleteSiteDataType(sdi));
        }

        /// <summary>
        /// Update SiteDataType
        /// </summary>
        /// <remarks>
        /// Update a fully defined SiteDataType 
        /// </remarks>
        /// <param name="sdi">HDB SiteDataType</param>
        /// <returns></returns>
        [HttpPatch, Route("sitedatatypes/")]
        public IHttpActionResult Patch([FromBody] Models.SiteDatatypeModel.HdbSiteDatatype sdi)
        {
            var sdiProcessor = new HdbApi.DataAccessLayer.SiteDataTypeRepository();
            return Ok(sdiProcessor.UpdateSiteDataType(sdi));
        }

        /// <summary>
        /// Add SiteDataType
        /// </summary>
        /// <remarks>
        /// Add a fully defined SiteDataType 
        /// </remarks>
        /// <param name="sdi">HDB SiteDataType</param>
        /// <returns></returns>
        [HttpPut, Route("sitedatatypes/")]
        public IHttpActionResult Put([FromBody] Models.SiteDatatypeModel.HdbSiteDatatype sdi)
        {
            var sdiProcessor = new HdbApi.DataAccessLayer.SiteDataTypeRepository();
            return Ok(sdiProcessor.InsertSiteDataType(sdi));
        }


        public class SiteDatatypeExample : IExamplesProvider
        {
            public object GetExamples()
            {
                var sdi = new Models.SiteDatatypeModel.HdbSiteDatatype
                {
                    datatype_id = 1393,
                    site_id = 919,
                    site_dataype_id = 2101
                };
                return sdi;
            }
        }

    }
}
