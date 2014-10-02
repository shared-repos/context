using System;
using System.Collections.Generic;

namespace Context.Interfaces.Content.Persistance
{
    public interface IRelationMapping
    {
        IContentRelation Relation { get; }
    }
}
