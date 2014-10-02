using System;
using Context.Interfaces.Services;

namespace Context.Interfaces.Data
{
    public interface IDataProvider
    {
        string ProviderName { get; }

        IDataSource CreateDataSource(IContext context);
    }
}
