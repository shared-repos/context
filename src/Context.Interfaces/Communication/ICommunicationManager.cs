using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Context.Interfaces.Communication
{
    [Guid("682DF4D3-8F96-49f8-906D-2F9B9CD5396D")]
    public interface ICommunicationManager
    {
        void RegisterChannel(ICommunicationChannel channel);

        void UnregisterChannel(ICommunicationChannel channel);

        ICommunicationChannel[] ListCommunicationChannels();

        ICommunicationChannel GetChannel(string channelId);

        bool Active { get; set; }

        event MessageEventHandler OnMessage;
    }

    public delegate void MessageEventHandler(ICommunicationChannel channel, string address);
}
