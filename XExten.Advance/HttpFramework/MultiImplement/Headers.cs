using XExten.Advance.HttpFramework.MultiCommon;
using XExten.Advance.HttpFramework.MultiInterface;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace XExten.Advance.HttpFramework.MultiImplement
{
    /// <summary>
    /// Header
    /// </summary>
    public class Headers : IHeaders
    {

        /// <summary>
        /// 构建
        /// </summary>
        /// <param name="TimeOut">超时:秒</param>
        /// <param name="UseHttps"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public IBuilder Build(int TimeOut = 60, Boolean UseHttps = false, Action<HttpClientHandler> action = null)
        {
            return HttpMultiClientWare.Builder.Build(TimeOut, UseHttps);
        }

        /// <summary>
        /// Add Header
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public IHeaders Header(string key, string value)
        {
            HttpMultiClientWare.HeaderMaps.Add(new Dictionary<string, string>() { { key, value } });
            return HttpMultiClientWare.Headers;
        }

        /// <summary>
        /// Add Header
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        public IHeaders Header(Dictionary<string, string> headers)
        {
            HttpMultiClientWare.HeaderMaps.Add(headers);
            return HttpMultiClientWare.Headers;
        }

        /// <summary>
        /// Add Cookie
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public ICookies Cookie(string uri, Dictionary<string, string> pairs)
        {
            return HttpMultiClientWare.Cookies.Cookie(uri, pairs);
        }

        /// <summary>
        /// Add Cookie
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="cookies"></param>
        /// <returns></returns>
        public ICookies Cookie(Uri uri, CookieCollection cookies)
        {
            return HttpMultiClientWare.Cookies.Cookie(uri, cookies);
        }

        /// <summary>
        /// Add Cookie
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="path"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public ICookies Cookie(string name, string value, string path, string domain)
        {
            return HttpMultiClientWare.Cookies.Cookie(name, value, path, domain);
        }

        /// <summary>
        /// Add Path
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="Type"></param>
        /// <param name="Encoding"></param>
        /// <param name="UseCache"></param>
        /// <param name="Weight"></param>
        /// <returns></returns>
        public INode AddNode(string Path, RequestType Type = RequestType.GET, string Encoding = "UTF-8", bool UseCache = false, int Weight = 50)
        {
            return HttpMultiClientWare.Nodes.AddNode(Path, Type, Encoding, UseCache, Weight);
        }

        /// <summary>
        /// Add Path
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="Param"></param>
        /// <param name="Type"></param>
        /// <param name="Encoding"></param>
        /// <param name="UseCache"></param>
        /// <param name="Weight"></param>
        /// <returns></returns>
        public INode AddNode(string Path, string Param, RequestType Type = RequestType.GET, string Encoding = "UTF-8", bool UseCache = false, int Weight = 50)
        {
            return HttpMultiClientWare.Nodes.AddNode(Path, Param, Type, Encoding, UseCache, Weight);
        }

        /// <summary>
        /// Add Path
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="Param"></param>
        /// <param name="Type"></param>
        /// <param name="Encoding"></param>
        /// <param name="UseCache"></param>
        /// <param name="Weight"></param>
        /// <returns></returns>
        public INode AddNode(string Path, List<KeyValuePair<String, String>> Param, RequestType Type = RequestType.GET, string Encoding = "UTF-8", bool UseCache = false, int Weight = 50)
        {
            return HttpMultiClientWare.Nodes.AddNode(Path, Param, Type, Encoding, UseCache, Weight);
        }

        /// <summary>
        /// Add Path
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Path"></param>
        /// <param name="Param"></param>
        /// <param name="MapFied"></param>
        /// <param name="Type"></param>
        /// <param name="Encoding"></param>
        /// <param name="UseCache"></param>
        /// <param name="Weight"></param>
        /// <returns></returns>
        public INode AddNode<T>(string Path, T Param, IDictionary<string, string> MapFied = null, RequestType Type = RequestType.GET, string Encoding = "UTF-8", bool UseCache = false, int Weight = 50) where T : class, new()
        {
            return HttpMultiClientWare.Nodes.AddNode(Path, Param, MapFied, Type, Encoding, UseCache, Weight);
        }
    }
}
