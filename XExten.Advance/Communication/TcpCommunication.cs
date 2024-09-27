using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.Advance.Communication
{
    internal class TcpCommunication:ICommunication
    {
        public bool IsConnected { get; set; }

        public bool DisposeReceived { get; set; }

        public event Action<byte[]> Received;

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Connect(ConnectParams input)
        {
            throw new NotImplementedException();
        }

        public void UseAsyncReceived(bool flag)
        {
            throw new NotImplementedException();
        }
    }
}
