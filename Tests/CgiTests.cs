using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using NUnit.Framework;

namespace HdbApi.Tests
{
    [TestFixture()]
    public class CgiTests
    {
        private HdbApi.DataAccessLayer.CgiRepository cgiProcessor = new HdbApi.DataAccessLayer.CgiRepository();

        [Test()]
        public void QueryV1DateFormat()
        {
            var conx = cgiProcessor.connect_hdb("lchdb");
            var urlString = @"http://localhost:31614/cgi?svr=lchdb&sdi=1928,1930&tstp=DY&t1=1/1/2015&t2=1/10/2015&format=csv";
            var outFile = cgiProcessor.get_cgi_data(conx, urlString);
            Assert.AreEqual(@"<BR>01/10/2015 00:00,          NaN,      1088.48", outFile[16].ToString());
        }

        [Test()]
        public void QueryIsoDateFormat()
        {
            var conx = cgiProcessor.connect_hdb("lchdb");
            var urlString = @"http://localhost:31614/cgi?svr=lchdb&sdi=1928,1930&tstp=DY&t1=2015-01-01T00:00&t2=2015-01-10T00:00&format=1";
            var outFile = cgiProcessor.get_cgi_data(conx, urlString);
            Assert.AreEqual(@"<BR>01/10/2015 00:00,          NaN,      1088.48", outFile[34].ToString());
        }

        [Test()]
        public void QueryModeledData()
        {
            var conx = cgiProcessor.connect_hdb("lchdb");
            var urlString = @"http://localhost:31614/cgi?svr=lchdb&sdi=1930&tstp=MN&t1=2017-12-01T00:00&t2=2018-05-01T00:00&table=M&mrid=3012&format=88";
            var outFile = cgiProcessor.get_cgi_data(conx, urlString);
            Assert.AreEqual(@"05/01/2018 00:00,         3012, 1062.152106639250" + "\n", outFile[6].ToString());
        }

        [Test()]
        public void QueryInstantData()
        {
            var conx = cgiProcessor.connect_hdb("lchdb");
            var urlString = @"http://localhost:31614/cgi?svr=lchdb&sdi=25401&tstp=IN&t1=2018-05-07T05:00&t2=2018-05-07T08:00&table=R&mrid=&format=table";
            var outFile = cgiProcessor.get_cgi_data(conx, urlString);
            Assert.AreEqual(13, outFile.Count);
        }

        [Test()]
        public void TestJSON()
        {
            var conx = cgiProcessor.connect_hdb("lchdb");
            var urlString = @"http://localhost:31614/cgi?svr=lchdb2&sdi=1928,1930&tstp=DY&t1=2015-01-01T00:00&t2=2015-01-10T00:00&format=json";
            try
            {
                var outFile = JsonConvert.SerializeObject(cgiProcessor.get_cgi_data(conx, urlString));
                Assert.AreEqual(0, 0);
            }
            catch
            {
                Assert.Fail("Output is not a valid JSON");
            }
        }

    }
}