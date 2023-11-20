using XExten.Advance.LinqFramework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace XExten.Advance.InternalFramework.Email
{
    /// <summary>
    /// 邮箱工具类
    /// </summary>
    public class EmailUtile
    {
        /// <summary>
        ///  发送邮件
        /// </summary>
        /// <param name="View"></param>
        /// <returns></returns>
        public static void SendMail(EmailViewModel View)
        {
            if (EmailSetting.SmtpHost.IsNullOrEmpty() || EmailSetting.EmailAccount.IsNullOrEmpty() ||
                EmailSetting.EmailPassWord.IsNullOrEmpty())
                throw new Exception($"Please check mail setting,There has some  error in the setting at ：{typeof(EmailSetting).FullName}");
            SmtpClient Client = new SmtpClient
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Host = EmailSetting.SmtpHost,
                EnableSsl = true,
                Credentials = new NetworkCredential(EmailSetting.EmailAccount, EmailSetting.EmailPassWord)
            };
            MailAddress From = new MailAddress(EmailSetting.EmailAccount);
            MailAddress To = new MailAddress(View.AcceptedAddress);
            MailMessage Msg = new MailMessage(From, To)
            {
                Subject = View.Title,
                Body = View.Content,
                BodyEncoding = Encoding.UTF8,
                IsBodyHtml = true,
                Priority = MailPriority.Normal
            };
            Client.Send(Msg);
        }
    }
}
