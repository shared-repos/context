using System;
using System.Collections;
using Context.Interfaces.Services;

namespace Context.Core
{
    internal class Context : IContext
    {
        private readonly string name;
        private readonly Context parent;
        private readonly Hashtable items;
        private readonly IServiceProvider manager;
        private readonly IPropertyProvider properties;

        public Context(string name, Context parent, IServiceProvider manager, IPropertyProvider properties)
        {
            this.name = name;
            this.parent = parent;
            this.manager = manager;
            this.properties = properties;
            items = new Hashtable();
        }

        #region IContext Members

        public string Name
        {
            get
            {
                return name;
            }
        }

        public object this[string name]
        {
            get
            {
                if (name == null)
                {
                    throw new ArgumentNullException("name");
                }

                object value;
                if (GetValue(name, out value))
                {
                    return value;
                }

                if (parent != null)
                {
                    if (parent.GetValue(this.name + "." + name, out value))
                    {
                        return value;
                    }
                }

                int i = name.IndexOf('.');
                if (i > 0)
                {
                    string scopeName = name.Substring(0, i);
                    name = name.Substring(i + 1);
                    Context context = FindContext(scopeName);
                    if (context == null)
                    {
                        context = ContextService.FindContext(scopeName);
                    }

                    if (context != null)
                    {
                        if (context.GetValue(name, out value))
                        {
                            return value;
                        }
                    }
                }

                return null;
            }
            set
            {
                if (name == null)
                {
                    throw new ArgumentNullException("name");
                }

                if (SetValue(name, value))
                {
                    return;
                }

                if (parent != null)
                {
                    if (parent.SetValue(this.name + "." + name, value))
                    {
                        return;
                    }
                }

                int i = name.IndexOf('.');
                if (i > 0)
                {
                    string scopeName = name.Substring(0, i);
                    name = name.Substring(i + 1);
                    Context context = FindContext(scopeName);
                    if (context == null)
                    {
                        context = ContextService.FindContext(scopeName);
                    }

                    if (context != null)
                    {
                        if (context.SetValue(name, value))
                        {
                            return;
                        }
                    }
                }

                items[name] = value;
            }
        }

        private Context FindContext(string name)
        {
            if (this.name == name)
            {
                return this;
            }

            if (parent != null)
            {
                return parent.FindContext(name);
            }

            return null;
        }

        private bool GetValue(string name, out object value)
        {
            if (properties != null && properties.IsPropertySupported(name))
            {
                value = properties[name];
                return true;
            }

            if (items.Contains(name))
            {
                value = items[name];
                return true;
            }

            value = null;
            return false;
        }

        private bool SetValue(string name, object value)
        {
            if (properties != null && properties.IsPropertySupported(name))
            {
                properties[name] = value;
                return true;
            }

            if (items.Contains(name))
            {
                items[name] = value;
                return true;
            }

            return false;
        }

        #endregion

        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            object service = manager.GetService(serviceType);
            if (service != null)
            {
                return service;
            }

            return items[serviceType];
        }

        #endregion
    }
}