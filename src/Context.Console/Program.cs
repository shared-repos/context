using System;
using System.Collections.Generic;
using System.Text;
using Context.Interfaces.Services;
using Context.Core;

namespace Context.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            using (IStartupObject startup = new Startup())
            {
                ConsoleApplication.Start(startup, args);
            }
        }
    }
}
