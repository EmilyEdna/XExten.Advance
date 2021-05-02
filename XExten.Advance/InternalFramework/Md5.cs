using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace XExten.Advance.InternalFramework
{
    /// <summary>
    /// md5
    /// </summary>
    internal class Md5
    {
        /// <summary>
        /// 32bit
        /// </summary>
        /// <param name="param"></param>
        internal static string Md5By32(string param)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(param));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                string hash = sBuilder.ToString();
                return hash.ToUpper();
            }
        }
        /// <summary>
        /// 16bit
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        internal static string Md5By16(string param)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(param));
                //转换成字符串，并取9到25位
                string sBuilder = BitConverter.ToString(data, 4, 8);
                //BitConverter转换出来的字符串会在每个字符中间产生一个分隔符，需要去除掉
                sBuilder = sBuilder.Replace("-", "");
                return sBuilder.ToString().ToUpper();
            }
        }
    }
}
