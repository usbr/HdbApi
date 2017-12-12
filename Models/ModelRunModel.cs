using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HdbApi.Models
{
    public class ModelRunModel
    {
        /// <summary>
        /// HDB_MODEL and REF_MODEL_RUN table contents
        /// </summary>
        public class HdbModelRun
        {
            /// <summary>
            /// Unique model run ID
            /// </summary>
            public int model_run_id { get; set; }

            /// <summary>
            /// Model run name
            /// </summary>
            public string model_run_name { get; set; }

            /// <summary>
            /// Date model run loaded
            /// </summary>
            public DateTime date_time_loaded { get; set; }

            /// <summary>
            /// Date model last ran
            /// </summary>
            public DateTime run_date { get; set; }

            /// <summary>
            /// User with last modification
            /// </summary>
            public string user_name { get; set; }

            /// <summary>
            /// Model run description
            /// </summary>
            public string model_run_cmmnt { get; set; }

            /// <summary>
            /// Model ID
            /// </summary>
            public int model_id { get; set; }

            /// <summary>
            /// Model name
            /// </summary>
            public string model_name { get; set; }

            /// <summary>
            /// Model description
            /// </summary>
            public string model_cmmnt { get; set; }
        }

    }
}