using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rsk.Saml.Services;
using openiddictidp.ViewModels.Saml;
using System;
using System.Threading.Tasks;

namespace openiddictidp.Controllers;

[Authorize]
public class SamlController : Controller
{
    private readonly ISamlInteractionService samlInteractionService;

    public SamlController(ISamlInteractionService samlInteractionService)
    {
        this.samlInteractionService = samlInteractionService ?? throw new ArgumentNullException(nameof(samlInteractionService));
    }

    [HttpGet]
    public async Task<IActionResult> IdpInitiatedSso()
    {
        var serviceProviders = await samlInteractionService.GetIdpInitiatedSsoCompatibleServiceProviders();
        var vm = new IdpInitiatedSsoViewModel(serviceProviders);

        return View("IdpInitiatedSso", vm);
    }

    [HttpPost]
    public async Task ExecuteSpSso(string serviceProviderId)
    {
        var ssoResponse = await samlInteractionService.CreateIdpInitiatedSsoResponse(serviceProviderId);

        await samlInteractionService.ExecuteIdpInitiatedSso(HttpContext, ssoResponse);
    }
}