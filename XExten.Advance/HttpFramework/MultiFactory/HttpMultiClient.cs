using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using XExten.Advance.CacheFramework;
using XExten.Advance.HttpFramework.MultiCommon;
using XExten.Advance.HttpFramework.MultiOption;
using XExten.Advance.LinqFramework;
using XExten.Advance.StaticFramework;

namespace XExten.Advance.HttpFramework.MultiFactory
{
    internal class HttpMultiClient : IHttpMultiClient, IDisposable
    {
        private WebProxy Proxy;
        private CookieContainer Container;
        private int CacheSecond = 60;
        private HttpClient Client;
        public HttpMultiClient()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        #region 私有
        private HttpClientHandler Handler(BuilderOption Option, Action<HttpClientHandler> action = null)
        {
            HttpClientHandler Handler = new HttpClientHandler();
            if (Container != null)
            {
                Handler.AllowAutoRedirect = true;
                Handler.UseCookies = true;
                Handler.CookieContainer = Container;
            }
            if (Proxy != null)
                Handler.Proxy = Proxy;
            if (Option.UseHttps)
            {
                Handler.SslProtocols = SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12;
                Handler.ServerCertificateCustomValidationCallback = (Message, Certificate, Chain, Error) => true;
            }
            if (Option.UseZip)
                Handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            action?.Invoke(Handler);
            return Handler;
        }
        private IHttpMultiClient BuildProvider(BuilderOption Option = null, Action<HttpClientHandler> handle = null)
        {
            Client = new HttpClient(Handler(Option, handle))
            {
                Timeout = new TimeSpan(0, 0, Option.TimeOut)
            };
            Client.DefaultRequestHeaders.Clear();
            if (MultiConfig.HeaderOpt.Count != 0)
                MultiConfig.HeaderOpt.ForEach(item =>
                {
                    foreach (var KeyValuePair in item)
                    {
                        Client.DefaultRequestHeaders.Add(KeyValuePair.Key, KeyValuePair.Value);
                    }
                });
            return this;
        }
        /// <summary>
        /// 请求
        /// </summary>
        /// <param name="Item"></param>
        /// <param name="Containers"></param>
        /// <returns></returns>
        private Byte[] RequestBytes(NodeOption Item, Action<CookieContainer, Uri> Containers = null)
        {
            byte[] result = null;
            if (Item.ReqType == MultiType.GET)
                result = Client.GetAsync(Item.URI).Result.Content.ReadAsByteArrayAsync().Result;
            else if (Item.ReqType == MultiType.DELETE)
                result = Client.DeleteAsync(Item.URI).Result.Content.ReadAsByteArrayAsync().Result;
            else if (Item.ReqType == MultiType.POST)
                result = Client.PostAsync(Item.URI, Item.Contents).Result.Content.ReadAsByteArrayAsync().Result;
            else
                result = Client.PutAsync(Item.URI, Item.Contents).Result.Content.ReadAsByteArrayAsync().Result;
            Containers?.Invoke(Container, Item.URI);
            return result;
        }
        /// <summary>
        /// 请求
        /// </summary>
        /// <param name="Item"></param>
        /// <param name="Containers"></param>
        /// <returns></returns>
        private string RequestString(NodeOption Item, Action<CookieContainer, Uri> Containers = null)
        {
            Stream stream = null;
            if (Item.ReqType == MultiType.GET)
                stream = Client.GetAsync(Item.URI).Result.Content.ReadAsStreamAsync().Result;
            else if (Item.ReqType == MultiType.DELETE)
                stream = Client.DeleteAsync(Item.URI).Result.Content.ReadAsStreamAsync().Result;
            else if (Item.ReqType == MultiType.POST)
                stream = Client.PostAsync(Item.URI, Item.Contents).Result.Content.ReadAsStreamAsync().Result;
            else
                stream = Client.PutAsync(Item.URI, Item.Contents).Result.Content.ReadAsStreamAsync().Result;
            if (stream.Length < 0) return null;
            using StreamReader reader = new StreamReader(stream, Encoding.GetEncoding(Item.Encoding));
            string result = reader.ReadToEnd();
            Containers?.Invoke(Container, Item.URI);
            return result;
        }
        #endregion

        public IHttpMultiClient InitWebProxy(MultiProxy proxy)
        {
            if (Client != null) throw new Exception("Client已初始化，不能在初始化代理");

            if (proxy.IP.IsNullOrEmpty() || proxy.Port == -1)
                return this;
            Proxy = new WebProxy(proxy.IP, proxy.Port);
            if (!proxy.UserName.IsNullOrEmpty() && !proxy.PassWord.IsNullOrEmpty())
                Proxy.Credentials = new NetworkCredential(proxy.UserName, proxy.PassWord);
            return this;
        }

        public IHttpMultiClient InitCookie()
        {
            if (Client != null) throw new Exception("Client已初始化，不能在初始化认证信息");

            if (Container == null)
                Container = new CookieContainer();
            return this;
        }

        public IHttpMultiClient AddCookie(Action<CookieOption> action)
        {
            if (Client != null) throw new Exception("Client已初始化，不能再添加认证信息");

            CookieOption Option = new CookieOption();
            action(Option);
            Option.SetCookie(Container);
            return this;
        }

        public IHttpMultiClient AddHeader(Action<HeaderOption> action)
        {
            if (Client != null) throw new Exception("Client已初始化，不能再添加头");

            HeaderOption Option = new HeaderOption();
            action(Option);
            Option.SetHeader();
            return this;
        }

        public IHttpMultiClient AddNode(Action<NodeOption> action)
        {
            if (Client != null) throw new Exception("Client已初始化，不能再添加节点");

            NodeOption Option = new NodeOption();
            action(Option);
            Option.SetNode();
            return this;
        }

        public IHttpMultiClient Build(Action<BuilderOption> action = null, Action<HttpClientHandler> handle = null)
        {
            BuilderOption Option = new BuilderOption();
            action?.Invoke(Option);
            CacheSecond = Option.CacheTime;
            return BuildProvider(Option, handle);
        }

        public List<string> RunString(Action<CookieContainer, Uri> Container = null)
        {
            if (Client == null) throw new NullReferenceException("Client未构建请先调用Build()方法");
           
            try
            {
                List<string> Result = new List<string>();
                MultiConfig.NodeOpt.ForEach(item =>
                {
                    if (item.CacheNode)
                    {
                        var Data = Caches.RunTimeCacheGet<string>(item.NodePath.ToMd5());
                        if (Data.IsNullOrEmpty())
                        {
                            Result.Add(RequestString(item, Container));
                            Caches.RunTimeCacheSet(item.NodePath.ToMd5(), Result.FirstOrDefault(), CacheSecond, true);
                        }
                        else
                            Result.Add(Data);
                    }
                    else
                        Result.Add(RequestString(item, Container));
                });
                Dispose();
                return Result;
            }
            catch(Exception ex)
            {
                Dispose();
                throw ex;
            }
        }

        public string RunStringFirst(Action<CookieContainer, Uri> Container = null)
        {
            return RunString(Container).FirstOrDefault();
        }

        public List<byte[]> RunBytes(Action<CookieContainer, Uri> Container = null)
        {
            if (Client == null) throw new NullReferenceException("Client未构建请先调用Build()方法");
            try
            {
                List<Byte[]> Result = new List<Byte[]>();
                MultiConfig.NodeOpt.ForEach(item =>
                {
                    if (item.CacheNode)
                    {
                        var Data = Caches.RunTimeCacheGet<Byte[]>(item.NodePath.ToMd5());
                        if (Data == null)
                        {
                            Result.Add(RequestBytes(item, Container));
                            Caches.RunTimeCacheSet(item.NodePath.ToMd5(), Result.FirstOrDefault(), CacheSecond, true);
                        }
                        else
                            Result.Add(Data);
                    }
                    else
                        Result.Add(RequestBytes(item, Container));
                });
                Dispose();
                return Result;
            }
            catch (Exception ex)
            {
                Dispose();
                throw ex;
            }
        }

        public byte[] RunBytesFirst(Action<CookieContainer, Uri> Container = null)
        {
            return RunBytes(Container).FirstOrDefault();
        }

        public async Task<List<string>> RunStringAsync(Action<CookieContainer, Uri> Container = null)
        {
            if (Client == null) throw new NullReferenceException("Client未构建请先调用Build()方法");
            return await Task.Run(() => RunString(Container));
        }

        public async Task<string> RunStringFirstAsync(Action<CookieContainer, Uri> Container = null)
        {
            if (Client == null) throw new NullReferenceException("Client未构建请先调用Build()方法");
            return await Task.Run(() => RunStringFirst(Container));
        }

        public async Task<List<byte[]>> RunBytesAsync(Action<CookieContainer, Uri> Container = null)
        {
            if (Client == null) throw new NullReferenceException("Client未构建请先调用Build()方法");
            return await Task.Run(() => RunBytes(Container));
        }

        public async Task<byte[]> RunBytesFirstAsync(Action<CookieContainer, Uri> Container = null)
        {
            if (Client == null) throw new NullReferenceException("Client未构建请先调用Build()方法");
            return await Task.Run(() => RunBytesFirst(Container));
        }

        public void Dispose()
        {
            Client = null;
            Container = null;
            MultiConfig.HeaderOpt.Clear();
            MultiConfig.NodeOpt.Clear();
        }
    }
}
