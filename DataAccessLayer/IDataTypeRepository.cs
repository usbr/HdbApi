using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HdbApi.Models;
using Dapper;
using System.Data;

namespace HdbApi.DataAccessLayer
{
    /// <summary>
    /// Processors for Datatype objects
    /// </summary>
    internal interface IDataTypeRepository
    {
        List<DatatypeModel.HdbDatatype> GetDataTypes(IDbConnection db, int[] id);

        bool InsertDataType(IDbConnection db, DatatypeModel.HdbDatatype dtype);

        bool UpdateDataType(IDbConnection db, DatatypeModel.HdbDatatype dtype);

        bool DeleteDataType(IDbConnection db, int id);
    }

    
    public class DataTypeRepository : IDataTypeRepository
    {
        //private System.Data.IDbConnection db = HdbApi.App_Code.DbConnect.Connect();
        
        public List<DatatypeModel.HdbDatatype> GetDataTypes(IDbConnection db, int[] id)
        {
            string sqlString = "select * " +
                "from HDB_DATATYPE A, HDB_UNIT B where A.UNIT_ID = B.UNIT_ID ";
            if (id != null)
            {
                string ids = "";
                foreach (int ithId in id)
                {
                    ids += ithId + ",";
                }
                sqlString += "and A.DATATYPE_ID in (" + ids.TrimEnd(',') + ") ";
            }
            sqlString += "order by A.DATATYPE_ID";
            
            return (List<Models.DatatypeModel.HdbDatatype>)db.Query<DatatypeModel.HdbDatatype>(sqlString);
        }

        public bool InsertDataType(IDbConnection db, DatatypeModel.HdbDatatype dtype)
        {
            throw new NotImplementedException();
        }

        public bool UpdateDataType(IDbConnection db, DatatypeModel.HdbDatatype dtype)
        {
            throw new NotImplementedException();
        }

        public bool DeleteDataType(IDbConnection db, int id)
        {
            throw new NotImplementedException();
        }
    }
}
