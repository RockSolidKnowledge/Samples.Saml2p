using System.Collections.Generic;

namespace IdentityServer4.Quickstart.UI.IdpInitiated
{
    public class IdpInitiatedViewModel
    {
        public IEnumerable<Rsk.Saml.Models.ServiceProvider> serviceProviders { get; set; }
    }
}