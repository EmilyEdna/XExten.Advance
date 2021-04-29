using Synctool.CacheFramework;
using Synctool.HttpFramework.MultiCommon;
using Synctool.HttpFramework.MultiInterface;
using Synctool.LinqFramework;
using Synctool.StaticFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace Synctool.HttpFramework.MultiImplement
{
    /// <summary>
    /// 构建器
    /// </summary>
    public class Builder : IBuilder, IDisposable
    {

        private static int CacheSecond = 30;

        private HttpClientHandler Handler(Boolean UseHttps = false) 
        {
            HttpClientHandler Handler = new HttpClientHandler();
            if (HttpMultiClientWare.Container != null)
            {
                Handler.AllowAutoRedirect = true;
                Handler.UseCookies = true;
                Handler.CookieContainer = HttpMultiClientWare.Container;
            }
            if (HttpMultiClientWare.Proxy != null)
            {
                Handler.Proxy = HttpMultiClientWare.Proxy;
            }
            if (UseHttps)
            {
                Handler.SslProtocols = SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12;
                Handler.ServerCertificateCustomValidationCallback = (Message, Certificate, Chain, Error) => true;
            }
            return Handler;
        }

        /// <summary>
        /// 构建
        /// </summary>
        /// <param name="TimeOut">超时:秒</param>
        /// <param name="UseHttps"></param>
        /// <returns></returns>
        public IBuilder Build(int TimeOut = 60, Boolean UseHttps = false)
        {
            if (HttpMultiClientWare.WeightPath.FirstOrDefault().URL == null)
                throw new Exception("Request address is not set!");
            SyncStatic.TryCatch(() =>
            {
                HttpClient Client = new HttpClient(Handler(UseHttps))
                {
                    Timeout = new TimeSpan(0, 0, TimeOut)
                };
                if (HttpMultiClientWare.HeaderMaps.Count != 0)
                    HttpMultiClientWare.HeaderMaps.ForEach(item =>
                    {
                        foreach (var KeyValuePair in item)
                        {
                            Client.DefaultRequestHeaders.Add(KeyValuePair.Key, KeyValuePair.Value);
                        }
                    });
                HttpMultiClientWare.FactoryClient = Client;
            }, ex => throw ex);
            return HttpMultiClientWare.Builder;
        }

        /// <summary>
        /// 设置缓存时间
        /// </summary>
        /// <param name="CacheSeconds">单位：Seconds</param>
        /// <returns></returns>
        public IBuilder CacheTime(int CacheSeconds = 60)
        {
            CacheSecond = CacheSeconds;
            return this;
        }

        /// <summary>
        /// 执行 bytes
        /// </summary>
        /// <param name="Container"></param>
        /// <param name="LoggerExcutor"></param>
        /// <returns></returns>
        public List<Byte[]> RunBytes(Action<CookieContainer, Uri> Container = null,Action<byte[], Stopwatch> LoggerExcutor = null)
        {
            List<Byte[]> Result = new List<Byte[]>();
            HttpMultiClientWare.WeightPath.OrderByDescending(t => t.Weight).ForEnumerEach(item =>
            {
                if (item.UseCache)
                {
                    var Data = Caches.RunTimeCacheGet<Byte[]>(item.URL.AbsoluteUri);
                    if (Data == null)
                    {
                        Result.Add(RequestBytes(item, Container,LoggerExcutor));
                        Caches.RunTimeCacheSet(item.URL.AbsoluteUri, Result.FirstOrDefault(), CacheSecond, true);
                    }
                    else
                        Result.Add(Data);
                }
                else
                    Result.Add(RequestBytes(item, Container, LoggerExcutor));
            });
            Dispose();
            return Result;
        }

        /// <summary>
        /// 执行 bytes
        /// </summary>
        /// <param name="Container"></param>
        /// <param name="LoggerExcutor"></param>
        /// <returns></returns>
        public async Task<List<Byte[]>> RunBytesAsync(Action<CookieContainer, Uri> Container = null,Action<byte[], Stopwatch> LoggerExcutor = null)
        {
            return await Task.FromResult(RunBytes(Container,LoggerExcutor));
        }

        /// <summary>
        /// 执行 default UTF-8
        /// </summary>
        /// <param name="Container"></param>
        /// <param name="LoggerExcutor"></param>
        /// <returns></returns>
        public List<string> RunString(Action<CookieContainer, Uri> Container = null,Action<string, Stopwatch> LoggerExcutor = null)
        {
            List<string> Result = new List<string>();
            HttpMultiClientWare.WeightPath.OrderByDescending(t => t.Weight).ForEnumerEach(item =>
            {
                if (item.UseCache)
                {
                    var Data = Caches.RunTimeCacheGet<string>(item.URL.AbsoluteUri);
                    if (Data.IsNullOrEmpty())
                    {
                        Result.Add(RequestString(item, Container, LoggerExcutor));
                        Caches.RunTimeCacheSet(item.URL.AbsoluteUri, Result.FirstOrDefault(), CacheSecond, true);
                    }
                    else
                        Result.Add(Data);
                }
                else
                    Result.Add(RequestString(item, Container, LoggerExcutor));
            });
            Dispose();
            return Result;
        }

        /// <summary>
        /// 执行 default UTF-8
        /// </summary>
        /// <param name="Container"></param>
        /// <param name="LoggerExcutor"></param>
        /// <returns></returns>
        public async Task<List<string>> RunStringAsync(Action<CookieContainer, Uri> Container = null,Action < String, Stopwatch> LoggerExcutor = null)
        {
            return await Task.FromResult(RunString(Container,LoggerExcutor));
        }

        /// <summary>
        /// 请求
        /// </summary>
        /// <param name="Item"></param>
        /// <param name="Container"></param>
        /// <param name="LoggerExcutor"></param>
        /// <returns></returns>
        private string RequestString(WeightURL Item, Action<CookieContainer, Uri> Container = null, Action<String, Stopwatch> LoggerExcutor = null)
        {
            if (Item.Request == RequestType.GET)
            {
                Stopwatch wath = new Stopwatch();
                wath.Start();
                Stream stream = HttpMultiClientWare.FactoryClient.GetAsync(Item.URL).Result.Content.ReadAsStreamAsync().Result;
                if (stream.Length < 0) return null;
                using StreamReader reader = new StreamReader(stream, Encoding.GetEncoding(Item.Encoding));
                string result = reader.ReadToEnd();
                wath.Stop();
                Container?.Invoke(HttpMultiClientWare.Container, Item.URL);
                LoggerExcutor?.Invoke(result, wath);
                return result;
            }
            else if (Item.Request == RequestType.DELETE)
            {
                Stopwatch wath = new Stopwatch();
                wath.Start();
                Stream stream = HttpMultiClientWare.FactoryClient.DeleteAsync(Item.URL).Result.Content.ReadAsStreamAsync().Result;
                if (stream.Length < 0) return null;
                using StreamReader reader = new StreamReader(stream, Encoding.GetEncoding(Item.Encoding));
                string result = reader.ReadToEnd();
                wath.Stop();
                 Container?.Invoke(HttpMultiClientWare.Container, Item.URL);
                LoggerExcutor?.Invoke(result, wath);
                return result;
            }
            else if (Item.Request == RequestType.POST)
            {
                Stopwatch wath = new Stopwatch();
                wath.Start();
                Stream stream = HttpMultiClientWare.FactoryClient.PostAsync(Item.URL,Item.Contents).Result.Content.ReadAsStreamAsync().Result;
                if (stream.Length < 0) return null;
                using StreamReader reader = new StreamReader(stream, Encoding.GetEncoding(Item.Encoding));
                string result = reader.ReadToEnd();
                wath.Stop();
                 Container?.Invoke(HttpMultiClientWare.Container, Item.URL);
                LoggerExcutor?.Invoke(result, wath);
                return result;
            }
            else
            {
                Stopwatch wath = new Stopwatch();
                wath.Start();
                Stream stream = HttpMultiClientWare.FactoryClient.PutAsync(Item.URL,Item.Contents).Result.Content.ReadAsStreamAsync().Result;
                if (stream.Length < 0) return null;
                using StreamReader reader = new StreamReader(stream, Encoding.GetEncoding(Item.Encoding));
                string result = reader.ReadToEnd();
                wath.Stop();
                 Container?.Invoke(HttpMultiClientWare.Container, Item.URL);
                LoggerExcutor?.Invoke(result, wath);
                return result;
            }
        }

        /// <summary>
        /// 请求
        /// </summary>
        /// <param name="Item"></param>
        /// <param name="Container"></param>
        /// <param name="LoggerExcutor"></param>
        /// <returns></returns>
        private Byte[] RequestBytes(WeightURL Item, Action<CookieContainer, Uri> Container = null,Action<byte[], Stopwatch> LoggerExcutor = null)
        {
            if (Item.Request == RequestType.GET)
            {
                Stopwatch wath = new Stopwatch();
                wath.Start();
                byte[] result = HttpMultiClientWare.FactoryClient.GetAsync(Item.URL).Result.Content.ReadAsByteArrayAsync().Result;
                wath.Stop();
                 Container?.Invoke(HttpMultiClientWare.Container, Item.URL);
                LoggerExcutor?.Invoke(result, wath);
                return result;
            }
            else if (Item.Request == RequestType.DELETE)
            {
                Stopwatch wath = new Stopwatch();
                wath.Start();
                byte[] result = HttpMultiClientWare.FactoryClient.DeleteAsync(Item.URL).Result.Content.ReadAsByteArrayAsync().Result;
                wath.Stop();
                 Container?.Invoke(HttpMultiClientWare.Container, Item.URL);
                LoggerExcutor?.Invoke(result, wath);
                return result;
            }
            else if (Item.Request == RequestType.POST)
            {
                Stopwatch wath = new Stopwatch();
                wath.Start();
                byte[] result = HttpMultiClientWare.FactoryClient.PostAsync(Item.URL, Item.Contents).Result.Content.ReadAsByteArrayAsync().Result;
                wath.Stop();
                 Container?.Invoke(HttpMultiClientWare.Container, Item.URL);
                LoggerExcutor?.Invoke(result, wath);
                return result;
            }
            else
            {
                Stopwatch wath = new Stopwatch();
                wath.Start();
                byte[] result = HttpMultiClientWare.FactoryClient.PutAsync(Item.URL, Item.Contents).Result.Content.ReadAsByteArrayAsync().Result;
                wath.Stop();
                 Container?.Invoke(HttpMultiClientWare.Container, Item.URL);
                LoggerExcutor?.Invoke(result, wath);
                return result;
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            HttpMultiClientWare.FactoryClient.Dispose();
            HttpMultiClientWare.Container = null;
            HttpMultiClientWare.HeaderMaps.Clear();
            HttpMultiClientWare.WeightPath.Clear();
        }
    }
}
