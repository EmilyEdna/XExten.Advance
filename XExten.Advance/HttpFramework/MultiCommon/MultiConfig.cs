using System;
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
        internal static Dictionary<string, IResolver> ResolverOpt = new Dictionary<string, IResolver>();
        internal static CookieContainer Container { get; set; }
        internal static HttpClient FactoryClient { get; set; }
        internal static WebProxy Proxy { get; set; }
        internal static IBuilders Builder { get; set; }
        internal static ICookies Cookies { get; set; }
        internal static IHeaders Headers { get; set; }
        internal static INodes Nodes { get; set; }
    }
}
