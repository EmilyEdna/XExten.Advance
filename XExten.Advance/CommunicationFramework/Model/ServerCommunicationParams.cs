using System;
using System.Net.Sockets;

namespace XExten.Advance.CommunicationFramework.Model
{
    /// <summary>
    /// 服务配置参数
    /// </summary>
    internal class ServerCommunicationParams
    {
        internal Guid Id {  get; set; }
        internal TcpClient Client {  get; set; }
        internal Action<ServerCommunicationParams> Callback { get; set; }
        internal bool IsAlive {  get; set; }
        internal DateTime OnlineDate { get; set; }
        
    }
}
