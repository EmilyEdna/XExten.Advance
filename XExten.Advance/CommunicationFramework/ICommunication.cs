using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XExten.Advance.CommunicationFramework.Model;

namespace XExten.Advance.CommunicationFramework
{
    /// <summary>
    /// 通信
    /// </summary>
    public interface ICommunication
    {
        /// <summary>
        /// 是否连接成功
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// 当前连接对象
        /// </summary>
        object CommunicationObject { get; }

        /// <summary>
        /// 缓存区
        /// </summary>
        List<byte> Cache {  get; }

        /// <summary>
        /// 打开链接
        /// </summary>
        /// <param name="input"></param>
        void Connect(CommunicationParams input);

        /// <summary>
        /// 断开连接
        /// </summary>
        void Close();

        /// <summary>
        /// 发送并回读
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        Task<byte[]> SendAndReadByteAsync(string cmd);

        /// <summary>
        /// 发送并回读
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        Task<string> SendAndReadStringAsync(string cmd);

        /// <summary>
        /// 发送不回读也不取缓存
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        Task SendAndNoRead(string cmd);

        /// <summary>
        /// 发送需自己处理缓存区
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        Task SendAndReadInCache(string cmd);

        /// <summary>
        /// 发送并回读
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        Task<byte[]> SendAndReadByteAsync(byte[] cmd);

        /// <summary>
        /// 发送并回读
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        Task<string> SendAndReadStringAsync(byte[] cmd);

        /// <summary>
        /// 发送不回读也不取缓存
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        Task SendAndNoRead(byte[] cmd);

        /// <summary>
        /// 发送需自己处理缓存区
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        Task SendAndReadInCache(byte[] cmd);
    }
}
