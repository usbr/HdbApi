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
        /// Gets all IDs
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("sites")]
        public IHttpActionResult Get()
        {
            return Ok(new List<string> { "Test 1", "Test 2" });
        }

        /// <summary>
        /// Gets site given an ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("sites/{id:int}")]
        public IHttpActionResult Get(int id)
        {
            return Ok(id * id);
        }

        /// <summary>
        /// Gets site types
        /// </summary>
        /// <param name="type">all</param>
        /// <returns></returns>
        [HttpGet, Route("sites/type/{type=type}")]
        public IHttpActionResult Get(string type = "all")
        {
            return Ok();
        }
    }
}
