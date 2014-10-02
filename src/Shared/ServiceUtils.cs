using System;
using System.IO;
using System.Reflection;
using System.Security.AccessControl;
using System.Runtime.InteropServices;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.ServiceProcess;
using Microsoft.Win32;

namespace Context
{
    internal static class ServiceUtils
    {
        public static void StartService(string serviceName)
        {
            using (ServiceController controller = new ServiceController(serviceName))
            {
                if (controller.Status != ServiceControllerStatus.Running)
                {
                    controller.Start();
                    controller.WaitForStatus(ServiceControllerStatus.Running);
                }
            }
        }

        public static void StartService(string serviceName, TimeSpan timeout)
        {
            using (ServiceController controller = new ServiceController(serviceName))
            {
                if (controller.Status != ServiceControllerStatus.Running)
                {
                    controller.Start();
                    controller.WaitForStatus(ServiceControllerStatus.Running, timeout);
                }
            }
        }

        public static void StopService(string serviceName)
        {
            using (ServiceController controller = new ServiceController(serviceName))
            {
                if (controller.Status != ServiceControllerStatus.Stopped)
                {
                    controller.Stop();
                    controller.WaitForStatus(ServiceControllerStatus.Stopped);
                }
            }
        }

        public static void StopService(string serviceName, TimeSpan timeout)
        {
            using (ServiceController controller = new ServiceController(serviceName))
            {
                if (controller.Status != ServiceControllerStatus.Stopped)
                {
                    controller.Stop();
                    controller.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                }
            }
        }

        public static string GetServiceImagePath(string serviceName)
        {
            string registryPath = @"SYSTEM\CurrentControlSet\Services\" + serviceName;
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryPath))
            {
                string value = Convert.ToString(key.GetValue("ImagePath"));
                value = value.Trim('\"');
                return value;
            }
        }

        public static bool IsInstalled(string name)
        {
            var services = ServiceController.GetServices();
            try
            {
                foreach (ServiceController service in services)
                {
                    if (string.Compare(service.ServiceName, name, true) == 0)
                    {
                        return true;
                    }
                }
            }
            finally
            {
                foreach (ServiceController service in services)
                {
                    service.Dispose();
                }
            }

            return false;
        }
    }
}
