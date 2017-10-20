using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HdbApi.Models
{
    public class SiteModel
    {
        /// <summary>
        /// HDB_SITE table contents
        /// </summary>
        public class HdbSite
        {
            /// <summary>
            /// Unique site ID
            /// </summary>
            public int site_id { get; set; }

            /// <summary>
            /// Site name
            /// </summary>
            public string site_name { get; set; }

            /// <summary>
            /// Site common name
            /// </summary>
            public string site_common_name { get; set; }

            /// <summary>
            /// Site description
            /// </summary>
            public string description { get; set; }

            /// <summary>
            /// Site geographic elevation (if applicable)
            /// </summary>
            public float elevation { get; set; }

            /// <summary>
            /// Site geographic latitude (if applicable)
            /// </summary>
            public string lat { get; set; }

            /// <summary>
            /// Site geographic longitude (if applicable)
            /// </summary>
            public string longi { get; set; }

            /// <summary>
            /// Site HDB membership
            /// </summary>
            public string db_site_code { get; set; }

            /// <summary>
            /// Site Object Type ID
            /// </summary>
            public int objecttype_id { get; set; }

            /// <summary>
            /// Site Object Type name
            /// </summary>
            public string objecttype_name { get; set; }

            /// <summary>
            /// Basin (if applicable)
            /// </summary>
            public int basin_id { get; set; }

            /// <summary>
            /// HUC (if applicable)
            /// </summary>
            public string hydrologic_unit { get; set; }

            /// <summary>
            /// RM (if applicable)
            /// </summary>
            public float river_mile { get; set; }

            /// <summary>
            /// Segment (if applicable)
            /// </summary>
            public int segment_no { get; set; }

            /// <summary>
            /// State ID
            /// </summary>
            public int state_id { get; set; }

            /// <summary>
            /// State Code
            /// </summary>
            public string state_code { get; set; }

            /// <summary>
            /// USGS ID (if applicable)
            /// </summary>
            public string usgs_id { get; set; }

            /// <summary>
            /// NWS ID (if applicable)
            /// </summary>
            public string nws_code { get; set; }

            /// <summary>
            /// SHEF ID (if applicable)
            /// </summary>
            public string shef_code { get; set; }

            /// <summary>
            /// SCS ID (if applicable)
            /// </summary>
            public string scs_id { get; set; }

            /// <summary>
            /// Parent site Object Type (if applicable)
            /// </summary>
            public int parent_objecttype_id { get; set; }

            /// <summary>
            /// Parent site Site ID (if applicable)
            /// </summary>
            public int parent_site_id { get; set; }
        }

    }
}