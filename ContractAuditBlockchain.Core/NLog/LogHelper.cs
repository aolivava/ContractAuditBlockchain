using System;
using System.Collections.Concurrent;
using NLog;

namespace ContractAuditBlockchain.Core.NLog
{
    public static class LogHelper
    {
        private static ConcurrentDictionary<string, ILogWrapper> loggerCache = new ConcurrentDictionary<string, ILogWrapper>();

        public static void Write(string logger, LogLevel level, string message) => GetWrapper(logger).Write(level, message);
        public static void Write(string logger, LogLevel level, string format, params object[] args) => GetWrapper(logger).Write(level, null, format, args);

        public static void Debug(string name, string message) => GetWrapper(name).Debug(message);
        public static void Debug(string name, string format, params object[] args) => GetWrapper(name).Write(LogLevel.Debug, null, format, args);

        public static void Error(string name, string message) => GetWrapper(name).Error(message);
        public static void Error(string name, string format, params object[] args) => GetWrapper(name).Write(LogLevel.Error, null, format, args);

        public static void Exception(string name, string message, Exception e) => GetWrapper(name).Error(e, message);

        public static void Info(string name, string message) => GetWrapper(name).Info(message);
        public static void Info(string name, string format, params object[] args) => GetWrapper(name).Write(LogLevel.Info, null, format, args);

        public static void Trace(string name, string message) => GetWrapper(name).Trace(message);

        public static void Trace(string name, string format, params object[] args) => GetWrapper(name).Write(LogLevel.Trace, null, format, args);

        public static void Warn(string name, string message) => GetWrapper(name).Warn(message);
        public static void Warn(string name, string format, params object[] args) => GetWrapper(name).Write(LogLevel.Warn, null, format, args);

        private static ILogWrapper GetWrapper(string name)
        {
            // If there is a race an extra ILogWrapper instance may be created, but it
            // will be picked up by GC.
            return loggerCache.GetOrAdd(name, n => LogWrapper.Get(n));
        }
    }
}