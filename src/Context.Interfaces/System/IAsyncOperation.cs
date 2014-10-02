using System;

namespace Context.Interfaces.System
{
    public interface IAsyncOperation
    {
        event EventHandler<AsyncOperationEventArgs> Canceled;
        event EventHandler<AsyncOperationEventArgs> Completed;
        event EventHandler<AsyncOperationEventArgs> Done;
        event EventHandler<AsyncExceptionEventArgs> Failed;

        void Cancel();
        bool CancelAndWait();
        void Invoke();
        void Start();
        bool WaitUntilDone();

        bool CancelRequested { get; }
        bool IsDone { get; }
        string Name { get; set; }
    }
}
