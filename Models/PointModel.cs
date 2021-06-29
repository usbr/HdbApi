using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HdbApi.Models
{
    /// <summary>
    /// Data Model for time-series point data
    /// </summary>
    public class PointModel
    {
        /// <summary>
        /// Data Point Object for Writing Data to HDB R-Tables
        /// </summary>
        public class ObservedPoint
        {
            /// <summary>
            /// SDI 
            /// </summary>
            public int site_datatype_id { get; set; }

            /// <summary>
            /// HDB interval table
            /// </summary>
            public string interval { get; set; }

            /// <summary>
            /// Start date-time
            /// </summary>
            public DateTime start_date_time { get; set; }

            ///// <summary>
            ///// End date-time
            ///// </summary>
            //public DateTime end_date_time { get; set; }

            /// <summary>
            /// Data Value
            /// </summary>
            public double value { get; set; }

            ///// <summary>
            ///// Agency ID 
            ///// </summary>
            //public int agen_id { get; set; }

            /// <summary>
            /// Overwrite Flag
            /// </summary>
            public bool overwrite_flag { get; set; }

            /// <summary>
            /// Validation Flag 
            /// </summary>
            public char validation { get; set; }

            ///// <summary>
            ///// Collection System ID
            ///// </summary>
            //public int collection_system_id { get; set; }

            ///// <summary>
            ///// Loading Application ID
            ///// </summary>
            public int loading_application_id { get; set; }

            ///// <summary>
            ///// Method ID
            ///// </summary>
            //public int method_id { get; set; }

            ///// <summary>
            ///// Computation ID
            ///// </summary>
            public int computation_id { get; set; }

            /// <summary>
            /// Switch for inserting or updating data
            /// </summary>
            public bool do_update_y_or_n { get; set; }

            /// <summary>
            /// Data Flags
            /// </summary>
            public string data_flags { get; set; }

        }

        /// <summary>
        /// Data Point Object for Writing Data to HDB M-Tables
        /// </summary>
        public class ModeledPoint
        {
            /// <summary>
            /// Model run id
            /// </summary>
            public int model_run_id { get; set; }

            /// <summary>
            /// SDI 
            /// </summary>
            public int site_datatype_id { get; set; }

            /// <summary>
            /// Start date-time
            /// </summary>
            public DateTime start_date_time { get; set; }

            ///// <summary>
            ///// End date-time
            ///// </summary>
            //public DateTime end_date_time { get; set; }

            /// <summary>
            /// Data value
            /// </summary>
            public double value { get; set; }

            /// <summary>
            /// HDB interval table
            /// </summary>
            public string interval { get; set; }

            /// <summary>
            /// Switch for inserting or updating data
            /// </summary>
            public bool do_update_y_or_n { get; set; }
        }
    }
}