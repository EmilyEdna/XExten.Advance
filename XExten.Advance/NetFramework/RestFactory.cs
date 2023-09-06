using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using XExten.Advance.CacheFramework;
using XExten.Advance.LinqFramework;
using XExten.Advance.NetFramework.Options;

namespace XExten.Advance.NetFramework
{
    internal class RestFactory : INetFactory
    {
        private CookieContainer CookieContainer;
        private List<DefaultHeader> Headers;
        private List<DefaultNodes> Nodes;
        private DefaultBuilder Builder;
        private string BaseUri;
        private RestClientOptions Options;
        private Action<CookieContainer, Uri> CookieHandler;
        private RestClient Client;
        private RestRequest Request;
        private WebProxy Proxy;
        public RestFactory()
        {
            Headers = new List<DefaultHeader>();
            Nodes = new List<DefaultNodes>();
            Builder = new DefaultBuilder();
            CookieContainer = new CookieContainer();
            Options = new RestClientOptions();
            Request = new RestRequest();
        }

        public INetFactory AddProxy(string IP, int Port)
        {
            Proxy = new WebProxy(IP, Port);
            return this;
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
            try
            {
                List<byte[]> Result = new List<byte[]>();
                foreach (var node in Nodes)
                {
                    if (Builder.UseCache)
                    {
                        var key = node.Node.ToMd5();
                        var result = Caches.RunTimeCacheGet<byte[]>(key);
                        if (result == null)
                        {
                            var response = await ConfigRequest(node);
                            Caches.RunTimeCacheSet(key, response.RawBytes, Builder.CacheSpan);
                            Result.Add(response.RawBytes);
                        }
                        else
                            Result.Add(result);
                    }
                    else
                    {
                        var response = await ConfigRequest(node);
                        Result.Add(response.RawBytes);
                    }
                }

                return Result;
            }
            catch (Exception ex)
            {
                HttpEvent.RestActionEvent?.Invoke(Client, ex);
                return new List<byte[]>();
            }
        }

        public async Task<List<string>> RunString()
        {
            try
            {
                List<string> Result = new List<string>();
                foreach (var node in Nodes)
                {
                    if (Builder.UseCache)
                    {
                        var key = node.Node.ToMd5();
                        var result = Caches.RunTimeCacheGet<string>(key);
                        if (result.IsNullOrEmpty())
                        {
                            var response = await ConfigRequest(node);
                            result = response.Content;
                            await Caches.RunTimeCacheSetAsync(key, result, Builder.CacheSpan);
                            Result.Add(result);
                        }
                        else
                            Result.Add(result);
                    }
                    else
                    {
                        var response = await ConfigRequest(node);
                        Result.Add(response.Content);
                    }
                }

                return Result;
            }
            catch (Exception ex)
            {
                HttpEvent.RestActionEvent?.Invoke(Client, ex);
                return new List<string>();
            }
        }

        public async Task<List<Stream>> RunStream()
        {
            try
            {
                List<Stream> Result = new List<Stream>();
                foreach (var node in Nodes)
                {
                    if (Builder.UseCache)
                    {
                        var key = node.Node.ToMd5();
                        var result = Caches.RunTimeCacheGet<Stream>(key);
                        if (result == null)
                        {
                            var response = await ConfigRequest(node);
                            var stream = new MemoryStream(response.RawBytes);
                            Caches.RunTimeCacheSet(key, stream, Builder.CacheSpan);
                            Result.Add(stream);
                        }
                        else
                            Result.Add(result);
                    }
                    else
                    {
                        var response = await ConfigRequest(node);
                        var stream = new MemoryStream(response.RawBytes);
                        Result.Add(stream);
                    }
                }

                return Result;
            }
            catch (Exception ex)
            {
                HttpEvent.RestActionEvent?.Invoke(Client, ex);
                return new List<Stream>();
            }
        }

        #region  私有方法
        private void BuildClient()
        {

            if (Builder.UseCookie)
                Options.CookieContainer = CookieContainer;
            if (Builder.UseBaseUri)
                Options.BaseUrl = new Uri(BaseUri);
            if (Builder.IgnoreHttps)
                Options.RemoteCertificateValidationCallback = delegate { return true; };
            if (Builder.Gzip)
                Options.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            if (Proxy != null)
                Options.Proxy = Proxy;
            if (Nodes.Count <= 0)
            {
                HttpEvent.RestActionEvent?.Invoke(Client, new ArgumentNullException("未调用AddNode"));
                return;
            }
            Request.Timeout = (int)Builder.Timeout.TotalSeconds * 1000;
            Request.AddHeader(ConstDefault.UserAgent, ConstDefault.GetPlatformAgentValue());
            if (Builder.DelDefHeader)
            {
                var Value = ConstDefault.GetPlatformAgentValue(Builder.PlatformHeader);
                Request.AddOrUpdateHeader(ConstDefault.UserAgent, Value);
            }
            if (Headers.Count > 0)
                Headers.ForEach(item =>
                {
                    Request.AddHeader(item.Key, item.Value);
                });
        }

        private async Task<RestResponse> ConfigRequest(DefaultNodes Node)
        {
            Options.Encoding = Encoding.GetEncoding(Node.Encoding);
            Client = new RestClient(this.Options);
            switch (Node.Method)
            {
                case Enums.Method.GET:
                    Request.RequestFormat = DataFormat.None;
                    Request.Resource = Node.Node;
                    Request.Method = Method.Get;
                    break;
                case Enums.Method.POST:
                    Request.Resource = Node.Node;
                    Request.Method = Method.Post;
                    if (Node.Category == Enums.Category.Json)
                        Request.AddJsonBody(Node.Parameter);
                    if (Node.Category == Enums.Category.Form)
                        if (Node.Parameter is List<KeyValuePair<String, String>> Target)
                            Target.ForEach(item =>
                            {
                                Request.AddParameter(item.Key, item.Value);
                            });
                        else
                            Node.Parameter.GetType().GetProperties().ForEnumerEach(item =>
                            {
                                var value = Node.Parameter.GetType().GetProperty(item.Name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).GetValue(Node.Parameter).ToString();
                                Request.AddParameter(item.Name, value);
                            });
                    break;
                case Enums.Method.PUT:
                    Request.Resource = Node.Node;
                    Request.Method = Method.Put;
                    if (Node.Category == Enums.Category.Json)
                        Request.AddJsonBody(Node.Parameter);
                    if (Node.Category == Enums.Category.Form)
                        if (Node.Parameter is List<KeyValuePair<String, String>> Target)
                            Target.ForEach(item =>
                            {
                                Request.AddParameter(item.Key, item.Value);
                            });
                        else
                            Node.Parameter.GetType().GetProperties().ForEnumerEach(item =>
                            {
                                var value = Node.Parameter.GetType().GetProperty(item.Name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).GetValue(Node.Parameter).ToString();
                            });
                    break;
                case Enums.Method.DELETE:
                    Request.RequestFormat = DataFormat.None;
                    Request.Resource = Node.Node;
                    Request.Method = Method.Delete;
                    break;
                default:
                    break;
            }
            var response = await Client.ExecuteAsync(Request);
            if (CookieHandler != null)
            {
                var Container = new CookieContainer();
                Container.Add(response.Cookies);
                CookieHandler.Invoke(Container, new Uri(Node.Node));
            }
            return response;
        }
        #endregion
    }
}
