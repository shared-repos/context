using System;
using System.Xml;
using Context.Interfaces.Services;

namespace Context.Interfaces.Configuration
{
    public interface IConfigurationSection : IPropertyProvider
    {
        string Name { get; }
        void SetDefaults();
        void DefineProperty(string propertyName);
        void LoadSection(XmlNode sectionData);
        void SaveSection(XmlNode sectionData);
    }
}
