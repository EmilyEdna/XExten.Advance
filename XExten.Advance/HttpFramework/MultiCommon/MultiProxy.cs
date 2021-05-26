using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.Advance.HttpFramework.MultiCommon
{
    /// <summary>
    /// 代理
    /// </summary>
    public class MultiProxy
    {
        /// <summary>
        /// IP
        /// </summary>
        public string IP { get; set; } = "";
        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; } = -1;
        /// <summary>
        /// 用户
        /// </summary>
        public string UserName { get; set; } = "";
        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord { get; set; } = "";
    }
}
