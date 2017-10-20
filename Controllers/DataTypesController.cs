using System.Net;
using System.Web.Http;
using Swashbuckle.Swagger.Annotations;
using Swashbuckle.Examples;

namespace HdbApi.Controllers
{
    public class DataTypesController : ApiController
    {
        /// <summary>
        /// Get DataType(s)
        /// </summary>
        /// <remarks>
        /// Get metadata for available DataType(s) 
        /// </remarks>
        /// <param name="id">(Optional) HDB DataType ID(s) of interest. Blank for all DataTypes</param>
        /// <returns></returns>
        [HttpGet, Route("datatypes/")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Models.DatatypeModel.HdbDatatype))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(DatatypeExample))]
        public IHttpActionResult Get([FromUri] int[] id = null)
        {
            var dtypeProcessor = new HdbApi.DataAccessLayer.DataTypeRepository();
            return Ok(dtypeProcessor.GetDataTypes(id));
        }

        /// <summary>
        /// Delete DataType
        /// </summary>
        /// <remarks>
        /// Delete specified DataType 
        /// </remarks>
        /// <param name="id">HDB Datatype ID</param>
        /// <returns></returns>
        [HttpDelete, Route("datatypes/")]
        public IHttpActionResult Delete([FromUri] int id)
        {
            var dtypeProcessor = new HdbApi.DataAccessLayer.DataTypeRepository();
            return Ok(dtypeProcessor.DeleteDataType(id));
        }

        /// <summary>
        /// Update DataType
        /// </summary>
        /// <remarks>
        /// Update a fully defined DataType 
        /// </remarks>
        /// <param name="dtype">HDB DataType</param>
        /// <returns></returns>
        [HttpPatch, Route("datatypes/")]
        public IHttpActionResult Patch([FromBody] Models.DatatypeModel.HdbDatatype dtype)
        {
            var dtypeProcessor = new HdbApi.DataAccessLayer.DataTypeRepository();
            return Ok(dtypeProcessor.UpdateDataType(dtype));
        }

        /// <summary>
        /// Add DataType
        /// </summary>
        /// <remarks>
        /// Add a fully defined DataType 
        /// </remarks>
        /// <param name="dtype">HDB DataType</param>
        /// <returns></returns>
        [HttpPut, Route("datatypes/")]
        public IHttpActionResult Put([FromBody] Models.DatatypeModel.HdbDatatype dtype)
        {
            var dtypeProcessor = new HdbApi.DataAccessLayer.DataTypeRepository();
            return Ok(dtypeProcessor.InsertDataType(dtype));
        }


        public class DatatypeExample : IExamplesProvider
        {
            public object GetExamples()
            {
                var site = new Models.DatatypeModel.HdbDatatype
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
                };
                return site;
            }
        }

    }
}
