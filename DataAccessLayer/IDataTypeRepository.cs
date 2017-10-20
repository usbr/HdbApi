using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HdbApi.Models;
using Dapper;

namespace HdbApi.DataAccessLayer
{
    internal interface IDataTypeRepository
    {
        List<DatatypeModel.HdbDatatype> GetDataTypes(int[] id);

        bool InsertDataType(DatatypeModel.HdbDatatype dtype);

        bool UpdateDataType(DatatypeModel.HdbDatatype dtype);

        bool DeleteDataType(int id);
    }

    
    public class DataTypeRepository : IDataTypeRepository
    {
        private System.Data.IDbConnection db = HdbApi.Code.DbConnect.Connect();

        public List<DatatypeModel.HdbDatatype> GetDataTypes(int[] id)
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

        public bool InsertDataType(DatatypeModel.HdbDatatype dtype)
        {
            throw new NotImplementedException();
        }

        public bool UpdateDataType(DatatypeModel.HdbDatatype dtype)
        {
            throw new NotImplementedException();
        }

        public bool DeleteDataType(int id)
        {
            throw new NotImplementedException();
        }
    }
}
