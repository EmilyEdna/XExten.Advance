using XExten.Advance.AopFramework;
using XExten.Advance.LinqFramework;
using DryIoc;
using System;
using System.Collections.Generic;

namespace Test.AopTest
{
    public class TestClass
    {
        public void TestMethod()
        {
            PriorityQueue<string, int> queue = new PriorityQueue<string, int>(); //优先队列
            queue.Enqueue("A", 85);
            queue.Enqueue("B", 11);

            var xx = new Container().RegistAop<IMyInterface>();

            xx.Resolve<IMyInterface>().TestMethod(123);

            /* var ins1 = AopProxy.CreateProxyOfInherit<MyClass>();

             var ins2 = AopProxy.CreateProxyOfRealize<IMyInterface, MyClass>();

             var ins3 = AopProxy.CreateProxyOfInherit(typeof(MyClass));

             var ins4 = AopProxy.CreateProxyOfRealize(typeof(IMyInterface), typeof(MyClass));*/
        }


    }
}
