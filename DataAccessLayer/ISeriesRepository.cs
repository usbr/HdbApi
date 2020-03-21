using System;
using System.Collections.Generic;
using HdbApi.Models;
using System.Text.RegularExpressions;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using Dapper;

namespace HdbApi.DataAccessLayer
{
    /// <summary>
    /// Processors for Series objects
    /// </summary>
    internal interface ISeriesRepository
    {
        SeriesModel.TimeSeries GetSeries(IDbConnection db, string id, string tstep, DateTime startDate, DateTime endDate, string sourceTable = "R", int mrid = 0, bool rbase = false, string instantMinutes = "60");

        bool InsertSeries();

        bool UpdateSeries();

        bool DeleteSeries();
    }

    /// <summary>
    /// Processors for Series objects
    /// </summary>
    public class SeriesRepository : ISeriesRepository
    {
        //private System.Data.IDbConnection db = HdbApi.App_Code.DbConnect.Connect();

        public SeriesModel.TimeSeries GetSeries(IDbConnection db, string id, string tstep, DateTime startDate, DateTime endDate, string sourceTable = "R", int mrid = 0, bool useRbase = false, string instantMinutes = "60")
        {
            if (db != null)
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
                    retrieved = DateTime.Now,
                    rbase = useRbase
                };

                string queryTable = "";

                if (sourceTable == "R" && useRbase)
                {
                    queryTable = string.Format("(SELECT SITE_DATATYPE_ID, START_DATE_TIME, VALUE FROM R_BASE WHERE START_DATE_TIME BETWEEN " +
                        "to_date('{0}','dd-mon-yyyy hh24:mi') AND to_date('{1}','dd-mon-yyyy hh24:mi') AND SITE_DATATYPE_ID " +
                        "IN ({2}) AND INTERVAL = '{3}')",
                        startDate.ToString("dd-MMM-yyyy HH:mm"), endDate.ToString("dd-MMM-yyyy HH:mm"), id, tstep.ToLower());
                }
                else
                {
                    queryTable = string.Format("{0}_{1}", sourceTable.ToUpper(), tstep.ToUpper());
                }
                // GET TS DATA
                //string sqlString = string.Format("SELECT START_DATE_TIME AS DATETIME, VALUE AS VALUE FROM {0}_{1} WHERE " +
                //    "SITE_DATATYPE_ID IN ({2}) AND START_DATE_TIME >= to_date('{3}','dd-mon-yyyy hh24:mi') AND " +
                //    "START_DATE_TIME <= to_date('{4}','dd-mon-yyyy hh24:mi')", sourceTable.ToUpper(), tstep.ToUpper(),
                //    id.ToString("F0"), startDate.ToString("dd-MMM-yyyy HH:mm"), endDate.ToString("dd-MMM-yyyy HH:mm"));

                // Fills missing with NULL
                string dateArrayFunction = "DATES_BETWEEN";
                string dateArrayIdentifier = "'" + tstep.ToLower() + "'";
                if (tstep.ToUpper() == "INSTANT")
                {
                    dateArrayFunction = "INSTANTS_BETWEEN";
                    dateArrayIdentifier = instantMinutes;
                }
                string sqlString = string.Format("SELECT t.DATE_TIME AS DATETIME, CAST(NVL(VALUE,NULL) AS VARCHAR(10)) " +
                    "AS VALUE FROM " + queryTable + " v RIGHT OUTER JOIN TABLE(" + dateArrayFunction + "(to_date('{0}','dd-mon-yyyy hh24:mi'), " +
                    "to_date('{1}','dd-mon-yyyy hh24:mi')," + dateArrayIdentifier + ")) t ON v.START_DATE_TIME = t.DATE_TIME AND " +
                    "v.SITE_DATATYPE_ID IN ({2})", startDate.ToString("dd-MMM-yyyy HH:mm"), endDate.ToString("dd-MMM-yyyy HH:mm"), id);

                if (sourceTable.ToUpper() == "M")
                {
                    sqlString += string.Format(" AND MODEL_RUN_ID = {0}", mrid);
                }
                sqlString += " ORDER BY t.DATE_TIME";
                List<SeriesModel.TimeSeriesPoint> tsData = (List<SeriesModel.TimeSeriesPoint>)db.Query<SeriesModel.TimeSeriesPoint>(sqlString);

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
            else
            {
                // GET QUERY VARS
                var tsQuery = new Models.SeriesModel.TimeSeriesQuery
                {
                    hdb = "hydromet",
                    sdi = id,
                    t1 = startDate,
                    t2 = endDate,
                    interval = tstep.ToUpper(),
                    table = "NA",
                    mrid = -1,
                    retrieved = DateTime.Now,
                    rbase = useRbase
                };

                // GET TS DATA
                var tsTable = App_Code.HydrometCommands.GetTsDataTable(tstep, id, startDate, endDate);
                List<SeriesModel.TimeSeriesPoint> tsData = new List<SeriesModel.TimeSeriesPoint>();
                foreach (DataRow row in tsTable.Rows)
                {
                    tsData.Add(new SeriesModel.TimeSeriesPoint() { datetime = Convert.ToDateTime(row[0]), value = row[1].ToString() });
                }

                // [JR] GET TS METADATA
                var seriesMetaProcessor = new HdbApi.DataAccessLayer.SiteDataTypeRepository();
                var tsMeta = seriesMetaProcessor.GetSiteDataTypeForSeries(null, id);

                // BUILD OUTPUT
                var ts = new Models.SeriesModel.TimeSeries
                {
                    query = tsQuery,
                    metadata = tsMeta,
                    data = tsData
                };

                return ts;
            }
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
