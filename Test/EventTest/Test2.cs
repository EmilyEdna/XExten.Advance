using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XExten.Advance.EventFramework.EventSources;
using XExten.Advance.EventFramework.SubscriptEvent;

namespace Test.EventTest
{
    public class Test2 : IEventSubscriber
    {
        [EventSubscribe("Test")]
        public Task Test22(IEventSource args)
        {
            var x = args;
            return Task.CompletedTask;
        }
    }
    public class Test3 : IEventSubscriber
    {
        [EventSubscribe("Test1")]
        public Task Test33(IEventSource args)
        {
            var x = args;
            return Task.CompletedTask;
        }
    }
}
