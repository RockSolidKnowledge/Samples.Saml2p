using System.Collections.Generic;
using System.Threading.Tasks;
using Rsk.Saml.Services;

namespace Rsk.Saml.Samples.Services;

public class CustomSamlRelayStateValidator : ISamlRelayStateValidator
{
    public Task<bool> Validate(string relayState, ICollection<string> knownValues)
    {
        return Task.FromResult(true);
    }
}