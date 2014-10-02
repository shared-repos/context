using System;
using Context.Interfaces.Services;
using System.Runtime.InteropServices;

namespace Context.Interfaces.Configuration
{
    [Guid("ACCBAD6F-A59C-48d1-8102-6952D2B30833")]
    public interface IConfigurationManager : IPropertyProvider, IConfigurationProvider
    {
        IConfigurationSection GetSection(string sectionName);

        event ConfigurationChangedHandler ConfigurationChanged;
    }

    public delegate void ConfigurationChangedHandler();
}
