using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractAuditBlockchain.Core.NLog
{
    /// <summary>
    /// Keep track of the current web request id in a way that will allow
    /// use outside of web requests; and avoid dependency loops.
    /// </summary>
    public interface ILogWebRequestTracker
    {
        /// <summary>
        /// The current web request id. (Or null if unknown or not a web request.)
        /// </summary>
        string GetCurrentRequestId { get; }

        /// <summary>
        /// Start a new web request if this is a web request.
        /// <para>
        /// This will be available while processes this request, across concurrent calls.
        /// </para>
        /// </summary>
        /// <returns>The new id, or null.</returns>
        string StartNewWebRequest();
    }
}
