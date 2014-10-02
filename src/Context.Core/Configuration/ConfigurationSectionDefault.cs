using System;
using Context.Interfaces.Configuration;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

namespace Context.Core
{
    internal class ConfigurationSectionDefault : IConfigurationSection
    {
        private readonly string name;
        private readonly Hashtable properties;

        public ConfigurationSectionDefault(string name)
        {
            this.name = name;
            this.properties = new Hashtable();
        }

        #region IConfigurationSection implementation

        public void SetDefaults()
        {
            properties.Clear();
            properties.Add("Items", new List<string>());
        }

        public void DefineProperty(string propertyName)
        {
            if (!properties.ContainsKey(propertyName))
            {
                properties.Add(propertyName, null);
            }
        }

        public void LoadSection(XmlNode sectionData)
        {
            SetDefaults();

            foreach (XmlAttribute attr in sectionData.Attributes)
            {
                properties[attr.Name] = attr.Value;
            }

            foreach (XmlNode node in sectionData.ChildNodes)
            {
                if (node.NodeType == XmlNodeType.Element)
                {
                    string name = node.Name;
                    name = name.ToLower();
                    switch (name)
                    {
                        case "add":
                            HandleAdd(node);
                            break;
                        case "remove":
                            HandleRemove(node);
                            break;
                        case "clear":
                            HandleClear(node);
                            break;
                        default:
                            throw new InvalidOperationException(string.Format("Config unrecognized element: {0}", name));
                    }
                }
            }

        }

        private string GetAttributeValue(XmlNode node, string attributeName, bool required)
        {
            XmlAttribute attr = node.Attributes[attributeName];
            if (attr == null)
            {
                attr = node.Attributes[attributeName.ToLower()];
            }

            if (attr != null)
            {
                return attr.Value;
            }
            else
            {
                if (required)
                {
                    throw new InvalidOperationException(string.Format("Config required attribute missing: '{0}'", attributeName));
                }

                return null;
            }
        }

        private List<string> EnsureItems()
        {
            List<string> items = properties["Items"] as List<string>;
            if (items != null)
            {
                return items;
            }

            items = new List<string>();
            properties["Items"] = items;
            return items;
        }


        private void HandleAdd(XmlNode node)
        {
            string name = GetAttributeValue(node, "Name", true);
            List<string> items = EnsureItems();
            items.Add(name);
            foreach (XmlAttribute attr in node.Attributes)
            {
                properties[name + "." + attr.Name] = attr.Value;
            }
        }

        private void HandleRemove(XmlNode node)
        {
            string name = GetAttributeValue(node, "Name", true);
            List<string> items = EnsureItems();
            for (int i = 0; i < items.Count; i++)
            {
                if (string.Compare(name, items[i], true) == 0)
                {
                    items.RemoveAt(i);
                    break;
                }
            }

            string prefix = name + ".";
            List<string> toDelete = new List<string>();
            foreach (string key in properties.Keys)
            {
                if (key.StartsWith(prefix))
                {
                    toDelete.Add(key);
                }
            }

            foreach (string key in toDelete)
            {
                properties.Remove(key);
            }
        }

        private void HandleClear(XmlNode node)
        {
            SetDefaults();
        }

        public void SaveSection(XmlNode sectionData)
        {
            throw new NotImplementedException();
        }

        public string Name
        {
            get
            {
                return name;
            }
        }
        #endregion

        #region IPropertyProvider implementation

        public bool IsPropertySupported(string propertyName)
        {
            return properties.Contains(propertyName);
        }

        public object GetDefaultValue(string propertyName)
        {
            return string.Empty;
        }

        public object this[string propertyName]
        {
            get
            {
                return properties[propertyName];
            }
            set
            {
                properties[propertyName] = value;
            }
        }

        #endregion
    }
}
