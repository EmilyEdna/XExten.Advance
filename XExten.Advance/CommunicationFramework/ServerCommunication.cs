using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using XExten.Advance.Communication.Model;
using XExten.Advance.LinqFramework;
using XExten.Advance.LogFramework;

namespace XExten.Advance.CommunicationFramework
{
    internal class ServerCommunication : IServerCommunication
    {
        private List<ServerCommunicationParams> Session { get; set; }
        private CancellationTokenSource _CancellationTokenSource { get; set; }

        public event Action<Guid, byte[]> Received;
        public event Action<Exception> Error;

        private TcpListener _TcpListener;
        private int _DataSize;

        public void Start(string ip, int port, int dataSize = 1024)
        {
            _DataSize = dataSize;
            _CancellationTokenSource = new CancellationTokenSource();
            _TcpListener = new TcpListener(IPAddress.Parse(ip), port);
            _TcpListener.Start();
            //监听上线客户端
            Task.Factory.StartNew(() =>
            {
                while (!_CancellationTokenSource.IsCancellationRequested)
                {
                    var Client = _TcpListener.AcceptTcpClient();
                    ServerCommunicationParams session = new ServerCommunicationParams
                    {
                        Id = Guid.NewGuid(),
                        IsAlive = true,
                        Callback = CallbackReceived,
                        Client = Client,
                        OnlineDate = DateTime.Now,
                    };
                    Session.Add(session);
                    $"The online ID of the TcpClient is [{session.Id}], and the online time is[{session.OnlineDate:yyyy-MM-dd HH:mm:ss}]".Debug();
                }
            }, _CancellationTokenSource.Token);
            //数据监听
            Task.Factory.StartNew(() =>
            {
                while (!_CancellationTokenSource.IsCancellationRequested)
                {
                    if (Session.Count > 0)
                    {
                        for (int index = 0; index < Session.Count; index++)
                        {
                            var clientSession = Session.ElementAtOrDefault(index);
                            clientSession?.Callback(clientSession);
                        }
                    }
                }
            }, _CancellationTokenSource.Token);
            //客户端失活监听
            Task.Factory.StartNew(() =>
            {
                while (!_CancellationTokenSource.IsCancellationRequested)
                {
                    Thread.Sleep(3600 * 1000);
                    if (Session.Count > 0)
                    {
                        Session.RemoveAll(t => !t.IsAlive && DateTime.Now.Subtract(t.OnlineDate).TotalHours >= 1);

                        $"Clean up the inactive agents, and the remaining active agents are [{string.Join(",", Session.Select(t=>t.Id))}]".Debug();
                    }
                }
            }, _CancellationTokenSource.Token);
        }

        public void Send(Guid clientId, byte[] data)
            => Session.FirstOrDefault(t => t.Id == clientId).Client.GetStream().Write(data, 0, data.Length);

        public void SendAll(byte[] data)
            => Session.ForEach(item => item.Client.GetStream().Write(data, 0, data.Length));


        public void Close()
        {
            _CancellationTokenSource.Cancel();
            _TcpListener.Stop();
        }

        public void RemoveSession(Guid clientId)
            => Session.RemoveAll(t => t.Id == clientId);


        public void ClearSession()
            => Session.Clear();

        private void CallbackReceived(ServerCommunicationParams input)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    byte[] bytes = new byte[_DataSize];
                    var stream = input.Client.GetStream();
                    input.IsAlive = false;
                    if (stream.DataAvailable)
                    {
                        input.IsAlive = true;
                        input.OnlineDate = DateTime.Now;
                        stream.Read(bytes, 0, bytes.Length);

                        int index = 0;
                        for (index = bytes.Length - 1; index >= 0; index--)
                        {
                            if (bytes[index] != 0)
                            {
                                break;
                            }
                        }
                        bytes = bytes.Take(index + 1).ToArray();
                        $"TcpServer Received Data Is [{bytes.WithByteHex()}]".Info();
                        Received?.Invoke(input.Id, bytes);
                    }
                }
                catch (Exception ex)
                {
                    Session.RemoveAll(t => t.Id == input.Id);
                    Error?.Invoke(ex);
                }
            });
        }

    }
}
