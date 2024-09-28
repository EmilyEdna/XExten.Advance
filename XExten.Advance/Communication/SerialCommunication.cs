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
            catch
            {
                IsConnected = false;
            }
        }
        public void SendCommand(byte[] cmd)
        {
            if (Client != null && IsConnected)
            {
                Client.Write(cmd, 0, cmd.Length);
                if (DisposeReceived) return;
                if (!IsAsync)
                {
                    int Timeout = 0;
                    while (Client.BytesToRead < 2)
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
                        Received?.Invoke(bytes);
                        Array.Clear(bytes, 0, bytes.Length);
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
            if (Client != null && IsConnected)
            {
                if (IsAsync)
                    Client.DataReceived += ReceivedMessage;
                else
                    Client.DataReceived -= ReceivedMessage;
            }

        }
        #endregion

        #region 私有
        private void ReceivedMessage(object sender, SerialDataReceivedEventArgs e)
        {
            if (DisposeReceived) return;
            var com = (SerialPort)sender;
            byte[] bytes = new byte[com.BytesToRead];
            com.Read(bytes, 0, bytes.Length);
            Received?.Invoke(bytes);
            Array.Clear(bytes, 0, bytes.Length);
        }
        #endregion

    }
}
