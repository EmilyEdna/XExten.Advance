using XExten.Advance.HttpFramework.MultiCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace XExten.Advance.HttpFramework.MultiInterface
{
    /// <summary>
    /// 
    /// </summary>
    public interface INode
    {
        /// <summary>
        /// 构建
        /// </summary>
        /// <param name="TimeOut">超时:秒</param>
        /// <param name="UseHttps"></param>
        /// <returns></returns>
        IBuilder Build(int TimeOut = 60, Boolean UseHttps = false);
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
    }
}
