using System;
using System.Collections;
using System.Collections.Generic;
using Context.Interfaces.Content.Query;

namespace Context.Interfaces.Content.Persistance
{
    public interface IPersister
    {
        // read
        IContentReader ExecuteQuery(IQuery query, IContentProvider provider, IContentType type);
        IContentReader GetRangeReader(IEnumerable<IContentItem> items, IContentType type, IContentProvider provider, bool lazy);
        IContentReader GetNestedReader(IContentItem item, IContentRelation relation, IContentProvider provider);

        // cache
        IContentItem GetNestedItem(IContentItem item, IContentRelation relation, IContentProvider provider);
        IList<IContentItem> GetNestedCollection(IContentItem item, IContentRelation relation, IContentProvider provider);

        // store
        void MakePersistent(IContentItem item, IContentType type);
        bool PersistItem(IContentType type, IEnumerable<IContentRelation> relations, IContentItem item, IDictionary changes, IDictionary root, IContentProvider provider);
        bool PersistCollection(IContentType type, IContentItem item, IContentRelation relation, IList<IContentItem> items, IList addedItems, IList removedItems, IDictionary root, IContentProvider provider);
        void DeleteItem(IContentType type, IEnumerable<IContentRelation> relations, IContentItem item, IContentProvider provider);
        void GetNestedChanges(IContentItem item, IContentItem reference, IContentRelation relation, IDictionary changes);

        // identity
        IContentId GetContentId(IContentItem item, IContentType type, IContentProvider provider);
        IContentItem GetContentItem(IContentId id, IContentType type, IContentProvider provider);

        // config
        string ConfigData { get; set; }
    }
}
