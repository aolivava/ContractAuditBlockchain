using System;
using NLog;

namespace ContractAuditBlockchain.Core.NLog
{
    public class LogWrapper : ILogWrapper
    {
        public const string KeywordProperty = "Keyword";
        public const string WebRequestProperty = "WebRequest";

        // Default to null object.
        private static ILogWebRequestTracker requestTracker = new LogNullWebRequestTracker();


        public static ILogWebRequestTracker RequestTracker => requestTracker;

        private Logger logger { get; }

        private LogWrapper(Logger logger) => this.logger = logger;

        public static ILogWrapper Get(string name)
        {
            var l = LogManager.GetLogger(name);
            return new LogWrapper(l);
        }

        public static ILogWrapper GetForClass()
        {
            var cn = StackUtils.GetCallersClassName(1);
            return Get(cn);
        }


        /// <summary>
        /// In a Web App, at Application Start use this to enable web request tracking.
        /// </summary>
        /// <param name="tracker"></param>
        public static void SetRequestTracker(ILogWebRequestTracker tracker)
        {
            requestTracker = tracker;
        }


        //
        // ILogWrapper: explicit implementation to encourage use of the interface!
        //

        void ILogWrapper.Error(string message) => ((ILogWrapper)this).Error((string)null, message);

        void ILogWrapper.Error(string keyword, string message)
        {
            Log(LogLevel.Error, keyword, message);
        }

        void ILogWrapper.Error(Exception e, string message) => ((ILogWrapper)this).Error((string)null, e, message);

        void ILogWrapper.Error(string keyword, Exception e, string message)
        {
            Log(LogLevel.Error, keyword, message, e);
        }

        void ILogWrapper.ErrorF(FormattableString message) => ((ILogWrapper)this).ErrorF((string)null, message);
        void ILogWrapper.ErrorF(string keyword, FormattableString message)
        {
            LogF(LogLevel.Error, keyword, message);
        }

        void ILogWrapper.ErrorF(Exception e, FormattableString message) => ((ILogWrapper)this).ErrorF((string)null, e, message);
        void ILogWrapper.ErrorF(string keyword, Exception e, FormattableString message)
        {
            LogF(LogLevel.Error, keyword, message, e);
        }

        void ILogWrapper.Debug(string message) => ((ILogWrapper)this).Debug((string)null, message);
        void ILogWrapper.Debug(string keyword, string message)
        {
            Log(LogLevel.Debug, keyword, message);
        }

        void ILogWrapper.DebugF(FormattableString message) => ((ILogWrapper)this).DebugF((string)null, message);
        void ILogWrapper.DebugF(string keyword, FormattableString message)
        {
            LogF(LogLevel.Debug, keyword, message);
        }

        void ILogWrapper.Info(string message) => ((ILogWrapper)this).Info((string)null, message);
        void ILogWrapper.Info(string keyword, string message)
        {
            Log(LogLevel.Info, keyword, message);
        }

        void ILogWrapper.InfoF(FormattableString message) => ((ILogWrapper)this).InfoF((string)null, message);
        void ILogWrapper.InfoF(string keyword, FormattableString message)
        {
            LogF(LogLevel.Info, keyword, message);
        }

        void ILogWrapper.Trace(string message) => ((ILogWrapper)this).Trace((string)null, message);
        void ILogWrapper.Trace(string keyword, string message)
        {
            Log(LogLevel.Trace, keyword, message);
        }

        void ILogWrapper.TraceF(FormattableString message) => ((ILogWrapper)this).TraceF((string)null, message);
        void ILogWrapper.TraceF(string keyword, FormattableString message)
        {
            LogF(LogLevel.Trace, keyword, message);
        }

        void ILogWrapper.Warn(string message) => ((ILogWrapper)this).Warn((string)null, message);
        void ILogWrapper.Warn(string keyword, string message)
        {
            Log(LogLevel.Warn, keyword, message);
        }

        void ILogWrapper.WarnF(FormattableString message) => ((ILogWrapper)this).WarnF((string)null, message);
        void ILogWrapper.WarnF(string keyword, FormattableString message)
        {
            LogF(LogLevel.Warn, keyword, message);
        }

        void ILogWrapper.Write(LogLevel level, string message) => Log(level, null, message);
        void ILogWrapper.Write(LogLevel level, string keyword, string message) => Log(level, keyword, message);
        void ILogWrapper.Write(LogLevel level, string keyword, string format, params object[] args)
            => Log(level, keyword, format, args);
        void ILogWrapper.WriteF(LogLevel level, FormattableString message) => LogF(level, null, message);
        void ILogWrapper.WriteF(LogLevel level, string keyword, FormattableString message) => LogF(level, keyword, message);

        //
        // Internals
        //

        private void Log(LogLevel level, string keyword, string message, Exception ex = null)
        {
            if (logger.IsEnabled(level))
            {
                LogInner(level, keyword, message, ex);
            }
        }

        private void Log(LogLevel level, string keyword, string format, params object[] args)
        {
            if (logger.IsEnabled(level))
            {
                LogInner(level, keyword, String.Format(format, args), null);
            }
        }

        private void LogF(LogLevel level, string keyword, FormattableString message, Exception ex = null)
        {
            if (logger.IsEnabled(level))
            {
                Log(level, keyword, message.ToString(), ex);
            }
        }

        private void LogInner(LogLevel level, string keyword, string message, Exception ex)
        {
            var x = new LogEventInfo
            {
                Exception = ex,
                Level = level,
                LoggerName = logger.Name,
                Message = message
            };
            x.Properties.Add(WebRequestProperty, requestTracker.GetCurrentRequestId);
            if (!String.IsNullOrWhiteSpace(keyword))
            {
                x.Properties.Add(KeywordProperty, keyword);
            }
            logger.Log(x);
        }
    }
}