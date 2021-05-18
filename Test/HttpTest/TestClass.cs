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
            }).AddNode("https://www.bilibili.com/audio/music-service-c/web/url?sid=1280236").Build(action:h=> {

            }).RunString(); 
            Assert.NotNull(data);
        }
    }
}
