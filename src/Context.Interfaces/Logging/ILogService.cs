using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Context.Interfaces.Services;

namespace Context.Interfaces.Logging
{
    [Guid("0B627327-D170-452c-90A9-10F62CD29AD2")]
    public interface ILogService
    {
        bool Enabled { get; set; }

        void Log(LogLevel level, IContext context, IFormatProvider formatProvider, string message, params object[] args);

        void Log(LogLevel level, IContext context, string message, params object[] args);

        void Log(LogLevel level, IFormatProvider formatProvider, string message, params object[] args);

        void Log(LogLevel level, string message, params object[] args);

        ILog GetLog(IContext context, IFormatProvider formatProvider);

        void Flush(TimeSpan timeout);

        void Flush();
    }
}
