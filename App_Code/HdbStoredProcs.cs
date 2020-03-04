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
        IEnumerable<dynamic> modify_r_base_raw(IDbConnection db, decimal sdi,
            string interval, // 'instant', 'other', 'hour', 'day', 'month', 'year', 'wy', 'table interval'
            DateTime t, double val, bool overwrite, //   'O'  'null' 
            char s_validation, bool isNewEntry);//, int loadingApplicationId);

        IEnumerable<dynamic> modify_m_table_raw(IDbConnection db, decimal mrid, decimal sdi, DateTime t, double val, string interval, bool isNewEntry);

        IEnumerable<dynamic> delete_from_hdb(IDbConnection db, decimal sdi, DateTime t, string interval, decimal mrid = 0);

        DataTable get_hdb_cgi_data(IDbConnection db, string sdi, string tstp, System.DateTime t1, System.DateTime t2, string table = "R", int mrid = 0);

        DataTable get_hdb_cgi_data_sql(IDbConnection db, string sdi, string tstp, System.DateTime t1, System.DateTime t2, string table = "R", string mrid = "0");

        DataTable get_hdb_cgi_instant_data(IDbConnection db, string sdi, System.DateTime t1, System.DateTime t2, string table="R");

        DataTable get_hdb_cgi_info(IDbConnection db, string sdiString);

        string get_sdis_from_mrid(IDbConnection db, string mridString, string interval);
    }


    public class HdbCommands : IHdbCommands
    {
        /////////////////////////////////////////////////////////////////////////////////////////////////////
        // [JR] CODE CONVENTIONS AND STORED PROCS BELOW ADOPTED FROM THE POET CODEBASE
        //
        // STORED PROCS REQUIRED TO RUN THE API ARE AS FOLLOWS:
        //      - MODIFY_R_BASE_RAW
        //      - MODIFY_M_TABLE_RAW
        //      - DELETE_FROM_HDB
        //      - GET_HDB_CGI_DATA
        //      - LOOKUP_APPLICATION (THIS REQUIRES AN 'HDB API' ENTRY IN THE HDB_LOADING_APPLICATION TABLE)
        //
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
            char s_validation, bool isNewEntry)//, int loadingApplicationId = -99)
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
            //if (loadingApplicationId == -99)
            //{ p.Add("LOADING_APPLICATION_ID", value: s_LOADING_APPLICATION_ID, dbType: OracleDbType.Decimal); }
            //else
            //{ p.Add("LOADING_APPLICATION_ID", value: loadingApplicationId, dbType: OracleDbType.Decimal); }
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
        public DataTable get_hdb_cgi_data(IDbConnection db, string sdi, string tstp, System.DateTime t1, System.DateTime t2, string table = "R", int mrid = 0)
        {
            var p = new OracleDynamicParameters();
            p.Add("o_cursorOutput", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            p.Add("i_sdiList", value: sdi, dbType: OracleDbType.Varchar2);
            p.Add("i_tStep", value: tstp, dbType: OracleDbType.Varchar2);
            p.Add("i_startDate", value: t1.ToString("dd-MMM-yyyy"), dbType: OracleDbType.Varchar2);
            p.Add("i_endDate", value: t2.ToString("dd-MMM-yyyy"), dbType: OracleDbType.Varchar2);
            p.Add("i_sourceTable", value: table, dbType: OracleDbType.Varchar2);
            p.Add("i_modelRunIds", value: mrid, dbType: OracleDbType.Varchar2);
            //var result = db.Query<dynamic>("GET_HDB_CGI_DATA", param: p, commandType: CommandType.StoredProcedure);

            var dr = db.ExecuteReader("GET_HDB_CGI_DATA", param: p, commandType: CommandType.StoredProcedure);
            var dTab = new DataTable();
            dTab.Load(dr);

            var a = dTab.Copy();
            Console.WriteLine(a.Rows.Count);
            Console.WriteLine(a.Columns.Count);

            return dTab;
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
        public DataTable get_hdb_cgi_data_sql(IDbConnection db, string sdi, string tstp, System.DateTime t1, System.DateTime t2, string table = "R", string mrid = "0")
        {

            /*
             * -- R TABLES
             select 
              DATE_TIME as HDB_DATETIME,
              SDI_4841,
              SDI_8067,
              SDI_4842,
              SDI_8068
            from TABLE(DATES_BETWEEN('18-JAN-2017 00:00','25-JAN-2017 00:00',LOWER('HOUR'))) t
            LEFT OUTER JOIN
            (
              SELECT * FROM 
                  (SELECT SITE_DATATYPE_ID, START_DATE_TIME, VALUE FROM R_HOUR WHERE SITE_DATATYPE_ID IN (4841,8067,4842,8068) AND START_DATE_TIME BETWEEN '18-JAN-2017 00:00' and '25-JAN-2017 00:00')
              PIVOT (MAX(VALUE) FOR (SITE_DATATYPE_ID) IN ('4841' as SDI_4841,'8067' as SDI_8067,'4842' as SDI_4842,'8068' as SDI_8068))
            ) v
            ON t.date_time=v.start_date_time
            ORDEr BY t.DATE_TIME ASC;
            * -- M TABLES
            select 
              DATE_TIME as HDB_DATETIME,
              model_run_id,
              SDI_2097,
              SDI_2101,
              SDI_2096,
              SDI_2086
            from TABLE(DATES_BETWEEN('28-JAN-2017 00:00','30-JAN-2017 00:00',LOWER('HOUR'))) t
            LEFT OUTER JOIN
            (
              SELECT * FROM 
                  (SELECT SITE_DATATYPE_ID, model_run_id, START_DATE_TIME, VALUE FROM M_HOUR WHERE SITE_DATATYPE_ID IN (2097,2101,2096,2086) AND START_DATE_TIME BETWEEN '28-JAN-2017 00:00' and '30-JAN-2017 00:00' and MODEL_RUN_ID in (2))
              PIVOT (MAX(VALUE) FOR (SITE_DATATYPE_ID) IN ('2097' as SDI_2097,'2101' as SDI_2101,'2096' as SDI_2096,'2086' as SDI_2086))
            ) v
            ON t.date_time=v.start_date_time
            ORDEr BY t.DATE_TIME ASC;
            */

            string sql = "select t.DATE_TIME as HDB_DATETIME";
            string modelrunfieldname = "";
            string modelRunSearchString = "";
            if (table.ToLower() == "m")
            {
                modelrunfieldname = ", MODEL_RUN_ID ";
                modelRunSearchString = " AND MODEL_RUN_ID IN (" + mrid + ") ";
            }

            var sdisString = "";
            var pivotString = "PIVOT (MAX(VALUE) FOR (SITE_DATATYPE_ID) IN (";

            sql += modelrunfieldname;
            foreach (string sdiItem in sdi.Split(','))
            {
                int sdiValue;
                if (int.TryParse(sdiItem.Trim(), out sdiValue))
                {
                    sql += ",SDI_" + sdiValue.ToString("F0");
                    sdisString += sdiValue.ToString("F0") + ",";
                    pivotString += "'" + sdiValue.ToString("F0") + "' as SDI_" + sdiValue.ToString("F0") + ",";
                }
            }
            string tableName = table + "_" + tstp;
            string rbaseIntervalQuery = "";
            if (table.ToLower() == "b")
            {
                tableName = "R_BASE";
                rbaseIntervalQuery = " AND LOWER(interval)='" + tstp.ToLower() + "' ";
            }

            sql += " FROM TABLE(DATES_BETWEEN(TO_DATE('" + t1.ToString("dd-MMM-yyyy") + "','DD-MON-YYYY'),";
            sql += " TO_DATE('" + t2.ToString("dd-MMM-yyyy") + "','DD-MON-YYYY'),LOWER('" + tstp + "'))) t LEFT OUTER JOIN ( SELECT * FROM";
            sql += " (select SITE_DATATYPE_ID" + modelrunfieldname + ", START_DATE_TIME, VALUE FROM " + tableName;
            sql += " WHERE SITE_DATATYPE_ID IN (" + sdisString.TrimEnd(',') + ")";
            sql += " AND START_DATE_TIME BETWEEN TO_DATE('" + t1.ToString("dd-MMM-yyyy") + "','DD-MON-YYYY')";
            sql += " AND TO_DATE('" + t2.ToString("dd-MMM-yyyy") + "','DD-MON-YYYY')" + modelRunSearchString + rbaseIntervalQuery + ")";
            sql += " " + pivotString.TrimEnd(',') + "))";
            sql += " ) v ON t.DATE_TIME=v.START_DATE_TIME";
            sql += " ORDER BY t.DATE_TIME ASC ";

            var dr = db.ExecuteReader(sql, commandType: CommandType.Text);
            var dTab = new DataTable();
            dTab.Load(dr);

            return dTab;
        }

        public DataTable get_hdb_cgi_instant_data(IDbConnection db, string sdi, System.DateTime t1, System.DateTime t2, string table = "R")
        {
            /*
             * SELECT 
             *      START_DATE_TIME as HDB_DATETIME, SDI_4841, SDI_8067, SDI_4842, SDI_8068 
             * FROM 
             *      (SELECT SITE_DATATYPE_ID, START_DATE_TIME, VALUE FROM R_INSTANT WHERE SITE_DATATYPE_ID IN (4841,8067,4842,8068) AND START_DATE_TIME BETWEEN '18-JAN-2017 00:00' and '25-JAN-2017 00:00')
             * PIVOT 
             *      (MAX(VALUE) FOR (SITE_DATATYPE_ID) IN ('4841' as SDI_4841,'8067' as SDI_8067,'4842' as SDI_4842,'8068' as SDI_8068))
             * ORDER BY START_DATE_TIME ASC;
            */
            var sql = "select START_DATE_TIME as HDB_DATETIME";
            var sdisString = "";
            var pivotString = "pivot (max(VALUE) for (SITE_DATATYPE_ID) IN (";
            foreach (string sdiItem in sdi.Split(','))
            {
                int sdiValue;
                if (int.TryParse(sdiItem.Trim(), out sdiValue))
                {
                    sql += ",SDI_" + sdiValue.ToString("F0");
                    sdisString += sdiValue.ToString("F0") + ",";
                    pivotString += "'" + sdiValue.ToString("F0") + "' as SDI_" + sdiValue.ToString("F0") + ",";
                }
            }

            string tableName = "R_INSTANT";
            string rbaseIntervalQuery = "";
            if (table.ToLower() != "r")
            {
                tableName = "R_BASE";
                rbaseIntervalQuery = " AND LOWER(interval)='instant' ";
            }

            sql += " from (select SITE_DATATYPE_ID, START_DATE_TIME, VALUE from " + tableName + " where SITE_DATATYPE_ID in (" + sdisString.TrimEnd(',') + ")";
            sql += " and START_DATE_TIME between to_date('" + t1.ToString("dd-MMM-yyyy") + "','DD-MON-YYYY') and to_date('" + t2.ToString("dd-MMM-yyyy") + "','DD-MON-YYYY')";
            sql += " " + rbaseIntervalQuery + ") ";
            sql += " " + pivotString.TrimEnd(',') + "))";
            sql += " order by START_DATE_TIME asc";
            var dr = db.ExecuteReader(sql, commandType: CommandType.Text);
            var dTab = new DataTable();
            dTab.Load(dr);

            return dTab;
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
        public DataTable get_hdb_cgi_info(IDbConnection db, string sdiString)
        {
            var p = new OracleDynamicParameters();
            p.Add("o_cursorOutput", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            p.Add("i_sdiList", value: sdiString, dbType: OracleDbType.Varchar2);
            //var result = db.Query<dynamic>("GET_HDB_CGI_INFO", param: p, commandType: CommandType.StoredProcedure);

            var dr = db.ExecuteReader("GET_HDB_CGI_INFO", param: p, commandType: CommandType.StoredProcedure);
            var dTab = new DataTable();
            dTab.Load(dr);

            return dTab;
        }


        /// <summary>
        /// Gets unique SDIs given a particular MRID and M-Table interval
        /// </summary>
        /// <param name="conx"></param>
        /// <param name="mridString"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public string get_sdis_from_mrid(IDbConnection db, string mridString, string interval)
        {
            // Initialize stuff...
            string sdiString = "";
            string sql = "SELECT UNIQUE(SITE_DATATYPE_ID) FROM M_" + interval + " WHERE MODEL_RUN_ID IN (" + mridString + ")";
            //var result = db.Query<dynamic>(sql, commandType: CommandType.Text);

            var dr = db.ExecuteReader(sql, commandType: CommandType.Text);
            
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Build a string of SDIS with a comma delimiter
            while (dr.Read())
            {
                sdiString = sdiString + dr[0].ToString() + ",";
            }           
            
            return sdiString;
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