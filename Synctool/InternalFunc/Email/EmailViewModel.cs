using System;
using System.Collections.Generic;
using System.Text;

namespace Synctool.InternalFunc.Email
{
    /// <summary>
    /// 邮箱内容
    /// </summary>
    public class EmailViewModel
    {
        /// <summary>
        /// 收件人邮箱
        /// </summary>
        public string AcceptedAddress { get; set; }
        /// <summary>
        /// 发送标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
    }
}
