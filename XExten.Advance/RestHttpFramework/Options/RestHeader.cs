using System;
using System.Collections.Generic;
using System.Text;
using XExten.Advance.LinqFramework;

namespace XExten.Advance.RestHttpFramework.Options
{
    /// <summary>
    /// 头
    /// </summary>
    public class RestHeader
    {
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
            if (!HeaderKey.IsNullOrEmpty() && !HeaderValue.IsNullOrEmpty())
                OptionBuilder.Header.Add(HeaderKey, HeaderValue);
            if (Headers != null && Headers.Count > 0)
                Headers.ForDicEach((key, par) =>
                {
                    OptionBuilder.Header.Add(key, par);
                });
        }
    }
}
