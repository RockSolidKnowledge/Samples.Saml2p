using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Rsk.Saml.Services;

namespace IdentityServerHost.Pages.Logout;

[AllowAnonymous]
public class SamlIterativeSlo : PageModel
{
    private readonly ISamlInteractionService _samlInteractionService;
    private readonly IIdentityServerInteractionService _interactionService;

    public SamlIterativeSlo(ISamlInteractionService samlInteractionService, IIdentityServerInteractionService interactionService)
    {
        _samlInteractionService = samlInteractionService;
        _interactionService = interactionService;
    }

    public async Task<IActionResult> OnGet(string logoutId, string requestId)
    {
        //If no logout Id the session is orphaned and no logout information exists
        if (string.IsNullOrWhiteSpace(logoutId))
        {
            return Redirect("~/");
        }
        string completionUrl;
        
        //If SLO was started by a SAML Client they should be redirected back to the SAML SLO endpoint
        //to complete the original request
        if (!string.IsNullOrWhiteSpace(requestId))
        {
            completionUrl = await _samlInteractionService.GetLogoutCompletionUrl(requestId);
        }
        else
        {
            //If no SAML Client was involved, redirect to the original post logout redirect URI once SLO is complete
            var logout = await _interactionService.GetLogoutContextAsync(logoutId);
            completionUrl = logout?.PostLogoutRedirectUri;
        }

        await _samlInteractionService.ExecuteIterativeSlo(HttpContext, logoutId, completionUrl);
        return new EmptyResult();
    }
}