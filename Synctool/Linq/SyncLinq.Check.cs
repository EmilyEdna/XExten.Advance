using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Synctool.Linq
{
    public static partial class SyncLinq
    {
        /// <summary>
        ///检查字符
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string param)
        {
            return string.IsNullOrEmpty(param);
        }

        /// <summary>
        /// 检查集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> param)
        {
            return param != null && param.Count() != 0;
        }

        /// <summary>
        /// 是否身份证
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static bool IsIDCard(this string param)
        {
            if (param.IsNullOrEmpty()) return false;
            return Regex.IsMatch(param, @"^(\d{15}$|^\d{18}$|^\d{17}(\d|X|x))$");
        }
    }
}
