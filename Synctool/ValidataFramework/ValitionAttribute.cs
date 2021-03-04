using Synctool.ValidataFramework.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Synctool.ValidataFramework
{
    /// <summary>
    /// 参数验证
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class ValitionAttribute : Attribute
    {
        /// <summary>
        /// 验证消息
        /// </summary>
        public string InfoMsg { get; set; }
        /// <summary>
        /// 验证类型
        /// </summary>
        public ValitionEnum ValiType { get; set; } = ValitionEnum.NotNull;
        /// <summary>
        /// 正在表达式
        /// </summary>
        public string RegexStr { get; set; } = string.Empty;
        /// <summary>
        /// 是否使用不满足要求的自动名称代入返回信息
        /// </summary>
        public bool UsageAppendField { get; set; } = false;
        /// <summary>
        /// 自定义验证
        /// </summary>
        public Type CustomerValition { get; set; } = null;
    }
}
