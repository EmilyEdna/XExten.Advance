using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using XExten.Advance.CacheFramework;
using XExten.Advance.LinqFramework;
using XExten.Advance.RestHttpFramewor.Options;
using XExten.Advance.StaticFramework;
using System.Linq;
using System.Reflection;

namespace XExten.Advance.RestHttpFramewor
{
    /// <summary>
    /// RestHttpClient
    /// </summary>
    internal class RestHttpClient : IRestHttpClient
    {
        private RestClientOptions Options;
        private RestRequest Request;
        /// <summary>
        /// 构造
        /// </summary>
        public RestHttpClient()
        {
            this.Options = new RestClientOptions();
            this.Request = new RestRequest();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
        /// <summary>
        /// 使用代理
        /// </summary>
        /// <param name="Proxy"></param>
        /// <returns></returns>
        public IRestHttpClient UseProxy(RestProxy Proxy)
        {
            if (Proxy.IP.IsNullOrEmpty() || Proxy.Port == -1)
                return this;
            this.Options.Proxy = new WebProxy(Proxy.IP, Proxy.Port);
            if (!Proxy.UserName.IsNullOrEmpty() && !Proxy.PassWord.IsNullOrEmpty())
                this.Options.Proxy.Credentials = new NetworkCredential(Proxy.UserName, Proxy.PassWord);
            return this;
        }
        /// <summary>
        /// 忽略HTTPS证书
        /// </summary>
        /// <returns></returns>
        public IRestHttpClient UseHttps()
        {
            this.Options.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => { return true; };
            return this;
        }
        /// <summary>
        /// 启用数据压缩
        /// </summary>
        /// <returns></returns>
        public IRestHttpClient UseZip()
        {
            this.Options.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            return this;
        }
        /// <summary>
        /// 使用Cookie
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public IRestHttpClient UseCookie(Action<RestCookie> action)
        {
            RestCookie cookie = new RestCookie();
            action.Invoke(cookie);
            cookie.SetCookie();
            return this;
        }
        /// <summary>
        /// 使用头
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public IRestHttpClient UseHeader(Action<RestHeader> action)
        {
            RestHeader header = new RestHeader();
            action.Invoke(header);
            header.SetHeader();
            return this;
        }
        /// <summary>
        /// 创建节点
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public IRestHttpClient UseNode(Action<RestNode> action)
        {
            RestNode node = new RestNode();
            action.Invoke(node);
            node.SetNode(node);
            return this;
        }
        /// <summary>
        /// 构建
        /// </summary>
        /// <returns></returns>
        public IRestHttpClient Build()
        {
            if (OptionBuilder.Cookies.Count > 0)
                this.Options.CookieContainer = OptionBuilder.Cookies;
            if (OptionBuilder.Header.Count > 0)
                this.Request.AddHeaders(OptionBuilder.Header);
            return this;
        }

        /// <summary>
        /// 返回string
        /// </summary>
        /// <param name="action"></param>
        /// <param name="RetryTimes"></param>
        /// <param name="IntervalTime"></param>
        /// <returns></returns>
        public async Task<List<string>> RunStringAsync(Action<RestResponse> action=null, int RetryTimes = 3, int IntervalTime = 10)
        {
            try
            {
                return await SyncStatic.DoRetryWait(async () =>
                {
                    RestClient client = new RestClient(this.Options);
                    List<string> Result = new List<string>();
                    foreach (RestNode node in OptionBuilder.Nodes)
                    {
                        var key = node.Route.ToMd5();
                        var result = Caches.RunTimeCacheGet<string>(key);
                        if (result.IsNullOrEmpty())
                        {
                            var response = await ConfigRequest(client, node, action);
                            var stream = new MemoryStream(response);
                            using StreamReader reader = new StreamReader(stream, Encoding.GetEncoding(node.Encoding));
                            result = reader.ReadToEnd();
                            await Caches.RunTimeCacheSetAsync(key, result, node.CacheSpan);
                            Result.Add(result);
                        }
                        else
                            Result.Add(result);
                    }

                    return Result;

                }, (ex, count, span) =>
                {
                    if (RetryTimes == count) Dispose();
                }, RetryTimes, IntervalTime);
            }
            catch
            {
                return new List<string>();
            }
            finally
            {
                Dispose();
            }
        }

        /// <summary>
        /// 返回Byte
        /// </summary>
        /// <param name="action"></param>
        /// <param name="RetryTimes"></param>
        /// <param name="IntervalTime"></param>
        /// <returns></returns>
        public async Task<List<byte[]>> RunByteAsync(Action<RestResponse> action = null,int RetryTimes = 3, int IntervalTime = 10)
        {
            try
            {
                return await SyncStatic.DoRetryWait(async () =>
                {
                    RestClient client = new RestClient(this.Options);
                    List<byte[]> Result = new List<byte[]>();
                    foreach (RestNode node in OptionBuilder.Nodes)
                    {
                        var key = node.Route.ToMd5();
                        var result = Caches.RunTimeCacheGet<byte[]>(key);
                        if (result == null)
                        {
                            var response = await ConfigRequest(client,node,action);
                            await Caches.RunTimeCacheSetAsync(key, response, node.CacheSpan);
                            Result.Add(result);
                        }
                        else
                            Result.Add(result);
                    }
                    return Result;
                }, (ex, count, span) =>
                {
                    if (RetryTimes == count) Dispose();
                }, RetryTimes, IntervalTime);
            }
            catch
            {
                return new List<byte[]>();
            }
            finally
            {
                Dispose();
            }
        }

        private async Task<byte[]> ConfigRequest(RestClient client, RestNode Node, Action<RestResponse> action = null)
        {
            switch (Node.Provider)
            {
                case RestProviderMethod.GET:
                    this.Request.RequestFormat = DataFormat.None;
                    this.Request.Resource = Node.Route;
                    this.Request.Method = Method.Get;
                    break;
                case RestProviderMethod.POST:
                    this.Request.Resource = Node.Route;
                    this.Request.Method = Method.Post;
                    if(Node.ProviderType== RestProviderType.JSON)
                        this.Request.AddJsonBody(Node.Param);
                    if (Node.ProviderType == RestProviderType.XML)
                        this.Request.AddXmlBody(Node.Param);
                    if (Node.ProviderType == RestProviderType.FORM)
                        Node.PropertyNames.ForEnumerEach(item =>
                        {
                            var value = Node.Param.GetType().GetProperty(item, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).GetValue(Node.Param).ToString();
                            this.Request.AddParameter(item, value);
                        });
                    break;
                case RestProviderMethod.PUT:
                    this.Request.Resource = Node.Route;
                    this.Request.Method = Method.Put;
                    if (Node.ProviderType == RestProviderType.JSON)
                        this.Request.AddJsonBody(Node.Param);
                    if (Node.ProviderType == RestProviderType.XML)
                        this.Request.AddXmlBody(Node.Param);
                    if (Node.ProviderType == RestProviderType.FORM)
                        Node.PropertyNames.ForEnumerEach(item =>
                        {
                            var value = Node.Param.GetType().GetProperty(item, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).GetValue(Node.Param).ToString();
                            this.Request.AddParameter(item, value);
                        });
                    break;
                case RestProviderMethod.DELETE:
                    this.Request.RequestFormat = DataFormat.None;
                    this.Request.Resource = Node.Route;
                    this.Request.Method = Method.Delete;
                    break;
                default:
                    break;
            }

            var response = await client.ExecuteAsync(this.Request);
            action?.Invoke(response);
            return response.RawBytes;
        }

        private void Dispose()
        {
            OptionBuilder.Nodes = null;
            OptionBuilder.Header = null;
            OptionBuilder.Cookies = null;
            this.Request = null;
        }
    }
}
