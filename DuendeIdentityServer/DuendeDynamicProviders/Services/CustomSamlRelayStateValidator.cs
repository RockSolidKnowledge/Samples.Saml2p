using Rsk.Saml.Services;

namespace DuendeDynamicProviders.Services;

public class CustomSamlRelayStateValidator : ISamlRelayStateValidator
{
    public Task<bool> Validate(string relayState, ICollection<string> knownValues)
    {
        return Task.FromResult(true);
    }
}