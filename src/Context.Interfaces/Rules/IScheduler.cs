using System;
using System.Collections.Generic;

namespace Context.Interfaces.Rules
{
    public interface IScheduler
    {
        IList<IScheduledItem> Schedules { get; }

        event ScheduledItemEventHandler Elapsed;
    }
}
