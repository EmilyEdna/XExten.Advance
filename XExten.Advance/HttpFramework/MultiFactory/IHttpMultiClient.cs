using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using XExten.Advance.HttpFramework.MultiCommon;
using XExten.Advance.HttpFramework.MultiOption;

namespace XExten.Advance.HttpFramework.MultiFactory
{
    /// <summary>
    /// Http
    /// </summary>
    public interface IHttpMultiClient
    {
        /// <summary>
        /// Instance
        /// </summary>
        static IHttpMultiClient HttpMulti => new HttpMultiClient();
        /// <summary>
        /// 使用代理
        /// </summary>
        /// <param name="proxy"></param>
        /// <returns></returns>
        IHttpMultiClient InitWebProxy(MultiProxy proxy);
        /// <summary>
        /// Header
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IHttpMultiClient AddHeader(Action<HeaderOption> action);
        /// <summary>
        /// Cookie
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IHttpMultiClient AddCookie(Action<CookieOption> action);
        /// <summary>
        /// Node
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IHttpMultiClient AddNode(Action<NodeOption> action);
        /// <summary>
        /// 构建
        /// </summary>
        /// <param name="action"></param>
        /// <param name="handle"></param>
        /// <returns></returns>
        IHttpMultiClient Build(Action<BuilderOption> action = null, Action<HttpClientHandler> handle = null);

        #region 执行
        /// <summary>
        /// 返回
        /// </summary>
        /// <returns></returns>
        List<string> RunString(Action<CookieContainer, Uri> Container = null, int IntervalTime = 10);
        /// <summary>
        /// 返回
        /// </summary>
        /// <returns></returns>
        string RunStringFirst(Action<CookieContainer, Uri> Container = null, int IntervalTime = 10);
        /// <summary>
        /// 返回
        /// </summary>
        /// <returns></returns>
        List<Byte[]> RunBytes(Action<CookieContainer, Uri> Container = null, int IntervalTime = 10);
        /// <summary>
        /// 返回
        /// </summary>
        /// <returns></returns>
        Byte[] RunBytesFirst(Action<CookieContainer, Uri> Container = null, int IntervalTime = 10);
        #endregion

        #region 异步执行
        /// <summary>
        /// 返回
        /// </summary>
        /// <returns></returns>
        Task<List<string>> RunStringAsync(Action<CookieContainer, Uri> Container = null, int IntervalTime = 10);
        /// <summary>
        /// 返回
        /// </summary>
        /// <returns></returns>
        Task<string> RunStringFirstAsync(Action<CookieContainer, Uri> Container = null, int IntervalTime = 10);
        /// <summary>
        /// 返回
        /// </summary>
        /// <returns></returns>
        Task<List<Byte[]>> RunBytesAsync(Action<CookieContainer, Uri> Container = null, int IntervalTime = 10);
        /// <summary>
        /// 返回
        /// </summary>
        /// <returns></returns>
        Task<Byte[]> RunBytesFirstAsync(Action<CookieContainer, Uri> Container = null, int IntervalTime = 10);
        #endregion
    }
}
