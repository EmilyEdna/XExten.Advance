using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using XExten.Advance.CommunicationFramework.Model;
using XExten.Advance.LinqFramework;
using XExten.Advance.LogFramework;
using XExten.Advance.ThreadFramework;

namespace XExten.Advance.CommunicationFramework
{
    /// <summary>
    /// Tcp通信
    /// </summary>
    internal class TcpCommunication : ICommunication
    {
       
        private TcpClient Client;
        private CommunicationParams Params;
        private NetworkStream Stream;
        private StreamReader Reader;
        private StreamWriter Writer;
        private bool ReadBack;

        public bool IsConnected { get; private set; }

        public object CommunicationObject { get; private set; }

        public List<byte> Cache { get; private set; }

        public TcpCommunication()
        {
            Cache= new List<byte>();
            CommunicationObject = Client= new TcpClient();
            ReadBack = false;
        }

        public void Connect(CommunicationParams input)
        {
            try
            {
                Params = input;
                Client.Connect(input.Host, input.Port);
                Client.NoDelay = true;
                Stream = Client.GetStream();
                Reader = new StreamReader(Stream);
                Writer = new StreamWriter(Stream);
                IsConnected = Client.Connected;
                ThreadFactory.Instance.StartWithRestart(Received, Params.ThreadKey.IsNullOrEmpty() ? nameof(TcpCommunication) : Params.ThreadKey, ex => ex.Fatal(ex.Message));
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
                ThreadFactory.Instance.IncludeDispose(Params.ThreadKey.IsNullOrEmpty() ? nameof(TcpCommunication) : Params.ThreadKey);
                Client.Close();
                IsConnected = Client.Connected;
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
                await Writer.WriteLineAsync(cmd);
                while (!Stream.DataAvailable)
                {
                    await Task.Delay(25);
                }
                byte[] bytes = new byte[Client.Available];
                Stream.Read(bytes,0, bytes.Length);
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
                await Writer.WriteLineAsync(cmd);
                var data = await Reader.ReadLineAsync();
                Record(null, data, false);
                ReadBack = false;
                return data;
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
                await Writer.WriteLineAsync(cmd);
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
                await Writer.WriteLineAsync(cmd);
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
                Stream.Write(cmd,0,cmd.Length);
                while (!Stream.DataAvailable)
                {
                    await Task.Delay(25);
                }
                byte[] bytes = new byte[Client.Available];
                Stream.Read(bytes, 0, bytes.Length);
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
                Stream.Write(cmd, 0, cmd.Length);
                var data = await Reader.ReadLineAsync();
                Record(null, data, false);
                ReadBack = false;
                return data;
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
                Stream.Write(cmd, 0, cmd.Length);
                await Task.CompletedTask;
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
                Stream.Write(cmd, 0, cmd.Length);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Received()
        {
            if (ReadBack == false)
            {
                if (Stream.DataAvailable)
                {
                    byte[] bytes = new byte[Client.Available];
                    Stream.Read(bytes, 0, bytes.Length);
                    Record(bytes, null, false);
                    Cache.AddRange(bytes);
                }
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
