using System.Collections.Generic;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Identity.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryIdentityResources(new IdentityResource[]
                    {
                        new IdentityResources.OpenId()
                    })
                .AddInMemoryApiResources(new List<ApiResource>
                    {
                        new ApiResource("BookApi")
                        {
                            ApiSecrets = { new Secret("book_secret_key".Sha256()) },
                            Scopes = new [] { "BookApi"  }
                        }
                    })
                .AddInMemoryApiScopes(new List<ApiScope>
                {
                    new ApiScope("BookApi")
                })
                .AddInMemoryClients(new List<Client>
                    {
                        new Client
                        {
                            ClientId = "client_id",
                            ClientSecrets = { new Secret("client_key".Sha256()) },
                            AccessTokenType = AccessTokenType.Jwt,
                            AllowedGrantTypes = GrantTypes.ClientCredentials,
                            AllowedScopes = { "BookApi" },
                        }
                    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseIdentityServer();
        }
    }
}
