using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace XExten.Advance.InternalFramework.FileWatch
{
    /// <summary>
    /// 文件模式
    /// </summary>
    public class FileModule
    {
        /// <summary>
        /// 文件路径
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 文件类型
        /// </summary>
        public string Pattern { get; set; }
        /// <summary>
        /// 是否包含文件夹
        /// </summary>
        public bool IsInclude { get; set; } = false;
        /// <summary>
        /// false 只是监听文件,true 监听并读取
        /// </summary>
        public bool Module { get; set; } = false;
        /// <summary>
        /// 监听并读取
        /// </summary>
        public Action Action { get; set; }
        /// <summary>
        /// 监听文件
        /// </summary>
        public Action<object, FileSystemEventArgs> Events { get; set; }
    }
}
