using System;
using System.Collections.Generic;
using System.Text;
using XExten.Advance.NetFramework.Enums;

namespace XExten.Advance.NetFramework
{
    /// <summary>
    /// 头常量
    /// </summary>
    public class ConstDefault
    {
        /// <summary>
        /// UserAgentValue
        /// </summary>
        public const string UserAgentValue = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/112.0.0.0 Safari/537.36 Edg/112.0.1722.48";
        /// <summary>
        ///UserAgentValueAndroid
        /// </summary>
        public const string UserAgentValueAndroid = "Mozilla/5.0 (Linux; Android 13; PGT-AN10) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.162 Mobile Safari/537.36 Edg/113.0.0.0";
        /// <summary>
        /// UserAgentValueIPhone
        /// </summary>
        public const string UserAgentValueIPhone = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_2_3 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0.3 Mobile/15E148 Safari/604.1 Edg/113.0.0.0";
        /// <summary>
        /// UserAgent
        /// </summary>
        public const string UserAgent = "User-Agent";
        /// <summary>
        /// Referer
        /// </summary>
        public const string Referer = "Referer";
        /// <summary>
        /// Hot
        /// </summary>
        public const string Host = "Host";
        /// <summary>
        /// AcceptEncoding
        /// </summary>
        public const string AcceptEncoding = "Accept-Encoding";
        /// <summary>
        /// Origin
        /// </summary>
        public const string Origin = "Origin";
        /// <summary>
        /// Authorization
        /// </summary>
        public const string Authorization = "Authorization";
        /// <summary>
        /// Accept
        /// </summary>
        public const string Accept = "Accept";
        /// <summary>
        /// ContentType
        /// </summary>
        public const string ContentType = "Content-Type";
        /// <summary>
        /// AcceptLanguage
        /// </summary>
        public const string AcceptLanguage = "Accept-Language";
        /// <summary>
        /// Cookie
        /// </summary>
        public const string Cookie = "Cookie";
        internal static Platform Platform { get; set; }
        internal static string GetPlatformAgentValue()
        {
            return Platform switch
            {
                Platform.Windows => UserAgentValue,
                Platform.IOS => UserAgentValueIPhone,
                Platform.Android => UserAgentValueAndroid,
                _ => UserAgentValue,
            };
        }
        internal static string GetPlatformAgentValue(Platform platform)
        {
            return platform switch
            {
                Platform.Windows => UserAgentValue,
                Platform.IOS => UserAgentValueIPhone,
                Platform.Android => UserAgentValueAndroid,
                _ => UserAgentValue,
            };
        }
    }
}
