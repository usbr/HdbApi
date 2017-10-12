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
            return Ok("Querying Site Types");
        }

        /// <summary>
        /// Get Sites by Site Type
        /// </summary>
        /// <remarks>
        /// Get a list of available Sites under a particular site type
        /// </remarks>
        /// <param name="type">all</param>
        /// <returns></returns>
        [HttpGet, Route("sites/type/{type=type}")]
        public IHttpActionResult Get(string type)
        {
            return Ok("Querying Site Type = " + type);
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
            return Ok("Querying Site ID = " + id);
        }

        /// <summary>
        /// Get Sites by IDs
        /// </summary>
        /// <remarks>
        /// Get a list of Sites given Site IDs
        /// </remarks>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("sites/{id:int}")]
        public IHttpActionResult Get(List<string> ids)
        {
            return Ok("Querying Site IDs = " + ids.ToArray());
        }
    }
}
