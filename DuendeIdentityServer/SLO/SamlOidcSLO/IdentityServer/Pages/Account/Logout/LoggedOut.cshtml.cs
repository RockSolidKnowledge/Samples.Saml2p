using System;
using System.Threading.Tasks;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rsk.Saml.Services;

namespace IdentityServerHost.Pages.Logout;

[SecurityHeaders]
[AllowAnonymous]
public class LoggedOut : PageModel
{
    private readonly IIdentityServerInteractionService _interactionService;

    public LoggedOutViewModel View { get; set; }

    public LoggedOut(IIdentityServerInteractionService interactionService, ISamlInteractionService samlInteractionService)
    {
        _interactionService = interactionService;
    }

    public async Task OnGet(string logoutId, string requestId)
    {
        // get context information (client name, post logout redirect URI and iframe for federated signout)
        var logout = await _interactionService.GetLogoutContextAsync(logoutId);

        View = new LoggedOutViewModel
        {
            AutomaticRedirectAfterSignOut = LogoutOptions.AutomaticRedirectAfterSignOut,
            ClientName = String.IsNullOrEmpty(logout?.ClientName) ? logout?.ClientId : logout?.ClientName,
            SignOutIframeUrl =  logout?.SignOutIFrameUrl
        };
        
        // Configure iterative SAML-Initiated SLO
        View.PostLogoutRedirectUri = Url.Page("/account/logout/SamlIterativeSlo", new { logoutId = logoutId, requestId = requestId });
        View.AutomaticRedirectAfterSignOut = true;
       
    }
}