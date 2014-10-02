using System;
using System.Collections.Generic;
using Context.Interfaces.Content.Hierarchy;

namespace Context.Interfaces.Hosting
{
    public interface IWebSite
    {
        Guid ID { get; }

        string Name { get; }

        IList<Uri> Bindings { get; }

        IHierarchy Content { get; }
    }
}
