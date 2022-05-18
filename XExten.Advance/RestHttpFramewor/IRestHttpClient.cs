using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using XExten.Advance.RestHttpFramewor.Options;

namespace XExten.Advance.RestHttpFramewor
{
    /// <summary>
    /// RestClient
    /// </summary>
    public interface IRestHttpClient
    {
        /// <summary>
        /// 创建实例
        /// </summary>
        static IRestHttpClient Rest => new Lazy<RestHttpClient>().Value;
        /// <summary>
        /// 使用代理
        /// </summary>
        /// <param name="Proxy"></param>
        /// <returns></returns>
        IRestHttpClient UseProxy(RestProxy Proxy);
        /// <summary>
        /// 忽略HTTPS证书
        /// </summary>
        /// <returns></returns>
        IRestHttpClient UseHttps();
        /// <summary>
        /// 启用数据压缩
        /// </summary>
        /// <returns></returns>
        IRestHttpClient UseZip();
        /// <summary>
        /// 使用Cookie
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IRestHttpClient UseCookie(Action<RestCookie> action);
        /// <summary>
        /// 使用头
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IRestHttpClient UseHeader(Action<RestHeader> action);
        /// <summary>
        /// 创建节点
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IRestHttpClient UseNode(Action<RestNode> action);
        /// <summary>
        /// 构建
        /// </summary>
        /// <returns></returns>
        IRestHttpClient Build();
        #region 异步
        /// <summary>
        /// 返回集合
        /// </summary>
        /// <param name="action"></param>
        /// <param name="RetryTimes"></param>
        /// <param name="IntervalTime"></param>
        /// <returns></returns>
        Task<List<string>> RunStringAsync(Action<RestResponse> action = null,int RetryTimes = 3, int IntervalTime = 10);
        /// <summary>
        /// 返回单个
        /// </summary>
        /// <param name="action"></param>
        /// <param name="RetryTimes"></param>
        /// <param name="IntervalTime"></param>
        /// <returns></returns>
        Task<string> RunStringFirstAsync(Action<RestResponse> action = null, int RetryTimes = 3, int IntervalTime = 10);
        /// <summary>
        /// 返回集合
        /// </summary>
        /// <param name="action"></param>
        /// <param name="RetryTimes"></param>
        /// <param name="IntervalTime"></param>
        /// <returns></returns>
        Task<List<byte[]>> RunByteAsync(Action<RestResponse> action = null, int RetryTimes = 3, int IntervalTime = 10);
        /// <summary>
        /// 返回单个
        /// </summary>
        /// <param name="action"></param>
        /// <param name="RetryTimes"></param>
        /// <param name="IntervalTime"></param>
        /// <returns></returns>
        Task<byte[]> RunByteFirstAsync(Action<RestResponse> action = null, int RetryTimes = 3, int IntervalTime = 10);
        #endregion
        #region 同步
        /// <summary>
        /// 返回集合
        /// </summary>
        /// <param name="action"></param>
        /// <param name="RetryTimes"></param>
        /// <param name="IntervalTime"></param>
        /// <returns></returns>
        List<string> RunString(Action<RestResponse> action = null, int RetryTimes = 3, int IntervalTime = 10);
        /// <summary>
        /// 返回单个
        /// </summary>
        /// <param name="action"></param>
        /// <param name="RetryTimes"></param>
        /// <param name="IntervalTime"></param>
        /// <returns></returns>
        string RunStringFirst(Action<RestResponse> action = null, int RetryTimes = 3, int IntervalTime = 10);
        /// <summary>
        /// 返回集合
        /// </summary>
        /// <param name="action"></param>
        /// <param name="RetryTimes"></param>
        /// <param name="IntervalTime"></param>
        /// <returns></returns>
        List<byte[]> RunByte(Action<RestResponse> action = null, int RetryTimes = 3, int IntervalTime = 10);
        /// <summary>
        /// 返回单个
        /// </summary>
        /// <param name="action"></param>
        /// <param name="RetryTimes"></param>
        /// <param name="IntervalTime"></param>
        /// <returns></returns>
        byte[] RunByteFirst(Action<RestResponse> action = null, int RetryTimes = 3, int IntervalTime = 10);
        #endregion
    }
}
