using System;
using System.Collections.Generic;
using System.Text;

namespace Context.Interfaces.Communication
{
    public class ProgressEventArgs : EventArgs
    {
        public string Name { get; private set; }

        public string Address { get; private set; }

        public long BytesSent { get; private set; }

        public long TotalBytesToSend { get; private set; }

        public bool Cancel { get; set; }

        public string Operation { get; set; }

        public ProgressEventArgs(string name, string address, long bytesSent, long totalBytesToSend, string operation)
        {
            Name = name;
            Address = address;
            BytesSent = bytesSent;
            TotalBytesToSend = totalBytesToSend;
            Operation = operation;
        }
    }
}
