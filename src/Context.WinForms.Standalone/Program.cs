using System;
using Context.Core;
using Context.Interfaces.Services;

namespace Context.WinForms.Standalone
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (IStartupObject startup = new Startup())
            {
                Context.WinForms.UI.Standalone.Start(startup, args);
            }
        }
    }
}
