using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace XExten.Advance.LinqFramework
{
    public static partial class SyncLinq
    {
        #region Check
        /// <summary>
        ///检查字符
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string param) => string.IsNullOrEmpty(param);

        /// <summary>
        /// 检查集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> param)=> param != null && param.Any();

        /// <summary>
        /// 是否身份证
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static bool Is18IDCard(this string param)
        {
            if (param.IsNullOrEmpty()) return false;
            return Regex.IsMatch(param, @"[1-9]\d{5}(19|20)\d{2}((0[1-9])|(1[0-2]))(([0-2][1-9])|10|20|30|31)\d{3}[0-9Xx]");
        }

        /// <summary>
        /// 是否身份证
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static bool Is15IDCard(this string param)
        {
            if (param.IsNullOrEmpty()) return false;
            return Regex.IsMatch(param, @"[1-9]\d{5}\d{2}((0[1-9])|(1[0-2]))(([0-2][1-9])|10|20|30|31)\d{2}[0-9Xx]");
        }

        /// <summary>
        /// 验证手机号
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static bool IsPhone(this string param)
        {
            if (param.IsNullOrEmpty()) return false;
            return Regex.IsMatch(param, @"(13[0-9]|14[014-9]|15[0-35-9]|16[2567]|17[0-8]|18[0-9]|19[0-35-9])\d{8}");
        }
        #endregion

        #region Async
        /// <summary>
        ///检查字符
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static async Task<bool> IsNullOrEmptyAsync(this string param) => await Task.Run(() => IsNullOrEmpty(param));

        /// <summary>
        /// 检查集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        public static async Task<bool> IsNullOrEmptyAsync<T>(this IEnumerable<T> param) => await Task.Run(() => IsNullOrEmpty(param));

        /// <summary>
        /// 是否身份证
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static async Task<bool> Is18IDCardAsync(this string param) => await Task.Run(() => Is18IDCard(param));

        /// <summary>
        /// 是否身份证
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static async Task<bool> Is15IDCardAsync(this string param) => await Task.Run(() => Is15IDCard(param));

        /// <summary>
        /// 验证手机号
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static async Task<bool> IsPhoneAsync(this string param) => await Task.Run(() => IsPhone(param));
        #endregion
    }
}
