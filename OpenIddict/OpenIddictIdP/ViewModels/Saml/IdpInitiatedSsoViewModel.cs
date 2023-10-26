using Rsk.Saml.Models;
using System.Collections.Generic;

namespace openiddictidp.ViewModels.Saml;

public class IdpInitiatedSsoViewModel
{
    public List<ServiceProvider> IdpInitiatedSsoEnabledProviders;

    public string ServiceProviderId { get; set; }

    public IdpInitiatedSsoViewModel(IEnumerable<ServiceProvider> serviceProviders)
    {
        IdpInitiatedSsoEnabledProviders = new List<ServiceProvider>(serviceProviders);
    }
}