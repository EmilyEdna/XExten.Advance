using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using XExten.Advance.AopFramework.AopAttribute;
using XExten.Advance.EventFramework;
using XExten.Advance.EventFramework.EventSources;
using XExten.Advance.EventFramework.PublishEvent;
using XExten.Advance.EventFramework.SubscriptEvent;
using XExten.Advance.StaticFramework;
using XExten.Advance.LinqFramework;
using XExten.Advance.CacheFramework;
using XExten.Advance.CacheFramework.RunTimeCache;
using DryIoc;
using XExten.Advance.NetFramework;
using System.Linq;

namespace Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(SyncStatic.Translate("hello world"));

            NetFactoryExtension.RegisterNetFramework();
            var data = NetFactoryExtension.GetService<INetFactory>().AddNode(t => t.Node = "https://www.baidu.com").Build().RunString().Result;
            Console.WriteLine(data.FirstOrDefault());
            //EventTest.EventTestClassMethod();
            //AopTestClass.AopTestClassMethod();
            //NormalTestClass.NormalTestClassMethod();
            Console.ReadKey();
        }
    }

    #region EventTest

    public class EventTest
    {
        public static void EventTestClassMethod()
        {
            EventBus.Lancher(Assembly.Load(typeof(EventTest).Assembly.GetName().Name));

            IEventPublish.Instance.PublishAsync(t =>
            {
                t.Payload = new { Name = "张三", Age = 20 };
                t.EventId = "Json";
            });
        }
    }
    public class EventSubTest : XExten.Advance.EventFramework.SubscriptEvent.IEventSubscriber
    {
        [EventSubscribe("Json")]
        public Task Tests(IEventSource args)
        {
            Console.WriteLine(args.Payload);
            return Task.CompletedTask;
        }
    }
    #endregion

    #region AopTest
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class InterceptorAttribute : InterceptorBaseAttribute
    {
        public override object Invoke(object obj, string methodName, object[] parameters)
        {
            Console.WriteLine(obj.GetType().Name);
            return base.Invoke(obj, methodName, parameters);
        }
    }
    public class ActionAttribute : AopBaseActionAttribute
    {
        public ActionAttribute() : base()
        {

        }
        public ActionAttribute(string Code) : base(Code)
        {
        }
        public override void Before(string methodName, Type classInfo, object[] parameters)
        {
            Console.WriteLine(methodName);
            Console.WriteLine(string.Join(",", parameters));
        }
        public override object After(string methodName, Type classInfo, object result)
        {
            Console.WriteLine(methodName);
            Console.WriteLine(result);
            return result;
        }
    }
    public interface IAopTest
    {
        void AopTestMethod();
    }
    [Interceptor]
    public class AopTest : IAopTest
    {
        [Action("AopTest")]
        public void AopTestMethod()
        {
            Console.WriteLine("AopTest");
        }
    }
    public class AopTestClass
    {
        public static void AopTestClassMethod()
        {
            var container = new Container().RegistAop<IAopTest>();
            container.Resolve<IAopTest>().AopTestMethod();
        }
    }
    #endregion


    #region NormalTest
    public class NormalTestClass
    {
        public static void NormalTestClassMethod()
        {
            Caches.RunTimeCacheSet("1", "1", 600, true);
            Caches.RunTimeCacheSet("2", "2", 600, true);
            MemoryCaches.RemoveAllCache();
            var ret = Caches.RunTimeCacheGet<string>("1");
            Console.WriteLine();
        }
    }
    #endregion
}


