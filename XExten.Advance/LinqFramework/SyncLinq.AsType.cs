using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace XExten.Advance.LinqFramework
{
    public partial class SyncLinq
    {
        #region As
        /// <summary>
        /// To Int
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static int AsInt(this string param)
        {
            int.TryParse(param, out int result);
            return result;
        }
        /// <summary>
        /// To Float
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static float AsFloat(this string param)
        {
            float.TryParse(param, out float result);
            return result;
        }
        /// <summary>
        /// To Double
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static double AsDouble(this string param)
        {
            double.TryParse(param, out double result);
            return result;
        }
        /// <summary>
        /// To Long
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static long AsLong(this string param)
        {
            long.TryParse(param, out long result);
            return result;
        }
        /// <summary>
        /// To DateTime
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static DateTime AsDateTime(this string param)
        {
            DateTime.TryParse(param, out DateTime result);
            return result;
        }
        /// <summary>
        /// To String
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string AsString(this object param)
        {
            return param?.ToString();
        }
        /// <summary>
        /// To Bool
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static bool AsBool(this string param)
        {
            bool.TryParse(param, out bool result);
            return result;
        }
        /// <summary>
        /// To Bool
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static bool AsBool(this int param)
        {
            if (param == 1) return true;
            else return false;
        }
        /// <summary>
        /// To Bytes
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static byte[] AsBytes(this string param)
        {
            return Encoding.Default.GetBytes(param);
        }
        /// <summary>
        /// To Decimal
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static decimal AsDecimal(this string param)
        {
            decimal.TryParse(param, out decimal result);
            return result;
        }

        /// <summary>
        /// 将错误的JSON转为正确结果
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string AsOkJson(this string param)
        {
            return Regex.Replace(param, "(?<=([^{:,]))(\")(?=([^}:,]))", "");
        }
        #endregion

        #region Async
        /// <summary>
        /// To Int
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static async Task<int> AsIntAsync(this string param) => await Task.Run(() => AsInt(param));

        /// <summary>
        /// To Float
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static async Task<float> AsFloatAsync(this string param) => await Task.Run(() => AsFloat(param));

        /// <summary>
        /// To Double
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static async Task<double> AsDoubleAsync(this string param) => await Task.Run(() => AsDouble(param));

        /// <summary>
        /// To Long
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static async Task<long> AsLongAsync(this string param) => await Task.Run(() => AsLong(param));

        /// <summary>
        /// To DateTime
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static async Task<DateTime> AsDateTimeAsync(this string param) => await Task.Run(() => AsDateTime(param));

        /// <summary>
        /// To String
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static async Task<string> AsStringAsync(this object param) => await Task.Run(() => AsString(param));

        /// <summary>
        /// To Bool
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static async Task<bool> AsBoolAsync(this string param) => await Task.Run(() => AsBool(param));

        /// <summary>
        /// To Bool
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static async Task<bool> AsBoolAsync(this int param) => await Task.Run(() => AsBool(param));

        /// <summary>
        /// To Bytes
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static async Task<byte[]> AsBytesAsync(this string param) => await Task.Run(() => AsBytes(param));

        /// <summary>
        /// To Decimal
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static async Task<decimal> AsDecimalAsync(this string param) => await Task.Run(() => AsDecimal(param));

        /// <summary>
        /// 将错误的JSON转为正确结果
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static async Task<string> AsOkJsonAsync(this string param) => await Task.Run(() => AsOkJson(param));

        #endregion
    }
}
