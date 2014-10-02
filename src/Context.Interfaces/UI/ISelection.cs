using System;

namespace Context.Interfaces.UI
{
    public interface ISelection
    {
        event EventHandler SelectionChanged;
        event EventHandler SelectionChanging;

        bool IsItemSelected(object item);
        object[] GetSelectedItems();
        void SetSelectedItems(object[] items);
        void SetSelectedItems(object[] items, SelectionTypes selectionType);

        void Clear();
        void SelectAll();

        object PrimarySelection { get; }
        int SelectionCount { get; }
    }
}
