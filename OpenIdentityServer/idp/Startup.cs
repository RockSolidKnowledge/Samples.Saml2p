// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Security.Cryptography.X509Certificates;
using Open.IdentityServer;
using IdentityServerHost.Quickstart.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Rsk.Saml.Configuration;

namespace IdentityServer
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            var builder = services.AddIdentityServer()
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryClients(Config.Clients)
                .AddTestUsers(TestUsers.Users)
                .AddSamlPlugin(options =>
                {
                    options.Licensee = "DEMO";
                    options.LicenseKey = "eyJhdXRoIjoiREVNTyIsImV4cCI6IjIwMjYtMTEtMDVUMDA6MDA6MDAiLCJpYXQiOiIyMDI2LTA1LTA1VDEyOjEwOjEwLjg5NTQzMzVaIiwib3JnIjoiREVNTyIsImF1ZCI6Mn0=.ejftCxcuVd5MnRqry115R4HqQVj+/bPU6l9IomwLhKMzCAZu0bnuu2K8gVFqcnCnVM4Dz6slB+5ZWQbbDGIpDJ7+/MROvuAZafzAX+Jx0zGOjThJGYWiWOHLqfksAlk5Ur2epj8izYm2GjVd2v5mM9B+6p7NVTj3/NzXfRXDJu7PQTjoa6jmTwf18oqdie83itKPUUEhOtTSP6Szuw1nGqEjFxFSBi/hKHm8K9AU69vNn28j+X2WcGhCBu7mjYYZYgQYmZFAleLoz/dSomoRPgwf14XAc0SKKmJ34QhIfUtz8BWnGB2486Z6g1pzgdl4ZqlcOxi8cSPg0WsHlTIF838DIQr4pvlWH5xBMhDBJWMJF0gYrNjwTPGzcmAwUPRmQ/xBQlDILco8O0Mu4cwJEG8sIrEjF5p+sP+6hbjghTBUxeIEPQ3j/+gsx0Q9/7cq3AWvorS3w4SC1XINOOBsuIRnOn8bfvyL0MidgkwaGYsavVwXnAQHBhfIaeJ+XB8BisLpZgA2/2ivz9RJI/o6WQTiNY526gBignQ+cVcBPA0NFDm5gYJwNoJZST63433XxGB45Oztbm+W6uC4S5BwrRAaB+4xNXtMlaluCwvX44+havBviZCuITAto11CjhgQ0uvc5A4/Tig8idjBPJkaNGA7sud1a7WvaGHjHCYRyVo=";
                })
                .AddInMemoryServiceProviders(Config.ServiceProviders);

            builder.AddSigningCredential(X509CertificateLoader.LoadPkcs12FromFile("testclient.pfx", "test"));
            
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServer()
                .UseIdentityServerSamlPlugin();
            
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
