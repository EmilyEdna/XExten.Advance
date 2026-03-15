using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using XExten.Advance.AopFramework.AopAttribute;
using XExten.Advance.CommunicationFramework;
using XExten.Advance.CommunicationFramework.Model;
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
            AxConvert.RunConvert();
            EventBus.Lancher(Assembly.Load(typeof(EventTest).Assembly.GetName().Name));
            CommunicationModule.Initialize();
            while (true)
            {
                Console.WriteLine("1【RSA】2【EVENT】3【Aop】4【通信】`【退出】");
                var input = Console.ReadLine();
                if (input == "`") break;
                if (input == "1")
                    RSAHelper.RsaTest();
                if (input == "2")
                    EventTest.EventTestClassMethod();
                if (input == "3")
                    AopTestClass.AopTestClassMethod();
                if (input == "4")
                    CommuincationClass.UdpTest();
            }
        }
    }

    #region Covert

    public class MauiRoot
    {
        /// <summary>
        /// 
        /// </summary>
        public int Category { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Cover { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Route { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Hash { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Commom { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Span { get; set; }
    }

    public class PCRoot
    {
        /// <summary>
        /// 
        /// </summary>
        public string Cover { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Route { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Duration { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ViewCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Latest { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Platfrom { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PId { get; set; }
    }

    public class AxConvert
    {
        public static void RunConvert()
        {
            var Root = SyncStatic.ReadFile("D:\\Export.txt").ToModel<List<MauiRoot>>();
            var Data = Root.Select(t => new PCRoot
            {

                PId = t.Id,
                Cover = t.Cover,
                Duration = null,
                Latest = DateTime.Now.ToFmtDate(3, "yyyy-MM-dd"),
                Route = t.Route,
                Title = t.Name,
                ViewCount = "1",
                Platfrom = t.Route.Contains("javbangers") ? "Jav" : (t.Route.Contains("spankbang") ? "Skb" : "A24")

            }).ToList().ToJson();

        }
    }
    #endregion


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

    #region Commuincation
    public class CommuincationClass
    {
        public static void TcpTest()
        {
            ICommunication Tcp = IocDependency.ResolveByNamed<ICommunication>(CommunicationEnum.TCP);
            Tcp.Connect(new CommunicationParams
            {
                Host = "127.0.0.1",
                Port = 9000
            });
            Console.WriteLine(Tcp.IsConnected);
            while (true)
            {
                var input = Console.ReadLine();
                if (input.ToUpper() != "ESC")
                {
                    Tcp.SendAndReadInCache(input.ByBytes());
                    Console.WriteLine(Tcp.Cache.ToArray().ByString());
                }
                else
                    break;
            }
        }

        public static void UdpTest()
        {
            ICommunication Udp = IocDependency.ResolveByNamed<ICommunication>(CommunicationEnum.UDP);
            Udp.Connect(new CommunicationParams
            {
                Host = "127.0.0.1",
                Port = 777,
                BindPort = 999,
            });
            Console.WriteLine(Udp.IsConnected);
            while (true)
            {
                var input = Console.ReadLine();
                if (input.ToUpper() != "ESC")
                {
                    Udp.SendAndReadInCache(input.ByBytes());
                    Console.WriteLine(Udp.Cache.ToArray().ByString());
                }
                else
                    break;
            }
        }
    }
    #endregion
}


