using System;
using Context.Interfaces.Schema;
using System.Collections;

namespace Context.Core
{
    internal class SchemaService : ISchemaService
    {
        private readonly Hashtable schemas;

        public SchemaService()
        {
            schemas = new Hashtable();
        }

        #region ISchemaService implementation

        public void RegisterSchema(string schemaName, object schema)
        {
            schemas[schemaName] = schema;
        }

        public object GetSchema(string schemaName)
        {
            return schemas[schemaName];
        }

        #endregion
    }
}
