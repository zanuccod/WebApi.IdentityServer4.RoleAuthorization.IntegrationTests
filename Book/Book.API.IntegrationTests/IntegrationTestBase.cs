using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Contrib.AspNetCore.Testing.Builder;
using IdentityServer4.Contrib.AspNetCore.Testing.Configuration;
using IdentityServer4.Contrib.AspNetCore.Testing.Services;
using IdentityServer4.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Book.API.IntegrationTests
{
    public class IntegrationTestBase
    {
        protected readonly IdentityServerWebHostProxy _identityServerProxy;
        private readonly ClientConfiguration _clientConf;

        public IntegrationTestBase()
        {
            _clientConf = new ClientConfiguration("client_id", "client_key");
            var random = new Random();

            var webHostBuilder = new IdentityServerTestWebHostBuilder()
                .AddIdentityResources(new IdentityResource[]
                 {
                     new IdentityResources.OpenId()
                 })
                .AddClients(new Client[]
                {
                    new Client
                    {
                        ClientId = _clientConf.Id,
                        AccessTokenType = AccessTokenType.Jwt,
                        AllowedGrantTypes = new[] { GrantType.ClientCredentials },
                        ClientSecrets = { new Secret(_clientConf.Secret.Sha256()) },

                        AllowedScopes = { "BookApi" }
                    }
                })
                .AddApiResources(new ApiResource[]
                {
                    new ApiResource
                    {
                        Name = "BookApi",
                        DisplayName = "BookAPI Service",
                        Scopes = new List<string> { "BookApi" },
                        ApiSecrets = { new Secret("book_secret_key".Sha256()) }
                    }
                })
                .AddApiScopes(new ApiScope[]
                {
                    new ApiScope
                    {
                        Name = "BookApi"
                    }
                })
                .UseIdentityServerBuilder(services => services
                    .AddIdentityServer()
                    .AddDefaultEndpoints()
                    // we use this trick to allow multiple parallels test in the same time
                    .AddDeveloperSigningCredential(true, "tempkey_" + random.Next())
                )
                .CreateWebHostBuider();

            _identityServerProxy = new IdentityServerWebHostProxy(webHostBuilder);
        }

        protected async Task<string> GetToken()
        {
            var tokenResponse = await _identityServerProxy
                .GetClientAccessTokenAsync(
                _clientConf,
                "BookApi");

            return tokenResponse.AccessToken;
        }
    }
}
