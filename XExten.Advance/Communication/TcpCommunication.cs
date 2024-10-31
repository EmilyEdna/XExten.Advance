using SixLabors.ImageSharp.Memory;
using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using XExten.Advance.Communication.Model;

namespace XExten.Advance.Communication
{
    /// <summary>
    /// Tcp通信
    /// </summary>
    internal class TcpCommunication : ICommunication
    {
        #region 字段
        private TcpClient Client;

        private NetworkStream Stream;

        private Task ReceivedTask;

        private CancellationTokenSource TokenSource = new CancellationTokenSource();

        private bool DisposeReceived = true;

        private bool IsAsync = false;
        #endregion

        #region 接口属性
        public bool IsConnected { get; set; } = false;

        public event Action<byte[]> Received;
        public event Action<Exception> Error;
        #endregion

        #region 接口实现
        public void Connect(CommunicationParams input)
        {
            try
            {
                Client ??= new TcpClient
                {
                    SendTimeout = input.SendTimeout,
                    ReceiveTimeout = input.ReplayTimeout,
                };
                Client.Connect(input.Host, input.Port);
                Stream = Client.GetStream();
                IsConnected = Client.Connected;
            }
            catch (Exception ex)
            {
                IsConnected = false;
                Error?.Invoke(ex);
            }
        }

        public byte[] SendCommand(byte[] cmd, bool DisposeReceived = true)
        {
            try
            {
                this.DisposeReceived = DisposeReceived;
                if (Client != null && IsConnected)
                {
                    UseAsyncReceived(false);
                    Stream.Write(cmd, 0, cmd.Length);
                    int Timeout = 0;
                    while (Timeout < Client.ReceiveTimeout)
                    {
                        if (Stream.DataAvailable)
                        {
                            byte[] bytes = new byte[Client.ReceiveBufferSize];
                            Stream.Read(bytes, 0, bytes.Length);
                            Stream.Flush();
                            int index = 0;
                            for (index = bytes.Length - 1; index >= 0; index--)
                            {
                                if (bytes[index] != 0)
                                {
                                    break;
                                }
                            }
                            bytes = bytes.Take(index + 1).ToArray();
                            if (!DisposeReceived)
                                return bytes;
                            break;
                        }
                        Thread.Sleep(20);
                        Timeout += 20;
                        if (Timeout >= Client.ReceiveTimeout)
                            break;
                    }
                }
                return Array.Empty<byte>();
            }
            catch (Exception ex)
            {
                Error?.Invoke(ex);
                return Array.Empty<byte>();
            }
        }

        public void SendCommandAsync(byte[] cmd, bool DisposeReceived = true)
        {
            try
            {
                this.DisposeReceived = DisposeReceived;
                if (Client != null && IsConnected) 
                {
                    UseAsyncReceived(true);
                    Stream.Write(cmd, 0, cmd.Length);
                    Stream.Flush();
                    Thread.Sleep(20);
                }
            }
            catch (Exception ex)
            {
                Error?.Invoke(ex);
            }
        }


        public void Close()
        {
            if (Client != null && IsConnected)
            {
                Client.Close();
                IsConnected = false;
            }
        }
        #endregion

        #region 私有
        private async Task ReceivedMessage()
        {
            if (Client != null && IsConnected)
            {
                while (IsAsync)
                {
                    if (Stream.DataAvailable)
                    {
                        byte[] bytes = new byte[Client.ReceiveBufferSize];
                        await Stream.ReadAsync(bytes, 0, bytes.Length);
                        Stream.Flush();
                        int index = 0;
                        for (index = bytes.Length - 1; index >= 0; index--)
                        {
                            if (bytes[index] != 0)
                            {
                                break;
                            }
                        }
                        bytes = bytes.Take(index + 1).ToArray();
                        if (!DisposeReceived)
                            Received?.Invoke(bytes);
                    }
                }
            }
        }
        private void UseAsyncReceived(bool flag)
        {
            IsAsync = flag;
            if (IsAsync)
            {
                if (TokenSource.IsCancellationRequested || ReceivedTask == null)
                {
                    TokenSource = new CancellationTokenSource();
                    ReceivedTask = Task.Factory.StartNew(ReceivedMessage, TokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
                }
            }
            else
            {
                TokenSource.Cancel();
                ReceivedTask?.Dispose();
            }
        }
        #endregion
    }
}
