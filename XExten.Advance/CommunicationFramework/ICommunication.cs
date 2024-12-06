using System;
using System.Collections.Generic;
using System.Text;
using XExten.Advance.CommunicationFramework.Model;

namespace XExten.Advance.CommunicationFramework
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
        /// 异常事件
        /// </summary>
        event Action<Exception> Error;

        /// <summary>
        /// 是否连接成功
        /// </summary>
        bool IsConnected { get; set; }

        /// <summary>
        /// 打开链接
        /// </summary>
        /// <param name="input"></param>
        void Connect(CommunicationParams input);

        /// <summary>
        /// 发送指令
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="DisposeReceived">是否丢弃数据</param>
        byte[] SendCommand(byte[] cmd, bool DisposeReceived=true);

        /// <summary>
        /// 发送指令
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="DisposeReceived">是否丢弃数据</param>
        void SendCommandAsync(byte[] cmd, bool DisposeReceived=true);

        /// <summary>
        /// 断开连接
        /// </summary>
        void Close();
    }
}
