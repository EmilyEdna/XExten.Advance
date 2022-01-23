using DnsClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XExten.Advance.LinqFramework;

namespace Test.DnsTest
{
    public class TestClass
    {
        public  void TestMethod()
        {
            var dnsserver = @"1.1.1.1,1.0.0.1";

            dnsserver.Split(",").ToList().ForEach(item => {
                LookupClient client = new LookupClient(System.Net.IPAddress.Parse(item));
                var res  =  client.Query("konachan.com", QueryType.A).AllRecords.FirstOrDefault();
                Console.WriteLine(res);
            });
        }
    }
}
