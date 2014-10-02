using System;
using System.Collections.Generic;
using System.Text;
using Context.Interfaces.Configuration;
using Context.Interfaces.Services;

namespace Context.Core
{
    public static class ConsoleApplication
    {
        private const string ApplicationSection = "Application";

        public static void Start(IStartupObject startup, string[] args)
        {
            IConfigurationManager config = (IConfigurationManager)startup.GetService(typeof(IConfigurationManager));
            if (config != null && args != null)
            {
                ParseCommandLineArguments(config, args);
            }

            Start(startup);
        }

        public static void Start(IStartupObject startup)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(OnUnhandledException);

            StartApplication(startup);
        }

        private static void StartApplication(IStartupObject startup)
        {
            startup.Start();
        }

        private static void ParseCommandLineArguments(IConfigurationManager config, string[] args)
        {
            string name = null;
            int i = 0;

            IConfigurationSection section = config.GetSection(ApplicationSection);
            if (section == null)
            {
                return;
            }

            foreach (string arg in args)
            {
                if (!string.IsNullOrEmpty(arg) && (arg[0] == '/' || StringHelpers.IsDashSymbol(arg[0])))
                {
                    name = arg.Substring(1);
                    section.DefineProperty(name);
                    section[name] = true;
                    continue;
                }

                if (name == null)
                {
                    name = "arg" + i.ToString();
                    i++;
                    section.DefineProperty(name);
                }

                section[name] = arg;
                name = null;
            }
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
