using XExten.Advance.HttpFramework.MultiFactory;
using System.Linq;

namespace Test.HttpTest
{
    public class TestClass
    {
        public string TestMethod()
        {
            return IHttpMultiClient.HttpMulti
                .AddNode(opt =>
                {
                    opt.NodePath = "https://www.bilibili.com";
                })
                .Build(opt =>
                {
                    opt.UseHttps = true;
                    opt.UseZip = true;
                }).RunString().FirstOrDefault();

        }
    }
}
