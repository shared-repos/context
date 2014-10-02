using System;
using System.Collections;
using System.Configuration.Install;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using Context.Interfaces.Common;
using Context.Interfaces.Services;

namespace Context.Core
{
    public class ServiceRoot : ServiceBase, IServiceInstaller
    {
        private const string PromptDescription = "Type 'exit' to stop service.";
        private const string Prompt = ">";
        private const int UninstallIntervalInMinutes = 5;

        private const string MessageHelp = "\r\nUsage: <service> [-i] | [-install] | [-u] | [-uninstall] | [-r] | [-run]\r\n\r\nOptions:\r\n\t-i -install\tInstall the service.\r\n\t-u -uninstall\tUninstall the service.\r\n\t-r -run  \tRun the service in the console.\r\n\t-? -help  \tDisplay this help screen.";
        private const string MessageInvalidArguments = "Invalid command line arguments: use -i/-install to install service\r\nor -u/-uninstall to uninstall service.\r\nTo run service in the console mode use -r/-run option.";

        private readonly IStartupObject startup;
        private readonly IProductInfo product;

        public ServiceRoot(IStartupObject startup)
        {
            this.startup = startup;
            product = (IProductInfo)startup.GetService(typeof(IProductInfo));
            startup.AddService(typeof(IServiceInstaller), this);

            this.ServiceName = product.ApplicationName;
        }

        public event EventHandler Install;

        public event EventHandler Uninstalling;

        public event EventHandler Uninstall;

        public static void Run(string[] args, ServiceRoot service)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(service.OnUnhandledException);

            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Use <service> -? to display help screen.");

                ServiceBase[] servicesToRun = new ServiceBase[] { service };
                ServiceBase.Run(servicesToRun);
                return;
            }

            if (args.Length == 1 && !string.IsNullOrEmpty(args[0]))
            {
                string arg = args[0].ToLower();
                if (StringHelpers.IsDashSymbol(arg[0]))
                {
                    arg = "/" + arg.Substring(1);
                }

                switch (arg)
                {
                    case "/?":
                    case "/help":
                        Console.WriteLine(MessageHelp);
                        break;
                    case "/i":
                    case "/install":
                        DoInstall(service);
                        break;
                    case "/u":
                    case "/uninstall":
                        DoUninstall(service);
                        break;
                    case "/r":
                    case "/run":
                        RunInConsole(service.startup);
                        break;
                    default:
                        if (Environment.UserInteractive)
                        {
                            Console.WriteLine(MessageInvalidArguments);
                        }
                        else
                        {
                            ServiceBase[] servicesToRun = new ServiceBase[] { service };
                            ServiceBase.Run(servicesToRun);
                        }
                        break;
                }
            }
            else
            {
                if (Environment.UserInteractive)
                {
                    Console.WriteLine(MessageInvalidArguments);
                }
                else
                {
                    ServiceBase[] servicesToRun = new ServiceBase[] { service };
                    ServiceBase.Run(servicesToRun);
                }
            }
        }

        private static void DoInstall(ServiceRoot service)
        {
            try
            {
                if (!ServiceUtils.IsInstalled(service.ServiceName))
                {
                    InstallService(service);
                    if (!ServiceUtils.IsInstalled(service.ServiceName))
                    {
                        Thread.Sleep(5000);
                        if (!ServiceUtils.IsInstalled(service.ServiceName))
                        {
                            throw new Exception(string.Format("Service '{0}' is not installed.", service.ServiceName));
                        }
                    }
                    ServiceUtils.StartService(service.ServiceName, TimeSpan.FromSeconds(30));
                    service.OnInstall();
                }
                else
                {
                    ServiceUtils.StartService(service.ServiceName, TimeSpan.FromSeconds(30));
                }
            }
            catch (Exception ex)
            {
                service.LogError(string.Format("Error installing service '{0}': {1}:{2}, StackTrace: {3}", service.ServiceName, ex.GetType().Name, ex.Message, ex.StackTrace));
            }
        }

        private static void DoUninstall(ServiceRoot service)
        {
            try
            {
                if (ServiceUtils.IsInstalled(service.ServiceName))
                {
                    ServiceUtils.StopService(service.ServiceName, TimeSpan.FromSeconds(30));
                    ProcessUtils.KillProcesses();
                    UninstallService(service);
                    service.OnUninstall();
                }
            }
            catch (Exception ex)
            {
                service.LogError(string.Format("Error uninstalling service '{0}': {1}:{2}, StackTrace: {3}", service.ServiceName, ex.GetType().Name, ex.Message, ex.StackTrace));
            }
        }

        private static void RunInConsole(IStartupObject startup)
        {
            try
            {
                startup.Start();
                Console.WriteLine(PromptDescription);
                Console.WriteLine();
                while (true)
                {
                    Console.Write(Prompt);
                    string line = Console.ReadLine();
                    if (line.ToLower() == "exit")
                    {
                        if (startup.QueryClose())
                        {
                            break;
                        }
                    }
                    if (line.ToLower() == "stop")
                    {
                        startup.QueryClose();
                    }
                    if (line.ToLower() == "start")
                    {
                        startup.Start();
                    }
                }
            }
            finally
            {
                startup.Dispose();
            }
        }

        protected virtual void LogError(string message)
        {
            try
            {
                Console.WriteLine(message);
                string source = product.ApplicationName;
                string log = "Application";

                if (!EventLog.SourceExists(source))
                {
                    EventLog.CreateEventSource(source, log);
                }

                EventLog.WriteEntry(source, message, EventLogEntryType.Error);
            }
            catch
            {
            }
        }

        protected virtual void LogInfo(string message)
        {
            try
            {
                Console.WriteLine(message);
                string source = product.ApplicationName;
                string log = "Application";

                if (!EventLog.SourceExists(source))
                {
                    EventLog.CreateEventSource(source, log);
                }

                EventLog.WriteEntry(source, message, EventLogEntryType.Information);
            }
            catch
            {
            }
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
            RequestAdditionalTime(100000);
            try
            {
                LogInfo(string.Format("Starting service '{0}'", this.ServiceName));
                startup.Start();
                LogInfo(string.Format("Started service '{0}'", this.ServiceName));
            }
            catch (Exception ex)
            {
                LogError(string.Format("Error starting service '{0}': {1}:{2}, StackTrace: {3}", this.ServiceName, ex.GetType().Name, ex.Message, ex.StackTrace));
            }
        }

        protected override void OnStop()
        {
            base.OnStop();
            RequestAdditionalTime(60000);
            try
            {
                startup.QueryClose();
                LogInfo(string.Format("Stopped service '{0}'", this.ServiceName));
            }
            catch (Exception ex)
            {
                LogError(string.Format("Error stopping service '{0}': {1}:{2}, StackTrace: {3}", this.ServiceName, ex.GetType().Name, ex.Message, ex.StackTrace));
            }
        }

        protected virtual void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception ex = e.ExceptionObject as Exception;
                if (ex != null)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);

                    string source = product.ApplicationName;
                    string log = "Application";

                    if (!EventLog.SourceExists(source))
                    {
                        EventLog.CreateEventSource(source, log);
                    }

                    string sevent = string.Format("Error: {0}:{1}, StackTrace: {2}", ex.GetType().Name, ex.Message, ex.StackTrace);

                    EventLog.WriteEntry(source, sevent, EventLogEntryType.Error);
                }
            }
            catch
            {
            }
        }

        private void OnInstall()
        {
            try
            {
                if (Install != null)
                {
                    Install(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                LogError(string.Format("Error in Install event: {0}:{1}, StackTrace: {2}", ex.GetType().Name, ex.Message, ex.StackTrace));
            }
        }

        private void OnUninstall()
        {
            try
            {
                if (Uninstalling != null)
                {
                    Uninstalling(this, EventArgs.Empty);
                }

                var uninstalledAt = new DateTime(Convert.ToInt64(SystemUtils.RegistryGetValues(product.RegistryRoot)["UninstalledAt"]));
                SystemUtils.RegistrySetValues(product.RegistryRoot, "UninstalledAt", DateTime.UtcNow.Ticks);
                if (uninstalledAt.AddMinutes(UninstallIntervalInMinutes) > DateTime.UtcNow)
                {
                    return;
                }

                if (Uninstall != null)
                {
                    Uninstall(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                LogError(string.Format("Error in Uninstall event: {0}:{1}, StackTrace: {2}", ex.GetType().Name, ex.Message, ex.StackTrace));
            }
        }

        private static AssemblyInstaller CreateInstaller(ServiceBase service)
        {
            AssemblyInstaller installer = new AssemblyInstaller(service.GetType().Assembly, null);
            installer.UseNewContext = true;
            return installer;
        }

        private static void InstallService(ServiceBase service)
        {
            using (AssemblyInstaller installer = CreateInstaller(service))
            {
                IDictionary state = new Hashtable();
                try
                {
                    installer.Install(state);
                    installer.Commit(state);
                }
                catch
                {
                    try
                    {
                        installer.Rollback(state);
                    }
                    catch
                    {
                    }
                    throw;
                }
            }
        }

        private static void UninstallService(ServiceBase service)
        {
            using (AssemblyInstaller installer = CreateInstaller(service))
            {
                IDictionary state = new Hashtable();
                installer.Uninstall(state);
            }
        }
    }
}
