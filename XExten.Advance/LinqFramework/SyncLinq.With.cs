using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace XExten.Advance.LinqFramework
{
    public static partial class SyncLinq
    {

        #region With

        /// <summary>
        /// Bytes转hex
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string WithByteHex(this byte[] bytes)
            => string.Join("", bytes.Select(t => t.ToString("x2")));

        /// <summary>
        /// hex转bytes
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static byte[] WithHexByte(this string hex)
             => hex.Split(" ").Select(t => (byte)Convert.ToInt32(t, 16)).ToArray();

        /// <summary>
        /// 返回实体中所有的字段名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        public static List<string> WithNames<T>(this IEnumerable<T> param) where T : class, new()
        {
            List<String> Names = new List<String>();
            param.FirstOrDefault().GetType().GetProperties().ForEnumerEach(t =>
            {
                Names.Add(t.Name);
            });
            return Names;
        }

        /// <summary>
        /// 返回一个实体中所有数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Dictionary<string, object> WithKeyValue<T>(T param) where T : class, new()
        {
            var result = new Dictionary<string, object>();

            param.GetType().GetProperties().ForEnumerEach(item =>
            {
                result.Add(item.Name, item.GetValue(param));
            });

            return result;
        }

        /// <summary>
        /// 正则匹配
        /// </summary>
        /// <param name="param"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static string WithRegex(this string param, string pattern)
        {
            return Regex.Match(param, pattern).Value.ToString();
        }

        /// <summary>
        /// 正则替换
        /// </summary>
        /// <param name="param"></param>
        /// <param name="pattern"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string WithRegexReplace(this string param, string pattern,string value)
        {
            return Regex.Replace(param, pattern, value);
        }

        #endregion

        #region Async

        /// <summary>
        /// Bytes转hex
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static Task<string> WithByteHexAsync(this byte[] bytes)
            => Task.Run(() => WithByteHex(bytes));

        /// <summary>
        /// hex转bytes
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static Task<byte[]> WithHexByteAsync(this string hex)
            => Task.Run(() => WithHexByte(hex));

        /// <summary>
        /// 返回实体中所有的字段名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        public static async Task<List<string>> WithNamesAsync<T>(this IEnumerable<T> param) where T : class, new() => await Task.Run(() => WithNames(param));

        /// <summary>
        /// 返回一个实体中所有数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        public static async Task<Dictionary<string, object>> WithKeyValueAsync<T>(T param) where T : class, new() => await Task.Run(() => WithKeyValue(param));

        /// <summary>
        /// 正则匹配
        /// </summary>
        /// <param name="param"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static async Task<string> WithRegexAsync(this string param, string pattern)=> await Task.Run(() => WithRegex(param, pattern));
      
        /// <summary>
        /// 正则替换
        /// </summary>
        /// <param name="param"></param>
        /// <param name="pattern"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static async Task<string> WithRegexReplaceAsync(this string param, string pattern, string value) => await Task.Run(() => WithRegexReplace(param, pattern, value));

        #endregion
    }
}
