using System.Collections.Generic;
using System.Web.Http;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using Dapper;
using Swashbuckle.Swagger.Annotations;

namespace HdbApi.Controllers
{
    /// <summary>
    /// Represents test controller that should be removed.
    /// </summary>
    public class TestController : ApiController
    {
        [HttpGet, Route("tests/")]
        [SwaggerOperation(Tags = new[] { "Testing Sandbox" })]
        public IHttpActionResult Get()
        {
            IDbConnection db = HdbController.Connect(this.Request.Headers);
            var sprocProcessor = new HdbApi.DataAccessLayer.SprocRepository();
            return Ok(sprocProcessor.GetData(db));
        }

    }
}
