using Rsk.Saml.OpenIddict.Quartz.Configuration.DependencyInjection;
using IdentityModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Rsk.Saml.Configuration;
using Rsk.Saml.OpenIddict.AspNetCore.Identity.Configuration.DependencyInjection;
using Rsk.Saml.OpenIddict.Configuration.DependencyInjection;
using openiddictidp.Data;
using Rsk.Saml.OpenIddict.EntityFrameworkCore.Configuration.DependencyInjection;
using Rsk.Saml.Samples;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace openiddictidp;

public class Startup
{
    public Startup(IConfiguration configuration)
        => Configuration = configuration;

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllersWithViews();
        services.AddRazorPages();
        var defaultConnectionString = Configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            //Configure the database provider to use.
            options.UseSqlServer(defaultConnectionString);

            // Register the entity sets needed by OpenIddict.
            // Note: use the generic overload if you need
            // to replace the default OpenIddict entities.
            options.UseOpenIddict();
        });

        services.AddDatabaseDeveloperPageExceptionFilter();

        // Register the Identity services.
        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
            .AddDefaultUI();

        services.Configure<IdentityOptions>(options =>
        {
            options.ClaimsIdentity.UserIdClaimType = JwtClaimTypes.Subject;
            options.ClaimsIdentity.UserNameClaimType = JwtClaimTypes.Name;
            options.ClaimsIdentity.RoleClaimType = JwtClaimTypes.Role;
            options.ClaimsIdentity.EmailClaimType = JwtClaimTypes.Email;
        });
        
        // OpenIddict offers native integration with Quartz.NET to perform scheduled tasks
        // (like pruning orphaned authorizations/tokens from the database) at regular intervals.
        services.AddQuartz(options =>
        {
            options.UseSimpleTypeLoader();
            options.UseInMemoryStore();
        });

        // Register the Quartz.NET service and configure it to block shutdown until jobs are complete.
        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        services.AddOpenIddict()

            // Register the OpenIddict core components.
            .AddCore(options =>
            {
                // Configure OpenIddict to use the Entity Framework Core stores and models.
                // Note: call ReplaceDefaultEntities() to replace the default OpenIddict entities.
                options.UseEntityFrameworkCore()
                       .UseDbContext<ApplicationDbContext>();

                // Enable Quartz.NET integration.
                options.UseQuartz();
            })

            // Register the OpenIddict server components.
            .AddServer(options =>
            {
                //SAML requires this for metadata generation, looks like you can run this without setting it
                //There does seem to be some validation on the validation options class but it doesn't throw an exception
                //An additional check may need to be done to ensure this is set
                //options.SetIssuer("https://localhost:5003");

                // Enable the authorization, logout, token and userinfo endpoints.
                options.SetAuthorizationEndpointUris("connect/authorize")
                       .SetLogoutEndpointUris("connect/logout")
                       .SetTokenEndpointUris("connect/token")
                       .SetUserinfoEndpointUris("connect/userinfo");

                // Mark the "email", "profile" and "roles" scopes as supported scopes.
                options.RegisterScopes(Scopes.Email, Scopes.Profile, Scopes.Roles);

                // Note: this sample only uses the authorization code flow but you can enable
                // the other flows if you need to support implicit, password or client credentials.
                options.AllowAuthorizationCodeFlow();

                // Register the signing and encryption credentials.
                options.AddDevelopmentEncryptionCertificate()
                       .AddDevelopmentSigningCertificate();

                // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
                options.UseAspNetCore()
                    .EnableAuthorizationEndpointPassthrough()
                       .EnableLogoutEndpointPassthrough()
                       .EnableTokenEndpointPassthrough()
                       .EnableUserinfoEndpointPassthrough()
                       .EnableStatusCodePagesIntegration();

                options.AddSamlPlugin(builder =>
                {
                    //use quartz to prune old SAML messages every 14 days.
                    builder.PruneSamlMessages();
                    
                    //Already added the DbContext above
                    builder.UseSamlEntityFrameworkCore()
                        .AddSamlMessageDbContext(optionsBuilder =>
                        {
                            //Configure the database provider to use.
                            optionsBuilder.UseSqlServer(defaultConnectionString, x =>x.MigrationsAssembly(typeof(Startup).Assembly.FullName));
                        })
                        .AddSamlConfigurationDbContext(optionsBuilder =>
                        {
                            //Configure the database provider to use.
                            optionsBuilder.UseSqlServer(defaultConnectionString,
                                x => x.MigrationsAssembly(typeof(Startup).Assembly.FullName));
                        });

                    builder.ConfigureSamlOpenIddictServerOptions(serverOptions =>
                        {
                            serverOptions.HostOptions = new SamlHostUserInteractionOptions()
                            {
                                LoginUrl = "/Identity/Account/Login",
                                LogoutUrl = "/connect/logout",
                            };

                            serverOptions.IdpOptions = new SamlIdpOptions()
                            {
                                Licensee = LicenseKey.Licensee,
                                LicenseKey =LicenseKey.Key,
                                UseIFramesForSlo = false

                            };
                        });

                    builder.AddSamlAspIdentity<ApplicationUser>();
                });
            })

            // Register the OpenIddict validation components.
            .AddValidation(options =>
            {
                // Import the configuration from the local OpenIddict server instance.
                options.UseLocalServer();

                // Register the ASP.NET Core host.
                options.UseAspNetCore();
            });

        // Register the worker responsible for seeding the database.
        // Note: in a real world application, this step should be part of a setup script.
        services.AddHostedService<Worker>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseStatusCodePagesWithReExecute("~/error");
            //app.UseExceptionHandler("~/error");

            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //app.UseHsts();
        }
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseOpenIddictSamlPlugin();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapDefaultControllerRoute();
            endpoints.MapRazorPages();
        });
    }
}
