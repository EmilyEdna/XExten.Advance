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
    /// 百度翻译
    /// </summary>
    internal class BaiduTranslate
    {
        internal static string Translation(string query, string from, string to)
        {
            string Sign = $"20161214000033991{query}xexten2022HMlukU9THx2Twx1I14Hz".ToMd5().ToLower();

            string Host = $"https://fanyi-api.baidu.com/api/trans/vip/translate?q={query}&from={from}&to={to}&appid=20161214000033991&salt=xexten2022&sign={Sign}";

            HttpClient Client = new HttpClient();
            var result = Regex.Unescape(Client.GetStringAsync(Host).Result).ToModel<JObject>();
            return result["trans_result"][0]["dst"].ToString();
        }
    }
}
