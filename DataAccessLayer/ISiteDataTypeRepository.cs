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
    /// Processors for site datatype objects
    /// </summary>
    internal interface ISiteDataTypeRepository
    {
        List<SiteDatatypeModel.HdbSiteDatatype> GetSiteDataTypes(IDbConnection db, int[] sdi);

        bool InsertSiteDataType(IDbConnection db, SiteDatatypeModel.HdbSiteDatatype sdi);

        bool UpdateSiteDataType(IDbConnection db, SiteDatatypeModel.HdbSiteDatatype sdi);

        bool DeleteSiteDataType(IDbConnection db, int sdi);
    }

    
    public class SiteDataTypeRepository : ISiteDataTypeRepository
    {
        //private System.Data.IDbConnection db = HdbApi.App_Code.DbConnect.Connect();

        public List<SiteDatatypeModel.HdbSiteDatatype> GetSiteDataTypes(IDbConnection db, int[] id)
        {
            string sqlString = "select * " +
                "from HDB_SITE_DATATYPE A, HDB_SITE B, HDB_DATATYPE C where " +
                "A.SITE_ID = B.SITE_ID and A.DATATYPE_ID = C.DATATYPE_ID ";
            if (id != null)
            {
                string ids = "";
                foreach (int ithId in id)
                {
                    ids += ithId + ",";
                }
                sqlString += "and A.SITE_DATATYPE_ID in (" + ids.TrimEnd(',') + ") ";
            }
            sqlString += "order by A.SITE_DATATYPE_ID";
            
            // MULTIMAP 
            var result = (List<SiteDatatypeModel.HdbSiteDatatype>)db.Query<
                SiteDatatypeModel.HdbSiteDatatype,
                SiteModel.HdbSite,
                DatatypeModel.HdbDatatype,
                SiteDatatypeModel.HdbSiteDatatype>(
                    sqlString,
                    (sdi, sdimeta, dtypemeta) =>
                    {
                        sdi.metadata = new SiteDatatypeModel.SiteDataTypeMetadata
                        {
                            site_metadata = sdimeta,
                            datatype_metadata = dtypemeta
                        };
                        return sdi;
                    },
                    commandType: System.Data.CommandType.Text,
                    splitOn: "site_id,datatype_id"
            );

            return result;
        }

        public SiteDatatypeModel.SiteDataTypeMetadata GetSiteDataTypeForSeries(IDbConnection db, int id)
        {
            string sqlString = "select * " +
                "from HDB_SITE_DATATYPE A, HDB_SITE B, HDB_DATATYPE C where " +
                "A.SITE_ID = B.SITE_ID and A.DATATYPE_ID = C.DATATYPE_ID and " + 
                "A.SITE_DATATYPE_ID = " + id;

            // MULTIMAP 
            var result = (List<SiteDatatypeModel.SiteDataTypeMetadata>)db.Query<
                SiteDatatypeModel.SiteDataTypeMetadata,
                SiteModel.HdbSite,
                DatatypeModel.HdbDatatype,
                SiteDatatypeModel.SiteDataTypeMetadata>(
                    sqlString,
                    (sdiMeta, sdimeta, dtypemeta) =>
                    {
                        sdiMeta = new SiteDatatypeModel.SiteDataTypeMetadata
                        {
                            site_metadata = sdimeta,
                            datatype_metadata = dtypemeta
                        };
                        return sdiMeta;
                    },
                    commandType: System.Data.CommandType.Text,
                    splitOn: "site_id,datatype_id"
            );
            
            return result.FirstOrDefault();
        }


        public bool InsertSiteDataType(IDbConnection db, SiteDatatypeModel.HdbSiteDatatype dtype)
        {
            throw new NotImplementedException();
        }

        public bool UpdateSiteDataType(IDbConnection db, SiteDatatypeModel.HdbSiteDatatype dtype)
        {
            throw new NotImplementedException();
        }

        public bool DeleteSiteDataType(IDbConnection db, int id)
        {
            throw new NotImplementedException();
        }
    }
}
