using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace XExten.Advance.RestHttpFramework.Options
{
    internal sealed class OptionBuilder
    {
        internal static Dictionary<string, string> Header { get; set; } = Header ?? new Dictionary<string, string>();
        internal static CookieContainer Cookies { get; set; } = Cookies ?? new CookieContainer();
        internal static List<RestNode> Nodes { get; set; } = Nodes ?? new List<RestNode>();
    }
}
