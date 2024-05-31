using System;
using System.Reflection;
using System.Threading.Tasks;
using XExten.Advance.AopFramework.AopAttribute;
using XExten.Advance.EventFramework;
using XExten.Advance.EventFramework.EventSources;
using XExten.Advance.EventFramework.PublishEvent;
using XExten.Advance.EventFramework.SubscriptEvent;
using XExten.Advance.IocFramework;
using XExten.Advance.LinqFramework;
using XExten.Advance.StaticFramework;

namespace Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string json = "{\"bb\":[{\"a\":\"12\"8888dfs\"3\",\"b\":[\"xx\",\"xx1\"],\"c\":\"pp\"}],\"aa\":[{\"a\":\"8888\"88\"8888\"},{\"mm\":\"888\"888\"8888\"}]}";
            Console.WriteLine(json.AsOkJson());
            //RSAHelper.RsaTest();
            //EventTest.EventTestClassMethod();
            //AopTestClass.AopTestClassMethod();
            Console.ReadKey();
        }
    }

    #region RSA
    public class RSAHelper
    {
        public static void RsaTest()
        {
            SyncStatic.GenerateRSAKey("D:\\Project");
            var p = SyncStatic.RSA("你好", true);
            var m = SyncStatic.RSA(p, false);
        }
    }
    #endregion

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
    public class EventSubTest : IEventSubscriber
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
            SyncStatic.RegistAop<IAopTest>();
            IocDependency.Resolve<IAopTest>().AopTestMethod();
        }
    }
    #endregion
}


