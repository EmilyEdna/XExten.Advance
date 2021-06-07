using XExten.Advance.HttpFramework.MultiFactory;
using System.Linq;

namespace Test.HttpTest
{
    public class TestClass
    {
        public string TestMethond()
        {
            return IHttpMultiClient.HttpMulti
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
