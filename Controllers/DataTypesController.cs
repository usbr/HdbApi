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
        /// Gets all Datatypes
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("datatypes")]
        public IHttpActionResult Get()
        {
            return Ok(new List<string> {"Test 1", "Test 2"});
        }

        /// <summary>
        /// Gets IDs given and ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("datatypes/{id=id}")]
        public IHttpActionResult Get(int id)
        {
            return Ok(id * id);
        }

        /// <summary>
        /// Gets dataypes by units
        /// </summary>
        /// <param name="units"></param>
        /// <returns></returns>
        [HttpGet, Route("datatypes/{units=units}")]
        public IHttpActionResult Get(string units)
        {
            return Ok(units.ToString());
        }
    }
}
