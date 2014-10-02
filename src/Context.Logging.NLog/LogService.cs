using System;
using System.Collections.Generic;
using System.Text;
using Context.Interfaces.Logging;
using Context.Interfaces.Services;
using L = NLog;

namespace Context.Logging.NLog
{
    public class LogService : ILogService
    {
        public const string GlobalLogName = "System";

        private readonly IContextService contextService;

        public LogService(IContextService contextService)
        {
            this.contextService = contextService;
        }

        #region ILogService Members

        public bool Enabled
        {
            get
            {
                return L.LogManager.IsLoggingEnabled();
            }
            set
            {
                if (value)
                {
                    L.LogManager.EnableLogging();
                }
                else
                {
                    L.LogManager.DisableLogging();
                }
            }
        }

        public void Log(LogLevel level, IContext context, IFormatProvider formatProvider, string message, params object[] args)
        {
            L.Logger logger = L.LogManager.GetLogger(GetLogName(context));
            logger.Log(GetLogLevel(level), formatProvider, message, args);
        }

        public void Log(LogLevel level, IContext context, string message, params object[] args)
        {
            L.Logger logger = L.LogManager.GetLogger(GetLogName(context));
            logger.Log(GetLogLevel(level), message, args);
        }

        public void Log(LogLevel level, IFormatProvider formatProvider, string message, params object[] args)
        {
            L.Logger logger = L.LogManager.GetLogger(GetLogName(null));
            logger.Log(GetLogLevel(level), formatProvider, message, args);
        }

        public void Log(LogLevel level, string message, params object[] args)
        {
            L.Logger logger = L.LogManager.GetLogger(GetLogName(null));
            logger.Log(GetLogLevel(level), message, args);
        }

        private L.LogLevel GetLogLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Trace:
                    return L.LogLevel.Trace;
                case LogLevel.Debug:
                    return L.LogLevel.Debug;
                case LogLevel.Info:
                    return L.LogLevel.Info;
                case LogLevel.Warn:
                    return L.LogLevel.Warn;
                case LogLevel.Error:
                    return L.LogLevel.Error;
                case LogLevel.Fatal:
                    return L.LogLevel.Fatal;
            }

            throw new ArgumentException("level");
        }

        private string GetLogName(IContext context)
        {
            if (context == null)
            {
                context = this.contextService.Current;
            }

            if (context == null)
            {
                return GlobalLogName;
            }
            else
            {
                return context.Name;
            }
        }

        public ILog GetLog(IContext context, IFormatProvider formatProvider)
        {
            return new ContextLog(this, context, formatProvider);
        }

        public void Flush(TimeSpan timeout)
        {
            L.LogManager.Flush(timeout);
        }

        public void Flush()
        {
            L.LogManager.Flush();
        }

        #endregion

        private class ContextLog : ILog
        {
            private LogService service;
            private IContext context;
            private IFormatProvider formatProvider;

            public ContextLog(LogService service, IContext context, IFormatProvider formatProvider)
            {
                this.service = service;
                this.context = context;
                this.formatProvider = formatProvider;
            }

            #region ILog Members

            public void Log(LogLevel level, string message, params object[] args)
            {
                if (formatProvider == null)
                {
                    service.Log(level, context, message, args);
                }
                else
                {
                    service.Log(level, context, formatProvider, message, args);
                }
            }

            public void LogDebug(string message, params object[] args)
            {
                Log(LogLevel.Debug, message, args);
            }

            public void LogInfo(string message, params object[] args)
            {
                Log(LogLevel.Info, message, args);
            }

            public void LogWarning(string message, params object[] args)
            {
                Log(LogLevel.Warn, message, args);
            }

            public void LogError(string message, params object[] args)
            {
                Log(LogLevel.Error, message, args);
            }

            #endregion
        }

    }
}
