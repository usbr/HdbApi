using System;
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

        /// <summary>
        /// Perform a SQL-SELECT Query
        /// </summary>
        /// <remarks>
        /// Run a SQL statement to perform a SQL-SELECT operation
        /// </remarks>
        /// <param name="svr"></param>
        /// <param name="sqlStatement"></param>
        /// <returns></returns>
        [HttpGet, Route("select/")]
        [SwaggerOperation(Tags = new[] { "HDB Tables" })]
        public IHttpActionResult Get([FromUri] string svr, [FromUri] string sqlStatement)
        {
            var cgiProcessor = new HdbApi.DataAccessLayer.CgiRepository();

            // Connect to HDB
            bool hostFound = false;
            List<string[]> hostList = cgiProcessor.get_host_list();
            foreach (string[] hostInfo in hostList)
            {
                if (svr == hostInfo[0].ToString())
                {
                    hostFound = true;
                    this.Request.Headers.Add("api_hdb", hostInfo[0]);
                    this.Request.Headers.Add("api_user", hostInfo[4]);
                    this.Request.Headers.Add("api_pass", hostInfo[5]);
                }
            }
            if (!hostFound)
            {
                throw new Exception("HDB Database not recognized.");
            }
            IDbConnection db = HdbController.Connect(this.Request.Headers);

            var sprocProcessor = new HdbApi.DataAccessLayer.SprocRepository();
            var retVal = sprocProcessor.RunSqlSelect(db, sqlStatement);
            db.Dispose();
            try
            {
                db.Close();
            }
            catch
            {

            }
            return Ok(retVal);
        }

    }
}
