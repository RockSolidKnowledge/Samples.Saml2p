using Duende.IdentityServer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace DuendeDynamicProviders.Controllers;

public class IdpInitiatedCompletedController : Controller
{
    [HttpGet]
    public async Task<IActionResult> RedirectToRelayState()
    {
        var externalCookie = await HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
        
        string relayState = externalCookie?.Properties?.Items["RelayState"];
        
        if (string.IsNullOrEmpty(relayState))
        {
            return BadRequest("Relay state cannot be null or empty.");
        }

        // Redirect to the specified relay state URL
        return Redirect(relayState);
    }
}