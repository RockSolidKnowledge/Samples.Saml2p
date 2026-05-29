using Rsk.AspNetCore.Authentication.Saml2p;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = "cookie";
        options.DefaultChallengeScheme = "saml";
    })
    .AddCookie("cookie")
    .AddSaml2p("saml",options=>
    {
        options.Licensee = "<insert here>";
        options.LicenseKey = "<insert here>";
        
        options.IdentityProviderMetadataAddress = "https://localhost:5003/saml/metadata";

        options.CallbackPath = "/saml/sso";
        
        options.SignInScheme = "cookie";
        
        options.ServiceProviderOptions = new SpOptions
        {
            EntityId = "https://localhost:5002/saml",
            MetadataPath = "/saml/metadata",
            SignAuthenticationRequests = false,
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddRazorPages();

var app = builder.Build();

app.UseAuthentication();


app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(c =>
{
    c.MapDefaultControllerRoute();
});
    


app.Run();