using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HdbApi.Models
{
    public class CgiModel
    {
        public class HdbCgiJson
        {
            public string QueryDate { get; set; }
            public string StartDate { get; set; }
            public string EndDate { get; set; }
            public string TimeStep { get; set; }
            public string DataSource { get; set; }
            public List<Sites> Series { get; set; }
        }

        public class Sites
        {
            public string SDI { get; set; }
            public string SiteName { get; set; }
            public string DataTypeName { get; set; }
            public string DataTypeUnit { get; set; }
            public string Latitude { get; set; }
            public string Longitude { get; set; }
            public string Elevation { get; set; }
            public string DB { get; set; }
            public string MRID { get; set; }
            public List<Data> Data { get; set; }
        }

        public class Data
        {
            public string t { get; set; }
            public string v { get; set; }
        }

    }
}