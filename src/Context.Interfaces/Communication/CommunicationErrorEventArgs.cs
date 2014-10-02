using System;
using System.Collections.Generic;
using System.Text;

namespace Context.Interfaces.Communication
{
    public class CommunicationErrorEventArgs : EventArgs
    {
        public Exception Exception { get; private set; }

        public CommunicationErrorEventArgs(Exception exception)
        {
            Exception = exception;
        }
    }
}
