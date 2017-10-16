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
        /// Get Site(s)
        /// </summary>
        /// <remarks>
        /// Get metadata for available Site(s) 
        /// </remarks>
        /// <param name="id">(Optional) HDB Site ID(s) of interest. Blank for all Sites</param>
        /// <returns></returns>
        [HttpGet, Route("sites/")]
        public IHttpActionResult Get([FromUri] int[] id = null)
        {
            return Ok("Querying Site Types");
        }
    }
}
