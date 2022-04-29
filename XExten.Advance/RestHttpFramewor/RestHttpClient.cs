using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using XExten.Advance.LinqFramework;
using XExten.Advance.RestHttpFramewor.Options;

namespace XExten.Advance.RestHttpFramewor
{
    /// <summary>
    /// RestHttpClient
    /// </summary>
    public class RestHttpClient : IRestHttpClient
    {
        private RestClientOptions Options;
        private RestRequest Request;
        /// <summary>
        /// 构造
        /// </summary>
        public RestHttpClient()
        {
            this.Options = new RestClientOptions();
            this.Request = new RestRequest();
        }
        /// <summary>
        /// 使用代理
        /// </summary>
        /// <param name="Proxy"></param>
        /// <returns></returns>
        public IRestHttpClient UseProxy(RestProxy Proxy)
        {
            if (Proxy.IP.IsNullOrEmpty() || Proxy.Port == -1)
                return this;
            this.Options.Proxy = new WebProxy(Proxy.IP, Proxy.Port);
            if (!Proxy.UserName.IsNullOrEmpty() && !Proxy.PassWord.IsNullOrEmpty())
                this.Options.Proxy.Credentials = new NetworkCredential(Proxy.UserName, Proxy.PassWord);
            return this;
        }
        /// <summary>
        /// 使用Cookie
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public IRestHttpClient UseCookie(Action<RestCookie> action)
        {
            RestCookie cookie = new RestCookie();
            action.Invoke(cookie);
            cookie.SetCookie();
            Options.CookieContainer = OptionBuilder.Cookies;
            return this;
        }
        /// <summary>
        /// 使用头
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public IRestHttpClient UseHeader(Action<RestHeader> action)
        {
            RestHeader header = new RestHeader();
            action.Invoke(header);
            header.SetHeader();
            this.Request.AddHeaders(OptionBuilder.Header);
            return this;
        }
    }
}
