using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XExten.Advance.EventFramework.PublishEvent;

namespace Test.EventTest
{
    public class Test1
    {
        public void PublishTest() 
        {
            IEventPublish.Instance.DelayPublishAsync(t => {
                t.Payload = "";
                t.EventId = "Test";
            },1000);
        }
    }
}
