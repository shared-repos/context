using System;

namespace Context.Interfaces.Services
{
    public interface IPropertyProvider
    {
        bool IsPropertySupported(string propertyName);
        object GetDefaultValue(string propertyName);
        object this[string propertyName] { get; set; }
    }
}
