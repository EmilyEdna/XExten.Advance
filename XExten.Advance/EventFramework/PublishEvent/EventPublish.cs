using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XExten.Advance.EventFramework.EventContext;
using XExten.Advance.EventFramework.EventSources;

namespace XExten.Advance.EventFramework.PublishEvent
{
    internal sealed class EventPublish : IEventPublish
    {
        public Task DelayPublishAsync(Action<IEventSource> source, long delay)
        {
            IEventSource eventSource = new EventSource();
            source.Invoke(eventSource);

            return Task.Run(async () =>
             {
                 await Task.Delay(TimeSpan.FromMilliseconds(delay));

                 await EventContainer.Instance.Resolve<IEventChangeStore>().WriteAsync(eventSource);
             });
        }

        public Task PublishAsync(Action<IEventSource> source)
        {
            IEventSource eventSource = new EventSource();
            source.Invoke(eventSource);
            return EventContainer.Instance.Resolve<IEventChangeStore>().WriteAsync(eventSource);
        }
    }
}
