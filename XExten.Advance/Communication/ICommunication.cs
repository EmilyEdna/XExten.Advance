using System;
using System.Collections.Generic;
using System.Text;
using XExten.Advance.Communication.Model;

namespace XExten.Advance.Communication
{
    /// <summary>
    /// 通信
    /// </summary>
    public interface ICommunication
    {
        /// <summary>
        /// 数据接收事件
        /// </summary>
        event Action<byte[]> Received;

        /// <summary>
        /// 是否连接成功
        /// </summary>
        bool IsConnected { get; set; }

        /// <summary>
        /// 是否丢弃数据
        /// </summary>
        bool DisposeReceived { get; set; }

        /// <summary>
        /// 打开链接
        /// </summary>
        /// <param name="input"></param>
        void Connect(CommunicationParams input);

        /// <summary>
        /// 发送指令
        /// </summary>
        /// <param name="cmd"></param>
        void SendCommand(byte[] cmd);

        /// <summary>
        /// 断开连接
        /// </summary>
        void Close();

        /// <summary>
        /// 使用同步还是异步接收数据
        /// </summary>
        /// <param name="flag"></param>
        void UseAsyncReceived(bool flag);
    }
}
