using System.Text;
using XExten.Advance.Communication.Model;
using XExten.Advance.IocFramework;

namespace XExten.Advance.Communication
{
    /// <summary>
    /// 通信实例模块
    /// </summary>
    public class CommunicationModule
    {
        static CommunicationModule()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Initialize()
        {
            IocDependency.RegisterByNamed<ICommunication, SerialCommunication>(CommunicationEnum.COM, 1);
            IocDependency.RegisterByNamed<ICommunication, TcpCommunication>(CommunicationEnum.TCP, 1);
            IocDependency.RegisterByNamed<ICommunication, UdpCommunication>(CommunicationEnum.UDP, 1);

            IocDependency.Register<IServerCommunication, ServerCommunication>();
        }
    }
}
