using Synctool.HttpFramework;
using Synctool.LinqFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Test.HttpTest
{
    public class TestClass
    {
        [Fact]
        public void TestMethond()
        {
            var data = IHttpMultiClient.HttpMulti.InitCookieContainer().AddNode("https://www.baidu.com/")
                   .Build(UseHttps: true).RunString((result, wather) =>
                   {
                       Trace.WriteLine(result);
                   });
            Assert.NotNull(data);
        }
    }
}
