using Synctool.HttpFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            var data = HttpMultiClient.HttpMulti.AddNode("https://www.baidu.com/")
                   .AddNode("http://news.2010.sina.com.cn/").Build(UseHttps: true).RunString((result, wather) =>
                   {
                       Trace.WriteLine(result);
                   });
            Assert.NotNull(data);
        }
    }
}
