using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContractAuditBlockchain.Core.NLog
{
    /// <summary>
    /// Default implementation of <code>ILogWebRequestTracker</code> for non-web-requests.
    /// </summary>
    public class LogNullWebRequestTracker : ILogWebRequestTracker
    {
        string ILogWebRequestTracker.GetCurrentRequestId => null;


        public string StartNewWebRequest() => null;
    }
}