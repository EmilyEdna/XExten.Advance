using XExten.Advance.HttpFramework.MultiCommon;
using XExten.Advance.HttpFramework.MultiInterface;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace XExten.Advance.HttpFramework.MultiFactory
{
    /// <summary>
    /// 负载请求
    /// </summary>
    public interface IHttpMultiClient
    {
        /// <summary>
        /// Instance
        /// </summary>
        static IHttpMultiClient HttpMulti
        {
            get { return new Lazy<HttpMultiClient>().Value; }
        }
        /// <summary>
        /// 初始化Cookie容器
        /// </summary>
        /// <returns></returns>
        IHttpMultiClient InitCookieContainer();
        /// <summary>
        /// 使用代理
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IHttpMultiClient InitWebProxy(Action<ProxyURL> action);
        /// <summary>
        /// 使用代理
        /// </summary>
        /// <param name="Proxy"></param>
        /// <returns></returns>
        IHttpMultiClient InitWebProxy(ProxyURL Proxy);
        /// <summary>
        /// Add Header
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        IHeaders Header(string key, string value);
        /// <summary>
        /// Add Header
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        IHeaders Header(Dictionary<string, string> headers);
        /// <summary>
        /// Add Cookie
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        ICookies Cookie(string uri, Dictionary<string, string> pairs);
        /// <summary>
        /// Add Cookie
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="cookies"></param>
        /// <returns></returns>
        ICookies Cookie(Uri uri, CookieCollection cookies);
        /// <summary>
        /// Add Cookie
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="path"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        ICookies Cookie(string name, string value, string path, string domain);
        /// <summary>
        /// Add Path
        /// </summary>
        /// <param name="Path">请求地址</param>
        /// <param name="Type">请求类型</param>
        /// <param name="Encoding">编码格式</param>
        /// <param name="UseCache">使用缓存</param>
        /// <param name="Weight">1~100区间</param>
        /// <returns></returns>
        INode AddNode(string Path, RequestType Type = RequestType.GET, string Encoding = "UTF-8", bool UseCache = false, int Weight = 50);
        /// <summary>
        /// Add Path
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="Param"></param>
        ///  <param name="Type">请求类型</param>
        /// <param name="Encoding">编码格式</param>
        /// <param name="UseCache">使用缓存</param>
        /// <param name="Weight"></param>
        /// <returns></returns>
        INode AddNode(string Path, string Param, RequestType Type = RequestType.GET, string Encoding = "UTF-8", bool UseCache = false, int Weight = 50);
        /// <summary>
        /// Add Path
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="Param"></param>
        /// <param name="Type">请求类型</param>
        /// <param name="Encoding">编码格式</param>
        /// <param name="UseCache">使用缓存</param>
        /// <param name="Weight">1~100区间</param>
        /// <returns></returns>
        INode AddNode(string Path, List<KeyValuePair<String, String>> Param, RequestType Type = RequestType.GET, string Encoding = "UTF-8", bool UseCache = false, int Weight = 50);
        /// <summary>
        /// Add Path
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Path"></param>
        /// <param name="Param">实体模型</param>
        /// <param name="MapFied">映射字段</param>
        ///  <param name="Type">请求类型</param>
        /// <param name="Encoding">编码格式</param>
        /// <param name="UseCache">使用缓存</param>
        /// <param name="Weight">1~100区间</param>
        /// <returns></returns>
        INode AddNode<T>(string Path, T Param, IDictionary<string, string> MapFied = null, RequestType Type = RequestType.GET, string Encoding = "UTF-8", bool UseCache = false, int Weight = 50) where T : class, new();
    }
}
