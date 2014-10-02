using System;
using System.Collections;
using System.Collections.Generic;
using Context.Interfaces.Data;
using Context.Interfaces.Services;

namespace Context.Data
{
    public class DataService : IDataService
    {
        private readonly Hashtable dataSources;

        public DataService(IContextService context)
        {
            this.dataSources = new Hashtable();
            List<string> items = context.Current["Items"] as List<string>;
            if (items == null)
            {
                return;
            }

            foreach (string name in items)
            {
                using (context.CreateScope(name))
                {
                    dataSources[name] = context.Current;
                }
            }
        }

        #region IDataService Members

        public IDataSource GetDataSource(string dataSource)
        {
            object obj = dataSources[dataSource];
            IDataSource source = obj as IDataSource;
            if (source != null)
            {
                return source;
            }

            IContext sourceContext = obj as IContext;
            if (sourceContext == null)
            {
                throw new InvalidOperationException(string.Format("Unknown data source: {0}", dataSource));
            }

            IDataProvider provider = GetDetDataProvider(Convert.ToString(sourceContext["DataProvider"]));
            source = provider.CreateDataSource(sourceContext);
            dataSources[dataSource] = source;

            return source;
        }

        #endregion

        private IDataProvider GetDetDataProvider(string providerName)
        {
            if (providerName == null)
            {
                throw new ArgumentNullException("providerName");
            }

            switch (providerName)
            {
                case "DataSet":
                    return new DataSetDataProvider();
                default:
                    throw new InvalidOperationException(string.Format("Unknow data provider: {0}", providerName));
            }
        }

    }
}
