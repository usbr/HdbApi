using System.Net;
using System.Web.Http;
using Swashbuckle.Swagger.Annotations;
using Swashbuckle.Examples;
using System.Data;
using System.Collections.Generic;

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
        /// <param name="sid">(Optional) HDB Site ID(s) of interest. Blank for all Sites</param>
        /// <param name="did">(Optional) HDB DataType ID(s) of interest. Blank for all DataTypes</param>
        /// <returns></returns>
        [HttpGet, Route("sitedatatypes/")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Models.SiteDatatypeModel.HdbSiteDatatype))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(SiteDatatypeExample))]
        [SwaggerOperation(Tags = new[] { "HDB Tables" })]
        public IHttpActionResult Get([FromUri] string[] sdi = null, [FromUri] string[] sid = null, [FromUri] string[] did = null)
        {
            IDbConnection db = HdbController.Connect(this.Request.Headers);
            var sdiProcessor = new HdbApi.DataAccessLayer.SiteDataTypeRepository();
            var result = sdiProcessor.GetSiteDataTypes(db, sdi, sid, did);

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

        /// <summary>
        /// Get SiteDataType(s)
        /// </summary>
        /// <remarks>
        /// Get metadata for available SiteDataType(s) 
        /// </remarks>
        /// <param name="sdi">(Optional) HDB SiteDataType ID(s) of interest. Blank for all SiteDataTypes</param>
        /// <param name="sid">(Optional) HDB Site ID(s) of interest. Blank for all Sites</param>
        /// <param name="did">(Optional) HDB DataType ID(s) of interest. Blank for all DataTypes</param>
        /// <returns></returns>
        [HttpPost, Route("sitedatatypes/")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Models.SiteDatatypeModel.HdbSiteDatatype))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(SiteDatatypeExample))]
        [SwaggerOperation(Tags = new[] { "HDB Tables" })]
        public IHttpActionResult Post([FromBody] List<dataTypeList.dataType> input)
        {
            IDbConnection db = HdbController.Connect(this.Request.Headers);
            var sdiProcessor = new HdbApi.DataAccessLayer.SiteDataTypeRepository();
            List<string> sdi = new List<string>();
            List<string> sid = new List<string>();
            List<string> did = new List<string>();
            foreach (dataTypeList.dataType dType in input)
            {
                //if (dType.sdi > 0)
                //{ sdi.Add(dType.sdi); }
                //if (dType.sid > 0)
                //{ sid.Add(dType.sid); }
                //if (dType.did > 0)
                //{ did.Add(dType.did); }
                sdi.Add(dType.sdi);
                sid.Add(dType.sid);
                did.Add(dType.did);
            }
            var result = sdiProcessor.GetSiteDataTypes(db, sdi.ToArray(), sid.ToArray(), did.ToArray());

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

        public class dataTypeList
        {
            public class dataType
            {
                public string sdi { get; set; }
                public string sid { get; set; }
                public string did { get; set; }
            }
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
                    datatype_id = "1393",
                    site_id = "919",
                    site_datatype_id = "2101"
                };
                return sdi;
            }
        }

    }
}
