using System;
using System.Collections.Generic;
using System.Text;
using Context.Interfaces.Configuration;
using System.Xml;
using System.Collections;
using NLog;
using NLog.Config;

namespace Context.Communication.Configuration
{
    internal class NLogSection : IConfigurationSection
    {
        internal NLogSection()
        {
        }

        #region IConfigurationSection Members

        public string Name
        {
            get { return "nlog"; }
        }

        public void SetDefaults()
        {
        }

        public void DefineProperty(string propertyName)
        {
            throw new NotSupportedException();
        }

        public void LoadSection(XmlNode sectionData)
        {
            try
            {
                XmlNamespaceManager mgr = new XmlNamespaceManager(sectionData.OwnerDocument.NameTable);
                mgr.AddNamespace("nlog", "http://www.nlog-project.org/schemas/NLog.xsd");
                XmlNodeList nlog = sectionData.SelectNodes("nlog:targets/nlog:target/nlog:target", mgr);
                if (nlog.Count == 1)
                {
                    XmlNode target = nlog[0];
                    XmlAttribute fileNameAttr = target.Attributes["fileName"];
                    XmlAttribute archiveFileNameAttr = target.Attributes["archiveFileName"];
                    if (fileNameAttr != null)
                    {
                        fileNameAttr.Value = FileUtils.EnsureDirectory(FileUtils.GetAbsolutePath(fileNameAttr.Value));
                    }
                    if (archiveFileNameAttr != null)
                    {
                        archiveFileNameAttr.Value = FileUtils.EnsureDirectory(FileUtils.GetAbsolutePath(archiveFileNameAttr.Value));
                    }
                }
            }
            catch
            {
            }

            XmlNodeReader reader = new XmlNodeReader(sectionData);
            LogManager.Configuration = new XmlLoggingConfiguration(reader, null, true);
        }

        public void SaveSection(System.Xml.XmlNode sectionData)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IPropertyProvider Members

        public bool IsPropertySupported(string propertyName)
        {
            return false;
        }

        public object GetDefaultValue(string propertyName)
        {
            return null;
        }

        public object this[string propertyName]
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        #endregion
    }
}
