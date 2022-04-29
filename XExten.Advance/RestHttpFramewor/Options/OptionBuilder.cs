using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace XExten.Advance.RestHttpFramewor.Options
{
    internal sealed class OptionBuilder
    {
        public static Dictionary<string, string> Header { get; set; } = Header ?? new Dictionary<string, string>();

        public static CookieContainer Cookies { get; set; } = Cookies ?? new CookieContainer();

    }
}
