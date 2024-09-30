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
                Client ??= new UdpClient(input.BindPort);
                Client.Client.ReceiveTimeout = input.ReplayTimeout;
                Client.Client.SendTimeout = input.SendTimeout;
                EndPoint = new IPEndPoint(IPAddress.Parse(input.Host), input.Port);
                Client.Connect(EndPoint);
                IsConnected = Client.Client.Connected;
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
                    Client.Send(cmd, cmd.Length);
                    int Timeout = 0;
                    while (Timeout < Client.Client.ReceiveTimeout)
                    {
                        if (Client.Available > 0)
                        {
                            byte[] bytes = Client.Receive(ref EndPoint);
                            if (!DisposeReceived)
                                return bytes;
                            break;
                        }
                        Thread.Sleep(20);
                        Timeout += 20;
                        if (Timeout >= Client.Client.ReceiveTimeout)
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
                    Client.Send(cmd, cmd.Length);
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

        private async Task ReceivedMessage()
        {
            try
            {
                if (Client != null && IsConnected)
                {
                    while (IsAsync)
                    {
                        if (Client.Available > 0)
                        {
                            byte[] bytes = (await Client.ReceiveAsync()).Buffer;
                            if (!DisposeReceived)
                                Received?.Invoke(bytes);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Error?.Invoke(ex);
            }
        }
        #endregion
    }
}
