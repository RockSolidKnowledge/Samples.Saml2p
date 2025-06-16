using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Rsk.Saml.Services;

namespace IdentityServer4.Quickstart.UI.IdpInitiated
{
    public class IdpInitiatedController : Controller
    {
        private readonly ISamlInteractionService _samlInteractionService;
        public IdpInitiatedController(ISamlInteractionService samlInteractionService)
        {
            _samlInteractionService = samlInteractionService;
        }
        
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var serviceProviders = await _samlInteractionService.GetIdpInitiatedSsoCompatibleServiceProviders();
            var viewModel = new IdpInitiatedViewModel
            {
                serviceProviders = serviceProviders
            };
            return View(viewModel);
        }
        
        [HttpPost]
        public async Task ExecuteSpSso(string serviceProviderId)
        {
            var ssoResponse = await _samlInteractionService.CreateIdpInitiatedSsoResponse(serviceProviderId, "https://localhost:5001/relay/state/url");

            await _samlInteractionService.ExecuteIdpInitiatedSso(HttpContext, ssoResponse);
        }
    }
}