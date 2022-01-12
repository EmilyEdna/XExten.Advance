using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.Advance.EventFramework.SubscriptEvent
{
    /// <summary>
    /// 订阅者
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class EventSubscribeAttribute : Attribute
    {
        /// <summary>
        /// 事件Id
        /// </summary>
        public string EventId { get; }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="EventId"></param>
        public EventSubscribeAttribute(string EventId)
        {
            this.EventId = EventId;
        }
    }
}
