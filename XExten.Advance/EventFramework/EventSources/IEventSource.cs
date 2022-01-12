using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.Advance.EventFramework.EventSources
{
    /// <summary>
    /// 事件源
    /// </summary>
    public interface IEventSource
    {
        /// <summary>
        /// 事件Id
        /// </summary>
        string EventId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        DateTime CreateTime { get; }
        /// <summary>
        /// 载体 
        /// </summary>
        object Payload { get; set; }
    }
}
