using DnsClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            LookupClient lookup = new LookupClient();
            var result = lookup.Query(Host, QueryType.A);
            var dns = result.Answers.ARecords().ToList();
            return dns.LastOrDefault().Address.ToString();
        }
    }
}
