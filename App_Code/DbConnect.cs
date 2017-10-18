using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace HdbApi.Code
{
    public class DbConnect
    {
        public static IDbConnection Connect()
        {
            IDbConnection db = new OracleConnection(System.Configuration.ConfigurationManager.ConnectionStrings["AppUserConnection"].ConnectionString);
            return db;
        }
    }
}