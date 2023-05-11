using System;
using System.Collections.Generic;
using System.Text;
using XExten.Advance.NetFramework.Enums;

namespace XExten.Advance.NetFramework.Options
{
    /// <summary>
    /// 构建
    /// </summary>
    public class DefaultBuilder
    {
        /// <summary>
        /// 使用顶级路径
        /// </summary>
        public bool UseBaseUri { get; set; }
        /// <summary>
        /// 启用控制
        /// </summary>
        public bool UseHandle { get; set; } = false;
        /// <summary>
        /// 超时
        /// </summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
        /// <summary>
        /// 启用缓存节点
        /// </summary>
        public bool UseCache { get; set; } = true;
        /// <summary>
        /// 缓存
        /// </summary>
        public int CacheSpan { get; set; } = 10;
        /// <summary>
        /// 压缩
        /// </summary>
        public bool Gzip { get; set; } = false;
        /// <summary>
        /// https验证
        /// </summary>
        public bool IgnoreHttps { get; set; } = false;
        /// <summary>
        /// 启用认证
        /// </summary>
        public bool UseCookie { get; set; } = false;
        /// <summary>
        /// 指定平台请求头
        /// </summary>
        public Platform PlatformHeader { get; set; } = Platform.Windows;
        /// <summary>
        /// 是否清除默认平台Header
        /// </summary>
        public bool DelDefHeader { get; set; }=false;
    }
}
