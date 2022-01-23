using XExten.Advance.HttpFramework.MultiFactory;
using System.Linq;

namespace Test.HttpTest
{
    public class TestClass
    {
        public string TestMethod()
        {
            var data = IHttpMultiClient.HttpMulti
                .AddHeader(t => {
                    t.HeaderKey = "Host";
                    t.HeaderValue = "konachan.com";
                })
                .AddNode(opt =>
                {
                    opt.NodePath = "https://:443104.21.4.105/post.json";
                })
                .Build(opt =>
                {
                    opt.UseHttps = true;
                    opt.UseZip = true;
                }).RunString().FirstOrDefault();
            return data;

        }
    }
}
