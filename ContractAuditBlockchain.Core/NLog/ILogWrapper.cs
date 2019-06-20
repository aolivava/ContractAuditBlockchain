using System;
using NLog;

namespace ContractAuditBlockchain.Core.NLog
{
    public interface ILogWrapper
    {
        void Error(string message);
        void Error(string keyword, string message);

        void Error(Exception e, string message);
        void Error(string keyword, Exception e, string message);

        void ErrorF(string keyword, FormattableString message);
        void ErrorF(FormattableString message);

        void ErrorF(string keyword, Exception e, FormattableString message);
        void ErrorF(Exception e, FormattableString message);


        void Debug(string keyword, string message);
        void Debug(string message);
        void DebugF(string keyword, FormattableString message);
        void DebugF(FormattableString message);

        void Info(string keyword, string message);
        void Info(string message);
        void InfoF(string keyword, FormattableString message);
        void InfoF(FormattableString message);

        void Trace(string keyword, string message);
        void Trace(string message);
        void TraceF(string keyword, FormattableString message);
        void TraceF(FormattableString message);

        void Warn(string keyword, string message);
        void Warn(string message);
        void WarnF(string keyword, FormattableString message);
        void WarnF(FormattableString message);

        void Write(LogLevel level, string message);
        void Write(LogLevel level, string keyword, string message);
        void Write(LogLevel level, string keyword, string format, params object[] args);
        void WriteF(LogLevel level, FormattableString message);
        void WriteF(LogLevel level, string keyword, FormattableString message);
    }
}
