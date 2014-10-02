using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Context.Interfaces.Data;

namespace Context.Data
{
    public class DataSetTableEnumerator : IEnumerator<IDataRow>
    {
        private readonly IEnumerator enumerator;
        private readonly DataSetSource datasource;

        public DataSetTableEnumerator(IEnumerator enumerator, DataSetSource datasource)
        {
            this.enumerator = enumerator;
            this.datasource = datasource;
        }

        #region IEnumerator<IDataRow> Members

        public IDataRow Current
        {
            get { return new DataTableRow((DataRow)enumerator.Current, datasource); }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            IDisposable disposable = enumerator as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        #endregion

        #region IEnumerator Members

        object IEnumerator.Current
        {
            get { return enumerator.Current; }
        }

        public bool MoveNext()
        {
            return enumerator.MoveNext();
        }

        public void Reset()
        {
            enumerator.Reset();
        }

        #endregion
    }
}
