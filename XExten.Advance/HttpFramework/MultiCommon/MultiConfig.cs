using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using XExten.Advance.HttpFramework.MultiInterface;
using XExten.Advance.HttpFramework.MultiOption;

namespace XExten.Advance.HttpFramework.MultiCommon
{
    internal class MultiConfig
    {
        internal static List<Dictionary<string, string>> HeaderOpt = new List<Dictionary<string, string>>();
        internal static List<NodeOption> NodeOpt = new List<NodeOption>();

        private static ConcurrentDictionary<string, HttpClient> FactoryClient;
        private static readonly object FactoryClientLocker = new object();
        public static ConcurrentDictionary<string, HttpClient> Instance
        {
            get
            {
                lock (FactoryClientLocker)
                {
                    if (FactoryClient == null)
                        FactoryClient = new ConcurrentDictionary<string, HttpClient>();
                }
                return FactoryClient;
            }
        }

        internal static CookieContainer Container { get; set; }
        internal static WebProxy Proxy { get; set; }
        internal static IBuilders Builder { get; set; }
        internal static ICookies Cookies { get; set; }
        internal static IHeaders Headers { get; set; }
        internal static INodes Nodes { get; set; }
    }
}
