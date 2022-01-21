using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XExten.Advance.EventFramework.EventSources;
using XExten.Advance.EventFramework.SubscriptEvent;

namespace Test.EventTest
{
    public class Test : IEventSubscriber
    {
        [EventSubscribe("Test")]
        public Task Tests(IEventSource args)
        {
            Console.WriteLine(args.Payload);
            return Task.CompletedTask;
        }
    }
}
