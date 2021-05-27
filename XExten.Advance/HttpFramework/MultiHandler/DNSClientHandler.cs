using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using XExten.Advance.HttpFramework.MultiCommon;
using XExten.Advance.HttpFramework.MultiInterface;

namespace XExten.Advance.HttpFramework.MultiHandler
{
    /// <summary>
    /// 自定义解析DNS
    /// </summary>
    internal class DNSClientHandler : HttpClientHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage Request, CancellationToken CancellationToken)
        {
            IResolver Resolver = MultiConfig.ResolverOpt.LastOrDefault();
            Request.Headers.Add("Host", Request.RequestUri.Host);
            var builder = new UriBuilder(Request.RequestUri)
            {
                Port= Request.RequestUri.Port,
                Host = Resolver.Resolve(Request.RequestUri.Host)
            };
            Request.RequestUri = builder.Uri;
            return base.SendAsync(Request, CancellationToken);
        }
    }
}
