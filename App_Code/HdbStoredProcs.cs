﻿using System;
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
        IEnumerable<dynamic> modify_r_base_raw(IDbConnection db, decimal sdi,
            string interval, // 'instant', 'other', 'hour', 'day', 'month', 'year', 'wy', 'table interval'
            DateTime t, double val, bool overwrite, //   'O'  'null' 
            char s_validation, bool isNewEntry);

        IEnumerable<dynamic> modify_m_table_raw(IDbConnection db, decimal mrid, decimal sdi, DateTime t, double val, string interval, bool isNewEntry);

        IEnumerable<dynamic> delete_from_hdb(IDbConnection db, decimal sdi, DateTime t, string interval, decimal mrid = 0);

        IEnumerable<dynamic> get_hdb_cgi_data(IDbConnection db, string sdi, string tstp, System.DateTime t1, System.DateTime t2, string table = "R", int mrid = 0);
    }


    public class HdbCommands : IHdbCommands
    {
        /////////////////////////////////////////////////////////////////////////////////////////////////////
        // [JR] CODE CONVENTIONS AND STORED PROCS BELOW ADOPTED FROM THE POET CODEBASE
        /////////////////////////////////////////////////////////////////////////////////////////////////////
        #region
        private static decimal HDB_INVALID_ID = -1;
        private static decimal s_AGEN_ID = HDB_INVALID_ID;
        private static decimal s_COLLECTION_SYSTEM_ID = HDB_INVALID_ID;
        private static decimal s_LOADING_APPLICATION_ID = HDB_INVALID_ID;
        private static decimal s_METHOD_ID = HDB_INVALID_ID;
        private static decimal s_COMPUTATION_ID = HDB_INVALID_ID;


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
        public IEnumerable<dynamic> modify_r_base_raw(IDbConnection db, decimal sdi,
            string interval, // 'instant', 'other', 'hour', 'day', 'month', 'year', 'wy', 'table interval'
            DateTime t, double val, bool overwrite, //   'O'  'null' 
            char s_validation, bool isNewEntry)
        {
            LookupApplication(db);

            var p = new OracleDynamicParameters();
            p.Add("SITE_DATATYPE_ID", value: sdi, dbType: OracleDbType.Decimal);
            p.Add("INTERVAL", value: interval, dbType: OracleDbType.Varchar2);
            p.Add("START_DATE_TIME", value: t, dbType: OracleDbType.Date);
            p.Add("END_DATE_TIME", value: DBNull.Value, dbType: OracleDbType.Date);
            p.Add("VALUE", value: val, dbType: OracleDbType.Double);
            p.Add("AGEN_ID", value: s_AGEN_ID, dbType: OracleDbType.Decimal);
            if (overwrite)
            { p.Add("OVERWRITE_FLAG", value: "O", dbType: OracleDbType.Varchar2); }
            else
            { p.Add("OVERWRITE_FLAG", value: DBNull.Value, dbType: OracleDbType.Varchar2); }
            p.Add("VALIDATION", value: s_validation, dbType: OracleDbType.Varchar2);
            p.Add("COLLECTION_SYSTEM_ID", value: s_COLLECTION_SYSTEM_ID, dbType: OracleDbType.Decimal);
            p.Add("LOADING_APPLICATION_ID", value: s_LOADING_APPLICATION_ID, dbType: OracleDbType.Decimal);
            p.Add("METHOD_ID", value: s_METHOD_ID, dbType: OracleDbType.Decimal);
            p.Add("COMPUTATION_ID", value: s_COMPUTATION_ID, dbType: OracleDbType.Decimal);
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
        public IEnumerable<dynamic> modify_m_table_raw(IDbConnection db, decimal mrid, decimal sdi, DateTime t, double val, string interval, bool isNewEntry)
        {
            var p = new OracleDynamicParameters();
            p.Add("MODEL_RUN_ID_IN", value: mrid, dbType: OracleDbType.Decimal);
            p.Add("SITE_DATATYPE_ID_IN", value: sdi, dbType: OracleDbType.Decimal);
            p.Add("START_DATE_TIME_IN", value: t, dbType: OracleDbType.Date);
            p.Add("END_DATE_TIME", value: DBNull.Value, dbType: OracleDbType.Date);
            p.Add("VALUE", value: val, dbType: OracleDbType.Double);
            p.Add("INTERVAL_IN", value: interval, dbType: OracleDbType.Varchar2);
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
        public IEnumerable<dynamic> delete_from_hdb(IDbConnection db, decimal sdi, DateTime t, string interval, decimal mrid = 0)
        {
            LookupApplication(db);

            var p = new OracleDynamicParameters();
            p.Add("SAMPLE_SDI", value: sdi, dbType: OracleDbType.Decimal);
            p.Add("SAMPLE_DATE_TIME", value: t, dbType: OracleDbType.Date);
            p.Add("SAMPLE_END_TIME", value: DBNull.Value, dbType: OracleDbType.Date);
            p.Add("SAMPLE_INTERVAL", value: interval, dbType: OracleDbType.Varchar2);
            p.Add("LOADING_APP_ID", value: s_LOADING_APPLICATION_ID, dbType: OracleDbType.Varchar2);
            p.Add("MODELRUN_ID", value: mrid, dbType: OracleDbType.Decimal);
            p.Add("AGENCY_ID", value: s_AGEN_ID, dbType: OracleDbType.Varchar2);
            p.Add("TIME_ZONE", value: DBNull.Value, dbType: OracleDbType.Varchar2);

            return db.Query<dynamic>("DELETE_FROM_HDB", param: p, commandType: CommandType.StoredProcedure);
        }


        /// <summary>
        /// Method that generates arrays of TS data used by legacy CGI program
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sdi"></param>
        /// <param name="tstp"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="table"></param>
        /// <param name="mrid"></param>
        /// <returns></returns>
        public IEnumerable<dynamic> get_hdb_cgi_data(IDbConnection db, string sdi, string tstp, System.DateTime t1, System.DateTime t2, string table = "R", int mrid = 0)
        {
            var p = new OracleDynamicParameters();
            p.Add("o_cursorOutput", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            p.Add("i_sdiList", value: sdi, dbType: OracleDbType.Varchar2);
            p.Add("i_tStep", value: tstp, dbType: OracleDbType.Varchar2);
            p.Add("i_startDate", value: t1.ToString("dd-MMM-yyyy"), dbType: OracleDbType.Varchar2);
            p.Add("i_endDate", value: t2.ToString("dd-MMM-yyyy"), dbType: OracleDbType.Varchar2);
            p.Add("i_sourceTable", value: table, dbType: OracleDbType.Varchar2);
            p.Add("i_modelRunIds", value: mrid, dbType: OracleDbType.Varchar2);
            var result = db.Query<dynamic>("GET_HDB_CGI_DATA", param: p, commandType: CommandType.StoredProcedure);

            return result;
        }


        /// <summary>
        /// LookupApplication calls a stored procedure to 
        /// determine what id the hdb-poet application is using for the 
        /// specific HDB you are logged into.
        /// 
        /// Some other id's are also returned from the stored procedure:
        /// 
        /// the values returned are:
        /// 
        /// static_AGEN_ID 
        /// static_COLLECTION_SYSTEM_ID
        /// static_LOADING_APPLICATION_ID
        /// static_METHOD_ID 
        /// static_COMPUTATION_ID 
        /// </summary>
        public void LookupApplication(IDbConnection db)
        {
            string agen_id_name = "Bureau of Reclamation";
            string coll_sys_name = "(see agency)";
            string load_app_name = "HDB API";
            string meth_name = "unknown";
            string comp_name = "unknown";

            var p = new OracleDynamicParameters();
            p.Add("AGEN_NAME", value: agen_id_name, dbType: OracleDbType.Varchar2);
            p.Add("COLLECTION_SYSTEM_NAME", value: coll_sys_name, dbType: OracleDbType.Varchar2);
            p.Add("LOADING_APPLICATION_NAME", value: load_app_name, dbType: OracleDbType.Varchar2);
            p.Add("METHOD_NAME", value: meth_name, dbType: OracleDbType.Varchar2);
            p.Add("COMPUTATION_NAME", value: comp_name, dbType: OracleDbType.Varchar2);
            p.Add("AGEN_ID", dbType: OracleDbType.Decimal, direction: ParameterDirection.Output);
            p.Add("COLLECTION_SYSTEM_ID", dbType: OracleDbType.Decimal, direction: ParameterDirection.Output);
            p.Add("LOADING_APPLICATION_ID", dbType: OracleDbType.Decimal, direction: ParameterDirection.Output);
            p.Add("METHOD_ID", dbType: OracleDbType.Decimal, direction: ParameterDirection.Output);
            p.Add("COMPUTATION_ID", dbType: OracleDbType.Decimal, direction: ParameterDirection.Output);
            var result = db.Query<dynamic>("LOOKUP_APPLICATION", param: p, commandType: CommandType.StoredProcedure);

            s_AGEN_ID = ToDecimal(p.Get<dynamic>("AGEN_ID"));
            s_COLLECTION_SYSTEM_ID = ToDecimal(p.Get<dynamic>("COLLECTION_SYSTEM_ID"));
            s_LOADING_APPLICATION_ID = ToDecimal(p.Get<dynamic>("LOADING_APPLICATION_ID"));
            s_METHOD_ID = ToDecimal(p.Get<dynamic>("METHOD_ID"));
            s_COMPUTATION_ID = ToDecimal(p.Get<dynamic>("COMPUTATION_ID"));
        }

        private static decimal ToDecimal(object o)
        {
            return Convert.ToDecimal(o.ToString());
        }
        #endregion

    }
}