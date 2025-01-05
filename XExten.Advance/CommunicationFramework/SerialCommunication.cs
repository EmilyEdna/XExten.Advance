using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading.Tasks;
using XExten.Advance.CommunicationFramework.Model;
using XExten.Advance.LinqFramework;
using XExten.Advance.LogFramework;
using XExten.Advance.ThreadFramework;

namespace XExten.Advance.CommunicationFramework
{
    /// <summary>
    /// 串口通信
    /// </summary>
    internal class SerialCommunication : ICommunication
    {

        private SerialPort Client;
        private CommunicationParams Params;
        private bool ReadBack;

        public bool IsConnected { get; private set; }

        public object CommunicationObject { get; private set; }

        public List<byte> Cache { get; private set; }

        public SerialCommunication()
        {
            ReadBack = false;
            Cache = new List<byte>();
            CommunicationObject = Client = new SerialPort();
        }

        public void Connect(CommunicationParams input)
        {
            try
            {
                Params = input;
                Client.Parity = input.Parity;
                Client.DataBits = input.DataBits;
                Client.BaudRate = input.BaudRate;
                Client.StopBits = input.StopBits;
                Client.PortName = input.Host;
                Client.Open();
                IsConnected = Client.IsOpen;
                ThreadFactory.Instance.StartWithRestart(Received, Params.ThreadKey.IsNullOrEmpty()? nameof(SerialCommunication): Params.ThreadKey, ex => ex.Fatal(ex.Message));
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
                ThreadFactory.Instance.IncludeDispose(Params.ThreadKey.IsNullOrEmpty() ? nameof(SerialCommunication) : Params.ThreadKey);
                Client.Close();
                IsConnected = Client.IsOpen;
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
                Cache.Clear();
                Record(null, cmd, true);
                ReadBack = true;
                Client.DiscardInBuffer();
                Client.DiscardOutBuffer();
                Client.WriteLine(cmd);
                while (Client.BytesToRead < 0)
                {
                    await Task.Delay(25);
                }
                byte[] bytes = new byte[Client.BytesToRead];
                Client.Read(bytes, 0, bytes.Length);
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
                Cache.Clear();
                Record(null, cmd, true);
                ReadBack = true;
                Client.DiscardInBuffer();
                Client.DiscardOutBuffer();
                Client.WriteLine(cmd);
                var data = Client.ReadLine();
                Record(null, data, false);
                ReadBack = false;
                await Task.CompletedTask;
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
                Client.DiscardInBuffer();
                Client.DiscardOutBuffer();
                Client.WriteLine(cmd);
                await Task.CompletedTask;
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
                Cache.Clear();
                ReadBack = false;
                Record(null, cmd, true);
                Client.DiscardInBuffer();
                Client.DiscardOutBuffer();
                Client.WriteLine(cmd);
                await Task.CompletedTask;
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
                Cache.Clear();
                Record(cmd, null, true);
                ReadBack = true;
                Client.DiscardInBuffer();
                Client.DiscardOutBuffer();
                Client.Write(cmd,0,cmd.Length);
                while (Client.BytesToRead < 0)
                {
                    await Task.Delay(25);
                }
                byte[] bytes = new byte[Client.BytesToRead];
                Client.Read(bytes, 0, bytes.Length);
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
                Cache.Clear();
                Record(cmd, null, true);
                ReadBack = true;
                Client.DiscardInBuffer();
                Client.DiscardOutBuffer();
                Client.Write(cmd,0,cmd.Length);
                var data = Client.ReadLine();
                Record(null, data, false);
                ReadBack = false;
                await Task.CompletedTask;
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
                Client.DiscardInBuffer();
                Client.DiscardOutBuffer();
                Client.Write(cmd, 0, cmd.Length);
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
                Cache.Clear();
                ReadBack = false;
                Record(cmd, null, true);
                Client.DiscardInBuffer();
                Client.DiscardOutBuffer();
                Client.Write(cmd, 0, cmd.Length);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Record(byte[] bytes,string cmd, bool IsSend)
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

        private void Received()
        {
            if (ReadBack == false)
            {
                byte[] bytes = new byte[Client.BytesToRead];
                Client.Read(bytes, 0, bytes.Length);
                Record(bytes, null, false);
                Cache.AddRange(bytes);
            }
        }
    }
}
