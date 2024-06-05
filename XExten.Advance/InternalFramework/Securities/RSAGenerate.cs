using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using XExten.Advance.InternalFramework.FileHandle;

namespace XExten.Advance.InternalFramework.Securities
{
    internal class RSAGenerate
    {
        internal static string SavePath { get; set; }
        internal static int Multiple { get; set; }

        /// <summary>
        /// 生成RSA私钥和公钥
        /// </summary>
        /// <param name="savePath"></param>
        /// <param name="multiple"></param>
        internal static void GenerateKey(string savePath, int multiple = 2)
        {
            Multiple = multiple;
            SavePath = savePath;

            var Pb = Path.Combine(SavePath, "PublicKey.xml");
            var Pv = Path.Combine(SavePath, "PrivateKey.xml");

            if (File.Exists(Pb) && File.Exists(Pv)) return;

            using var rsa = new RSACryptoServiceProvider(1024 * Multiple);
            var publiceKey = rsa.ToXmlString(false);
            var privateKey = rsa.ToXmlString(true);

            FileManager.WriteFile(Encoding.Default.GetBytes(publiceKey), Pb);
            FileManager.WriteFile(Encoding.Default.GetBytes(privateKey), Pv);
        }
        /// <summary>
        /// RSA加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        internal static string RSAEncrypt(string input)
        {
            var Pb = Path.Combine(SavePath, "PublicKey.xml");
            if (!File.Exists(Pb))
                throw new Exception("The PublicKey dose't exist");
            using var rsa = new RSACryptoServiceProvider(1024 * Multiple);
            var key = FileManager.ReadFile(Path.Combine(SavePath, "PublicKey.xml"));
            rsa.FromXmlString(key);
            var bytes = rsa.Encrypt(Encoding.Default.GetBytes(input), false);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        internal static string RSADecrypt(string input)
        {
            var Pv = Path.Combine(SavePath, "PrivateKey.xml");
            if (!File.Exists(Pv))
                throw new Exception("The PrivateKey dose't exist");
            using var rsa = new RSACryptoServiceProvider(1024 * Multiple);
            var key = FileManager.ReadFile(Path.Combine(SavePath, "PrivateKey.xml"));
            rsa.FromXmlString(key);
            var bytes = Convert.FromBase64String(input);
            return Encoding.Default.GetString(rsa.Decrypt(bytes, false));
        }
    }
}
