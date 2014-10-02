using System;
using System.Collections.Generic;
using System.Text;
using Context.Interfaces.Common;

namespace Context.Core
{
    internal class SystemInfo : ISystemInfo
    {
        #region ISystemInfo Members

        public SystemID System { get; internal set; }

        public Version SystemVersion { get; internal set; }

        public string ComponentInfos { get; internal set; }

        public string CultureName { get; internal set; }

        #endregion
    }
}
