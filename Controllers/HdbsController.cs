using System.Data;
using System.Collections.Generic;
using System.Web.Http;
using Dapper;
using Oracle.ManagedDataAccess.Client;

namespace HdbApi.Controllers
{
    /// <summary>
    /// Represents test controller that should be removed.
    /// </summary>
    public class HdbController : ApiController
    {
        /// <summary>
        /// Gets available HDBs
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("hdb")]
        public IHttpActionResult Get()
        {
            return Ok(new List<string> { "Test 1", "Test 2" });
        }

        /// <summary>
        /// Connect to HDB
        /// </summary>
        /// <remarks>
        /// Connect to HDB and authenticate credentials
        /// </remarks>
        /// <param name="hdb">HDB instance to connect to</param>
        /// <param name="username">User name given HDB</param>
        /// <param name="password">User credentials given HDB and User Name</param>
        /// <returns></returns>
        [HttpPost, Route("session/")]
        public IHttpActionResult Post(string hdb = "", string username = "", string password = "")//, HdbCredentials connectionDetails = null)
        {
            HdbCredentials connectionDetails = new HdbCredentials
            {
                HdbInstance = hdb,
                UserName = username,
                Password = password
            };
            return Ok(connectionDetails);
        }

        public static IDbConnection Connect(System.Net.Http.Headers.HttpRequestHeaders apiRequest)
        {
            string hdbKey, userKey, passKey;
            if (apiRequest.Contains("api_hdb") && apiRequest.Contains("api_user") && apiRequest.Contains("api_pass"))
            {
                hdbKey = apiRequest.GetValues("api_hdb").AsList<string>()[0];
                userKey = apiRequest.GetValues("api_user").AsList<string>()[0];
                passKey = apiRequest.GetValues("api_pass").AsList<string>()[0];
            }
            else
            {
                throw new KeyNotFoundException("HTTP Request Header Keys missing. Refer to the API guide for proper formatting of the API Request...");
            }

            // Log-in
            System.Data.IDbConnection db = new OracleConnection("Data Source=" + hdbKey + ";User Id=" + userKey + ";Password=" + passKey + ";");

            // Check ref_user_groups
            //string sqlString = "select * from ref_user_groups where lower(user_name) = '" + userKey + "'";

            return db;
        }

        /// <summary>
        /// Model for passing connection details to connect
        /// </summary>
        public class HdbCredentials
        {
            /// <summary>
            /// HDB Instance to connect to
            /// </summary>
            public string HdbInstance { get; set; }
            /// <summary>
            /// HDB Instance User Name
            /// </summary>
            public string UserName { get; set; }
            /// <summary>
            /// HDB Instance Password
            /// </summary>
            public string Password { get; set; }
        }
    }
}
