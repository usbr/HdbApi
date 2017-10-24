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
        /// List HDBs
        /// </summary>
        /// <remarks>
        /// List available HDB instances that the API can connect to
        /// </remarks>
        /// <returns></returns>
        [HttpGet, Route("hdb")]
        public IHttpActionResult Get()
        {
            return Ok(new List<string>
            {
                "LCHDB2 - LC Production HDB",
                "LCHDEV - LC Test HDB",
                "LCHDB - LC Oracle 12c Test HDB",
                "UCHDB2 - UC Production HDB",
                "YAOHDB - YAO Production HDB",
                "ECOHDB - ECAO Production HDB"
            });
        }

        /// <summary>
        /// Connect to HDB
        /// </summary>
        /// <remarks>
        /// Test connection to HDB and authenticate credentials
        /// </remarks>
        /// <param name="hdb">HDB instance to connect to</param>
        /// <param name="username">User name given HDB</param>
        /// <param name="password">User credentials given HDB and User Name</param>
        /// <returns></returns>
        [HttpGet, Route("connect/")]
        public IHttpActionResult Get([FromUri] string hdb, [FromUri]string username, [FromUri]string password)
        {
            var db = Connect(hdb, username, password);
            var result = db.Query<dynamic>("select * from all_users where username='" + username.ToUpper() + "'");
            return Ok(result);
        }


        /// <summary>
        /// Method to generate a DB Connection from HTTP Request
        /// </summary>
        /// <param name="apiRequest"></param>
        /// <returns></returns>
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
            //System.Data.IDbConnection db = new OracleConnection("Data Source=" + hdbKey + ";User Id=" + userKey + ";Password=" + passKey + ";");
            System.Data.IDbConnection db = Connect(hdbKey, userKey, passKey);

            // Check ref_user_groups
            //string sqlString = "select * from ref_user_groups where lower(user_name) = '" + userKey + "'";

            return db;
        }


        /// <summary>
        /// Overloaded method to generate a DB Connection from parse HTTP Request Headers
        /// </summary>
        /// <param name="hdb"></param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public static IDbConnection Connect(string hdb, string user, string pass)
        {
            // Log-in
            return new OracleConnection("Data Source=" + hdb + ";User Id=" + user + ";Password=" + pass + ";");
        }

        
    }
}
