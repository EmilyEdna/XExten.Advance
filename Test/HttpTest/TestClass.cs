using XExten.Advance.HttpFramework.MultiFactory;
using System.Diagnostics;
using Xunit;
using System.Linq;
using DnsClient;
using System.Net;
using XExten.Advance.HttpFramework.MultiInterface;

namespace Test.HttpTest
{
    public class TestClass
    {
        [Fact]
        public void TestMethond()
        {
          
            var data = IHttpMultiClient.HttpMulti
                .AddNode("https://bilibili.com")
                .Build().RunString().FirstOrDefault();

            var data1 = IHttpMultiClient.HttpMulti.AddNode("https://baidu.com").SetResolver(new T())
                .Build().RunString().FirstOrDefault();
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
