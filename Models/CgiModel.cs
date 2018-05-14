using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HdbApi.Models
{
    /// <summary>
    /// Data Model for time-series data returned by the cgi program
    /// </summary>
    public class CgiModel
    {
        /// <summary>
        /// CGI data model
        /// </summary>
        public class HdbCgiJson
        {
            /// <summary>
            /// Query date-time
            /// </summary>
            public string QueryDate { get; set; }
            /// <summary>
            /// Start date-time for data query
            /// </summary>
            public string StartDate { get; set; }
            /// <summary>
            /// End date-time for data query
            /// </summary>
            public string EndDate { get; set; }
            /// <summary>
            /// Timestep for data query
            /// </summary>
            public string TimeStep { get; set; }
            /// <summary>
            /// Data source
            /// </summary>
            public string DataSource { get; set; }
            /// <summary>
            /// Series object
            /// </summary>
            public List<Sites> Series { get; set; }
        }

        /// <summary>
        /// Site Object
        /// </summary>
        public class Sites
        {
            /// <summary>
            /// Site Datatype ID
            /// </summary>
            public string SDI { get; set; }
            /// <summary>
            /// Site name
            /// </summary>
            public string SiteName { get; set; }
            /// <summary>
            /// Datatype
            /// </summary>
            public string DataTypeName { get; set; }
            /// <summary>
            /// Data physical units
            /// </summary>
            public string DataTypeUnit { get; set; }
            /// <summary>
            /// Latitude coordinates
            /// </summary>
            public string Latitude { get; set; }
            /// <summary>
            /// Longitude coordinates
            /// </summary>
            public string Longitude { get; set; }
            /// <summary>
            /// Elevation
            /// </summary>
            public string Elevation { get; set; }
            /// <summary>
            /// DB source
            /// </summary>
            public string DB { get; set; }
            /// <summary>
            /// Model run id
            /// </summary>
            public string MRID { get; set; }
            /// <summary>
            /// Time-series data object
            /// </summary>
            public List<Data> Data { get; set; }
        }

        /// <summary>
        /// Time-series data array
        /// </summary>
        public class Data
        {
            /// <summary>
            /// Date time
            /// </summary>
            public string t { get; set; }
            /// <summary>
            /// Values
            /// </summary>
            public string v { get; set; }
        }

    }
}