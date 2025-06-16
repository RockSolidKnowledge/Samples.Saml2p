using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rsk.Saml.Services;

namespace DuendeIdP.Pages.IdpInitiated;

[Authorize]
public class Index : PageModel
{
    private readonly ISamlInteractionService _samlInteractionService;

    public Index(ISamlInteractionService samlInteractionService)
    {
        _samlInteractionService = samlInteractionService;
    }

    public ViewModel View { get; set; }
        
    public async Task OnGet()
    {
        var serviceProviders = await _samlInteractionService.GetIdpInitiatedSsoCompatibleServiceProviders();
        
        View = new ViewModel
        {
            serviceProviders = serviceProviders
        };
    }
}