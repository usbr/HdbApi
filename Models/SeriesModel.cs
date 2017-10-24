using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HdbApi.Models
{
    public class SeriesModel
    {
        /// <summary>
        /// Time-Series Object
        /// </summary>
        public class TimeSeries
        {
            /// <summary>
            /// Object Metadata
            /// </summary>
            public TimeSeriesQuery query { get; set; }

            /// <summary>
            /// Object Metadata
            /// </summary>
            public SiteDatatypeModel.SiteDataTypeMetadata metadata { get; set; }

            /// <summary>
            /// Object Time-Series Data
            /// </summary>
            public List<TimeSeriesPoint> data { get; set; }
        }

        /// <summary>
        /// Model for querying TS data
        /// </summary>
        public class TimeSeriesQuery
        {
            /// <summary>
            /// HDB Instance Name
            /// </summary>
            public string hdb { get; set; }

            /// <summary>
            /// SDI to query
            /// </summary>
            public int sdi { get; set; }

            /// <summary>
            /// Time-Series Time-Interval 
            /// </summary>
            public string interval { get; set; }

            /// <summary>
            /// Start Date
            /// </summary>
            public DateTime t1 { get; set; }

            /// <summary>
            /// End Date
            /// </summary>
            public DateTime t2 { get; set; }

            /// <summary>
            /// Date data retreived
            /// </summary>
            public DateTime retrieved { get; set; }

            /// <summary>
            /// HDB Table
            /// </summary>
            public string table { get; set; } = "r";

            /// <summary>
            /// Model Run ID
            /// </summary>
            public int mrid { get; set; } = 0;
        }

        /// <summary>
        /// Object Time-Series Data
        /// </summary>
        public class TimeSeriesData
        {
            /// <summary>
            /// Date-Time
            /// </summary>
            public List<TimeSeriesPoint> data { get; set; }
        }

        /// <summary>
        /// Object Time-Series Point
        /// </summary>
        public class TimeSeriesPoint
        {
            /// <summary>
            /// Date-Time
            /// </summary>
            public DateTime datetime { get; set; }

            /// <summary>
            /// Value
            /// </summary>
            public double value { get; set; }

            /// <summary>
            /// Flag
            /// </summary>
            public string flag { get; set; }
        }
    }

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

            /// <summary>
            /// End date-time
            /// </summary>
            public DateTime end_date_time { get; set; }

            /// <summary>
            /// Data Value
            /// </summary>
            public double value { get; set; }

            /// <summary>
            /// Agency ID 
            /// </summary>
            public int agen_id { get; set; }

            /// <summary>
            /// Overwrite Flag
            /// </summary>
            public string overwrite_flag { get; set; }

            /// <summary>
            /// Validation Flag 
            /// </summary>
            public char validation { get; set; }

            /// <summary>
            /// Collection System ID
            /// </summary>
            public int collection_system_id { get; set; }

            /// <summary>
            /// Loading Application ID
            /// </summary>
            public int loading_application_id { get; set; }

            /// <summary>
            /// Method ID
            /// </summary>
            public int method_id { get; set; }

            /// <summary>
            /// Computation ID
            /// </summary>
            public int computation_id { get; set; }

            /// <summary>
            /// Switch for inserting or updating data
            /// </summary>
            public string do_update_y_or_n { get; set; }

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

            /// <summary>
            /// End date-time
            /// </summary>
            public DateTime end_date_time { get; set; }

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
            public string do_update_y_or_n { get; set; }
        }
    }
}