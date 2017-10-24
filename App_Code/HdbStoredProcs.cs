using System;
using System.Collections.Generic;
using HdbApi.Models;
using System.Text.RegularExpressions;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using Dapper;

namespace HdbApi.App_Code
{
    internal interface IHdbCommands
    {
        IEnumerable<dynamic> modify_r_base_raw(IDbConnection db, int sdi,
            string interval, // 'instant', 'other', 'hour', 'day', 'month', 'year', 'wy', 'table interval'
            DateTime t, double val, bool overwrite, //   'O'  'null' 
            char s_validation, bool isNewEntry);

        IEnumerable<dynamic> modify_m_table_raw(IDbConnection db, int mrid, int sdi, DateTime t, double val, string interval, bool isNewEntry);

        IEnumerable<dynamic> delete_from_hdb(IDbConnection db, int sdi, DateTime t, string interval, int mrid = 0);
    }


    public class HdbCommands : IHdbCommands
    {
        /////////////////////////////////////////////////////////////////////////////////////////////////////
        // [JR] CODE CONVENTIONS AND STORED PROCS BELOW ADOPTED FROM THE POET CODEBASE
        /////////////////////////////////////////////////////////////////////////////////////////////////////
        #region
        private static int HDB_INVALID_ID = -1;
        private static int s_AGEN_ID = HDB_INVALID_ID;
        private static int s_COLLECTION_SYSTEM_ID = HDB_INVALID_ID;
        private static int s_LOADING_APPLICATION_ID = HDB_INVALID_ID;
        private static int s_METHOD_ID = HDB_INVALID_ID;
        private static int s_COMPUTATION_ID = HDB_INVALID_ID;


        /// <summary>
        /// Method to write data to R-Tables in HDB
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sdi"></param>
        /// <param name="interval"></param>
        /// <param name="t"></param>
        /// <param name="val"></param>
        /// <param name="overwrite"></param>
        /// <param name="s_validation"></param>
        /// <returns></returns>
        public IEnumerable<dynamic> modify_r_base_raw(IDbConnection db, int sdi,
            string interval, // 'instant', 'other', 'hour', 'day', 'month', 'year', 'wy', 'table interval'
            DateTime t, double val, bool overwrite, //   'O'  'null' 
            char s_validation, bool isNewEntry)
        {
            var p = new OracleDynamicParameters();
            p.Add("SITE_DATATYPE_ID", value: 2101, dbType: OracleDbType.Int32);
            p.Add("INTERVAL", value: interval, dbType: OracleDbType.Varchar2);
            p.Add("START_DATE_TIME", value: t, dbType: OracleDbType.Date);
            p.Add("END_DATE_TIME", value: DBNull.Value, dbType: OracleDbType.Date);
            p.Add("VALUE", value: val, dbType: OracleDbType.Double);
            p.Add("AGEN_ID", value: s_AGEN_ID, dbType: OracleDbType.Int32);
            if (overwrite)
            { p.Add("OVERWRITE_FLAG", value: "O", dbType: OracleDbType.Varchar2); }
            else
            { p.Add("OVERWRITE_FLAG", value: DBNull.Value, dbType: OracleDbType.Varchar2); }
            p.Add("VALIDATION", value: s_validation, dbType: OracleDbType.Varchar2);
            p.Add("COLLECTION_SYSTEM_ID", value: s_COLLECTION_SYSTEM_ID, dbType: OracleDbType.Int32);
            p.Add("LOADING_APPLICATION_ID", value: s_LOADING_APPLICATION_ID, dbType: OracleDbType.Int32);
            p.Add("METHOD_ID", value: s_METHOD_ID, dbType: OracleDbType.Int32);
            p.Add("COMPUTATION_ID", value: s_COMPUTATION_ID, dbType: OracleDbType.Int32);
            if (isNewEntry)
            { p.Add("DO_UPDATE_Y_OR_N", value: "N", dbType: OracleDbType.Varchar2); }
            else
            { p.Add("DO_UPDATE_Y_OR_N", value: "Y", dbType: OracleDbType.Varchar2); }
            p.Add("DATA_FLAGS", value: DBNull.Value, dbType: OracleDbType.Varchar2);
            p.Add("TIME_ZONE", value: DBNull.Value, dbType: OracleDbType.Varchar2);

            return db.Query<dynamic>("MODIFY_R_BASE_RAW", param: p, commandType: CommandType.StoredProcedure);
        }


        /// <summary>
        /// Method to write data to M-Tables in HDB
        /// </summary>
        /// <param name="db"></param>
        /// <param name="mrid"></param>
        /// <param name="sdi"></param>
        /// <param name="t"></param>
        /// <param name="val"></param>
        /// <param name="interval"></param>
        /// <param name="isNewEntry"></param>
        /// <returns></returns>
        public IEnumerable<dynamic> modify_m_table_raw(IDbConnection db, int mrid, int sdi, DateTime t, double val, string interval, bool isNewEntry)
        {
            var p = new OracleDynamicParameters();
            p.Add("MODEL_RUN_ID_IN", value: 2101, dbType: OracleDbType.Int32);
            p.Add("SITE_DATATYPE_ID_IN", value: 2101, dbType: OracleDbType.Int32);
            p.Add("START_DATE_TIME_IN", value: t, dbType: OracleDbType.Date);
            p.Add("END_DATE_TIME", value: DBNull.Value, dbType: OracleDbType.Date);
            p.Add("VALUE", value: val, dbType: OracleDbType.Double);
            p.Add("INTERVAL", value: interval, dbType: OracleDbType.Varchar2);
            if (isNewEntry)
            { p.Add("DO_UPDATE_Y_OR_N", value: "N", dbType: OracleDbType.Varchar2); }
            else
            { p.Add("DO_UPDATE_Y_OR_N", value: "Y", dbType: OracleDbType.Varchar2); }

            return db.Query<dynamic>("MODIFY_M_TABLE_RAW", param: p, commandType: CommandType.StoredProcedure);
        }


        /// <summary>
        /// Method to delete data from the R or M Tables in HDB
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sdi"></param>
        /// <param name="t"></param>
        /// <param name="interval"></param>
        /// <param name="mrid"></param>
        /// <returns></returns>
        public IEnumerable<dynamic> delete_from_hdb(IDbConnection db, int sdi, DateTime t, string interval, int mrid = 0)
        {
            var p = new OracleDynamicParameters();
            p.Add("SAMPLE_SDI", value: sdi, dbType: OracleDbType.Int32);
            p.Add("SAMPLE_DATE_TIME", value: t, dbType: OracleDbType.Date);
            p.Add("SAMPLE_END_TIME", value: DBNull.Value, dbType: OracleDbType.Date);
            p.Add("SAMPLE_INTERVAL", value: interval, dbType: OracleDbType.Varchar2);
            p.Add("LOADING_APP_ID", value: s_LOADING_APPLICATION_ID, dbType: OracleDbType.Varchar2);
            p.Add("MODELRUN_ID", value: mrid, dbType: OracleDbType.Int32);
            p.Add("AGENCY_ID", value: s_AGEN_ID, dbType: OracleDbType.Varchar2);
            p.Add("TIME_ZONE", value: DBNull.Value, dbType: OracleDbType.Varchar2);

            return db.Query<dynamic>("DELETE_FROM_HDB", param: p, commandType: CommandType.StoredProcedure);
        }
        #endregion

    }
}