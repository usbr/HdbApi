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
    /// Processors for Site objects
    /// </summary>
    internal interface ISiteRepository
    {
        List<SiteModel.HdbSite> GetSites(IDbConnection db, int[] id);

        bool InsertSite(IDbConnection db, SiteModel.HdbSite site);

        bool UpdateSite(IDbConnection db, SiteModel.HdbSite site);

        bool DeleteSite(IDbConnection db, int id);
    }

    
    public class SiteRepository : ISiteRepository
    {
        //private System.Data.IDbConnection db = HdbApi.App_Code.DbConnect.Connect();

        public List<SiteModel.HdbSite> GetSites(IDbConnection db, int[] id)
        {
            string sqlString = "select * " +
                "from HDB_SITE A, HDB_OBJECTTYPE B, HDB_STATE C where A.OBJECTTYPE_ID = B.OBJECTTYPE_ID and A.STATE_ID = C.STATE_ID ";
            if (id != null)
            {
                string ids = "";
                foreach (int ithId in id)
                {
                    ids += ithId + ",";
                }
                sqlString += "and A.SITE_ID in (" + ids.TrimEnd(',') + ") ";
            }
            sqlString += "order by A.SITE_ID";

            return (List<Models.SiteModel.HdbSite>)db.Query<SiteModel.HdbSite>(sqlString);
        }

        public bool InsertSite(IDbConnection db, SiteModel.HdbSite site)
        {
            throw new NotImplementedException();
        }

        public bool UpdateSite(IDbConnection db, SiteModel.HdbSite site)
        {
            throw new NotImplementedException();
        }

        public bool DeleteSite(IDbConnection db, int id)
        {
            throw new NotImplementedException();
        }
    }
}
