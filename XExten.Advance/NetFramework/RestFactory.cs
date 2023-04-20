using ImTools;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using NStandard.Evaluators;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
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
        public RestFactory()
        {
            Headers = new List<DefaultHeader>();
            Nodes = new List<DefaultNodes>();
            Builder = new DefaultBuilder();
            CookieContainer = new CookieContainer();
            Client = new RestClient();
            Request = new RestRequest();
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
            Options.MaxTimeout = (int)Builder.Timeout.TotalSeconds;
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
                RestClient client = new RestClient(this.Options);
                List<byte[]> Result = new List<byte[]>();
                foreach (var node in Nodes)
                {
                    if (Builder.UseCache)
                    {
                        var key = node.Node.ToMd5();
                        var result = Caches.RunTimeCacheGet<byte[]>(key);
                        if (result== null)
                        {
                            var response = await ConfigRequest(node);
                            Caches.RunTimeCacheSet(key, response, Builder.CacheSpan);
                            Result.Add(response);
                        }
                        else
                            Result.Add(result);
                    }
                    else
                    {
                        var response = await ConfigRequest(node);
           
                        Result.Add(response);
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
                RestClient client = new RestClient(this.Options);
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
                            var stream = new MemoryStream(response);
                            using StreamReader reader = new StreamReader(stream, Encoding.GetEncoding(node.Encoding));
                            result = reader.ReadToEnd();
                            await Caches.RunTimeCacheSetAsync(key, result, Builder.CacheSpan);
                            Result.Add(result);
                        }
                        else
                            Result.Add(result);
                    }
                    else
                    {
                        var response = await ConfigRequest(node);
                        var stream = new MemoryStream(response);
                        using StreamReader reader = new StreamReader(stream, Encoding.GetEncoding(node.Encoding));
                        var result = reader.ReadToEnd();
                        Result.Add(result);
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
                RestClient client = new RestClient(this.Options);
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
                            var stream = new MemoryStream(response);
                            Caches.RunTimeCacheSet(key, stream, Builder.CacheSpan);
                            Result.Add(stream);
                        }
                        else
                            Result.Add(result);
                    }
                    else
                    {
                        var response = await ConfigRequest(node);
                        var stream = new MemoryStream(response);
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
            if (Nodes.Count <= 0)
            {
                HttpEvent.RestActionEvent?.Invoke(Client, new ArgumentNullException("未调用AddNode"));
                return;
            }
            Request.AddHeader(ConstDefault.UserAgent, ConstDefault.UserAgentValue);
            if (Headers.Count > 0)
                Headers.ForEach(item =>
                {
                    Request.AddHeader(item.Key, item.Value);
                });
        }

        private async Task<byte[]> ConfigRequest(DefaultNodes Node)
        {
            switch (Node.Method)
            {
                case Enums.Method.GET:
                    this.Request.RequestFormat = DataFormat.None;
                    this.Request.Resource = Node.Node;
                    this.Request.Method = Method.Get;
                    break;
                case Enums.Method.POST:
                    this.Request.Resource = Node.Node;
                    this.Request.Method = Method.Post;
                    if (Node.Category == Enums.Category.Json)
                        this.Request.AddJsonBody(Node.Parameter);
                    if (Node.Category == Enums.Category.Form)
                        Node.Parameter.GetType().GetProperties().ForEnumerEach(item =>
                        {
                            var value = Node.Parameter.GetType().GetProperty(item.Name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).GetValue(Node.Parameter).ToString();
                            this.Request.AddParameter(item.Name, value);
                        });
                    break;
                case Enums.Method.PUT:
                    this.Request.Resource = Node.Node;
                    this.Request.Method = Method.Put;
                    if (Node.Category == Enums.Category.Json)
                        this.Request.AddJsonBody(Node.Parameter);
                    if (Node.Category == Enums.Category.Form)
                        Node.Parameter.GetType().GetProperties().ForEnumerEach(item =>
                        {
                            var value = Node.Parameter.GetType().GetProperty(item.Name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).GetValue(Node.Parameter).ToString();
                            this.Request.AddParameter(item.Name, value);
                        });
                    break;
                case Enums.Method.DELETE:
                    this.Request.RequestFormat = DataFormat.None;
                    this.Request.Resource = Node.Node;
                    this.Request.Method = Method.Delete;
                    break;
                default:
                    break;
            }

            var response = await Client.ExecuteAsync(this.Request);
            var Container = new CookieContainer();
            Container.Add(response.Cookies);
            CookieHandler.Invoke(Container, new Uri(Node.Node));
            return response.RawBytes;
        }
        #endregion
    }
}
