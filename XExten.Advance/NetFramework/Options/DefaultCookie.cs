using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace XExten.Advance.NetFramework.Options
{

    /// <summary>
    /// 认证信息
    /// </summary>
    public class DefaultCookie
    {
        /// <summary>
        /// Uri
        /// </summary>
        public string Uri { get; set; }
        /// <summary>
        /// Cookies
        /// </summary>
        public Dictionary<string, string> Cookies { get; set; }
        /// <summary>
        /// Collection
        /// </summary>
        public CookieCollection Collection { get; set; }
    }
}
