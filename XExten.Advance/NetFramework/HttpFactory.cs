using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
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
using XExten.Advance.LinqFramework;
using XExten.Advance.NetFramework.Enums;
using XExten.Advance.NetFramework.Options;

namespace XExten.Advance.NetFramework
{
    internal class HttpFactory : INetFactory
    {
        private CookieContainer CookieContainer;
        private List<DefaultHeader> Headers;
        private List<DefaultNodes> Nodes;
        private DefaultBuilder Builder;
        private string BaseUri;
        private Action<CookieContainer, Uri> CookieHandler;
        private HttpClient Client;
        public HttpFactory()
        {
            Headers = new List<DefaultHeader>();
            Nodes = new List<DefaultNodes>();
            Builder = new DefaultBuilder();
            CookieContainer = new CookieContainer();
        }
        public INetFactory AddCookie(Action<DefaultCookie> action)
        {
            DefaultCookie cookie = new DefaultCookie();
            action.Invoke(cookie);
            if (!cookie.Uri.IsNullOrEmpty() && cookie.Collection == null)
                cookie.Cookies.ForDicEach((key, val) =>
                {
                    CookieContainer.Add(new Uri(cookie.Uri), new Cookie(key, val));
                });
            else CookieContainer.Add(new Uri(cookie.Uri), cookie.Collection);
            return this;
        }

        public INetFactory AddHeader(Action<DefaultHeader> action)
        {
            DefaultHeader head = new DefaultHeader();
            action.Invoke(head);
            if (!head.Key.IsNullOrEmpty())
                Headers.Add(head);
            return this;
        }

        public INetFactory AddHeader(List<DefaultHeader> action)
        {
            if (action != null)
                foreach (var item in action)
                {
                    this.AddHeader(t =>
                    {
                        t.Key = item.Key;
                        t.Value = item.Value;
                    });
                }
            return this;
        }

        public INetFactory AddWhereHeader(bool condition, List<DefaultHeader> action)
        {
            if (condition)
            {
                if (action != null)
                    foreach (var item in action)
                    {
                        this.AddHeader(t =>
                        {
                            t.Key = item.Key;
                            t.Value = item.Value;
                        });
                    }
            }
            return this;
        }

        public INetFactory AddWhereHeader(bool condition, Action<DefaultHeader> action)
        {
            if (condition)
            {
                DefaultHeader head = new DefaultHeader();
                action.Invoke(head);
                if (!head.Key.IsNullOrEmpty())
                    Headers.Add(head);
            }
            return this;
        }

        public INetFactory AddNode(List<DefaultNodes> action)
        {
            if (action != null)
                foreach (var item in action)
                {
                    this.AddNode(t =>
                    {
                        t.Node = item.Node;
                        t.Method = item.Method;
                        t.MapFied = item.MapFied;
                        t.Parameter = item.Parameter;
                        t.Category = item.Category;
                        t.Encoding = item.Encoding;
                    });
                }
            return this;
        }

        public INetFactory AddNode(Action<DefaultNodes> action)
        {
            DefaultNodes node = new DefaultNodes();
            action.Invoke(node);
            node.SetNode();
            Nodes.Add(node);
            return this;
        }

        public INetFactory SetBaseUri(string Uri)
        {
            BaseUri = Uri;
            return this;
        }

        public INetFactory Build(Action<DefaultBuilder> action = null)
        {
            DefaultBuilder builder = new DefaultBuilder();
            action?.Invoke(builder);
            Builder = builder;
            BuildClient();
            return this;
        }

        public INetFactory GetCookie(Action<CookieContainer, Uri> action)
        {
            CookieHandler = action;
            return this;
        }

        public async Task<List<byte[]>> RunBytes()
        {
            List<byte[]> Result = new List<byte[]>();
            foreach (var item in Nodes)
            {
                if (Builder.UseCache)
                {
                    var Data = Caches.RunTimeCacheGet<byte[]>(item.Node.ToMd5());
                    if (Data == null)
                    {
                        Result.Add(await RequestBytes(item));
                        Caches.RunTimeCacheSet(item.Node.ToMd5(), Result.FirstOrDefault(), Builder.CacheSpan, false);
                    }
                    else
                        Result.Add(Data);
                }
                else
                    Result.Add(await RequestBytes(item));
            }
            return Result;
        }

        public async Task<List<string>> RunString()
        {
            List<string> Result = new List<string>();
            foreach (var item in Nodes)
            {
                if (Builder.UseCache)
                {
                    var Data = Caches.RunTimeCacheGet<string>(item.Node.ToMd5());
                    if (Data == null)
                    {
                        Result.Add(await RequestString(item));
                        Caches.RunTimeCacheSet(item.Node.ToMd5(), Result.FirstOrDefault(), Builder.CacheSpan, false);
                    }
                    else
                        Result.Add(Data);
                }
                else
                    Result.Add(await RequestString(item));
            }
            return Result;
        }

        public async Task<List<Stream>> RunStream()
        {
            List<Stream> Result = new List<Stream>();
            foreach (var item in Nodes)
            {
                if (Builder.UseCache)
                {
                    var Data = Caches.RunTimeCacheGet<Stream>(item.Node.ToMd5());
                    if (Data == null)
                    {
                        Result.Add(await RequestStream(item));
                        Caches.RunTimeCacheSet(item.Node.ToMd5(), Result.FirstOrDefault(), Builder.CacheSpan, false);
                    }
                    else
                        Result.Add(Data);
                }
                else
                    Result.Add(await RequestStream(item));
            }
            return Result;
        }

        #region  私有方法
        private void BuildClient()
        {
            var MessageHandle = NetFactoryExtension.GetService<IOptionsMonitor<HttpClientFactoryOptions>>()
                .Get(string.Empty).HttpMessageHandlerBuilderActions;
            MessageHandle.Clear();
            MessageHandle.Add(opt =>
                {
                    if (Builder.UseHandle)
                    {
                        var Handler = new HttpClientHandler
                        {
                            UseCookies = Builder.UseCookie,
                            CookieContainer = CookieContainer
                        };
                        if (Builder.IgnoreHttps)
                        {
                            Handler.SslProtocols = SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12;
                            Handler.ServerCertificateCustomValidationCallback = delegate { return true; };
                        }
                        if (Builder.Gzip)
                            Handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                        opt.PrimaryHandler = Handler;
                    }
                });
            Client = NetFactoryExtension.GetService<IHttpClientFactory>().CreateClient();
            Client.Timeout = Builder.Timeout;
            if (Nodes.Count <= 0)
            {
                HttpEvent.HttpActionEvent?.Invoke(Client, new ArgumentNullException("未调用AddNode"));
                return;
            }
            if (Builder.DelDefHeader)
            {
                Client.DefaultRequestHeaders.Clear();
                var Value = ConstDefault.GetPlatformAgentValue(Builder.PlatformHeader);
                Client.DefaultRequestHeaders.Add(ConstDefault.UserAgent, Value);
            }
            if (Headers.Count > 0)
                Headers.ForEach(item =>
                {
                    Client.DefaultRequestHeaders.Add(item.Key, item.Value);
                });
        }
        private async Task<byte[]> RequestBytes(DefaultNodes Item)
        {
            try
            {
                var route = Builder.UseBaseUri ? BaseUri + Item.Node : Item.Node;
                byte[] result = null;
                if (Item.Method == Method.GET)
                    result = await Client.GetAsync(route).Result.Content.ReadAsByteArrayAsync();
                else if (Item.Method == Method.DELETE)
                    result = await Client.DeleteAsync(route).Result.Content.ReadAsByteArrayAsync();
                else if (Item.Method == Method.POST)
                    result = await Client.PostAsync(route, Item.Contents).Result.Content.ReadAsByteArrayAsync();
                else
                    result = await Client.PutAsync(route, Item.Contents).Result.Content.ReadAsByteArrayAsync();
                CookieHandler?.Invoke(CookieContainer, new Uri(route));
                return result;
            }
            catch (Exception ex)
            {
                HttpEvent.HttpActionEvent?.Invoke(Client, ex);
                return Array.Empty<byte>();
            }
        }
        private async Task<string> RequestString(DefaultNodes Item)
        {
            try
            {
                var route = Builder.UseBaseUri ? BaseUri + Item.Node : Item.Node;
                Stream stream = null;
                if (Item.Method == Method.GET)
                    stream = await Client.GetAsync(route).Result.Content.ReadAsStreamAsync();
                else if (Item.Method == Method.DELETE)
                    stream = await Client.DeleteAsync(route).Result.Content.ReadAsStreamAsync();
                else if (Item.Method == Method.POST)
                    stream = await Client.PostAsync(route, Item.Contents).Result.Content.ReadAsStreamAsync();
                else
                    stream = await Client.PutAsync(route, Item.Contents).Result.Content.ReadAsStreamAsync();
                using StreamReader reader = new StreamReader(stream, Encoding.GetEncoding(Item.Encoding));
                string result = reader.ReadToEnd();
                CookieHandler?.Invoke(CookieContainer, new Uri(route));
                return result;
            }
            catch (Exception ex)
            {
                HttpEvent.HttpActionEvent?.Invoke(Client, ex);
                return string.Empty;
            }
        }
        private async Task<Stream> RequestStream(DefaultNodes Item)
        {
            try
            {
                var route = Builder.UseBaseUri ? BaseUri + Item.Node : Item.Node;
                Stream stream = null;
                if (Item.Method == Method.GET)
                    stream = await Client.GetAsync(route).Result.Content.ReadAsStreamAsync();
                else if (Item.Method == Method.DELETE)
                    stream = await Client.DeleteAsync(route).Result.Content.ReadAsStreamAsync();
                else if (Item.Method == Method.POST)
                    stream = await Client.PostAsync(route, Item.Contents).Result.Content.ReadAsStreamAsync();
                else
                    stream = await Client.PutAsync(route, Item.Contents).Result.Content.ReadAsStreamAsync();

                CookieHandler?.Invoke(CookieContainer, new Uri(route));
                return stream;
            }
            catch (Exception ex)
            {
                HttpEvent.HttpActionEvent?.Invoke(Client, ex);
                using var ms = new MemoryStream();
                return ms;
            }
        }
        #endregion
    }
}
