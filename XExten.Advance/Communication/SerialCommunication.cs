using System;
using System.IO.Ports;
using System.Threading;

namespace XExten.Advance.Communication
{
    internal class SerialCommunication : ICommunication
    {
        private SerialPort Serial;

        private bool IsAsync = false;

        public bool IsConnected { get; set; } = false;

        public bool DisposeReceived { get; set; } = false;

        public event Action<byte[]> Received;

        public void Connect(ConnectParams input)
        {
            try
            {
                Serial = new SerialPort
                {
                    Parity = input.Parity,
                    DataBits = input.DataBits,
                    BaudRate = input.BaudRate,
                    StopBits = input.StopBits,
                    PortName = input.Host,
                    ReadTimeout = input.ReplayTimeout,
                    WriteTimeout = input.SendTimeout,
                };
                Serial.Open();
                IsConnected = Serial.IsOpen;
            }
            catch
            {
                IsConnected = false;
            }
        }
        public void SendCommand(byte[] cmd)
        {
            if (Serial != null && IsConnected)
            {
                Serial.Write(cmd, 0, cmd.Length);
                if (DisposeReceived) return;
                if (!IsAsync)
                {
                    int Timeout = 0;
                    while (Serial.BytesToRead < 2)
                    {
                        Thread.Sleep(20);
                        Timeout += 20;
                        if (Timeout >= Serial.ReadTimeout)
                            break;
                    }
                    if (Timeout < Serial.ReadTimeout)
                    {
                        byte[] bytes = new byte[Serial.BytesToRead];
                        Serial.Read(bytes, 0, bytes.Length);
                        Received?.Invoke(bytes);
                    }
                }
            }
        }


        public void Close()
        {
            if (Serial != null && IsConnected)
            {
                Serial.Close();
                IsConnected = false;
            }
        }


        public void UseAsyncReceived(bool flag)
        {
            IsAsync = flag;
            if (Serial != null && IsConnected)
            {
                if (IsAsync)
                    Serial.DataReceived += Serial_DataReceived;
                else
                    Serial.DataReceived -= Serial_DataReceived;
            }

        }

        private void Serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (DisposeReceived) return;
            var com = (SerialPort)sender;
            byte[] bytes = new byte[com.BytesToRead];
            com.Read(bytes, 0, bytes.Length);
            Received?.Invoke(bytes);
        }
    }
}
