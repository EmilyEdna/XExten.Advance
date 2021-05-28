using XExten.Advance.HttpFramework.MultiFactory;
using System.Linq;
using DnsClient;
using XExten.Advance.HttpFramework.MultiInterface;
using XExten.Advance.HttpFramework.MultiCommon;
using System.Net;
using XExten.Advance.HttpFramework.MultiHandler;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

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
                    opt.NodePath = "https://www.bilibili.com";
                })
                .Build(opt =>
                {
                    //opt.UseDnsResolver = true;
                    opt.UseHttps = true;
                    opt.UseZip = true;
                }).RunString().FirstOrDefault();

        }
    }
}
