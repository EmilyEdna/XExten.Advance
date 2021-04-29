using Newtonsoft.Json.Linq;
using Synctool.HttpFramework.MultiCommon;
using Synctool.HttpFramework.MultiImplement;
using Synctool.HttpFramework.MultiInterface;
using Synctool.LinqFramework;
using Synctool.StaticFramework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Synctool.HttpFramework.MultiFactory
{
    /// <summary>
    /// 负载请求
    /// </summary>
    internal class HttpMultiClient : IHttpMultiClient
    {
        
        /// <summary>
        /// Constructor
        /// </summary>
        public HttpMultiClient()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            HttpMultiClientWare.Builder = new Builder();
            HttpMultiClientWare.Headers = new Headers();
            HttpMultiClientWare.Cookies = new Cookies();
            HttpMultiClientWare.Nodes = new Node();
        }

        #region Init

        /// <summary>
        /// 初始化Cookie容器
        /// </summary>
        /// <returns></returns>
        public IHttpMultiClient InitCookieContainer()
        {
            HttpMultiClientWare.Container = new CookieContainer();
            return this;
        }

        /// <summary>
        /// 使用代理
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public IHttpMultiClient InitWebProxy(Action<ProxyURL> action)
        {
            ProxyURL Proxy = new ProxyURL();
            action(Proxy);
            if (Proxy.IP.IsNullOrEmpty() || Proxy.Port == -1)
                return this;
            HttpMultiClientWare.Proxy = new WebProxy(Proxy.IP, Proxy.Port);
            if (!Proxy.UserName.IsNullOrEmpty() && !Proxy.PassWord.IsNullOrEmpty())
                HttpMultiClientWare.Proxy.Credentials = new NetworkCredential(Proxy.UserName, Proxy.PassWord);
            return this;
        }

        /// <summary>
        /// 使用代理
        /// </summary>
        /// <param name="Proxy"></param>
        /// <returns></returns>
        public IHttpMultiClient InitWebProxy(ProxyURL Proxy)
        {
            if (Proxy.IP.IsNullOrEmpty() || Proxy.Port == -1)
                return this;
            HttpMultiClientWare.Proxy = new WebProxy(Proxy.IP, Proxy.Port);
            if (!Proxy.UserName.IsNullOrEmpty() && !Proxy.PassWord.IsNullOrEmpty())
                HttpMultiClientWare.Proxy.Credentials = new NetworkCredential(Proxy.UserName, Proxy.PassWord);
            return this;
        }

        #endregion Init

        #region Header
        /// <summary>
        /// Add Header
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public IHeaders Headers(string key, string value)
        {
            HttpMultiClientWare.HeaderMaps.Add(new Dictionary<string, string>() { { key, value } });
            return HttpMultiClientWare.Headers;
        }
        /// <summary>
        /// Add Header
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        public IHeaders Headers(Dictionary<string, string> headers)
        {
            HttpMultiClientWare.HeaderMaps.Add(headers);
            return HttpMultiClientWare.Headers;
        }
        #endregion Header

        #region Cookie

        /// <summary>
        /// Add Cookie
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public ICookies Cookie(string uri, Dictionary<string, string> pairs)
        {
            pairs.ForDicEach((key, val) =>
            {
                HttpMultiClientWare.Container.Add(new Uri(uri), new Cookie(key, val));
            });
            return HttpMultiClientWare.Cookies;
        }

        /// <summary>
        /// Add Cookie
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="cookies"></param>
        /// <returns></returns>
        public ICookies Cookie(Uri uri, CookieCollection cookies)
        {
            HttpMultiClientWare.Container.Add(uri, cookies);
            return HttpMultiClientWare.Cookies;
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
            Cookie Cookie = new Cookie(name, value, path, domain);
            if (HttpMultiClientWare.Container == null) throw new NullReferenceException("Please initialize the InitCookieContainer method before calling the cookie method");
            HttpMultiClientWare.Container.Add(Cookie);
            return HttpMultiClientWare.Cookies;
        }
        #endregion Cookie

        #region URL
        /// <summary>
        /// Add Path
        /// </summary>
        /// <param name="Path">请求地址</param>
        /// <param name="Type">请求类型</param>
        /// <param name="Encoding">编码格式</param>
        /// <param name="UseCache">使用缓存</param>
        /// <param name="Weight">1~100区间</param>
        /// <returns></returns>
        public INode AddNode(string Path, RequestType Type = RequestType.GET, string Encoding = "UTF-8", bool UseCache = false, int Weight = 50)
        {
            WeightURL WeightUri = new WeightURL
            {
                Weight = Weight,
                URL = new Uri(Path),
                Request = Type,
                UseCache = UseCache,
                Encoding = Encoding
            };
            HttpMultiClientWare.WeightPath.Add(WeightUri);
            return HttpMultiClientWare.Nodes;
        }

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
        public INode AddNode(string Path, string Param, RequestType Type = RequestType.GET, string Encoding = "UTF-8", bool UseCache = false, int Weight = 50)
        {

            WeightURL WeightUri = new WeightURL
            {
                Weight = Weight,
                URL = new Uri(Path + ((Type == RequestType.GET || Type == RequestType.DELETE) ? Param.ToModel<JObject>().ByUri() : string.Empty)),
                Request = Type,
                Contents = (Type == RequestType.GET || Type == RequestType.DELETE) ? null : new StringContent(Param),
                UseCache = UseCache,
                Encoding = Encoding,
                MediaTypeHeader = (Type == RequestType.GET || Type == RequestType.DELETE) ? null : new MediaTypeHeaderValue("application/json")
            };
            HttpMultiClientWare.WeightPath.Add(WeightUri);
            return HttpMultiClientWare.Nodes;
        }

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
        public INode AddNode(string Path, List<KeyValuePair<String, String>> Param, RequestType Type = RequestType.GET, string Encoding = "UTF-8", bool UseCache = false, int Weight = 50)
        {
            return SyncStatic.TryCatch(() =>
             {
                 WeightURL WeightUri = new WeightURL
                 {
                     Weight = Weight,
                     URL = new Uri(Path + ((Type == RequestType.GET || Type == RequestType.DELETE) ? Param.ByUri() : string.Empty)),
                     Request = Type,
                     Contents = (Type == RequestType.GET || Type == RequestType.DELETE) ? null : new FormUrlEncodedContent(Param),
                     UseCache = UseCache,
                     Encoding = Encoding,
                     MediaTypeHeader = (Type == RequestType.GET || Type == RequestType.DELETE) ? null : new MediaTypeHeaderValue("application/x-www-form-urlencoded")
                 };
                 HttpMultiClientWare.WeightPath.Add(WeightUri);
                 return HttpMultiClientWare.Nodes;
             }, (Ex) => throw new Exception("The parameter type is incorrect. The parameter can only be a solid model."));
        }

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
        public INode AddNode<T>(string Path, T Param, IDictionary<string, string> MapFied = null, RequestType Type = RequestType.GET, string Encoding = "UTF-8", bool UseCache = false, int Weight = 50) where T : class, new()
        {
            return SyncStatic.TryCatch(() =>
            {
                WeightURL WeightUri = new WeightURL
                {
                    Weight = Weight,
                    URL = new Uri(Path + ((Type == RequestType.GET || Type == RequestType.DELETE) ? HttpKeyPairs.KeyValuePairs(Param, MapFied).ByUri() : string.Empty)),
                    Request = Type,
                    UseCache = UseCache,
                    Encoding = Encoding,
                    Contents = (Type == RequestType.GET || Type == RequestType.DELETE) ? null : new FormUrlEncodedContent(HttpKeyPairs.KeyValuePairs(Param, MapFied)),
                    MediaTypeHeader = (Type == RequestType.GET || Type == RequestType.DELETE) ? null : new MediaTypeHeaderValue("application/x-www-form-urlencoded")
                };
                HttpMultiClientWare.WeightPath.Add(WeightUri);
                return HttpMultiClientWare.Nodes;
            }, (Ex) => throw new Exception("The parameter type is incorrect. The parameter can only be a solid model."));
        }
        #endregion
    }
}
