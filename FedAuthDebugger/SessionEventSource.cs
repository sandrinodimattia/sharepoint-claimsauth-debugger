using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics.Tracing;

namespace FedAuthDebugger
{
    [EventSource(Name = "FedAuthDebugger-Session")]
    public class SessionEventSource : EventSource
    {
        public static SessionEventSource Log = new SessionEventSource();

        [NonEvent]
        internal void SecurityTokenCreated(string user, DateTime validFrom, DateTime validTo)
        {
            if (IsEnabled())
            {
                SecurityTokenCreated(user, validFrom.ToString(), validTo.ToString());
            }
        }

        [Event(1, Message = "Sesson: Token Created for {0}. [From:{1} | To:{2}]", Level = EventLevel.Verbose)]
        public void SecurityTokenCreated(string user, string validFrom, string validTo)
        {
            WriteEvent(1, user, validFrom, validTo);
        }

        [NonEvent]
        internal void SecurityTokenReceived(string user, double sessionDurationMinutes, TimeSpan logonTokenCacheExpirationWindow, DateTime validFrom, DateTime validTo)
        {
            if (IsEnabled())
            {
                SecurityTokenReceived(user, sessionDurationMinutes, logonTokenCacheExpirationWindow.ToString(), validFrom.ToString(), validTo.ToString());
            }
        }

        [Event(2, Message = "Session: Token Received for {0}. [Session:{1}min | CacheExp:{2} | From:{3} | To:{4}]", Level = EventLevel.Verbose)]
        public void SecurityTokenReceived(string user, double sessionLifetimeMinutes, string logonTokenCacheExpirationWindow, string validFrom, string validTo) 
        {
            WriteEvent(2, user, sessionLifetimeMinutes, logonTokenCacheExpirationWindow, validFrom, validTo);
        }

        [NonEvent]
        internal void SessionReissued(string user, DateTime validTo)
        {
            if (IsEnabled())
            {
                SessionReissued(user, validTo.ToString());
            }
        }

        [Event(3, Message = "Session: Reissued {0}. {1}", Level = EventLevel.Verbose)]
        public void SessionReissued(string user, string validTo)
        {
            WriteEvent(3, user, validTo);
        }

        [Event(4, Message = "Session: Signing out {0}.", Level = EventLevel.Verbose)]
        public void SigningOut(string rawUrl)
        {
            if (IsEnabled())
            {
                WriteEvent(4, rawUrl);
            }
        }

        [Event(5, Message = "Session: Signed out.", Level = EventLevel.Verbose)]
        public void SignedOut()
        {
            if (IsEnabled())
            {
                WriteEvent(5);
            }
        }

        [NonEvent]
        internal void SignOutError(Exception exception)
        {
            if (IsEnabled() && exception != null)
            {
                SignOutError(exception.Message);
            }
        }

        [Event(6, Message = "Session: Error singing out '{0}'.", Level = EventLevel.Error)]
        public void SignOutError(string error)
        {
            WriteEvent(6, error);
        }

        [Event(7, Message = "Session: Timeout for user.", Level = EventLevel.Verbose)]
        public void SessionTimeout()
        {
            WriteEvent(7);
        }
    }
}
