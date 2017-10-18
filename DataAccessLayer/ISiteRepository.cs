using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HdbApi.Models;
using Dapper;

namespace HdbApi.DataAccessLayer
{
    internal interface ISiteRepository
    {
        List<SiteModel.HdbSite> GetSites();

        List<SiteModel.HdbSite> GetSites(int[] id);

        bool InsertSite(SiteModel.HdbSite site);

        bool UpdateSite(SiteModel.HdbSite site);

        bool DeleteSite(int id);
    }

    
    public class SiteRepository : ISiteRepository
    {
        private System.Data.IDbConnection db = HdbApi.Code.DbConnect.Connect();

        public List<SiteModel.HdbSite> GetSites()
        {
            string sqlString = "select * from HDB_SITE order by SITE_ID";
            return (List<Models.SiteModel.HdbSite>)db.Query<SiteModel.HdbSite>(sqlString);
        }

        public List<SiteModel.HdbSite> GetSites(int[] id)
        {
            string sqlString = "select * from HDB_SITE";
            if (id != null)
            {
                string ids = "";
                foreach (int ithId in id)
                {
                    ids += ithId + ",";
                }
                sqlString += " where SITE_ID in (" + ids.TrimEnd(',') + ")";
            }
            sqlString += " order by SITE_ID";

            return (List<Models.SiteModel.HdbSite>)db.Query<SiteModel.HdbSite>(sqlString);
        }

        public bool InsertSite(SiteModel.HdbSite site)
        {
            throw new NotImplementedException();
        }

        public bool UpdateSite(SiteModel.HdbSite site)
        {
            throw new NotImplementedException();
        }

        public bool DeleteSite(int id)
        {
            throw new NotImplementedException();
        }
    }
}
