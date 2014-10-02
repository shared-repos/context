using System;
using System.Collections.Generic;
using System.Text;

namespace Context.Interfaces.Common
{
    public interface ISystemInfo
    {
        SystemID System { get; }

        Version SystemVersion { get; }

        string ComponentInfos { get; }

        string CultureName { get; }
    }
}
