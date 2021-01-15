using System;
using System.Collections.Generic;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Rsk.Saml.Models;
using Rsk.Saml.Services;

namespace IdentityServer4.Quickstart.UI
{
    [SecurityHeaders]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IIdentityServerInteractionService interactionService;
        private readonly ISamlInteractionService samlInteractionService;

        public HomeController(
            IIdentityServerInteractionService interactionService,
            ISamlInteractionService samlInteractionService)
        {
            this.interactionService = interactionService ?? throw new ArgumentNullException(nameof(interactionService));
            this.samlInteractionService = samlInteractionService ?? throw new ArgumentNullException(nameof(samlInteractionService));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var serviceProviders = await samlInteractionService.GetIdpInitiatedSsoCompatibleServiceProviders();
            var model = new IdpInitiatedSsoViewModel(serviceProviders);


            return View(model);
        }

        [HttpPost]
        public async Task Index(IdpInitiatedSsoInputModel model)
        {
            var ssoResponse = await samlInteractionService.CreateIdpInitiatedSsoResponse(model.ServiceProviderId);

            await samlInteractionService.ExecuteIdpInitiatedSso(HttpContext, ssoResponse);
        }

        public async Task<IActionResult> Error(string errorId)
        {
            var vm = new ErrorViewModel();

            var message = await interactionService.GetErrorContextAsync(errorId);
            if (message != null)
            {
                vm.Error = message;
                message.ErrorDescription = null;
            }

            return View("Error", vm);
        }
    }

    public class IdpInitiatedSsoViewModel : IdpInitiatedSsoInputModel
    {
        public IEnumerable<ServiceProvider> IdpInitiatedSsoEnabledProviders { get; }

        public IdpInitiatedSsoViewModel(IEnumerable<ServiceProvider> serviceProviders)
        {
            IdpInitiatedSsoEnabledProviders = serviceProviders;
        }
    }

    public class IdpInitiatedSsoInputModel
    {
        public string ServiceProviderId { get; set; }
    }
}