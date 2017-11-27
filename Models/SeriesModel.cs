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

            /// <summary>
            /// Use R-Base tables
            /// </summary>
            public bool rbase { get; set; } = false;
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
            public string value { get; set; }

            /// <summary>
            /// Flag
            /// </summary>
            public string flag { get; set; }
        }
    }
    
}