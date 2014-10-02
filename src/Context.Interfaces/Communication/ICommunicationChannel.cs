using System;
using System.Collections.Generic;

using System.Text;

namespace Context.Interfaces.Communication
{
    public interface ICommunicationChannel : IDisposable
    {
        string ID { get; }

        string Name { get; }

        string Prefix { get; }

        bool CanListen { get; }

        bool HasWait { set; }

        bool HasData { get; }

        bool IsConnected { get; }

        bool IsEmpty { get; }

        string LastErrorMessage { get; }

        string[] Listen();

        void StopListen();

        void Reset();

        object Receive(string address);

        bool Send(object source, string address);

        event EventHandler<ProgressEventArgs> Progress;

        event EventHandler<CommunicationErrorEventArgs> Error;
    }
}
