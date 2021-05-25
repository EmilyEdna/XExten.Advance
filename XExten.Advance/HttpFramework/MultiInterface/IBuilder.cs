using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace XExten.Advance.HttpFramework.MultiInterface
{
    /// <summary>
    /// 构建
    /// </summary>
    public interface IBuilder
    {
        /// <summary>
        /// 构建
        /// </summary>
        /// <param name="TimeOut">超时:秒</param>
        /// <param name="UseHttps"></param>
        /// <param name="UseDnsResolver"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        IBuilder Build(int TimeOut = 60, Boolean UseHttps = false, Boolean UseDnsResolver = true, Action<HttpClientHandler> action = null);
        /// <summary>
        /// 设置DNS解析器
        /// </summary>
        /// <param name="Resolver"></param>
        /// <returns></returns>
        IBuilder SetResolver(IResolver Resolver = null);
        /// <summary>
        /// 设置缓存时间
        /// </summary>
        /// <param name="CacheSeconds"></param>
        /// <returns></returns>
        IBuilder CacheTime(int CacheSeconds = 60);
        /// <summary>
        /// 执行 default UTF-8
        /// </summary>
        /// <param name="Container"></param>
        /// <param name="LoggerExcutor"></param>
        /// <returns></returns>
        List<String> RunString(Action<CookieContainer, Uri> Container = null, Action<String, Stopwatch> LoggerExcutor = null);
        /// <summary>
        /// 执行 default UTF-8
        /// </summary>
        /// <param name="Container"></param>
        /// <param name="LoggerExcutor"></param>
        /// <returns></returns>
        Task<List<String>> RunStringAsync(Action<CookieContainer, Uri> Container = null, Action<String, Stopwatch> LoggerExcutor = null);
        /// <summary>
        /// 执行 bytes
        /// </summary>
        /// <param name="Container"></param>
        /// <param name="LoggerExcutor"></param>
        /// <returns></returns>
        List<Byte[]> RunBytes(Action<CookieContainer, Uri> Container = null, Action<Byte[], Stopwatch> LoggerExcutor = null);
        /// <summary>
        /// 执行 bytes
        /// </summary>
        /// <param name="Container"></param>
        /// <param name="LoggerExcutor"></param>
        /// <returns></returns>
        Task<List<Byte[]>> RunBytesAsync(Action<CookieContainer, Uri> Container = null, Action<Byte[], Stopwatch> LoggerExcutor = null);
    }
}
