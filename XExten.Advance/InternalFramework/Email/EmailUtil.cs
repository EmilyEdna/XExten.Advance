﻿using XExten.Advance.LinqFramework;
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
    internal class EmailUtile
    {
        /// <summary>
        ///  发送邮件
        /// </summary>
        /// <param name="View"></param>
        /// <returns></returns>
        internal static void SendMail(EmailViewModel View)
        {
            if (EmailSetting.EmailAccount.IsNullOrEmpty() || EmailSetting.EmailAccount.IsNullOrEmpty() ||
                EmailSetting.EmailPassWord.IsNullOrEmpty() || EmailSetting.SendTitle.IsNullOrEmpty())
                throw new Exception($"Please check mail setting,There has some  error in the setting at ：{typeof(EmailSetting).FullName}");
            SmtpClient Client = new SmtpClient
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Host = EmailSetting.EmailAccount,
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(EmailSetting.EmailAccount, EmailSetting.EmailPassWord)
            };
            MailAddress From = new MailAddress(EmailSetting.EmailAccount, EmailSetting.SendTitle);
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
