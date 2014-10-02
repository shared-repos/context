using System;
using System.Collections.Generic;

namespace Context.Interfaces.AutoUpdate
{
    public interface IPackage
    {
        string Id { get; }

        string Title { get; }

        string Category { get; }

        string Description { get; }

        string InstallScript { get; }

        bool IsRestartRequired { get; }

        bool IsRestartUIRequired { get; }

        IList<IPackageItem> Items { get; }
    }
}
