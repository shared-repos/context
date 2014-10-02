using System;
using System.Collections;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using Microsoft.Win32;

namespace Context
{
    internal static class SystemUtils
    {
        public static object RegistryUserGetValue(string keyName, string valueName)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyName, false))
            {
                return key.GetValue(valueName);
            }
        }

        public static IDictionary RegistryGetValues(string keyName)
        {
            Hashtable values = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
            using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(keyName))
            {
                if (registryKey == null)
                {
                    return values;
                }

                var names = registryKey.GetValueNames();
                for (int i = 0; i < names.Length; i++)
                {
                    var name = names[i];
                    var value = registryKey.GetValue(name);
                    values[name] = value;
                }
            }

            return values;
        }

        public static void RegistrySetValues(string keyName, params object[] keyValues)
        {
            using (RegistryKey registryKey = OpenOrCreateSubKey(keyName))
            {
                for (int i = 1; i < keyValues.Length; i += 2)
                {
                    object value = keyValues[i];
                    string name = Convert.ToString(keyValues[i - 1]);
                    registryKey.SetValue(name, value);
                }

                registryKey.Flush();
            }
        }

        public static void RegistrySetValue(string keyName, string name, string value)
        {
            using (RegistryKey registryKey = OpenOrCreateSubKey(keyName))
            {
                registryKey.SetValue(name, value, RegistryValueKind.String);
                registryKey.Flush();
            }
        }

        private static RegistryKey OpenOrCreateSubKey(string name)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(name, true);
            if (key == null)
            {
                key = Registry.LocalMachine.CreateSubKey(name);
            }

            return key;
        }

        public static void SetEventWaitHandle(string eventName)
        {
            var wh = EventWaitHandle.OpenExisting(eventName);
            if (wh == null)
            {
                throw new Exception(string.Format("Event wait handle not found: '{0}'", eventName));
            }

            wh.Set();
        }

        public static EventWaitHandle CreateEventWaitHandle(string signalName, bool auto, out bool created)
        {
            var security = new EventWaitHandleSecurity();
            var users = new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null);
            var rule = new EventWaitHandleAccessRule(users, EventWaitHandleRights.FullControl, AccessControlType.Allow);
            security.AddAccessRule(rule);
            users = new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null);
            rule = new EventWaitHandleAccessRule(users, EventWaitHandleRights.FullControl, AccessControlType.Allow);
            security.AddAccessRule(rule);

            EventResetMode mode = auto ? EventResetMode.AutoReset : EventResetMode.ManualReset;
            EventWaitHandle eventHandle = new EventWaitHandle(false, mode, signalName, out created, security);
            return eventHandle;
        }

        public static Mutex CreateMutex(string name, out bool created)
        {
            return CreateMutex(name, true, out created);
        }

        public static Mutex CreateMutex(string name, bool initiallyOwned, out bool created)
        {
            var security = new MutexSecurity();
            var users = new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null);
            var rule = new MutexAccessRule(users, MutexRights.FullControl, AccessControlType.Allow);
            security.AddAccessRule(rule);
            users = new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null);
            rule = new MutexAccessRule(users, MutexRights.FullControl, AccessControlType.Allow);
            security.AddAccessRule(rule);

            Mutex mutex = new Mutex(initiallyOwned, name, out created, security);
            return mutex;
        }

        public static string GetCodeBase(Guid classId)
        {
            string keyName = string.Format(@"CLSID\{0}\InprocServer32", classId.ToString("B"));
            using (RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey(keyName))
            {
                Uri uri = new Uri(Convert.ToString(registryKey.GetValue("CodeBase")));
                return uri.AbsolutePath;
            }
        }
    }
}
