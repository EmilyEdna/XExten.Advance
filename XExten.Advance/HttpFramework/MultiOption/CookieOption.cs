using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using XExten.Advance.HttpFramework.MultiCommon;
using XExten.Advance.LinqFramework;
using XExten.Advance.StaticFramework;

namespace XExten.Advance.HttpFramework.MultiOption
{
    /// <summary>
    /// CookieOption
    /// </summary>
    public class CookieOption
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
            SyncStatic.TryCatch(() =>
            {
                if (MultiConfig.Container == null)
                    MultiConfig.Container = new CookieContainer();
                if (!URI.IsNullOrEmpty() && Cookies != null && Cookies.Count > 0)
                    Cookies.ForDicEach((key, val) =>
                    {
                        MultiConfig.Container.Add(new Uri(URI), new Cookie(key, val));
                    });
                else if (!URI.IsNullOrEmpty() && CookieColl != null && CookieColl.Count > 0)
                    MultiConfig.Container.Add(new Uri(URI), CookieColl);
                else if (!CookieName.IsNullOrEmpty() && !CookieValue.IsNullOrEmpty() && !CookiePath.IsNullOrEmpty())
                    MultiConfig.Container.Add(new Cookie(CookieName, CookieValue, CookiePath, CookieDomain));
                else
                    throw new Exception("Cookie配置不满足!");
            }, ex => throw ex);
        }
    }
}
