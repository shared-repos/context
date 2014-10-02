using System;
using System.Collections.Generic;
using System.Text;

namespace Context.Interfaces.Common
{
    public interface ICustomerInfo
    {
        Guid ID { get; }

        string LoginName { get; }

        string Name { get; }

        DateTime Registered { get; }
    }
}
