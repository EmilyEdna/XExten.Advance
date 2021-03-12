using Synctool.CacheFramework;
using Synctool.HttpFramework.MultiInterface;
using Synctool.LinqFramework;
using Synctool.StaticFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace Synctool.HttpFramework.MultiImplement
{
    /// <summary>
    /// 构建器
    /// </summary>
    public class Builder : IBuilder, IDisposable
    {

        private static int CacheSecond = 30;

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
                if (HttpMultiClientWare.Container != null)
                {
                    HttpClientHandler Handler = new HttpClientHandler
                    {
                        AllowAutoRedirect = true,
                        UseCookies = true,
                        CookieContainer = HttpMultiClientWare.Container
                    };
                    if (UseHttps)
                    {
                        Handler.SslProtocols = SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12;
                        Handler.ServerCertificateCustomValidationCallback = (Message, Certificate, Chain, Error) => true;
                    }
                    HttpClient Client = new HttpClient(Handler);
                    if (HttpMultiClientWare.HeaderMaps.Count != 0)
                        HttpMultiClientWare.HeaderMaps.ForEach(item =>
                        {
                            foreach (var KeyValuePair in item)
                            {
                                Client.DefaultRequestHeaders.Add(KeyValuePair.Key, KeyValuePair.Value);
                            }
                        });
                    HttpMultiClientWare.FactoryClient = Client;
                }
                else
                {
                    HttpClient Client = null;
                    if (UseHttps)
                    {
                        HttpClientHandler Handler = new HttpClientHandler
                        {
                            SslProtocols = SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12,
                            ServerCertificateCustomValidationCallback = (Message, Certificate, Chain, Error) => true,
                        };
                        Client = new HttpClient(Handler);
                    }
                    else
                        Client = new HttpClient();
                    if (HttpMultiClientWare.HeaderMaps.Count != 0)
                        HttpMultiClientWare.HeaderMaps.ForEach(item =>
                        {
                            foreach (var KeyValuePair in item)
                            {
                                Client.DefaultRequestHeaders.Add(KeyValuePair.Key, KeyValuePair.Value);
                            }
                        });
                    HttpMultiClientWare.FactoryClient = Client;
                }
                HttpMultiClientWare.FactoryClient.Timeout = new TimeSpan(0, 0, TimeOut);
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
        /// 执行
        /// </summary>
        /// <param name="LoggerExcutor"></param>
        /// <returns></returns>
        public List<Byte[]> RunBytes(Action<byte[], Stopwatch> LoggerExcutor = null)
        {
            List<Byte[]> Result = new List<Byte[]>();
            HttpMultiClientWare.WeightPath.OrderByDescending(t => t.Weight).ForEnumerEach(item =>
            {
                if (item.UseCache)
                {
                    var Data = Caches.RunTimeCacheGet<Byte[]>(item.URL.AbsoluteUri);
                    if (Data == null)
                    {
                        Result.Add(RequestBytes(item, LoggerExcutor));
                        Caches.RunTimeCacheSet(item.URL.AbsoluteUri, Result.FirstOrDefault(), CacheSecond, true);
                    }
                    else
                        Result.Add(Data);
                }
                else
                    Result.Add(RequestBytes(item, LoggerExcutor));
            });
            Dispose();
            return Result;
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="LoggerExcutor"></param>
        /// <returns></returns>
        public async Task<List<Byte[]>> RunBytesAsync(Action<byte[], Stopwatch> LoggerExcutor = null)
        {
            return await Task.FromResult(RunBytes(LoggerExcutor));
        }

        /// <summary>
        /// 执行 default UTF-8
        /// </summary>
        /// <param name="LoggerExcutor"></param>
        /// <returns></returns>
        public List<string> RunString(Action<string, Stopwatch> LoggerExcutor = null)
        {
            List<string> Result = new List<string>();
            HttpMultiClientWare.WeightPath.OrderByDescending(t => t.Weight).ForEnumerEach(item =>
            {
                if (item.UseCache)
                {
                    var Data = Caches.RunTimeCacheGet<string>(item.URL.AbsoluteUri);
                    if (Data.IsNullOrEmpty())
                    {
                        Result.Add(RequestString(item, LoggerExcutor));
                        Caches.RunTimeCacheSet(item.URL.AbsoluteUri, Result.FirstOrDefault(), CacheSecond, true);
                    }
                    else
                        Result.Add(Data);
                }
                else
                    Result.Add(RequestString(item, LoggerExcutor));
            });
            Dispose();
            return Result;
        }

        /// <summary>
        /// 执行 default UTF-8
        /// </summary>
        /// <param name="LoggerExcutor"></param>
        /// <returns></returns>
        public async Task<List<string>> RunStringAsync(Action<String, Stopwatch> LoggerExcutor = null)
        {
            return await Task.FromResult(RunString(LoggerExcutor));
        }

        /// <summary>
        /// 请求
        /// </summary>
        /// <param name="Item"></param>
        /// <param name="LoggerExcutor"></param>
        /// <returns></returns>
        private string RequestString(WeightURL Item, Action<String, Stopwatch> LoggerExcutor = null)
        {
            if (Item.Request == RequestType.GET)
            {
                Stopwatch wath = new Stopwatch();
                wath.Start();
                string result = HttpMultiClientWare.FactoryClient.GetAsync(Item.URL).Result.Content.ReadAsStringAsync().Result;
                wath.Stop();
                LoggerExcutor?.Invoke(result, wath);
                return result;
            }
            else if (Item.Request == RequestType.DELETE)
            {
                Stopwatch wath = new Stopwatch();
                wath.Start();
                string result = HttpMultiClientWare.FactoryClient.DeleteAsync(Item.URL).Result.Content.ReadAsStringAsync().Result;
                wath.Stop();
                LoggerExcutor?.Invoke(result, wath);
                return result;
            }
            else if (Item.Request == RequestType.POST)
            {
                Stopwatch wath = new Stopwatch();
                wath.Start();
                string result = HttpMultiClientWare.FactoryClient.PostAsync(Item.URL, Item.Contents).Result.Content.ReadAsStringAsync().Result;
                wath.Stop();
                LoggerExcutor?.Invoke(result, wath);
                return result;
            }
            else
            {
                Stopwatch wath = new Stopwatch();
                wath.Start();
                string result = HttpMultiClientWare.FactoryClient.PutAsync(Item.URL, Item.Contents).Result.Content.ReadAsStringAsync().Result;
                wath.Stop();
                LoggerExcutor?.Invoke(result, wath);
                return result;
            }
        }

        /// <summary>
        /// 请求
        /// </summary>
        /// <param name="Item"></param>
        /// <param name="LoggerExcutor"></param>
        /// <returns></returns>
        private Byte[] RequestBytes(WeightURL Item, Action<byte[], Stopwatch> LoggerExcutor = null)
        {
            if (Item.Request == RequestType.GET)
            {
                Stopwatch wath = new Stopwatch();
                wath.Start();
                byte[] result = HttpMultiClientWare.FactoryClient.GetAsync(Item.URL).Result.Content.ReadAsByteArrayAsync().Result;
                wath.Stop();
                LoggerExcutor?.Invoke(result, wath);
                return result;
            }
            else if (Item.Request == RequestType.DELETE)
            {
                Stopwatch wath = new Stopwatch();
                wath.Start();
                byte[] result = HttpMultiClientWare.FactoryClient.DeleteAsync(Item.URL).Result.Content.ReadAsByteArrayAsync().Result;
                wath.Stop();
                LoggerExcutor?.Invoke(result, wath);
                return result;
            }
            else if (Item.Request == RequestType.POST)
            {
                Stopwatch wath = new Stopwatch();
                wath.Start();
                byte[] result = HttpMultiClientWare.FactoryClient.PostAsync(Item.URL, Item.Contents).Result.Content.ReadAsByteArrayAsync().Result;
                wath.Stop();
                LoggerExcutor?.Invoke(result, wath);
                return result;
            }
            else
            {
                Stopwatch wath = new Stopwatch();
                wath.Start();
                byte[] result = HttpMultiClientWare.FactoryClient.PutAsync(Item.URL, Item.Contents).Result.Content.ReadAsByteArrayAsync().Result;
                wath.Stop();
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
