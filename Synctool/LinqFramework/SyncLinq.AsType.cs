using System;
using System.Collections.Generic;
using System.Text;

namespace Synctool.LinqFramework
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
        #endregion
    }
}
