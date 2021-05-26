using XExten.Advance.HttpFramework.MultiFactory;
using System.Diagnostics;
using Xunit;
using System.Linq;
using DnsClient;
using System.Net;
using XExten.Advance.HttpFramework.MultiInterface;
using XExten.Advance.HttpFramework.MultiCommon;

namespace Test.HttpTest
{
    public class TestClass
    {
        [Fact]
        public void TestMethond()
        {
          
            var data = IHttpMultiClient.HttpMulti
                .AddNode(opt=> {
                    opt.NodePath = "https://bilibili.com";
                }).Build().RunString().FirstOrDefault();


            Assert.NotNull(data);
        }
        public class T : IResolver
        {
            public string Resolve(string Host)
            {
                LookupClient lookup = new LookupClient();
                var result = lookup.Query(Host, QueryType.A);
                var dns = result.Answers.ARecords().ToList();
                return dns.FirstOrDefault().Address.ToString();
            }
        }
    }
}
