using System;

namespace Context.Interfaces.Objects
{
    public enum ObjectState
    {
        Unknown = 0x00,
        Created = 0x01,
        Reading = 0x02,
        Persisted = 0x04,
        Modified = 0x08,
        Deleted = 0x10,
        Removed = 0x20
    }
}
