using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Context.Interfaces.Configuration;
using System.IO;
using System.Threading;
using System.Configuration;

namespace Context.Core
{
    internal class ConfigurationManagerService : IConfigurationManager
    {
        private readonly List<IConfigurationSection> sections;
        private readonly List<IConfigurationPage> pages;
        private readonly Hashtable sectionsData;
        private readonly List<IServiceProvider> modules;
        private readonly FileMonitor configMonitor;

        private static ConfigurationManagerService instance;
        private static object lockObj = new object();

        private ConfigurationManagerService()
        {
            sections = new List<IConfigurationSection>();
            pages = new List<IConfigurationPage>();
            sectionsData = new Hashtable();
            modules = new List<IServiceProvider>();

            configMonitor = new FileMonitor(FileUtils.EntryFile + ".config", OnChanged);
        }

        internal static ConfigurationManagerService Instance
        {
            get
            {
                if (instance != null)
                {
                    return instance;
                }

                lock (lockObj)
                {
                    if (instance != null)
                    {
                        return instance;
                    }

                    instance = new ConfigurationManagerService();
                    ConfigurationManager.GetSection(ConfigurationSectionHandler.SectionName);
                    return instance;
                }
            }
        }

        private void OnChanged()
        {
            ConfigurationManager.RefreshSection(ConfigurationSectionHandler.SectionName);
            sections.Clear();
            pages.Clear();
            sectionsData.Clear();
            ConfigurationManager.GetSection(ConfigurationSectionHandler.SectionName);
            ReConfigureModules();

            if (ConfigurationChanged != null)
            {
                ConfigurationChanged();
            }
        }

        internal void SetSectionData(string name, XmlNode node)
        {
            sectionsData[name] = node;
        }

        internal void ValidateConfig()
        {
            if (sectionsData.Count == 0)
            {
                return;
            }

            StringBuilder sb = new StringBuilder();
            foreach (string name in sectionsData.Keys)
            {
                sb.Append(name);
                sb.Append(", ");
            }

            string plural = sectionsData.Count == 1 ? "" : "s";
            throw new InvalidOperationException(string.Format("Config unrecognized element{0}: {1}", plural, sb.ToString(0, sb.Length - 2)));
        }

        internal void ConfigureModule(IServiceProvider module)
        {
            modules.Add(module);

            ReConfigureModule(module);
        }

        private void ReConfigureModules()
        {
            foreach (IServiceProvider module in modules)
            {
                ReConfigureModule(module);
            }
        }

        private void ReConfigureModule(IServiceProvider module)
        {
            IConfigurationProvider provider = module as IConfigurationProvider;
            if (provider == null)
            {
                provider = (IConfigurationProvider)module.GetService(typeof(IConfigurationProvider));
            }

            if (provider == null)
            {
                return;
            }

            IConfigurationSection[] moduleSections = provider.GetConfigurationSections();
            foreach (IConfigurationSection section in moduleSections)
            {
                LoadSectionData(section);
            }

            sections.AddRange(moduleSections);
            pages.AddRange(provider.GetConfigurationPages());
        }

        private void LoadSectionData(IConfigurationSection section)
        {
            XmlNode data = (XmlNode)sectionsData[section.Name];
            if (data == null)
            {
                section.SetDefaults();
            }
            else
            {
                section.LoadSection(data);
            }
        }

        #region IConfigurationManager Members

        public IConfigurationSection GetSection(string sectionName)
        {
            foreach (IConfigurationSection section in sections)
            {
                if (string.Compare(section.Name, sectionName, true) == 0)
                {
                    return section;
                }
            }

            if (sectionsData.Contains(sectionName))
            {
                ConfigurationSectionDefault section = new ConfigurationSectionDefault(sectionName);
                LoadSectionData(section);
                sections.Add(section);
                // TODO: add default page
                return section;
            }

            return null;
        }

        public event ConfigurationChangedHandler ConfigurationChanged;

        #endregion

        #region IPropertyProvider Members

        private bool IsPropertySupported(ref string propertyName, out IConfigurationSection section)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException("propertyName");
            }

            int i = propertyName.IndexOf('.');
            if (i < 0)
            {
                section = null;
                return false;
            }

            string sectionName = propertyName.Substring(0, i);
            propertyName = propertyName.Substring(i + 1);
            section = GetSection(sectionName);
            if (section == null)
            {
                return false;
            }

            return section.IsPropertySupported(propertyName);
        }

        public bool IsPropertySupported(string propertyName)
        {
            IConfigurationSection section;
            return IsPropertySupported(ref propertyName, out section);
        }

        public object GetDefaultValue(string propertyName)
        {
            IConfigurationSection section;
            if (IsPropertySupported(ref propertyName, out section))
            {
                return section.GetDefaultValue(propertyName);
            }

            return null;
        }

        public object this[string propertyName]
        {
            get
            {
                IConfigurationSection section;
                if (IsPropertySupported(ref propertyName, out section))
                {
                    return section[propertyName];
                }

                return null;
            }
            set
            {
                IConfigurationSection section;
                if (IsPropertySupported(ref propertyName, out section))
                {
                    section[propertyName] = value;
                }

                throw new InvalidOperationException(string.Format("Property {0} is not supported.", propertyName));
            }
        }

        #endregion

        #region IConfigurationProvider Members

        public IConfigurationSection[] GetConfigurationSections()
        {
            return sections.ToArray();
        }

        public IConfigurationPage[] GetConfigurationPages()
        {
            return pages.ToArray();
        }

        #endregion
    }
}
