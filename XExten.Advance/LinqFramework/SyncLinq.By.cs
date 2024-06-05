using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XExten.Advance.LinqFramework
{
    public static partial class SyncLinq
    {

        #region By
        /// <summary>
        /// 将实体转为URL参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <param name="MapFied"></param>
        /// <returns></returns>
        public static string ByUri<T>(this T param, IDictionary<string, string> MapFied = null)
        {
            List<string> result = new List<string>();
            if (param is JObject)
            {
                var NEW = param as JObject;
                NEW.Properties().ForEnumerEach(t =>
                {
                    if (MapFied != null)
                    {
                        MapFied.TryGetValue(t.Name, out string value);
                        if (value.IsNullOrEmpty())
                            result.Add($"{t.Name.ToLower()}={t.Value}");
                        else
                            result.Add($"{value.ToLower()}={t.Value}");
                    }
                    else
                        result.Add($"{t.Name.ToLower()}={t.Value}");
                });
                return $"?{string.Join("&", result)}";
            }
            param.GetType().GetProperties().ForEnumerEach(t =>
            {
                if (MapFied != null)
                {
                    MapFied.TryGetValue(t.Name, out string value);
                    if (value.IsNullOrEmpty())
                        result.Add($"{t.Name.ToLower()}={t.GetValue(param)}");
                    else
                        result.Add($"{value.ToLower()}={t.GetValue(param)}");
                }
                result.Add($"{t.Name.ToLower()}={t.GetValue(param)}");
            });
            return $"?{string.Join("&", result)}";
        }

        /// <summary>
        /// 将键值对转为URL参数
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string ByUri(this List<KeyValuePair<string, string>> param)
        {
            List<string> result = new List<string>();
            param.ForEach(t =>
            {
                result.Add($"{t.Key.ToLower()}={t.Value}");
            });
            return $"?{string.Join("&", result)}";
        }
        #endregion

        #region Async
        /// <summary>
        /// 将实体转为URL参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <param name="MapFied"></param>
        /// <returns></returns>
        public static async Task<string> ByUriAsync<T>(this T param, IDictionary<string, string> MapFied = null) => await Task.Run(() => ByUri(param, MapFied));

        /// <summary>
        /// 将键值对转为URL参数
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static async Task<string> ByUriAsync(this List<KeyValuePair<string, string>> param) => await Task.Run(() => ByUri(param));

        #endregion
    }
}
