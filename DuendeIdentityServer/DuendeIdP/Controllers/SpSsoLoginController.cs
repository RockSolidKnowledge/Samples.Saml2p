using Microsoft.AspNetCore.Mvc;
using Rsk.Saml.Services;

namespace DuendeIdP.Controllers;


public class SpSsoLoginController : Controller
{
    private readonly ISamlInteractionService samlInteractionService;

    public SpSsoLoginController(ISamlInteractionService samlInteractionService)
    {
        this.samlInteractionService = samlInteractionService ?? throw new ArgumentNullException(nameof(samlInteractionService));
    }
    
    [HttpPost]
    public async Task ExecuteSpSso(string serviceProviderId)
    {
        
        var ssoResponse = await samlInteractionService.CreateIdpInitiatedSsoResponse(serviceProviderId, "https://localhost:5001/relay/state/url");

        await samlInteractionService.ExecuteIdpInitiatedSso(HttpContext, ssoResponse);
    }
}