using System;
using System.IO.Ports;
using System.Threading;
using XExten.Advance.Communication.Model;

namespace XExten.Advance.Communication
{
    /// <summary>
    /// 串口通信
    /// </summary>
    internal class SerialCommunication : ICommunication
    {
        #region 字段
        private SerialPort Client;
        private bool DisposeReceived = true;
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
                Client ??= new SerialPort
                {
                    Parity = input.Parity,
                    DataBits = input.DataBits,
                    BaudRate = input.BaudRate,
                    StopBits = input.StopBits,
                    PortName = input.Host,
                    ReadTimeout = input.ReplayTimeout,
                    WriteTimeout = input.SendTimeout,
                };
                Client.Open();
                IsConnected = Client.IsOpen;
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
                    Client.Write(cmd, 0, cmd.Length);
                    int Timeout = 0;
                    while (Client.BytesToRead < 0)
                    {
                        Thread.Sleep(20);
                        Timeout += 20;
                        if (Timeout >= Client.ReadTimeout)
                            break;
                    }
                    if (Timeout < Client.ReadTimeout)
                    {
                        byte[] bytes = new byte[Client.BytesToRead];
                        Client.Read(bytes, 0, bytes.Length);
                        if (!DisposeReceived)
                            return bytes;
                    }
                    else
                        return Array.Empty<byte>();
                }
            }
            catch (Exception ex)
            {
                Error?.Invoke(ex);
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
                    Client.Write(cmd, 0, cmd.Length);
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
            if (Client != null && IsConnected)
            {
                if (flag)
                    Client.DataReceived += ReceivedMessage;
                else
                    Client.DataReceived -= ReceivedMessage;
            }
        }

        private void ReceivedMessage(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                var com = (SerialPort)sender;
                var len = com.BytesToRead;
                if (len > 0)
                {
                    byte[] bytes = new byte[len];
                    com.Read(bytes, 0, bytes.Length);
                    if (!DisposeReceived)
                        Received?.Invoke(bytes);
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
