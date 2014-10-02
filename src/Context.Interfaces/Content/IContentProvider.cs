using System;
using System.Collections.Generic;
using Context.Interfaces.Content.Query;

namespace Context.Interfaces.Content
{
    public interface IContentProvider
    {
        void DefineType(IContentType type);

        void RemoveType(IContentType type);

        bool IsDefined(IContentType type);

        IContentType GetContentType(string id);

        IContentId GetItemId(IContentItem item);

        IContentType GetItemType(IContentItem item);

        void ChangeItemType(IContentItem item, IContentType type);

        IList<IContentItem> GetItems(IQuery query);

        IContentItem GetItem(IContentId id);

        IContentItem CreateItem(IContentType type);

        void UpdateItem(IContentItem item);

        void DeleteItem(IContentItem item);
    }
}
