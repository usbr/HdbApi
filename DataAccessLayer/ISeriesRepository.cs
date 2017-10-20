using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using HdbApi.Models;
using Dapper;
using Oracle.ManagedDataAccess.Client;

namespace HdbApi.DataAccessLayer
{
    internal interface ISeriesRepository
    {
        SeriesModel.TimeSeries GetSeries(string hdb, int id, string tstep, DateTime startDate, DateTime endDate, string sourceTable = "R", int mrid = 0);

        bool InsertSeries();

        bool UpdateSeries();

        bool DeleteSeries();
    }

    
    public class SeriesRepository : ISeriesRepository
    {
        private System.Data.IDbConnection db = HdbApi.Code.DbConnect.Connect();

        public SeriesModel.TimeSeries GetSeries(string hdbInst, int id, string tstep, DateTime startDate, DateTime endDate, string sourceTable = "R", int mrid = 0)
        {
            //// [JR] DONT KNOW WHY THIS WON'T WORK...
            //var p = new OracleDynamicParameters();
            //p.Add("o_cursorOutput", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            //p.Add("i_sdiList", value: id, dbType: OracleDbType.Char);
            //p.Add("i_tStep", value: tstep.ToUpper(), dbType: OracleDbType.Char);
            //p.Add("i_startDate", value: startDate.ToString("dd-MMM-yyyy HH:mm"), dbType: OracleDbType.Char);
            //p.Add("i_endDate", value: endDate.ToString("dd-MMM-yyyy HH:mm"), dbType: OracleDbType.Char);
            //p.Add("i_sourceTable", value: sourceTable.ToUpper(), dbType: OracleDbType.Char);
            //p.Add("i_modelRunIds", value: mrid, dbType: OracleDbType.Char);
            //var result = db.Query<dynamic>("GET_HDB_CGI_DATA", param: p, commandType: CommandType.StoredProcedure);
            //return new List<SeriesModel.TimeSeries>();

            // [JR] ADD LOGIC TO RESOLVE HDB CONNECTION

            // GET QUERY VARS
            var tsQuery = new Models.SeriesModel.TimeSeriesQuery
            {
                hdb = hdbInst.ToUpper(),
                sdi = id,
                t1 = startDate,
                t2 = endDate,
                interval = tstep.ToUpper(),
                table = sourceTable.ToUpper(),
                mrid = mrid,
                retrieved = DateTime.Now
            };

            // GET TS DATA
            string sqlString = string.Format("SELECT START_DATE_TIME AS DATETIME, VALUE AS VALUE FROM R_{0} WHERE " +
                "SITE_DATATYPE_ID IN ({1}) AND START_DATE_TIME >= to_date('{2}','dd-mon-yyyy hh24:mi') AND " +
                "START_DATE_TIME <= to_date('{3}','dd-mon-yyyy hh24:mi')", tstep.ToUpper(), id.ToString("F0"),
                startDate.ToString("dd-MMM-yyyy HH:mm"), endDate.ToString("dd-MMM-yyyy HH:mm"));
            List<SeriesModel.TimeSeriesPoint> tsData = (List<SeriesModel.TimeSeriesPoint>)db.Query<SeriesModel.TimeSeriesPoint>(sqlString);
            // Fills missing with NULL
            //string sqlString = string.Format("SELECT t.DATE_TIME AS DATETIME, CAST(NVL(VALUE,NULL) AS VARCHAR(10)) " +
            //    "AS VALUE FROM R_{0} v RIGHT OUTER JOIN TABLE(DATES_BETWEEN(to_date('{1}','dd-mon-yyyy hh24:mi'), "+
            //    "to_date('{2}','dd-mon-yyyy hh24:mi'),LOWER('{3}'))) t ON v.START_DATE_TIME = t.DATE_TIME WHERE "+
            //    "v.SITE_DATATYPE_ID IN ({4})", tstep.ToUpper(), startDate.ToString("dd-MMM-yyyy HH:mm"), 
            //    endDate.ToString("dd-MMM-yyyy HH:mm"),tstep.ToUpper(), id.ToString("F0"));

            // [JR] GET TS METADATA
            var seriesMetaProcessor = new HdbApi.DataAccessLayer.SiteDataTypeRepository();
            var tsMeta = seriesMetaProcessor.GetSiteDataTypeForSeries(id);

            // BUILD OUTPUT
            var ts = new Models.SeriesModel.TimeSeries
            {
                query = tsQuery,                
                metadata = tsMeta,
                data = tsData
            };

            return ts;
        }

        public bool InsertSeries()
        {
            throw new NotImplementedException();
        }

        public bool UpdateSeries()
        {
            throw new NotImplementedException();
        }

        public bool DeleteSeries()
        {
            throw new NotImplementedException();
        }
    }
}
