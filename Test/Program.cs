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
            Console.WriteLine(SyncStatic.Translate("hello world"));

            EventBus.Lancher(Assembly.Load("Test"));
            IEventPublish.Instance.PublishAsync(t =>
            {
                t.Payload = "1";
                t.EventId = "Test";
            });

            Console.WriteLine();
        }
    }
}
