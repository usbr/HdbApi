using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using NUnit.Framework;

namespace HdbApi.Tests
{
    [TestFixture()]
    public class ConnectionTests
    {
        private HdbApi.DataAccessLayer.CgiRepository cgiProcessor = new HdbApi.DataAccessLayer.CgiRepository();

        [Test()]
        public void GetHostList()
        {
            var hosts = cgiProcessor.get_host_list();
            Assert.Greater(hosts.Count, 0);
        }

        [Test()]
        public void ConnectLC()
        {
            var conx = cgiProcessor.connect_hdb("lchdb2");
            Assert.AreEqual("Closed", conx.State.ToString());
        }

        [Test()]
        public void ConnectUC()
        {
            var conx = cgiProcessor.connect_hdb("uchdb2");
            Assert.AreEqual("Closed", conx.State.ToString());
        }

        [Test()]
        public void ConnectYAO()
        {
            var conx = cgiProcessor.connect_hdb("yaohdb");
            Assert.AreEqual("Closed", conx.State.ToString());
        }

        [Test()]
        public void ConnectECAO()
        {
            var conx = cgiProcessor.connect_hdb("ecohdb");
            Assert.AreEqual("Closed", conx.State.ToString());
        }

        [Test()]
        public void ConnectLBAO()
        {
            var conx = cgiProcessor.connect_hdb("lbohdb");
            Assert.AreEqual("Closed", conx.State.ToString());
        }

    }
}