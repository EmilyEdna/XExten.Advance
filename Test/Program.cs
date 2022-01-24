using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Test.EventTest;
using Test.MapperTest;
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

            //EventBus.Lancher(Assembly.Load("Test"));
            //IEventPublish.Instance.PublishAsync(t =>
            //{
            //    t.Payload = "1";
            //    t.EventId = "Test";
            //});
            //IEventPublish.Instance.PublishAsync(t =>
            //{
            //    t.Payload = "2";
            //    t.EventId = "Test";
            //});
            //IEventPublish.Instance.PublishAsync(t =>
            //{
            //    t.Payload = "3";
            //    t.EventId = "Test";
            //});

         var x =   SyncStatic.DnsLookup("konachan.com");
            //new HttpTest.TestClass().TestMethod();
            Console.ReadKey();
            //new TestClass().Test();
        }
    }
}
