using XExten.Advance.HttpFramework.MultiFactory;
using System.Linq;
using DnsClient;
using XExten.Advance.HttpFramework.MultiInterface;

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
                    opt.NodePath = "http://www.bilibili.com";
                }).Build(opt =>
                {
                    opt.UseDnsResolver = true;
                }).RunString().FirstOrDefault();

        }
    }
}
