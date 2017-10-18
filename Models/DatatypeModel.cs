using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HdbApi.Models
{
    public class DatatypeModel
    {
        /// <summary>
        /// HDB_SITE table contents
        /// </summary>
        public class HdbDatatype
        {
            /// <summary>
            /// Unique site ID
            /// </summary>
            public int datatype_id { get; set; }

            /// <summary>
            /// Site name
            /// </summary>
            public string datatype_name { get; set; }

            /// <summary>
            /// Site common name
            /// </summary>
            public string datatype_common_name { get; set; }

            /// <summary>
            /// Site description
            /// </summary>
            public string physical_quantity_name { get; set; }

            /// <summary>
            /// Site geographic elevation (if applicable)
            /// </summary>
            public int unit_id { get; set; }

            /// <summary>
            /// Site geographic latitude (if applicable)
            /// </summary>
            public string allowable_intervals { get; set; }

            /// <summary>
            /// Agency ID
            /// </summary>
            public int agen_id { get; set; }

            /// <summary>
            /// Comment
            /// </summary>
            public string cmmnt { get; set; }
        }

    }
}