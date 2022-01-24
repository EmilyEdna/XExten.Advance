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
using XExten.Advance.HttpFramework.MultiInterface;
using XExten.Advance.HttpFramework.MultiOption;
using XExten.Advance.LinqFramework;
using XExten.Advance.StaticFramework;

namespace XExten.Advance.HttpFramework.MultiImplement
{
    internal class Builders : IBuilders, IDisposable
    {

        private int CacheSecond = 60;
        private HttpClientHandler Handler(BuilderOption Option, Action<HttpClientHandler> action = null)
        {
            HttpClientHandler Handler = new HttpClientHandler();
            if (MultiConfig.Container != null)
            {
                Handler.AllowAutoRedirect = true;
                Handler.UseCookies = true;
                Handler.CookieContainer = MultiConfig.Container;
            }
            if (MultiConfig.Proxy != null)
                Handler.Proxy = MultiConfig.Proxy;
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

        public IBuilders Build(Action<BuilderOption> action = null, Action<HttpClientHandler> handle = null)
        {
            SyncStatic.TryCatch(() =>
            {
                BuilderOption Option = new BuilderOption();
                action?.Invoke(Option);
                CacheSecond = Option.CacheTime;
                HttpClient Client = new HttpClient(Handler(Option, handle))
                {
                    Timeout = new TimeSpan(0, 0, Option.TimeOut)
                };
                if (MultiConfig.HeaderOpt.Count != 0)
                    MultiConfig.HeaderOpt.ForEach(item =>
                    {
                        foreach (var KeyValuePair in item)
                        {
                            Client.DefaultRequestHeaders.Add(KeyValuePair.Key, KeyValuePair.Value);
                        }
                    });
                if (MultiConfig.Instance.ContainsKey(nameof(HttpClient)))
                    throw new Exception("请等待上一次执行完成后操作");
                MultiConfig.Instance[nameof(HttpClient)] = Client;
            }, ex => throw ex);
            return this;
        }

        #region 同步

        /// <summary>
        /// 执行 bytes
        /// </summary>
        /// <param name="Container"></param>
        /// <returns></returns>
        public List<Byte[]> RunBytes(Action<CookieContainer, Uri> Container = null)
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

        /// <summary>
        /// 执行 default UTF-8
        /// </summary>
        /// <param name="Container"></param>
        /// <returns></returns>
        public List<string> RunString(Action<CookieContainer, Uri> Container = null)
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

        /// <summary>
        /// 执行 default UTF-8
        /// </summary>
        /// <param name="Container"></param>
        /// <returns></returns>
        public string RunStringFirst(Action<CookieContainer, Uri> Container = null)
        {
            return RunString(Container).FirstOrDefault();
        }

        /// <summary>
        /// 执行 bytes
        /// </summary>
        /// <param name="Container"></param>
        /// <returns></returns>
        public Byte[] RunBytesFirst(Action<CookieContainer, Uri> Container = null)
        {
            return RunBytes(Container).FirstOrDefault();
        }
        #endregion

        #region 异步

        /// <summary>
        /// 执行 bytes
        /// </summary>
        /// <param name="Container"></param>
        /// <returns></returns>
        public async Task<List<Byte[]>> RunBytesAsync(Action<CookieContainer, Uri> Container = null)
        {
            return await Task.FromResult(RunBytes(Container));
        }

        /// <summary>
        /// 执行 default UTF-8
        /// </summary>
        /// <param name="Container"></param>
        /// <returns></returns>
        public async Task<List<string>> RunStringAsync(Action<CookieContainer, Uri> Container = null)
        {
            return await Task.FromResult(RunString(Container));
        }

        /// <summary>
        /// 执行 default UTF-8
        /// </summary>
        /// <param name="Container"></param>
        /// <returns></returns>
        public async Task<string> RunStringFirstAsync(Action<CookieContainer, Uri> Container = null)
        {
            return (await RunStringAsync(Container)).FirstOrDefault();
        }

        /// <summary>
        /// 执行 bytes
        /// </summary>
        /// <param name="Container"></param>
        /// <returns></returns>
        public async Task<Byte[]> RunBytesFirstAsync(Action<CookieContainer, Uri> Container = null)
        {
            return (await RunBytesAsync(Container)).FirstOrDefault();
        }
        #endregion

        #region 私有

        /// <summary>
        /// 请求
        /// </summary>
        /// <param name="Item"></param>
        /// <param name="Container"></param>
        /// <returns></returns>
        private Byte[] RequestBytes(NodeOption Item, Action<CookieContainer, Uri> Container = null)
        {
            byte[] result = null;
            if (Item.ReqType == MultiType.GET)
                result = MultiConfig.Instance.Values.FirstOrDefault().GetAsync(Item.URI).Result.Content.ReadAsByteArrayAsync().Result;
            else if (Item.ReqType == MultiType.DELETE)
                result = MultiConfig.Instance.Values.FirstOrDefault().DeleteAsync(Item.URI).Result.Content.ReadAsByteArrayAsync().Result;
            else if (Item.ReqType == MultiType.POST)
                result = MultiConfig.Instance.Values.FirstOrDefault().PostAsync(Item.URI, Item.Contents).Result.Content.ReadAsByteArrayAsync().Result;
            else
                result = MultiConfig.Instance.Values.FirstOrDefault().PutAsync(Item.URI, Item.Contents).Result.Content.ReadAsByteArrayAsync().Result;
            Container?.Invoke(MultiConfig.Container, Item.URI);
            return result;
        }

        /// <summary>
        /// 请求
        /// </summary>
        /// <param name="Item"></param>
        /// <param name="Container"></param>
        /// <returns></returns>
        private string RequestString(NodeOption Item, Action<CookieContainer, Uri> Container = null)
        {
            Stream stream = null;
            if (Item.ReqType == MultiType.GET)
                stream = MultiConfig.Instance.Values.FirstOrDefault().GetAsync(Item.URI).Result.Content.ReadAsStreamAsync().Result;
            else if (Item.ReqType == MultiType.DELETE)
                stream = MultiConfig.Instance.Values.FirstOrDefault().DeleteAsync(Item.URI).Result.Content.ReadAsStreamAsync().Result;
            else if (Item.ReqType == MultiType.POST)
                stream = MultiConfig.Instance.Values.FirstOrDefault().PostAsync(Item.URI, Item.Contents).Result.Content.ReadAsStreamAsync().Result;
            else
                stream = MultiConfig.Instance.Values.FirstOrDefault().PutAsync(Item.URI, Item.Contents).Result.Content.ReadAsStreamAsync().Result;
            if (stream.Length < 0) return null;
            using StreamReader reader = new StreamReader(stream, Encoding.GetEncoding(Item.Encoding));
            string result = reader.ReadToEnd();
            Container?.Invoke(MultiConfig.Container, Item.URI);
            return result;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            MultiConfig.Instance.Clear();
            MultiConfig.Container = null;
            MultiConfig.Proxy = null;
            MultiConfig.HeaderOpt.Clear();
            MultiConfig.NodeOpt.Clear();
        }

        #endregion
    }
}
