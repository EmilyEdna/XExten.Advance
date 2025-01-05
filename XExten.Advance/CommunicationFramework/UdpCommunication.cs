using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using XExten.Advance.CommunicationFramework.Model;
using XExten.Advance.LinqFramework;
using XExten.Advance.LogFramework;
using XExten.Advance.ThreadFramework;

namespace XExten.Advance.CommunicationFramework
{
    /// <summary>
    /// Udp通信
    /// </summary>
    internal class UdpCommunication : ICommunication
    {

        private UdpClient Client;
        private CommunicationParams Params;
        private bool ReadBack;

        public bool IsConnected { get; private set; }

        public object CommunicationObject { get; private set; }

        public List<byte> Cache { get; private set; }

        public UdpCommunication()
        {
            ReadBack = false;
            Cache = new List<byte>();
        }

        public void Connect(CommunicationParams input)
        {
            try
            {
                Params = input;
                Client ??= new UdpClient(input.BindPort);
                Client.Connect(input.Host, input.Port);
                CommunicationObject = Client;
                IsConnected = Client.Client.Connected;
                ThreadFactory.Instance.StartWithRestart(Received, Params.ThreadKey.IsNullOrEmpty() ? nameof(UdpCommunication) : Params.ThreadKey, ex => ex.Fatal(ex.Message));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Close()
        {
            try
            {
                ThreadFactory.Instance.IncludeDispose(Params.ThreadKey.IsNullOrEmpty() ? nameof(UdpCommunication) : Params.ThreadKey);
                Client.Close();
                IsConnected = Client.Client.Connected;
                Client.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<byte[]> SendAndReadByteAsync(string cmd)
        {
            try
            {
                ReadBack = true;
                Cache.Clear();
                Record(null, cmd, true);
                var scmd = cmd.ByBytes();
                await Client.SendAsync(scmd, scmd.Length);
                byte[] bytes = (await Client.ReceiveAsync()).Buffer;
                Record(bytes, null, false);
                ReadBack = false;
                return bytes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> SendAndReadStringAsync(string cmd)
        {
            try
            {
                ReadBack = true;
                Cache.Clear();
                Record(null, cmd, true);
                var scmd = cmd.ByBytes();
                await Client.SendAsync(scmd, scmd.Length);
                byte[] bytes = (await Client.ReceiveAsync()).Buffer;
                Record(bytes, null, false);
                ReadBack = false;
                return bytes.ByString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task SendAndNoRead(string cmd)
        {
            try
            {
                Cache.Clear();
                Record(null, cmd, true);
                var scmd = cmd.ByBytes();
                await Client.SendAsync(scmd, scmd.Length);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task SendAndReadInCache(string cmd)
        {
            try
            {
                ReadBack = false;
                Cache.Clear();
                Record(null, cmd, true);
                var scmd = cmd.ByBytes();
                await Client.SendAsync(scmd, scmd.Length);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<byte[]> SendAndReadByteAsync(byte[] cmd)
        {
            try
            {
                ReadBack = true;
                Cache.Clear();
                Record(cmd, null, true);
                await Client.SendAsync(cmd, cmd.Length);
                byte[] bytes = (await Client.ReceiveAsync()).Buffer;
                Record(bytes, null, false);
                ReadBack = false;
                return bytes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> SendAndReadStringAsync(byte[] cmd)
        {
            try
            {
                ReadBack = true;
                Cache.Clear();
                Record(cmd, null, true);
                await Client.SendAsync(cmd, cmd.Length);
                byte[] bytes = (await Client.ReceiveAsync()).Buffer;
                Record(bytes, null, false);
                ReadBack = false;
                return bytes.ByString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task SendAndNoRead(byte[] cmd)
        {
            try
            {
                Cache.Clear();
                Record(cmd, null, true);
                await Client.SendAsync(cmd, cmd.Length);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task SendAndReadInCache(byte[] cmd)
        {
            try
            {
                ReadBack = false;
                Cache.Clear();
                Record(cmd, null, true);
                await Client.SendAsync(cmd, cmd.Length);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task Received()
        {
            if (ReadBack == false)
            {
                byte[] bytes = (await Client.ReceiveAsync()).Buffer;
                Record(bytes, null, false);
                Cache.AddRange(bytes);
            }
        }


        private void Record(byte[] bytes, string cmd, bool IsSend)
        {
            if (cmd.IsNullOrEmpty())
            {
                if (this.Params.IsDecodeWriteLog)
                    $"{this.Params.LogHead} {(IsSend ? "Send -->" : "Received <--")} {bytes.ByString()}".Info();
                else
                    $"{this.Params.LogHead} {(IsSend ? "Send -->" : "Received <--")} {bytes.WithByteHex()}".Info();
            }
            else
            {
                if (this.Params.IsDecodeWriteLog)
                    $"{this.Params.LogHead} {(IsSend ? "Send -->" : "Received <--")} {cmd}".Info();
                else
                    $"{this.Params.LogHead} {(IsSend ? "Send -->" : "Received <--")} {cmd}".Info();
            }
        }

    }
}
