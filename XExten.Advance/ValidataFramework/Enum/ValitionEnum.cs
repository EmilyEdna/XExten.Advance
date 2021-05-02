using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.Advance.ValidataFramework.Enum
{
    /// <summary>
    /// 验证类型
    /// </summary>
    public enum ValitionEnum
    {
        /// <summary>
        /// 忽略
        /// </summary>
        Ignore,
        /// <summary>
        /// 非空验证
        /// </summary>
        NotNull,
        /// <summary>
        /// 正则验证
        /// </summary>
        Regex,
        /// <summary>
        /// 自定义验证
        /// </summary>
        Customer
    }
}
