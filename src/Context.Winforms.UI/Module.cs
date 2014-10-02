using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Context.Interfaces.Services;
using Context.Interfaces.UI;
using Context.Interfaces.UI.CommandBars;
using Context.WinForms.UI.CommandBars;

namespace Context.WinForms.UI
{
    [Guid("3FD1A0B2-E80B-4338-80BB-6D1A6D48D5A1")]
    internal class Module : IModule, IMainFormProvider, ISetupUI, ICommandTarget
    {
        private static Guid IMainFormProviderGuid = typeof(IMainFormProvider).GUID;
        private static Guid ICommandBarServiceGuid = typeof(ICommandBarService).GUID;
        private static Guid IStatusBarServiceGuid = typeof(IStatusBarService).GUID;

        private readonly object syncLock;
        private IServiceManager manager;
        private CommandBarService barService;

        public Module()
        {
            syncLock = new object();
        }

        private CommandBarService BarService
        {
            get
            {
                if (barService == null)
                {
                    lock (syncLock)
                    {
                        if (barService == null)
                        {
                            barService = new CommandBarService(manager);
                        }
                    }
                }

                return barService;
            }
        }

        #region IModule Members

        public void Attach(IServiceManager manager)
        {
            this.manager = manager;
        }

        public void Detach()
        {
            barService.Close();
        }

        public object GetService(Guid serviceId)
        {
            if (serviceId == IMainFormProviderGuid)
            {
                return this;
            }

            if (serviceId == ICommandBarServiceGuid)
            {
                return BarService;
            }

            if (serviceId == IStatusBarServiceGuid)
            {
                return BarService;
            }

            return null;
        }

        #endregion

        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IMainFormProvider))
            {
                return this;
            }

            return null;
        }

        #endregion

        #region IMainFormProvider Members

        public object MainForm
        {
            get
            {
                return null;
            }
        }

        #endregion

        #region ISetupUI Members

        public void SetupUI()
        {
            ICommandService commands = (ICommandService)manager.GetService(typeof(ICommandService));
            ICommandBarService commandBars = (ICommandBarService)manager.GetService(typeof(ICommandBarService));

            commands.AddCommand("ApplicationExit", "File", "E&xit", "Exit Application", -1, CommandBarControlType.Menu, CommandBarControlBehavior.BeginGroup, Guid.Empty);
        }

        #endregion

        #region ICommandTarget Members

        public CommandStatus InvokeCommand(object sender, ICommand command, ICommandBarControl control)
        {
            if (command.Group == "File" && command.Name == "ApplicationExit")
            {
                Application.Exit();
                return CommandStatus.Handled;
            }

            return CommandStatus.Incompatible;
        }

        public CommandStatus UpdateCommand(object sender, ICommand command)
        {
            return CommandStatus.Incompatible;
        }

        #endregion
    }
}
