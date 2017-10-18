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
            IDbConnection db = new OracleConnection(System.Configuration.ConfigurationManager.ConnectionStrings["AppUserConnection"].ConnectionString);
            string sqlString = "select * from HDB_DATATYPE order by DATATYPE_ID";
            var dtypes = (List<Models.DatatypeModel.HdbDatatype>)db.Query<Models.DatatypeModel.HdbDatatype>(sqlString);
            return Ok(dtypes);
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
                    allowable_intervals = "non-instant",
                    agen_id = 0,
                    cmmnt = ""
                };
                return site;
            }
        }

    }
}
