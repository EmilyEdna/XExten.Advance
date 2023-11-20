using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.Advance.InternalFramework.Email
{
    /// <summary>
    /// 邮箱配置
    /// </summary>
    public class EmailSetting
    {
        /// <summary>
        /// 邮箱地址
        /// </summary>
        public static string SmtpHost { get; set; }
        /// <summary>
        /// 邮箱账号
        /// </summary>
        public static string EmailAccount { get; set; }
        /// <summary>
        /// 邮箱密码
        /// </summary>
        public static string EmailPassWord { get; set; }
        /// <summary>
        /// 设置
        /// </summary>
        public static void SetOption(string Host,string Account,string Pwd) 
        {
            SmtpHost = Host;
            EmailAccount = Account;
            EmailPassWord = Pwd;
        }
    }
}
