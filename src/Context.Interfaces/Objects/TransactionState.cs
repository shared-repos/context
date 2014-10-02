using System;

namespace Context.Interfaces.Objects
{
    public enum TransactionState
    {
        Started,
        Committed,
        RolledBack,
        Canceled
    }
}
