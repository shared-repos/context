using System;
using System.Threading;

namespace Context.Interfaces.System
{
    public class AsyncExceptionEventArgs : ThreadExceptionEventArgs
    {
        private IAsyncOperation asyncOperation;

        public AsyncExceptionEventArgs(IAsyncOperation asyncOperation, Exception exception)
            : base(exception)
        {
            this.asyncOperation = asyncOperation;
        }

        public IAsyncOperation AsyncOperation
        {
            get
            {
                return this.asyncOperation;
            }
        }
    }
}
