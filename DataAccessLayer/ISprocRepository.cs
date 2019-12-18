using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HdbApi.Models;
using Dapper;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace HdbApi.DataAccessLayer
{
    /// <summary>
    /// Processors for SPROC objects
    /// </summary>
    internal interface ISprocRepository
    {
        List<dynamic> GetData(IDbConnection db);
    }

    
    public class SprocRepository : ISprocRepository
    {
        //private System.Data.IDbConnection db = HdbApi.App_Code.DbConnect.Connect();

        public List<dynamic> GetData(IDbConnection db)
        {
            //working sproc with refcursor
            var p = new OracleDynamicParameters();
            p.Add("@o_cursorOutput", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            p.Add("@i_sdiList", "2100,2101", dbType: OracleDbType.Varchar2);
            var sprocCommand = new CommandDefinition("GET_HDB_CGI_INFO", p, commandType: CommandType.StoredProcedure);
            var sprocResult = (List<dynamic>)db.Query<dynamic>(sprocCommand);

            //working function with return using select from dual
            var fName = "HDB_UTILITIES.IS_SDI_IN_ACL";
            var fInputs = "2101";
            var sql = "Select " + fName + "(" + fInputs + ") from dual";
            var funcResultDual = (List<dynamic>)db.Query<dynamic>(sql,commandType: CommandType.Text);

            //working function with return 
            p = new OracleDynamicParameters();
            p.Add("@IS_SDI_IN_ACL", dbType: OracleDbType.Varchar2, direction: ParameterDirection.ReturnValue);
            p.Add("@P_SITE_DATATYPE_ID", 2101, dbType: OracleDbType.Decimal);
            var funcCommand = new CommandDefinition("HDB_UTILITIES.IS_SDI_IN_ACL", p, commandType: CommandType.StoredProcedure);
            var funcResult = (List<dynamic>)db.Query<dynamic>(funcCommand);

            //test passing in sql command
            var sqlString = "select * from hdb_site";
            var sqlStringResult = (List<dynamic>)db.Query<dynamic>(sqlString, commandType: CommandType.Text);

            return new List<dynamic>();
        }


        public List<dynamic> RunSqlSelect(IDbConnection db, string sqlStatement)
        {
            var badStrings = new[] { "UPDATE", "DELETE", "INSERT", "CREATE", "ALTER", "DROP" };
            if (badStrings.Any(sqlStatement.ToUpper().Contains))
            {
                throw new Exception("Users may only run select statements using this API end-point. ");
            }
            else
            {
                var sqlStringResult = (List<dynamic>)db.Query<dynamic>(sqlStatement, commandType: CommandType.Text);
                return sqlStringResult;
            }            
        }

    }
}
