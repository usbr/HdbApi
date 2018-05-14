using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using NUnit.Framework;

namespace HdbApi.Tests
{
    [TestFixture()]
    public class ApiReadTests
    {
        private static HdbApi.DataAccessLayer.CgiRepository cgiProcessor = new HdbApi.DataAccessLayer.CgiRepository();
        private System.Data.IDbConnection conx = cgiProcessor.connect_hdb("lchdb2");

        [Test()]
        public void SiteEndPointQuery()
        {
            var siteProcessor = new DataAccessLayer.SiteRepository();
            var result = siteProcessor.GetSites(conx, new int[] { 921 });
            if (result[0].site_common_name.ToLower() == "hdmlc" &&
                result[0].site_name.ToLower() == "lake mead" &&
                result[0].site_id == 921 &&
                result[0].state_code.ToLower() == "nv" &&
                result[0].objecttype_name.ToLower() == "reservoir" &&
                result[0].db_site_code.ToLower() == "lc")
            {
                Assert.AreEqual(1, 1);
            }
            else
            {
                Assert.AreEqual(0, 1);
            }
        }
        
        [Test()]
        public void DatatypeEndPointQuery()
        {
            var datatypeProcessor = new DataAccessLayer.DataTypeRepository();
            var result = datatypeProcessor.GetDataTypes(conx, new int[] { 49 });
            if (result[0].datatype_common_name.ToLower() == "pool elevation" &&
                result[0].datatype_name.ToLower() == "reservoir ws elevation, eop primary reading" &&
                result[0].datatype_id == 49 &&
                result[0].unit_name.ToLower() == "feet" &&
                result[0].physical_quantity_name.ToLower() == "water surface elevation")
            {
                Assert.AreEqual(1, 1);
            }
            else
            {
                Assert.AreEqual(0, 1);
            }
        }

        [Test()]
        public void SiteDatatypeEndPointQuery()
        {
            var sitedatatypeProcessor = new DataAccessLayer.SiteDataTypeRepository();
            var result = sitedatatypeProcessor.GetSiteDataTypes(conx, new int[] { 1930 });
            
            var siteProcessor = new DataAccessLayer.SiteRepository();
            var resultSite = siteProcessor.GetSites(conx, new int[] { 921 });

            var datatypeProcessor = new DataAccessLayer.DataTypeRepository();
            var resultDatatype = datatypeProcessor.GetDataTypes(conx, new int[] { 49 });
            
            if (result[0].datatype_id == 49 &&
                result[0].site_id == 921 &&
                result[0].site_datatype_id == 1930 &&
                result[0].metadata.site_metadata.site_id == resultSite[0].site_id &&
                result[0].metadata.site_metadata.state_id == resultSite[0].state_id &&
                result[0].metadata.site_metadata.objecttype_id == resultSite[0].objecttype_id &&
                result[0].metadata.datatype_metadata.datatype_id == resultDatatype[0].datatype_id &&
                result[0].metadata.datatype_metadata.unit_id == resultDatatype[0].unit_id)
            {
                Assert.AreEqual(1, 1);
            }
            else
            {
                Assert.AreEqual(0, 1);
            }
        }

        [Test()]
        public void ModelRunIDEndPointQuery()
        {
            var modelrunProcessor = new DataAccessLayer.ModelRunRepository();
            var result = modelrunProcessor.GetModelRun(conx, "model_run_id", new int[] { 299 }, null);
            if (result[0].model_run_id == 299 &&
                result[0].model_run_name.ToLower().Trim() == "most probable 24 month study" &&
                result[0].model_id == 2 &&
                result[0].model_name.ToLower() == "aop model" &&
                result[0].user_name.ToLower() == "app_user")
            {
                Assert.AreEqual(1, 1);
            }
            else
            {
                Assert.AreEqual(0, 1);
            }
        }

        [Test()]
        public void ModelIDEndPointQuery()
        {
            var modelrunProcessor = new DataAccessLayer.ModelRunRepository();
            var result = modelrunProcessor.GetModelRun(conx, "model_id", new int[] { 37 }, "current");
            if (result[0].model_run_id == 2 &&
                result[0].model_run_name.ToLower().Trim() == "current davis - parker unit schedule" &&
                result[0].model_id == 37 &&
                result[0].model_name.ToLower() == "davis-parker hourly operational schedule")
            {
                Assert.AreEqual(1, 1);
            }
            else
            {
                Assert.AreEqual(0, 1);
            }
        }

        [Test()]
        public void SeriesEndPointQuery()
        {
            var seriesProcessor = new DataAccessLayer.SeriesRepository();
            var result = seriesProcessor.GetSeries(conx, 1930, "month", new DateTime(1980, 1, 1, 0, 0, 0), new DateTime(1980, 1, 1, 0, 0, 0));
            if (result.data[0].value == "1198.98999" &&
                result.metadata.site_metadata.site_id == 921 &&
                result.metadata.datatype_metadata.datatype_id == 49)
            {
                Assert.AreEqual(1, 1);
            }
            else
            {
                Assert.AreEqual(0, 1);
            }
        }
    }
}