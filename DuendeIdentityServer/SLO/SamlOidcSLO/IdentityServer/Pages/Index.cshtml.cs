using System.Linq;
using System.Reflection;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServerHost.Pages.Home;

[AllowAnonymous]
public class Index : PageModel
{
    private readonly IUserSession _sessionService;
    public string Version;

    public Index(IUserSession sessionService)
    {
        _sessionService = sessionService;
    }
    public void OnGet()
    {
        Version = typeof(Duende.IdentityServer.Hosting.IdentityServerMiddleware).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion.Split('+').First();

        var clients = _sessionService.GetClientListAsync().Result;
    }
}