using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics.Tracing;

namespace FedAuthDebugger
{
    [EventSource(Name = "FedAuthDebugger-Federation")]
    public class FederationEventSource : EventSource
    {
        public static FederationEventSource Log = new FederationEventSource();

        [NonEvent]
        internal void SecurityTokenReceived(string id, DateTime validFrom, DateTime validTo)
        {
            if (IsEnabled())
            {
                SecurityTokenReceived(id, validFrom.ToString(), validTo.ToString());
            }
        }

        [Event(1, Message = "Federation: Token Received for {0}. [From:{1} | To:{2}]", Level = EventLevel.Verbose)]
        public void SecurityTokenReceived(string id, string validFrom, string validTo)
        {
            WriteEvent(1, id, validFrom, validTo);
        }

        [Event(2, Message = "Federation: Token validated '{0}'.", Level = EventLevel.Verbose)]
        public void SecurityTokenValidated(string user)
        {
            if (IsEnabled())
            {
                WriteEvent(2, user);
            }
        }

        [NonEvent]
        internal void SecurityTokenCreated(string user, DateTime validFrom, DateTime validTo)
        {
            if (IsEnabled())
            {
                SecurityTokenCreated(user, validFrom.ToString(), validTo.ToString());
            }
        }

        [Event(3, Message = "Federation: Token Created for {0}. [From:{1} | To:{2}]", Level = EventLevel.Verbose)]
        public void SecurityTokenCreated(string user, string validFrom, string validTo)
        {
            WriteEvent(3, user, validFrom, validTo);
        }

        [Event(4, Message = "Federation: Signed in '{0}'.", Level = EventLevel.Verbose)]
        public void SignedIn(string user)
        {
            if (IsEnabled())
            {
                WriteEvent(4, user);
            }
        }

        [NonEvent]
        internal void SessionSecurityTokenCreated(string user, DateTime validFrom, DateTime validTo)
        {
            if (IsEnabled())
            {
                SessionSecurityTokenCreated(user, validFrom.ToString(), validTo.ToString());
            }
        }

        [Event(5, Message = "Federation: Session Token Created for {0}. [From:{1} | To:{2}]", Level = EventLevel.Verbose)]
        public void SessionSecurityTokenCreated(string user, string validFrom, string validTo)
        {
            WriteEvent(5, user, validFrom, validTo);
        }

        [Event(6, Message = "Federation: Signing out {0}.", Level = EventLevel.Verbose)]
        public void SigningOut(string rawUrl)
        {
            if (IsEnabled())
            {
                WriteEvent(6, rawUrl);
            }
        }

        [Event(7, Message = "Federation: Signed out.", Level = EventLevel.Verbose)]
        public void SignedOut()
        {
            if (IsEnabled())
            {
                WriteEvent(7);
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

        [Event(8, Message = "Federation: Error singing out '{0}'.", Level = EventLevel.Error)]
        public void SignOutError(string error)
        {
            WriteEvent(8, error);
        }

        [NonEvent]
        internal void SignInError(Exception exception)
        {
            if (IsEnabled() && exception != null)
            {
                SignInError(exception.Message);
            }
        }

        [Event(9, Message = "Federation: Error singing in '{0}'.", Level = EventLevel.Error)]
        public void SignInError(string error)
        {
            WriteEvent(9, error);
        }
    }
}
