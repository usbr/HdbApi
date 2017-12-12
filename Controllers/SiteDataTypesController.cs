using System.Net;
using System.Web.Http;
using Swashbuckle.Swagger.Annotations;
using Swashbuckle.Examples;
using System.Data;

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
        /// <param name="sdi">(Optional) HDB SiteDataType ID(s) of interest. Blank for all SiteDataTypes</param>
        /// <returns></returns>
        [HttpGet, Route("sitedatatypes/")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Models.SiteDatatypeModel.HdbSiteDatatype))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(SiteDatatypeExample))]
        [SwaggerOperation(Tags = new[] { "HDB Tables" })]
        public IHttpActionResult Get([FromUri] int[] sdi = null)
        {
            IDbConnection db = HdbController.Connect(this.Request.Headers);
            var sdiProcessor = new HdbApi.DataAccessLayer.SiteDataTypeRepository();
            return Ok(sdiProcessor.GetSiteDataTypes(db, sdi));
        }

        /// <summary>
        /// Delete SiteDataType
        /// </summary>
        /// <remarks>
        /// Delete specified SiteDataType 
        /// </remarks>
        /// <param name="sdi">HDB SiteDataType ID</param>
        /// <returns></returns>
        //[HttpDelete, Route("sitedatatypes/")]
        //[SwaggerOperation(Tags = new[] { "HDB Tables" })]
        //public IHttpActionResult Delete([FromUri] int sdi)
        //{
        //    IDbConnection db = HdbController.Connect(this.Request.Headers);
        //    var sdiProcessor = new HdbApi.DataAccessLayer.SiteDataTypeRepository();
        //    return Ok(sdiProcessor.DeleteSiteDataType(db, sdi));
        //}

        /// <summary>
        /// Update SiteDataType
        /// </summary>
        /// <remarks>
        /// Update a fully defined SiteDataType 
        /// </remarks>
        /// <param name="sdi">HDB SiteDataType</param>
        /// <returns></returns>
        //[HttpPatch, Route("sitedatatypes/")]
        //[SwaggerOperation(Tags = new[] { "HDB Tables" })]
        //public IHttpActionResult Patch([FromBody] Models.SiteDatatypeModel.HdbSiteDatatype sdi)
        //{
        //    IDbConnection db = HdbController.Connect(this.Request.Headers);
        //    var sdiProcessor = new HdbApi.DataAccessLayer.SiteDataTypeRepository();
        //    return Ok(sdiProcessor.UpdateSiteDataType(db, sdi));
        //}

        /// <summary>
        /// Add SiteDataType
        /// </summary>
        /// <remarks>
        /// Add a fully defined SiteDataType 
        /// </remarks>
        /// <param name="sdi">HDB SiteDataType</param>
        /// <returns></returns>
        //[HttpPut, Route("sitedatatypes/")]
        //[SwaggerOperation(Tags = new[] { "HDB Tables" })]
        //public IHttpActionResult Put([FromBody] Models.SiteDatatypeModel.HdbSiteDatatype sdi)
        //{
        //    IDbConnection db = HdbController.Connect(this.Request.Headers);
        //    var sdiProcessor = new HdbApi.DataAccessLayer.SiteDataTypeRepository();
        //    return Ok(sdiProcessor.InsertSiteDataType(db, sdi));
        //}


        public class SiteDatatypeExample : IExamplesProvider
        {
            public object GetExamples()
            {
                var sdi = new Models.SiteDatatypeModel.HdbSiteDatatype
                {
                    datatype_id = 1393,
                    site_id = 919,
                    site_datatype_id = 2101
                };
                return sdi;
            }
        }

    }
}
