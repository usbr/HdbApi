using System.Collections.Generic;
using System.Web.Http;

namespace HdbApi.Controllers
{
    /// <summary>
    /// Represents test controller that should be removed.
    /// </summary>
    public class HdbsController : ApiController
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
