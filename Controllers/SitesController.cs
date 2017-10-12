using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HdbApi.Controllers
{
    public class SitesController : ApiController
    {
        /// <summary>
        /// Get Site Types
        /// </summary>
        /// <remarks>
        /// Get a list of available Site Types
        /// </remarks>
        /// <returns></returns>
        [HttpGet, Route("sites")]
        public IHttpActionResult Get()
        {
            return Ok(new List<string> { "Test 1", "Test 2" });
        }

        /// <summary>
        /// Get Sites Given Site Type
        /// </summary>
        /// <remarks>
        /// Get a list of available Sites under the input site type
        /// </remarks>
        /// <param name="type">all</param>
        /// <returns></returns>
        [HttpGet, Route("sites/type/{type=type}")]
        public IHttpActionResult Get(string type = "all")
        {
            return Ok();
        }

        /// <summary>
        /// Get Site by ID
        /// </summary>
        /// <remarks>
        /// Get a single Site given a Site ID
        /// </remarks>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("sites/{id:int}")]
        public IHttpActionResult Get(int id)
        {
            return Ok(id * id);
        }
    }
}
