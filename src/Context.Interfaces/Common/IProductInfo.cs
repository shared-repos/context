using System;
using System.Drawing;

namespace Context.Interfaces.Common
{
    public interface IProductInfo
    {
        string ApplicationName { get; }
        string Description { get; }
        string ProductId { get; }
        Version ProductVersion { get; }
        ProductType ProductType { get; }
        BuildType BuildType { get; }
        Icon ApplicationIcon { get; }
        string CultureName { get; }
        string CompanyName { get; }
        string ProductName { get; }
        string Edition { get; }
        string RegistryRoot { get; }
        string OptionsRoot { get; }
        string ConfigFolder { get; }
        string DataFolder { get; }
        string Copyright { get; }
        string GetInfoString(string type);
    }
}
