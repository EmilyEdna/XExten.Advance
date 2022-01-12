using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.Advance.EventFramework
{
    internal class EventExcutor
    {
        internal EventExcutor(string eventId)
        {
            EventId = eventId;
        }

        /// <summary>
        /// 事件 Id
        /// </summary>
        internal string EventId { get; set; }

        /// <summary>
        /// 事件处理程序
        /// </summary>
        internal Delegate Handler { get; set; }
        /// <summary>
        /// 是否符合条件执行处理程序
        /// </summary>
        /// <param name="eventId">事件 Id</param>
        /// <returns><see cref="bool"/></returns>
        internal bool ShouldRun(string eventId)
        {
            return EventId == eventId;
        }
    }
}
