using System;
using System.Reflection;
using System.Threading.Tasks;
using XExten.Advance.AopFramework.AopAttribute;
using XExten.Advance.Communication;
using XExten.Advance.Communication.Model;
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
                Port=9000
            });
            Console.WriteLine(Tcp.IsConnected);
            Tcp.Received += MsgReceived;
            while (true)
            {
                var input = Console.ReadLine();
                if (input.ToUpper() != "ESC")
                    Tcp.SendCommand(input.ByBytes());
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
                BindPort=999,
            });
            Console.WriteLine(Udp.IsConnected);
            Udp.Received += MsgReceived;
            while (true)
            {
                var input = Console.ReadLine();
                if (input.ToUpper() != "ESC")
                    Udp.SendCommand(input.ByBytes());
                else
                    break;
            }
        }

        private static void MsgReceived(byte[] obj)
        {
            Console.WriteLine(obj.ByString());
        }
    }
    #endregion
}


