﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using XExten.Advance.HttpFramework.MultiOption;

namespace XExten.Advance.HttpFramework.MultiCommon
{
    internal class MultiConfig
    {
        internal static List<Dictionary<string, string>> HeaderOpt = new List<Dictionary<string, string>>();
        internal static List<NodeOption> NodeOpt = new List<NodeOption>();
    }
}
