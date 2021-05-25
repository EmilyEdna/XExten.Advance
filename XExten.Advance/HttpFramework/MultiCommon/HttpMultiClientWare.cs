using XExten.Advance.HttpFramework.MultiInterface;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace XExten.Advance.HttpFramework.MultiCommon
{
    internal class HttpMultiClientWare
    {
        internal static List<Dictionary<String, String>> HeaderMaps = new List<Dictionary<string, string>>();
        internal static Dictionary<String, IResolver> ResolverMaps = new Dictionary<string, IResolver>(1);
        internal static List<LoadURL> LoadPath = new List<LoadURL>();
        internal static CookieContainer Container { get; set; }
        internal static WebProxy Proxy { get; set; }
        internal static HttpClient FactoryClient { get; set; }
        internal static IBuilder Builder { get; set; }
        internal static ICookies Cookies { get; set; }
        internal static IHeaders Headers { get; set; }
        internal static INode Nodes { get; set; }
    }
}
