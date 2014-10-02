using System;
using System.Runtime.InteropServices;

namespace Context.Interfaces.Schema
{
    [Guid("F6760256-0129-49e6-AC93-8BA0E5B873C4")]
    public interface ISchemaService
    {
        void RegisterSchema(string schemaName, object schema);

        object GetSchema(string schemaName);
    }
}
