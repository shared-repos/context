using System;
using System.Data;
using System.Configuration;
using System.Xml;
using System.Globalization;
using System.Collections;
using Context.Interfaces.Configuration;

namespace Context.Core
{
    public class ConfigurationSectionHandler : IConfigurationSectionHandler
    {
        internal const string SectionName = "Context";
        internal const string ModulesConfigurationSettings = "Modules";
        internal const string ServicesConfigurationSettings = "Services";
        internal const string IdColumn = "ID";
        internal const string NameColumn = "Name";
        internal const string DescriptionColumn = "Description";
        internal const string AssemblyColumn = "Assembly";
        internal const string ClassColumn = "Class";
        internal const string PathColumn = "Path";
        internal const string ServiceNameColumn = "ServiceName";
        internal const string ModuleColumn = "Module";
        internal const string ArgumentsColumn = "Arguments";
        internal const string RunOptionsColumn = "RunOptions";

        private DataSet CloneParent(DataSet parentConfig)
        {
            if (parentConfig == null)
            {
                parentConfig = CreateConfigTable();
            }
            else
            {
                parentConfig = parentConfig.Copy();
            }

            if (parentConfig.Tables[ModulesConfigurationSettings] == null)
            {
                parentConfig.Tables.Add(CreateModulesTable());
            }

            if (parentConfig.Tables[ServicesConfigurationSettings] == null)
            {
                parentConfig.Tables.Add(CreateServicesTable());
            }

            return parentConfig;
        }

        private DataTable CreateModulesTable()
        {
            DataColumn idCol = new DataColumn(IdColumn, typeof(string));
            DataColumn nameCol = new DataColumn(NameColumn, typeof(string));
            DataColumn descriptionCol = new DataColumn(DescriptionColumn, typeof(string));
            DataColumn assemblyCol = new DataColumn(AssemblyColumn, typeof(string));
            DataColumn classCol = new DataColumn(ClassColumn, typeof(string));
            DataColumn pathCol = new DataColumn(PathColumn, typeof(string));
            DataColumn serviceNameCol = new DataColumn(ServiceNameColumn, typeof(string));
            DataTable table = new DataTable(ModulesConfigurationSettings);
            table.Locale = CultureInfo.InvariantCulture;
            table.Columns.AddRange(new DataColumn[] { idCol, nameCol, descriptionCol, assemblyCol, classCol, pathCol, serviceNameCol });
            table.PrimaryKey = new DataColumn[] { nameCol };
            return table;
        }

        private DataTable CreateServicesTable()
        {
            DataColumn idCol = new DataColumn(IdColumn, typeof(string));
            DataColumn nameCol = new DataColumn(NameColumn, typeof(string));
            DataColumn moduleCol = new DataColumn(ModuleColumn, typeof(string));
            DataTable table = new DataTable(ServicesConfigurationSettings);
            table.Locale = CultureInfo.InvariantCulture;
            table.Columns.AddRange(new DataColumn[] { idCol, nameCol, moduleCol });
            table.PrimaryKey = new DataColumn[] { nameCol };
            return table;
        }

        private DataSet CreateConfigTable()
        {
            DataSet configTable = new DataSet(SectionName);
            configTable.CaseSensitive = true;
            configTable.Locale = CultureInfo.InvariantCulture;
            return configTable;
        }

        public virtual object Create(object parent, object configContext, XmlNode section)
        {
            DataSet config = parent as DataSet;
            if (section != null)
            {
                config = CloneParent(config);
                Hashtable nodeMap = new Hashtable();
                foreach (XmlNode node in section.ChildNodes)
                {
                    if (node.NodeType == XmlNodeType.Element)
                    {
                        string name = node.Name;
                        if (config.Tables[name] == null)
                        {
                            ConfigurationManagerService.Instance.SetSectionData(name, node);
                            continue;
                        }

                        if (nodeMap.Contains(name))
                        {
                            throw new InvalidOperationException(string.Format("Section '{0}' must be unique in '{1}'", SectionName, name));
                        }

                        nodeMap.Add(name, node);
                        HandleItems(config, configContext, node, name);
                    }
                }
            }
            return config;
        }

        private void HandleItems(DataSet config, object configContext, XmlNode section, string sectionName)
        {
            if (section != null)
            {
                DataTable table = config.Tables[sectionName];
                foreach (XmlNode node in section.ChildNodes)
                {
                    if (node.NodeType == XmlNodeType.Element)
                    {
                        string name = node.Name;
                        name = name.ToLower();
                        switch (name)
                        {
                            case "add":
                                HandleAdd(node, table);
                                break;
                            case "remove":
                                HandleRemove(node, table);
                                break;
                            case "clear":
                                HandleClear(node, table);
                                break;
                            default:
                                throw new InvalidOperationException(string.Format("Config unrecognized element: {0}", name));
                        }
                    }
                    table.AcceptChanges();
                }
            }
        }

        private void HandleAdd(XmlNode childInfo, DataTable config)
        {
            DataRow row = FindConfigRow(childInfo, config);
            if (row == null)
            {
                row = config.NewRow();
            }

            DataColumnCollection columns = config.Columns;
            foreach (XmlAttribute attr in childInfo.Attributes)
            {
                int i = columns.IndexOf(attr.Name);
                DataColumn col;
                if (i < 0)
                    col = columns.Add(attr.Name, typeof(string));
                else
                    col = columns[i];
                row[col] = attr.Value;
            }

            config.Rows.Add(row);
        }

        private DataRow FindConfigRow(XmlNode childInfo, DataTable config)
        {
            XmlAttribute attr = childInfo.Attributes["name"];
            if (attr == null)
            {
                attr = childInfo.Attributes["Name"];
            }

            if (attr != null)
            {
                string name = attr.Value;
                DataRow row = config.Rows.Find(name);
                return row;
            }
            else
            {
                throw new InvalidOperationException(string.Format("Config required attribute missing: 'Name'"));
            }
        }

        private void HandleRemove(XmlNode childInfo, DataTable config)
        {
            DataRow row = FindConfigRow(childInfo, config);
            if (row != null)
            {
                row.Delete();
            }
        }

        private void HandleClear(XmlNode childInfo, DataTable config)
        {
            config.Clear();
        }
    }
}
