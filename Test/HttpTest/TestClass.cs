using XExten.Advance.HttpFramework.MultiFactory;
using System.Diagnostics;
using Xunit;

namespace Test.HttpTest
{
    public class TestClass
    {
        [Fact]
        public void TestMethond()
        {
            var data = IHttpMultiClient.HttpMulti.InitCookieContainer().InitWebProxy(opt =>
            {
                //opt.IP = "203.74.120.79";
                //opt.Port = 3128;
            }).AddNode("https://www.baidu.com/").Build(360).RunString();
            Assert.NotNull(data);
        }
    }
}
