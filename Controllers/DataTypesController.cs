using System.Collections.Generic;
using System.Web.Http;

namespace HdbApi.Controllers
{
    /// <summary>
    /// Represents test controller that should be removed.
    /// </summary>
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
        public IHttpActionResult Get([FromUri] int[] id = null)
        {
            return Ok("Querying Data Types");
        }
    }
}
