using Microsoft.SharePoint;
using Microsoft.SharePoint.IdentityModel;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FedAuthDebugger
{
    public class FedAuthDebuggerHttpModule : IHttpModule
    {
        private SPFederationAuthenticationModule FederationModule 
        {
            get { return HttpContext.Current.ApplicationInstance.Modules["FederatedAuthentication"] as SPFederationAuthenticationModule; }   
        }

        private SPSessionAuthenticationModule SessionModule
        {
            get { return HttpContext.Current.ApplicationInstance.Modules["SessionAuthentication"] as SPSessionAuthenticationModule; }
        }

        public void Init(HttpApplication context)
        {
            FederationModule.SecurityTokenReceived += OnFederationSecurityTokenReceived;
            FederationModule.SecurityTokenValidated += OnFederationSecurityTokenValidated;
            FederationModule.SessionSecurityTokenCreated += OnFederationSessionSecurityTokenCreated;
            FederationModule.SignedIn += OnFederationSignedIn;
            FederationModule.SignInError += OnFederationSignInError;
            FederationModule.SigningOut += OnFederationSigningOut;
            FederationModule.SignedOut += OnFederationSignedOut;
            FederationModule.SignOutError += OnFederationSignOutError;

            SessionModule.SessionSecurityTokenCreated += OnTokenCreated;
            SessionModule.SessionSecurityTokenReceived += OnTokenReceived;
            SessionModule.SigningOut += OnSigningOut;
            SessionModule.SignedOut += OnSignedOut;
            SessionModule.SignOutError += OnSignOutError;
        }

        private void OnFederationSecurityTokenReceived(object sender, Microsoft.IdentityModel.Web.SecurityTokenReceivedEventArgs e)
        {
            FederationEventSource.Log.SecurityTokenReceived(e.SecurityToken.Id,
                e.SecurityToken.ValidFrom, e.SecurityToken.ValidTo);
        }

        private void OnFederationSecurityTokenValidated(object sender, Microsoft.IdentityModel.Web.SecurityTokenValidatedEventArgs e)
        {
            FederationEventSource.Log.SecurityTokenValidated(e.ClaimsPrincipal.Identity.Name);
        }

        private void OnFederationSessionSecurityTokenCreated(object sender, Microsoft.IdentityModel.Web.SessionSecurityTokenCreatedEventArgs e)
        {
            FederationEventSource.Log.SessionSecurityTokenCreated(e.SessionToken.ClaimsPrincipal.Identity.Name,
                e.SessionToken.ValidFrom, e.SessionToken.ValidTo);
        }

        private void OnFederationSignInError(object sender, Microsoft.IdentityModel.Web.Controls.ErrorEventArgs e)
        {
            FederationEventSource.Log.SignInError(e.Exception);
        }

        private void OnFederationSignedIn(object sender, EventArgs e)
        {
            FederationEventSource.Log.SignedIn(HttpContext.Current.User.Identity.Name);
        }

        public void OnFederationSigningOut(object sender, Microsoft.IdentityModel.Web.SigningOutEventArgs e)
        {
            FederationEventSource.Log.SigningOut(HttpContext.Current.Request.RawUrl);
        }

        public void OnFederationSignedOut(object sender, EventArgs e)
        {
            FederationEventSource.Log.SignedOut();
        }

        public void OnFederationSignOutError(object sender, Microsoft.IdentityModel.Web.Controls.ErrorEventArgs e)
        {
            FederationEventSource.Log.SignOutError(e.Exception);
        }

        private void OnTokenCreated(object sender, Microsoft.IdentityModel.Web.SessionSecurityTokenCreatedEventArgs e)
        {
            SessionEventSource.Log.SecurityTokenCreated(e.SessionToken.ClaimsPrincipal.Identity.Name,
                e.SessionToken.ValidFrom, e.SessionToken.ValidTo);
        }

        private void OnTokenReceived(object sender, Microsoft.IdentityModel.Web.SessionSecurityTokenReceivedEventArgs e)
        {
            var sessionLifetimeInMinutes = (e.SessionToken.ValidTo - e.SessionToken.ValidFrom).TotalMinutes;
            var logonTokenCacheExpirationWindow = TimeSpan.FromSeconds(1);

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                logonTokenCacheExpirationWindow = Microsoft.SharePoint.Administration.Claims.SPSecurityTokenServiceManager.Local.LogonTokenCacheExpirationWindow;
            });

            DateTime now = DateTime.UtcNow;
            DateTime validTo = e.SessionToken.ValidTo - logonTokenCacheExpirationWindow;
            DateTime validFrom = e.SessionToken.ValidFrom;

            SessionEventSource.Log.SecurityTokenReceived(e.SessionToken.ClaimsPrincipal.Identity.Name,  (now - validFrom).TotalMinutes,
                logonTokenCacheExpirationWindow, validFrom, validTo);

            if ((now < validTo) && (now > validFrom.AddMinutes((validTo - validFrom).TotalMinutes / 2)))
            {
                e.SessionToken = SessionModule.CreateSessionSecurityToken(e.SessionToken.ClaimsPrincipal, e.SessionToken.Context, now, now.AddMinutes(sessionLifetimeInMinutes), e.SessionToken.IsPersistent);
                e.ReissueCookie = true;

                SessionEventSource.Log.SessionReissued(e.SessionToken.ClaimsPrincipal.Identity.Name, 
                    now.AddMinutes(sessionLifetimeInMinutes));
            }
        }

        private void OnSigningOut(object sender, Microsoft.IdentityModel.Web.SigningOutEventArgs e)
        {
            if (HttpContext.Current.User == null)
            {
                SessionEventSource.Log.SessionTimeout();
            }
            else
            {
                SessionEventSource.Log.SigningOut(HttpContext.Current.User.Identity.Name);
            }
        }

        private void OnSignedOut(object sender, EventArgs e)
        {
            SessionEventSource.Log.SignedOut();
        }

        private void OnSignOutError(object sender, Microsoft.IdentityModel.Web.Controls.ErrorEventArgs e)
        {
            SessionEventSource.Log.SignOutError(e.Exception);
        }

        public void Dispose()
        {

        }
    }
}
