using SixLabors.ImageSharp.Memory;
using System;
using System.IO;
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

        private bool IsAsync = false;
        #endregion

        #region 接口属性
        public bool IsConnected { get; set; } = false;

        public bool DisposeReceived { get; set; } = false;

        public event Action<byte[]> Received;
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
            catch
            {
                IsConnected = false;
                throw;
            }
        }

        public void SendCommand(byte[] cmd)
        {
            if (Client != null && IsConnected)
            {
                Stream.Write(cmd, 0, cmd.Length);
                if (DisposeReceived) return;
                if (!IsAsync)
                {
                    int Timeout = 0;
                    while (Timeout<Client.ReceiveTimeout)
                    {
                        if (Stream.DataAvailable)
                        {
                            byte[] bytes = new byte[Client.ReceiveBufferSize];
                            Stream.Read(bytes, 0, bytes.Length);
                            Received?.Invoke(bytes);
                            Array.Clear(bytes, 0, bytes.Length);
                            break;
                        }
                        Thread.Sleep(20);
                        Timeout += 20;
                        if (Timeout >= Client.ReceiveTimeout)
                            break;
                    }
                }
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

        public void UseAsyncReceived(bool flag)
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

        #region 私有
        private async Task ReceivedMessage()
        {
            if (Client != null && IsConnected)
            {
                while (IsAsync)
                {
                    if (Stream.DataAvailable && !DisposeReceived)
                    {
                        byte[] bytes = new byte[Client.ReceiveBufferSize];
                        await Stream.ReadAsync(bytes, 0, bytes.Length);
                        Received?.Invoke(bytes);
                        Array.Clear(bytes, 0, bytes.Length);
                    }
                }
            }
        }
        #endregion
    }
}
