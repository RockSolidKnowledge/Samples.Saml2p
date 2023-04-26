using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography.X509Certificates;
using Common;
using Microsoft.AspNetCore.Authentication;
using Rsk.AspNetCore.Authentication.Saml2p;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = "Cookies";
        options.DefaultChallengeScheme = "saml";
    })
    .AddCookie("Cookies", options =>
    {
        //Change cookie name to prevent collisions when running multiple applications on localhost
        options.Cookie.Name = "ServiceProviderSAML1";
    })    
    .AddSaml2p("saml", options =>
    {
        options.Licensee = TestConstants.Licensee;
        options.LicenseKey = TestConstants.LicenseKey;

        options.IdentityProviderMetadataAddress = "https://localhost:5001/saml/metadata";

        options.ServiceProviderOptions = new SpOptions()
        {
            EntityId = "https://localhost:5003",
            SigningCertificate = new X509Certificate2("testclient.pfx", "test"),
            MetadataPath = new PathString("/saml/metadata") 
        };

        options.SignInScheme = "Cookies";
        options.CallbackPath = new PathString("/signin-saml");

        options.SignedOutCallbackPath = "/signout-saml";

        
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages().RequireAuthorization();

app.Run();
