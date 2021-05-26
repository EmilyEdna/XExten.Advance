using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.Advance.HttpFramework.MultiOption
{
    /// <summary>
    /// BuilderOption
    /// </summary>
    public class BuilderOption
    {
        /// <summary>
        /// TimeOut
        /// </summary>
        public int TimeOut { get; set; } = 60;
        /// <summary>
        /// CacheTime
        /// </summary>
        public int CacheTime { get; set; } = 60;
        /// <summary>
        /// UseHttps
        /// </summary>
        public bool UseHttps { get; set; } = false;
        /// <summary>
        /// 使用DNS解析器
        /// </summary>
        public bool UseDnsResolver { get; set; } = false;
        /// <summary>
        /// 数据压缩
        /// </summary>
        public bool UseZip { get; set; } = false;
    }
}
