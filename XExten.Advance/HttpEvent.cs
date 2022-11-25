using RestSharp;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace XExten.Advance
{
    /// <summary>
    /// 事件中心
    /// </summary>
    public class HttpEvent
    {
        /// <summary>
        /// 请求异常事件
        /// </summary>
        public static Action<HttpClient, Exception> HttpActionEvent { get; set; }
        /// <summary>
        /// 请求异常事件
        /// </summary>
        public static Action<RestClient, Exception> RestActionEvent { get; set; }
    }
}
