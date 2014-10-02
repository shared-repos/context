using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Context.Interfaces.Services;

namespace Context.Core
{
    internal class ModuleInfo : IModuleInfo
    {
        private Guid id;
        private string name;
        private string description;
        private string assemblyName;
        private string className;
        private string path;
        private string serviceName;
        private IModule module;
        private Dictionary<Guid, string> services;
        private Dictionary<string, object> props;

        public ModuleInfo(Guid id, string name, string description, string assemblyName, string className, string path, string serviceName, Dictionary<string, object> props)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.assemblyName = assemblyName;
            this.className = className;
            this.path = path;
            this.serviceName = serviceName;
            this.services = new Dictionary<Guid, string>();
            this.props = props;
        }

        #region IModuleInfo Members

        public Guid Id
        {
            get { return id; }
        }

        public string Name
        {
            get { return name; }
        }

        public string Description
        {
            get { return description; }
        }

        public string AssemblyName
        {
            get { return assemblyName; }
        }

        public string ClassName
        {
            get { return className; }
        }

        public string Path
        {
            get { return path; }
        }

        public string ServiceName
        {
            get { return serviceName; }
        }

        #endregion

        #region IPropertyProvider Members

        public bool IsPropertySupported(string propertyName)
        {
            return (props.ContainsKey(propertyName));
        }

        public object GetDefaultValue(string propertyName)
        {
            object value;
            props.TryGetValue(propertyName, out value);
            return value;
        }

        public object this[string propertyName]
        {
            get
            {
                object value;
                props.TryGetValue(propertyName, out value);
                return value;
            }
            set
            {
                props[propertyName] = value;
            }
        }

        #endregion

        internal Guid[] ListServices()
        {
            Guid[] arr = new Guid[services.Count];
            int i = 0;
            foreach (Guid serviceId in services.Keys)
            {
                arr[i] = serviceId;
                i++;
            }
            return arr;
        }

        internal void AddService(Guid serviceId, string name)
        {
            services.Add(serviceId, name);
        }

        internal bool IsLoaded
        {
            get
            {
                return module != null;
            }
        }

        internal IModule Module
        {
            get
            {
                return module;
            }
            set
            {
                module = value;
            }
        }
    }
}
