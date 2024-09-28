using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using XExten.Advance.Communication.Model;

namespace XExten.Advance.Communication
{
    /// <summary>
    /// Udp通信
    /// </summary>
    internal class UdpCommunication : ICommunication
    {
        #region 字段
        private UdpClient Client;

        private Task ReceivedTask;

        private IPEndPoint EndPoint;

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
                Client ??= new UdpClient(input.BindPort);
                Client.Client.ReceiveTimeout = input.ReplayTimeout;
                Client.Client.SendTimeout = input.SendTimeout;
                EndPoint = new IPEndPoint(IPAddress.Parse(input.Host), input.Port);
                Client.Connect(EndPoint);
                IsConnected = Client.Client.Connected;
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
                Client.Send(cmd, cmd.Length);
                if (DisposeReceived) return;
                if (!IsAsync)
                {
                    int Timeout = 0;
                    while (Timeout < Client.Client.ReceiveTimeout)
                    {
                        if (Client.Available > 0)
                        {
                            byte[] bytes = Client.Receive(ref EndPoint);
                            Received?.Invoke(bytes);
                            break;
                        }
                        Thread.Sleep(20);
                        Timeout += 20;
                        if (Timeout >= Client.Client.ReceiveTimeout)
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
                    if (Client.Available > 2 && !DisposeReceived)
                    {
                        byte[] bytes = (await Client.ReceiveAsync()).Buffer;
                        Received?.Invoke(bytes);
                        Array.Clear(bytes, 0, bytes.Length);
                    }
                }
            }
        }
        #endregion
    }
}
