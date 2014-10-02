using System;
using System.Collections.Generic;
using System.Text;

namespace Context.Interfaces.Logging
{
    public interface ILog
    {
        void Log(LogLevel level, string message, params object[] args);

        void LogDebug(string message, params object[] args);

        void LogInfo(string message, params object[] args);

        void LogWarning(string message, params object[] args);

        void LogError(string message, params object[] args);
    }
}
