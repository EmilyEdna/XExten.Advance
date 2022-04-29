using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using XExten.Advance.LinqFramework;
using XExten.Advance.StaticFramework;

namespace XExten.Advance.RestHttpFramewor.Options
{
    /// <summary>
    /// Cookie
    /// </summary>
    public class RestCookie
    {
        /// <summary>
        /// URI
        /// </summary>
        public string URI { get; set; }
        /// <summary>
        /// CookieName
        /// </summary>
        public string CookieName { get; set; }
        /// <summary>
        /// CookieValue
        /// </summary>
        public string CookieValue { get; set; }
        /// <summary>
        /// CookiePath
        /// </summary>
        public string CookiePath { get; set; }
        /// <summary>
        /// CookieDomain
        /// </summary>
        public string CookieDomain { get; set; }
        /// <summary>
        /// Cookies
        /// </summary>
        public Dictionary<string, string> Cookies { get; set; }
        /// <summary>
        /// CookieColl
        /// </summary>
        public CookieCollection CookieColl { get; set; }

        internal void SetCookie()
        {
            if (!URI.IsNullOrEmpty() && Cookies != null && Cookies.Count > 0)
                Cookies.ForDicEach((key, val) =>
                {
                    OptionBuilder.Cookies.Add(new Uri(URI), new Cookie(key, val));
                });
            if (!URI.IsNullOrEmpty() && CookieColl != null && CookieColl.Count > 0)
                OptionBuilder.Cookies.Add(new Uri(URI), CookieColl);
            if (!CookieName.IsNullOrEmpty() && !CookieValue.IsNullOrEmpty() && !CookiePath.IsNullOrEmpty())
                OptionBuilder.Cookies.Add(new Cookie(CookieName, CookieValue, CookiePath, CookieDomain));
        }
    }
}
