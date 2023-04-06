using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using XExten.Advance.LinqFramework;

namespace XExten.Advance.InternalFramework.Translate
{
    /// <summary>
    /// 翻译
    /// </summary>
    internal class Translation
    {
        internal static string Translatate(string query, string from, string to)
        {
            string Host = $"https://translate.appworlds.cn?text={query}&from=auto&to={to}";
            HttpClient Client = new HttpClient();
            var result = Client.GetStringAsync(Host).Result.ToModel<JObject>();
            return result["data"].ToString();
        }
    }
}
