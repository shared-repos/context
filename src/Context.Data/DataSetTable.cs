using System;
using System.Collections.Generic;
using System.Data;
using Context.Interfaces.Data;
using System.Collections;

namespace Context.Data
{
    public class DataSetTable : IDataTable
    {
        private readonly DataTable table;
        private readonly DataSetSource source;

        public DataSetTable(DataTable table, DataSetSource source)
        {
            this.table = table;
            this.source = source;
        }

        #region IDataTable Members

        public int FieldCount
        {
            get
            {
                return table.Columns.Count;
            }
        }

        public object GetField(int index)
        {
            return table.Columns[index];
        }

        public string GetFieldName(int index)
        {
            return table.Columns[index].ColumnName;
        }

        public IDataRow Create(object keyValue)
        {
            lock (source.SyncRoot)
            {
                return new DataTableRow(table.NewRow(), source);
            }
        }

        public IDataRow Find(object keyValue)
        {
            lock (source.SyncRoot)
            {
                DataRow row = table.Rows.Find(keyValue);
                if (row == null)
                {
                    return null;
                }

                return new DataTableRow(row, source);
            }
        }

        public void Update(object keyValue, IDataRow row)
        {
            Update(row);
        }

        public void Update(IDataRow row)
        {
            lock (source.SyncRoot)
            {
                DataTableRow tableRow = (DataTableRow)row;
                if (tableRow.Row.RowState == DataRowState.Detached)
                {
                    table.Rows.Add(tableRow.Row);
                }

                source.IsDirty = true;
            }
        }

        public void Delete(object keyValue)
        {
            lock (source.SyncRoot)
            {
                DataRow dataRow = table.Rows.Find(keyValue);
                if (dataRow == null)
                {
                    return;
                }

                dataRow.Delete();

                source.IsDirty = true;
            }
        }

        public void Delete(IDataRow row)
        {
            lock (source.SyncRoot)
            {
                DataTableRow tableRow = (DataTableRow)row;
                try
                {
                    tableRow.Row.Delete();
                }
                catch (Exception ex)
                {
                    source.LogError(ex);
                }

                source.IsDirty = true;
            }
        }

        #endregion

        #region IList<IDataRow> Members

        public int IndexOf(IDataRow item)
        {
            DataTableRow tableRow = (DataTableRow)item;
            return table.Rows.IndexOf(tableRow.Row);
        }

        public void Insert(int index, IDataRow item)
        {
            lock (source.SyncRoot)
            {
                DataTableRow tableRow = (DataTableRow)item;
                table.Rows.InsertAt(tableRow.Row, index);

                source.IsDirty = true;
            }
        }

        public void RemoveAt(int index)
        {
            lock (source.SyncRoot)
            {
                table.Rows.RemoveAt(index);

                source.IsDirty = true;
            }
        }

        public IDataRow this[int index]
        {
            get
            {
                lock (source.SyncRoot)
                {
                    return new DataTableRow(table.Rows[index], source);
                }
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        #endregion

        #region ICollection<IDataRow> Members

        public void Add(IDataRow item)
        {
            lock (source.SyncRoot)
            {
                DataTableRow tableRow = (DataTableRow)item;
                table.Rows.Add(tableRow.Row);

                source.IsDirty = true;
            }
        }

        public void Clear()
        {
            lock (source.SyncRoot)
            {
                table.Rows.Clear();

                source.IsDirty = true;
            }
        }

        public bool Contains(IDataRow item)
        {
            DataTableRow tableRow = (DataTableRow)item;
            return table.Rows.Contains(tableRow.Row);
        }

        public void CopyTo(IDataRow[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        public int Count
        {
            get { return table.Rows.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(IDataRow item)
        {
            lock (source.SyncRoot)
            {
                DataTableRow tableRow = (DataTableRow)item;
                table.Rows.Remove(tableRow.Row);
                source.IsDirty = true;
                return true;
            }
        }

        #endregion

        #region IEnumerable<IDataRow> Members

        public IEnumerator<IDataRow> GetEnumerator()
        {
            return new DataSetTableEnumerator(table.Rows.GetEnumerator(), source);
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new DataSetTableEnumerator(table.Rows.GetEnumerator(), source);
        }

        #endregion
    }
}
