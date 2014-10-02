using System;
using System.Collections.Generic;

namespace Context.Interfaces.AutoUpdate
{
    public interface IUpdateHistory
    {
        IPackage Package { get; }

        DateTime InstalledOn { get; }

        UpdateStatus Status { get; }

        string ErrorMessage { get; }
    }
}
