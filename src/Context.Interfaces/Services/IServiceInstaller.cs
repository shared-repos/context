using System;
using System.Collections.Generic;
using System.Text;

namespace Context.Interfaces.Services
{
    public interface IServiceInstaller
    {
        event EventHandler Install;

        event EventHandler Uninstalling;

        event EventHandler Uninstall;
    }
}
