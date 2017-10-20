using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HdbApi.Models
{
    public class SiteDatatypeModel
    {
        /// <summary>
        /// HDB_SITE_DATATYPE table contents
        /// </summary>
        public class HdbSiteDatatype
        {
            /// <summary>
            /// Unique Site Datatype ID
            /// </summary>
            public int site_dataype_id { get; set; }

            /// <summary>
            /// Site ID
            /// </summary>
            public int site_id { get; set; }

            /// <summary>
            /// Datatype ID
            /// </summary>
            public int datatype_id { get; set; } 
            
            /// <summary>
            /// Site and Datatype Metadata
            /// </summary>
            public SiteDataTypeMetadata metadata { get; set; }

        }



        public class SiteDataTypeMetadata
        {
            /// <summary>
            /// Site name
            /// </summary>
            public SiteModel.HdbSite site_metadata { get; set; }

            /// <summary>
            /// Datatype name
            /// </summary>
            public DatatypeModel.HdbDatatype datatype_metadata { get; set; }
        }

    }
}