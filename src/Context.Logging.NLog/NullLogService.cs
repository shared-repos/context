using System;
using System.Collections.Generic;
using System.Text;
using Context.Interfaces.Services;
using Context.Interfaces.Logging;

namespace Context.Logging.NLog
{
    public class NullLogService : ILogService
    {
        private const string GlobalLogName = "Global";

        private readonly IContextService contextService;

        public NullLogService(IContextService contextService)
        {
            this.contextService = contextService;
        }

        #region ILogService Members

        public bool Enabled
        {
            get
            {
                return true;
            }
            set
            {
            }
        }

        public void Log(LogLevel level, IContext context, IFormatProvider formatProvider, string message, params object[] args)
        {
        }

        public void Log(LogLevel level, IContext context, string message, params object[] args)
        {
        }

        public void Log(LogLevel level, IFormatProvider formatProvider, string message, params object[] args)
        {
        }

        public void Log(LogLevel level, string message, params object[] args)
        {
        }

        public ILog GetLog(IContext context, IFormatProvider formatProvider)
        {
            return new NullLog();
        }

        public void Flush(TimeSpan timeout)
        {
        }

        public void Flush()
        {
        }

        #endregion

        private class NullLog : ILog
        {
            #region ILog Members

            public void Log(LogLevel level, string message, params object[] args)
            {
            }

            public void LogDebug(string message, params object[] args)
            {
            }

            public void LogInfo(string message, params object[] args)
            {
            }

            public void LogWarning(string message, params object[] args)
            {
            }

            public void LogError(string message, params object[] args)
            {
            }

            #endregion
        }
    }
}
