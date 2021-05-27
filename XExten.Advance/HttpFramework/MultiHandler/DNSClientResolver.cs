using DnsClient;
using System.Net;
using System.Linq;
using XExten.Advance.LinqFramework;
using XExten.Advance.HttpFramework.MultiInterface;

namespace XExten.Advance.HttpFramework.MultiHandler
{
    /// <summary>
    /// DNS解析器
    /// </summary>
    internal class DNSClientResolver : IResolver
    {
        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="Host"></param>
        /// <returns></returns>
        public string Resolve(string Host)
        {
            LookupClient client = new LookupClient(IPAddress.Parse("101.101.101.101"));
            var Dns = client.Query(Host, QueryType.A).Answers.ARecords().ToList();
            var Ip = Dns.FirstOrDefault().Address.ToString();
            return Ip;
        }
    }
}
