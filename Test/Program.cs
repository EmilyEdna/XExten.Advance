using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Test.EventTest;
using Test.HttpTest;
using XExten.Advance.EventFramework;
using XExten.Advance.EventFramework.PublishEvent;
using XExten.Advance.StaticFramework;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            // var html = new TestClass().TestMethod();
            //var  x =  SyncStatic.HText(html);
            EventBus.Lancher(Assembly.Load("Test"));
            new Test1().PublishTest();

            Console.ReadKey();
            IEventPublish.Instance.PublishAsync(t => {
                t.Payload = "OK";
                t.EventId = "Test1";
            });
            Console.ReadKey();
        }
    }
}
