using System;

namespace Context.Interfaces.Configuration
{
    public interface IConfigurationProvider
    {
        IConfigurationSection[] GetConfigurationSections();
        IConfigurationPage[] GetConfigurationPages();
    }
}
