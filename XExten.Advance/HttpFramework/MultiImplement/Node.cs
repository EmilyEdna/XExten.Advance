using Newtonsoft.Json.Linq;
using XExten.Advance.HttpFramework.MultiCommon;
using XExten.Advance.HttpFramework.MultiInterface;
using XExten.Advance.LinqFramework;
using XExten.Advance.StaticFramework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace XExten.Advance.HttpFramework.MultiImplement
{
    /// <summary>
    /// URL
    /// </summary>
    public class Node : INode
    {
        /// <summary>
        /// 构建
        /// </summary>
        /// <param name="TimeOut">超时:秒</param>
        /// <param name="UseHttps"></param>
        /// <returns></returns>
        public IBuilder Build(int TimeOut = 60, Boolean UseHttps = false)
        {
            return HttpMultiClientWare.Builder.Build(TimeOut, UseHttps);
        }
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
                    URL = new Uri(Path + ((Type == RequestType.GET || Type == RequestType.DELETE) ? Param.ByUri() : string.Empty)),
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
        /// Add Header
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public IHeaders Header(string key, string value)
        {
            return HttpMultiClientWare.Headers.Header(key, value);
        }

        /// <summary>
        /// Add Header
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        public IHeaders Header(Dictionary<string, string> headers)
        {
            return HttpMultiClientWare.Headers.Header(headers);
        }
    }
}
