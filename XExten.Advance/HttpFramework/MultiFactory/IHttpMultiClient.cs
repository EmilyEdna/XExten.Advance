using System;
using System.Collections.Generic;
using System.Text;
using XExten.Advance.HttpFramework.MultiCommon;
using XExten.Advance.HttpFramework.MultiInterface;
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
        /// <param name="action"></param>
        /// <returns></returns>
        IHttpMultiClient InitWebProxy(Action<MultiProxy> action);
        /// <summary>
        /// 使用代理
        /// </summary>
        /// <param name="proxy"></param>
        /// <returns></returns>
        IHttpMultiClient InitWebProxy(MultiProxy proxy);
        /// <summary>
        /// 初始化Cookie
        /// </summary>
        /// <returns></returns>
        IHttpMultiClient InitCookie();
        /// <summary>
        /// Header
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IHeaders AddHeader(Action<HeaderOption> action);
        /// <summary>
        /// Cookie
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        ICookies AddCookie(Action<CookieOption> action);
        /// <summary>
        /// Node
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        INodes AddNode(Action<NodeOption> action);
    }
}
