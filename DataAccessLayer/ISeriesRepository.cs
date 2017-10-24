using System;
using System.Collections.Generic;
using HdbApi.Models;
using System.Text.RegularExpressions;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using Dapper;

namespace HdbApi.DataAccessLayer
{
    internal interface ISeriesRepository
    {
        SeriesModel.TimeSeries GetSeries(IDbConnection db, int id, string tstep, DateTime startDate, DateTime endDate, string sourceTable = "R", int mrid = 0);

        bool InsertSeries();

        bool UpdateSeries();

        bool DeleteSeries();
    }

    
    public class SeriesRepository : ISeriesRepository
    {
        //private System.Data.IDbConnection db = HdbApi.App_Code.DbConnect.Connect();

        public SeriesModel.TimeSeries GetSeries(IDbConnection db, int id, string tstep, DateTime startDate, DateTime endDate, string sourceTable = "R", int mrid = 0)
        {
            // RESOLVE HDB CONNECTION
            Regex regex = new Regex(@"Data Source=([^;]*);");
            Match match = regex.Match(db.ConnectionString);

            // GET QUERY VARS
            var tsQuery = new Models.SeriesModel.TimeSeriesQuery
            {
                hdb = match.Groups[1].Value.ToString().ToUpper(),
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
            var tsMeta = seriesMetaProcessor.GetSiteDataTypeForSeries(db, id);

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
