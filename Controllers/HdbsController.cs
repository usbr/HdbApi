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
        /// Gets all available HDBs
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("hdb")]
        public IHttpActionResult Get()
        {
            return Ok(new List<string> {"Test 1", "Test 2"});
        }

        /// <summary>
        /// Gets IDs given and ID
        /// </summary>
        /// <param name="hdbinstance"></param>
        /// <returns></returns>
        [HttpGet, Route("hdb/{hdbinstance=hdbinstance}")]
        public IHttpActionResult Get(string hdbinstance)
        {
            return Ok(hdbinstance.ToString());
        }

        /// <summary>
        /// Gets IDs given and ID
        /// </summary>
        /// <param name="connectionDetails"></param>
        /// <returns></returns>
        [HttpPost, Route("hdb/connect")]
        public IHttpActionResult Post(HdbCredentials connectionDetails)
        {
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
