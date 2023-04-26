using Duende.IdentityServer;
using Duende.IdentityServer.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Rsk.Saml.Configuration;
using Serilog;

namespace DuendeIdP;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddRazorPages();

        var isBuilder = builder.Services.AddIdentityServer(options =>
            {
                options.KeyManagement.Enabled = true;
                options.KeyManagement.SigningAlgorithms = new[] {
                    new SigningAlgorithmOptions("RS256") {UseX509Certificate = true}
                };

                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                // see https://docs.duendesoftware.com/identityserver/v5/fundamentals/resources/
                options.EmitStaticAudienceClaim = true;
            })
            .AddTestUsers(TestUsers.Users);

        // in-memory, code config
        isBuilder.AddInMemoryIdentityResources(Config.GetIdentityResources());
        isBuilder.AddInMemoryApiScopes(Config.GetApiScopes());
        isBuilder.AddInMemoryClients(Config.GetClients());

        isBuilder.AddSamlPlugin(options =>
            {
                options.Licensee = "DEMO";
                options.LicenseKey = "eyJTb2xkRm9yIjowLjAsIktleVByZXNldCI6NiwiU2F2ZUtleSI6ZmFsc2UsIkxlZ2FjeUtleSI6ZmFsc2UsIlJlbmV3YWxTZW50VGltZSI6IjAwMDEtMDEtMDFUMDA6MDA6MDAiLCJhdXRoIjoiREVNTyIsImV4cCI6IjIwMjMtMDMtMjRUMDA6MDA6MDAiLCJpYXQiOiIyMDIzLTAyLTI0VDA5OjIyOjA3Iiwib3JnIjoiREVNTyIsImF1ZCI6Mn0=.n2rMJsO+GWfSEYFnBoweBGqQjBLpUBELo4O2iHvJwFjuifXSsaTc6cyJG50SF3z3tnLylY+yueEmV2SXr86IG/IltNB/+Cip/V7g3l5tHMbla5QYr5aYbyaUPASfs+sPXW750sx++pR/4WC4sDzgckeDANhZl6A2fIPUOXM/BG+V8cvsb6xhY6+XfRCMAqPKW3XLxG8cPZBQ6teAdPrtDJuI1UNVtkFwtjBypjr/hgMHxW7oVT0GV7mBQknqqrvq6dQjLqGgxvdamxkmBWMTfFTrysqwvK2eVJsOV0IlIYUCwz2c2H//1cvW4o5K8tkSpwp/uwjXdpz1pB3jzwGPzl/kZ1PTiZOh1uFTEGhRhn2A93vFT1dcSaSsGDG0Excu2H66nuCw4OsUr4sUZm5+Y57/xHlFfTo5wbymSSMXLVpzL3brzfOvOewAawDq5HNjBmPjOaCpWaz6hygT/mOdqr+0T0W+l84XEdoxyP1GuVwN/eCL7qkroHUsksXqbmQmCUioV3wK5+sKeVmMB/vOBUQnJJR0snV6pBAvEVCLcGw/8Nu2+ZreYDdTNy6CSYlGjQi5b6GxKBMCifv15uoeVeEej/UXtKTJRVe72B6oS6tZupvw3evm8nruTM9QObUqESgF+M0hVuyB0/eVngC+gNo6DKkDdS8I++ZivAn2AsI=";

                options.WantAuthenticationRequestsSigned = false;
            })
            .AddInMemoryServiceProviders(Config.GetServiceProviders());

        // use different cookie name that sp...
        builder.Services.Configure<CookieAuthenticationOptions>(IdentityServerConstants.DefaultCookieAuthenticationScheme,
            cookie => { cookie.Cookie.Name = "idsrv.idp"; });

        return builder.Build();
    }
    
    public static WebApplication ConfigurePipeline(this WebApplication app)
    { 
        app.UseSerilogRequestLogging();
    
        app.UseDeveloperExceptionPage();

        app.UseStaticFiles();
        app.UseRouting();

        app.UseIdentityServer()
            .UseIdentityServerSamlPlugin(); // enables SAML endpoints (e.g. ACS and SLO)

        app.UseAuthorization();
        
        app.MapRazorPages()
            .RequireAuthorization();

        return app;
    }
}