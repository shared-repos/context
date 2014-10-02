using System;
using Context.Interfaces.Schema;
using Context.Interfaces.Data;
using Context.Interfaces.Services;
using Context.Interfaces.Logging;
using System.Globalization;

namespace Context.Data
{
    public class DataSetDataProvider : IDataProvider
    {

        public DataSetDataProvider()
        {
        }

        #region IDataProvider Members

        public string ProviderName
        {
            get
            {
                return "DataSet";
            }
        }

        public IDataSource CreateDataSource(IContext context)
        {
            ISchemaService schemaService = (ISchemaService)context.GetService(typeof(ISchemaService));
            ILogService logger = (ILogService)context.GetService(typeof(ILogService));
            string path = Convert.ToString(context["FileName"]);
            string lockName = Convert.ToString(context["Mutex"]);
            string name = context.Name;
            string trackChangesStr = Convert.ToString(context["TrackChanges"]);
            bool trackChanges = false;
            if (!string.IsNullOrEmpty(trackChangesStr))
            {
                trackChanges = Convert.ToBoolean(trackChangesStr, CultureInfo.InvariantCulture);
            }
            return new DataSetSource(context, schemaService, logger, path, name, lockName, trackChanges);
        }

        #endregion
    }
}
