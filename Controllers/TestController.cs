using System.Collections.Generic;
using System.Web.Http;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using Dapper;

namespace HdbApi.Controllers
{
    /// <summary>
    /// Represents test controller that should be removed.
    /// </summary>
    public class TestController : ApiController
    {
        [HttpGet, Route("tests")]
        public IHttpActionResult Get()
        {
            IDbConnection db = HdbController.Connect(this.Request.Headers);

            //// [JR] STOREDPROC CALL SAMPLE
            var p = new OracleDynamicParameters();
            p.Add("o_cursorOutput", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            p.Add("i_sdiList", value: 2101, dbType: OracleDbType.Varchar2);
            p.Add("i_tStep", value: "MONTH", dbType: OracleDbType.Varchar2);
            p.Add("i_startDate", value: (new System.DateTime(2000, 1, 1)).ToString("dd-MMM-yyyy"), dbType: OracleDbType.Varchar2);
            p.Add("i_endDate", value: (new System.DateTime(2005, 1, 1)).ToString("dd-MMM-yyyy"), dbType: OracleDbType.Varchar2);
            p.Add("i_sourceTable", value: "R", dbType: OracleDbType.Varchar2);
            p.Add("i_modelRunIds", value: 0, dbType: OracleDbType.Varchar2);
            var result = db.Query<dynamic>("GET_HDB_CGI_DATA", param: p, commandType: CommandType.StoredProcedure);


            return Ok(result);
        }

        [HttpGet, Route("tests/{id:int}")]
        public IHttpActionResult Get(int id)
        {
            return Ok(id * id);
        }
    }
}
