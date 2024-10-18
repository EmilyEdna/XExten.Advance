using System;
using System.Collections.Generic;
using System.Text;
using XExten.Advance.Communication.Model;

namespace XExten.Advance.Communication
{
    /// <summary>
    /// TCP服务端
    /// </summary>
    public interface IServerCommunication
    {
        /// <summary>
        /// 数据接收事件
        /// </summary>
        event Action<Guid, byte[]> Received;

        /// <summary>
        /// 异常事件
        /// </summary>
        event Action<Exception> Error;

        /// <summary>
        /// 启动
        /// </summary>
        /// <param name="ip">ip</param>
        /// <param name="port">port</param>
        /// <param name="dataSize">接受的数据大小</param>
        void Start(string ip,int port,int dataSize=1024);

        /// <summary>
        /// 关闭
        /// </summary>
        void Close();

        /// <summary>
        /// 广播消息
        /// </summary>
        /// <param name="data">数据</param>
        void SendAll(byte[] data);

        /// <summary>
        /// 对指定客户端发送消息
        /// </summary>
        /// <param name="clientId">客服端id</param>
        /// <param name="data">数据</param>
        void Send(Guid clientId, byte[] data);

        /// <summary>
        /// 删除指定的客服端会话
        /// </summary>
        /// <param name="clientId"></param>
        void RemoveSession(Guid clientId);

        /// <summary>
        /// 清空所有会话
        /// </summary>
        void ClearSession();
    }
}
