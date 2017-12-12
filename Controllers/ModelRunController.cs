using System;
using System.Net;
using System.Web.Http;
using Swashbuckle.Swagger.Annotations;
using Swashbuckle.Examples;
using System.Data;

namespace HdbApi.Controllers
{
    public class ModelRunController : ApiController
    {
        /// <summary>
        /// Get Model Run(s)
        /// </summary>
        /// <remarks>
        /// Get metadata for available Model Run(s) 
        /// </remarks>
        /// <param name="id">(Optional) HDB Model Run ID(s) of interest. Blank for all Model Runs</param>
        /// <param name="modelrunname">(Optional) HDB Model Run name of interest. Blank for all Model Runs</param>
        /// <returns></returns>
        [HttpGet, Route("modelruns/")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Models.ModelRunModel.HdbModelRun))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ModelRunExample))]
        [SwaggerOperation(Tags = new[] { "HDB Tables" })]
        public IHttpActionResult Get([FromUri] int[] id = null, [FromUri] string modelrunname = null)
        {
            IDbConnection db = HdbController.Connect(this.Request.Headers);
            var modelRunProcessor = new HdbApi.DataAccessLayer.ModelRunRepository();
            return Ok(modelRunProcessor.GetModelRun(db, id, modelrunname));
        }



        public class ModelRunExample : IExamplesProvider
        {
            public object GetExamples()
            {
                var modelRun = new Models.ModelRunModel.HdbModelRun
                {
                    model_run_id = 2,
                    model_run_name = "Official forecast from the Daily Operations Model (New BHOPS)",
                    date_time_loaded = new DateTime(2009, 5, 20, 0, 0, 0),
                    run_date = new DateTime(2012,2,24,0,0,0),
                    user_name = "jrocha",
                    model_run_cmmnt = "",
                    model_id = 6,
                    model_name = "Daily Water Operations Model (New BHOPS)",
                    model_cmmnt = "This model is run via PRSYM using simulation and generates daily interval data"
                };
                return modelRun;
            }
        }

    }
}
