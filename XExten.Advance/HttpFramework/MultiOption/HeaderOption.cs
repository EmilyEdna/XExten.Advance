using System;
using System.Collections.Generic;
using XExten.Advance.HttpFramework.MultiCommon;
using XExten.Advance.LinqFramework;
using XExten.Advance.StaticFramework;

namespace XExten.Advance.HttpFramework.MultiOption
{
    /// <summary>
    /// HeaderOption
    /// </summary>
    public class HeaderOption
    {
        #region Default Header Name
        public const string Referer = "Referer";
        public const string UserAgent = "UserAgent";
        public const string Host = "Host";
        public const string AcceptEncoding = "AcceptEncoding";
        public const string Origin = "Origin";
        public const string Authorization = "Authorization";
        public const string Accept = "Accept";
        public const string AcceptLanguage = "AcceptLanguage";
        #endregion

        /// <summary>
        /// HeaderKey
        /// </summary>
        public string HeaderKey { get; set; }
        /// <summary>
        /// HeaderValue
        /// </summary>
        public string HeaderValue { get; set; }
        /// <summary>
        /// Headers
        /// </summary>
        public Dictionary<string, string> Headers { get; set; }

        internal void SetHeader()
        {
            SyncStatic.TryCatch(() =>
            {
                if (Headers != null && Headers.Count > 0)
                    MultiConfig.HeaderOpt.Add(Headers);
                else if (!HeaderKey.IsNullOrEmpty())
                    MultiConfig.HeaderOpt.Add(new Dictionary<string, string> { { HeaderKey, HeaderValue } });
                else
                    throw new Exception("Header配置不满足!");
            }, ex => throw ex);
        }
    }
}
