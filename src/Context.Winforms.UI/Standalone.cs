using System;
using System.Threading;
using System.Windows.Forms;
using Context.Interfaces.Configuration;
using Context.Interfaces.Services;
using Context.Interfaces.UI;

namespace Context.WinForms.UI
{
    public static class Standalone
    {
        private const string ApplicationSection = "Application";
        private const string UpdateModuleIdSetting = "Application.UpdateModuleId";
        private const string UpdateAppSignalSetting = "Application.UpdateSignal";
        private const string OpenSignalSetting = "Application.OpenSignal";

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

            IConfigurationManager config = (IConfigurationManager)startup.GetService(typeof(IConfigurationManager));
            string openSignal = Convert.ToString(config[OpenSignalSetting]);

            if (CheckProcessStarted(openSignal))
            {
                return;
            }

            var updateModuleGuidStr = Convert.ToString(config[UpdateModuleIdSetting]);
            Guid updateModuleGuid = string.IsNullOrEmpty(updateModuleGuidStr) ? Guid.Empty : new Guid(updateModuleGuidStr);
            string updateSignal = Convert.ToString(config[UpdateAppSignalSetting]);

            if (StartApplicationWithUpdate(startup, updateSignal, updateModuleGuid))
            {
                return;
            }

            StartApplication(startup);
        }

        private static bool StartApplicationWithUpdate(IStartupObject startup, string updateSignal, Guid updateModuleGuid)
        {
            if (!string.IsNullOrEmpty(updateSignal))
            {
                bool created;
                EventWaitHandle eventHandle = SystemUtils.CreateEventWaitHandle(updateSignal, false, out created);
                bool finished = false;
                if (created)
                {
                    Thread updatorThread = new Thread(delegate()
                    {
                        eventHandle.Reset();
                        eventHandle.WaitOne();
                        if (!finished)
                        {
                            if (updateModuleGuid != Guid.Empty)
                            {
                                IModuleManager modules = (IModuleManager)startup.GetService(typeof(IModuleManager));
                                modules.LoadModule(updateModuleGuid);
                            }
                            Application.Exit();
                        }
                    });
                    updatorThread.Start();

                    try
                    {
                        StartApplication(startup);
                    }
                    finally
                    {
                        finished = true;
                        eventHandle.Set();
                    }
                    return true;
                }
            }

            return false;
        }

        private static bool CheckProcessStarted(string openSignal)
        {
            if (string.IsNullOrEmpty(openSignal))
            {
                openSignal = @"Global\OpenSignal";
            }

            if (ProcessUtils.ProcessStarted())
            {
                try
                {
                    SystemUtils.SetEventWaitHandle(openSignal);
                }
                catch
                {
                }

                return true;
            }

            return false;
        }

        private static void StartApplication(IStartupObject startup)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            startup.Start();

            IMainFormProvider provider = (IMainFormProvider)startup.GetService(typeof(IMainFormProvider));
            Form mainForm = null;
            if (provider != null)
            {
                mainForm = provider.MainForm as Form;
            }

            Application.Run(mainForm);
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
                MessageBox.Show(ex.StackTrace, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
        }
    }
}
