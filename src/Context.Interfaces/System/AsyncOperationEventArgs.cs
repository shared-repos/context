using System;

namespace Context.Interfaces.System
{
    public class AsyncOperationEventArgs : EventArgs
    {
        private IAsyncOperation asyncOperation;
        private object parameter;

        public AsyncOperationEventArgs(IAsyncOperation asyncOperation)
        {
            this.asyncOperation = asyncOperation;
        }

        public AsyncOperationEventArgs(IAsyncOperation asyncOperation, object parameter)
        {
            this.asyncOperation = asyncOperation;
            this.parameter = parameter;
        }

        public IAsyncOperation AsyncOperation
        {
            get
            {
                return this.asyncOperation;
            }
        }

        public object Parameter
        {
            get
            {
                return this.parameter;
            }
            set
            {
                this.parameter = value;
            }
        }
    }
}
