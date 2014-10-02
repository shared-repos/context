using System;
using Context.Interfaces.Data;
using System.Data;

namespace Context.Data
{
    public class DataTableRow : IDataRow
    {
        private readonly DataRow row;
        private readonly DataSetSource source;

        public DataTableRow(DataRow row, DataSetSource source)
        {
            this.row = row;
            this.source = source;
        }

        public DataRow Row
        {
            get
            {
                return row;
            }
        }

        #region IDataRow Members

        public object this[object field]
        {
            get
            {
                DataColumn column = field as DataColumn;
                if (column != null)
                {
                    return row[column];
                }

                throw new ArgumentException();
            }
            set
            {
                lock (source.SyncRoot)
                {
                    DataColumn column = field as DataColumn;
                    if (column != null)
                    {
                        row[column] = value;
                    }

                    throw new ArgumentException();
                }
            }
        }

        public object this[int fieldIndex]
        {
            get
            {
                return row[fieldIndex];
            }
            set
            {
                lock (source.SyncRoot)
                {
                    row[fieldIndex] = value;
                }
            }
        }

        #endregion

        public override bool Equals(object obj)
        {
            return Row.Equals(((DataTableRow)obj).Row);
        }

        public override int GetHashCode()
        {
            return Row.GetHashCode();
        }
    }
}
