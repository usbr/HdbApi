using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HdbApi.Models;
using Dapper;

namespace HdbApi.DataAccessLayer
{
    internal interface ISiteDataTypeRepository
    {
        List<SiteDatatypeModel.HdbSiteDatatype> GetSiteDataTypes(int[] sdi);

        bool InsertSiteDataType(SiteDatatypeModel.HdbSiteDatatype sdi);

        bool UpdateSiteDataType(SiteDatatypeModel.HdbSiteDatatype sdi);

        bool DeleteSiteDataType(int sdi);
    }

    
    public class SiteDataTypeRepository : ISiteDataTypeRepository
    {
        private System.Data.IDbConnection db = HdbApi.Code.DbConnect.Connect();

        public List<SiteDatatypeModel.HdbSiteDatatype> GetSiteDataTypes(int[] id)
        {
            string sqlString = "select * " +
                "from HDB_SITE_DATATYPE A ";
            if (id != null)
            {
                string ids = "";
                foreach (int ithId in id)
                {
                    ids += ithId + ",";
                }
                sqlString += "where A.SITE_DATATYPE_ID in (" + ids.TrimEnd(',') + ") ";
            }
            sqlString += "order by A.SITE_DATATYPE_ID";

            return (List<SiteDatatypeModel.HdbSiteDatatype>)db.Query<SiteDatatypeModel.HdbSiteDatatype>(sqlString);
        }

        public SiteDatatypeModel.SiteDataTypeMetadata GetSiteDataTypeForSeries(int id)
        {
            string sqlString = "select * " +
                "from HDB_SITE_DATATYPE A, HDB_SITE B, HDB_DATATYPE C where " +
                "A.SITE_ID = B.SITE_ID and A.DATATYPE_ID = C.DATATYPE_ID and " + 
                "A.SITE_DATATYPE_ID = " + id;
            
            SiteModel.HdbSite siteMeta = ((List<SiteModel.HdbSite>)db.Query<SiteModel.HdbSite>(sqlString))[0];
            DatatypeModel.HdbDatatype dtypeMeta = ((List<DatatypeModel.HdbDatatype>)db.Query<DatatypeModel.HdbDatatype>(sqlString))[0]; 

            var result = new SiteDatatypeModel.SiteDataTypeMetadata
            {
                site_metadata = siteMeta,
                datatype_metadata = dtypeMeta
            };

            return result;
        }


        public bool InsertSiteDataType(SiteDatatypeModel.HdbSiteDatatype dtype)
        {
            throw new NotImplementedException();
        }

        public bool UpdateSiteDataType(SiteDatatypeModel.HdbSiteDatatype dtype)
        {
            throw new NotImplementedException();
        }

        public bool DeleteSiteDataType(int id)
        {
            throw new NotImplementedException();
        }
    }
}
