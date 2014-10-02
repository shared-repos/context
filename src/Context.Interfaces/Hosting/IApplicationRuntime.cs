using System;

namespace Context.Interfaces.Hosting
{
    public interface IApplicationRuntime
    {
        IWebApplication BuildSiteApplication(IWebSite site);
    }
}
