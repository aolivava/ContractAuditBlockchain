using System;
using System.Diagnostics;

namespace ContractAuditBlockchain.Core
{
    public static class StackUtils
    {
        public static string GetCallersClassName(int skipFrames)
        {
            skipFrames++; // also skip this frame

            // Code derived from NLog's NLog.Internal.StackTraceUsageUtils.GetClassFullName
            string className = null;

            // But also skip system.*
            do
            {
                var frame = new StackFrame(skipFrames, false);
                var method = frame.GetMethod();
                var type = method.DeclaringType;
                className = type.FullName;

            }
            while (className.StartsWith("System.", StringComparison.Ordinal));

            return className;
        }
    }
}