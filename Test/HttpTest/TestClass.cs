using XExten.Advance.HttpFramework.MultiFactory;
using System.Linq;
using DnsClient;
using XExten.Advance.HttpFramework.MultiInterface;
using XExten.Advance.HttpFramework.MultiCommon;
using System.Net;

namespace Test.HttpTest
{
    public class TestClass
    {
        public string TestMethond()
        {
            return IHttpMultiClient.HttpMulti
                .SetResolver()
                .AddNode(opt =>
                {
                    opt.NodePath = "https://www.konachan.com";
                })
                .Build(opt =>
                {
                    opt.UseDnsResolver = true;
                }).RunString().FirstOrDefault();

        }
    }
}
