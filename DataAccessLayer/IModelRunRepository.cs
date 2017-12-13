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
    internal interface IModelRunRepository
    {
        List<ModelRunModel.HdbModelRun> GetModelRun(IDbConnection db, string idtype, int[] id, string modelrunname);

        bool InsertModelRun(IDbConnection db, SiteModel.HdbSite site);

        bool UpdateModelRun(IDbConnection db, SiteModel.HdbSite site);

        bool DeleteModelRun(IDbConnection db, int id);
    }

    
    public class ModelRunRepository : IModelRunRepository
    {
        //private System.Data.IDbConnection db = HdbApi.App_Code.DbConnect.Connect();

        public List<ModelRunModel.HdbModelRun> GetModelRun(IDbConnection db, string idtype, int[] id, string modelrunname)
        {
            string sqlString = "select b.model_run_id, b.model_run_name, b.date_time_loaded, b.run_date, "
                + "b.user_name, b.cmmnt as model_run_cmmnt, A.model_id, A.model_name, A.cmmnt as model_cmmnt "
                + "from hdb_model A, ref_model_run B where A.model_id = b.model_id ";
            if (id != null && id.Count() > 0)
            {
                string ids = "";
                foreach (int ithId in id)
                {
                    ids += ithId + ",";
                }
                sqlString += "and b."+ idtype + " in (" + ids.TrimEnd(',') + ") ";
            }
            else if (modelrunname != null)
            {
                sqlString += "and lower(b.model_run_name) like '%" + modelrunname.ToLower() + "%'";
            }
            else
            {

            }
            sqlString += "order by b.date_time_loaded desc";

            return (List<Models.ModelRunModel.HdbModelRun>)db.Query<ModelRunModel.HdbModelRun>(sqlString);
        }

        public bool InsertModelRun(IDbConnection db, SiteModel.HdbSite site)
        {
            throw new NotImplementedException();
        }

        public bool UpdateModelRun(IDbConnection db, SiteModel.HdbSite site)
        {
            throw new NotImplementedException();
        }

        public bool DeleteModelRun(IDbConnection db, int id)
        {
            throw new NotImplementedException();
        }
    }
}
