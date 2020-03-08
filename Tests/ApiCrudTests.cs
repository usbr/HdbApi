using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using NUnit.Framework;

namespace HdbApi.Tests
{
    [TestFixture()]
    public class ApiCrudTests
    {
        private static HdbApi.DataAccessLayer.CgiRepository cgiProcessor = new HdbApi.DataAccessLayer.CgiRepository();
        private System.Data.IDbConnection conx = cgiProcessor.connect_hdb("lchdb2");

        private string testSDI = "2104";
        private int testMRID = 3055;
        private string testInterval = "month";
        private DateTime testT = new DateTime(1980, 1, 1, 0, 0, 0);
        private double testVal = 5.0;
        private bool testBool = false;
        private char testFlag = 'Z';
        private int testLoadingApplicationId = 31;

        [Test()]
        public void SeriesRtableInsertEndPoint()
        {
            var hdbProcessor = new HdbApi.App_Code.HdbCommands();
            var result = hdbProcessor.modify_r_base_raw(conx, Convert.ToInt16(testSDI), testInterval, testT, testVal, testBool, testFlag, testBool);//, testLoadingApplicationId);

            var seriesProcessor = new DataAccessLayer.SeriesRepository();
            var verification = seriesProcessor.GetSeries(conx, testSDI, testInterval, testT, testT);

            Assert.AreEqual(testVal.ToString(), verification.data[0].value);
            hdbProcessor.delete_from_hdb(conx, Convert.ToInt16(testSDI), testT, testInterval);
        }

        [Test()]
        public void SeriesRtableDeleteEndPoint()
        {
            var hdbProcessor = new HdbApi.App_Code.HdbCommands();
            var result = hdbProcessor.modify_r_base_raw(conx, Convert.ToInt16(testSDI), testInterval, testT, testVal, testBool, testFlag, testBool);//, testLoadingApplicationId);
            hdbProcessor.delete_from_hdb(conx, Convert.ToInt16(testSDI), testT, testInterval);

            var seriesProcessor = new DataAccessLayer.SeriesRepository();
            var verification = seriesProcessor.GetSeries(conx, testSDI, testInterval, testT, testT);

            Assert.AreEqual(null, verification.data[0].value);
        }

        [Test()]
        public void SeriesMtableInsertEndPoint()
        {
            var hdbProcessor = new HdbApi.App_Code.HdbCommands();
            var result = hdbProcessor.modify_m_table_raw(conx,testMRID, Convert.ToInt16(testSDI), testT, testVal, testInterval, testBool);

            var seriesProcessor = new DataAccessLayer.SeriesRepository();
            var verification = seriesProcessor.GetSeries(conx, testSDI, testInterval, testT, testT, "M", testMRID);

            Assert.AreEqual(testVal.ToString(), verification.data[0].value);
            hdbProcessor.delete_from_hdb(conx, Convert.ToInt16(testSDI), testT, testInterval, testMRID);
        }

        [Test()]
        public void SeriesMtableDeleteEndPoint()
        {
            var hdbProcessor = new HdbApi.App_Code.HdbCommands();
            var result = hdbProcessor.modify_m_table_raw(conx, testMRID, Convert.ToInt16(testSDI), testT, testVal, testInterval, testBool);
            hdbProcessor.delete_from_hdb(conx, Convert.ToInt16(testSDI), testT, testInterval, testMRID);

            var seriesProcessor = new DataAccessLayer.SeriesRepository();
            var verification = seriesProcessor.GetSeries(conx, testSDI, testInterval, testT, testT, "M", testMRID);

            Assert.AreEqual(null, verification.data[0].value);
        }
    }
}