using System;
using System.Collections;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using Microsoft.Win32;
using Context.Core;

namespace Context.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new ProductService();
            service.Uninstalling += OnUninstalling;
            service.Uninstall += OnUninstall;
            ServiceRoot.Run(args, service);
        }

        private static void OnUninstalling(object sender, EventArgs e)
        {
            //string pattern = Constants.CompanyName + ".";
            //var comparison = StringComparison.InvariantCultureIgnoreCase;
            //ProcessUtils.KillProcessesLike(x => x.StartsWith(pattern, comparison) && !x.EndsWith("tmp", comparison));
            //FileUtils.DeleteOrMoveToTempFile(Path.Combine(FileUtils.EntryDirectory, "...ShellExtension.dll"));
        }

        private static void OnUninstall(object sender, EventArgs e)
        {
            //ProcessUtils.ShellExecute(Constants.FeedbackUrl, "open");
        }
    }
}
