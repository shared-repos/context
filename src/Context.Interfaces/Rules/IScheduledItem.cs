using System;

namespace Context.Interfaces.Rules
{
    public interface IScheduledItem
    {
        DateTime NextRunTime { get; }

        event EventHandler Changed;
    }
}
