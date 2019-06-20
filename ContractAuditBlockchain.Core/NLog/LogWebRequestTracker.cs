using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace ContractAuditBlockchain.Core.NLog
{
    /// <summary>
    /// Web request tracker using HttpContext and AsyncLocal to store the id.
    /// </summary>
    public class LogWebRequestTracker : ILogWebRequestTracker
    {
        /// <summary>
        /// Id into HttpContext.Items to hold a request sequence number.
        /// </summary>
        private const string ContextKey = "SeqRequestIndex";

        // Reset on web app restart.
        private static int seqNo = 0;

        /// <summary>
        /// This survives across async calls (and the associated thread shifts), but
        /// not across new LogicalCallContext (which ASP.NET sometimes gives us).
        /// </summary>
        private static AsyncLocal<string> currentSeqNo = new AsyncLocal<string>();

        string ILogWebRequestTracker.GetCurrentRequestId
        {
            get
            {
                var id = currentSeqNo.Value;
                if (id == null)
                {
                    var ctx = HttpContext.Current;
                    if (ctx == null)
                    {
                        // Give up!
                        return null;
                    }

                    id = ctx.Items[ContextKey] as string;
                    // Reset... sometimes we're in a new logical call context.
                    // use local to allow use of conditional breakpoint (currentSeqNo.Value can't be evaluated in a breakpoint directly!)
                    currentSeqNo.Value = id;
                }
                return id;
            }
        }

        public string StartNewWebRequest()
        {
            var thisSeqNo = Interlocked.Increment(ref seqNo);
            var id = thisSeqNo.ToString();

            var ctx = HttpContext.Current;
            ctx.Items[ContextKey] = id;
            currentSeqNo.Value = id;
            return id;
        }
    }
}