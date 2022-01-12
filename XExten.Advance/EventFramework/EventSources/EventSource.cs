using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.Advance.EventFramework.EventSources
{
    internal sealed class EventSource : IEventSource
    {
        public string EventId { get; set ; }
        public DateTime CreateTime => DateTime.Now;
        public object Payload { get ; set; }
    }
}
