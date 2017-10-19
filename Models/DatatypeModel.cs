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
            /// Unique Datatype ID
            /// </summary>
            public int datatype_id { get; set; }

            /// <summary>
            /// Datatype name
            /// </summary>
            public string datatype_name { get; set; }

            /// <summary>
            /// Datatype common name
            /// </summary>
            public string datatype_common_name { get; set; }

            /// <summary>
            /// Datatype physical quantity
            /// </summary>
            public string physical_quantity_name { get; set; }

            /// <summary>
            /// Datatype units ID
            /// </summary>
            public int unit_id { get; set; }

            /// <summary>
            /// Allowable time series intervals
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