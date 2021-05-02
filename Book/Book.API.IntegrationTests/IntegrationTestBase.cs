using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Book.API.IntegrationTests.GrantValidators;
using IdentityModel;
using IdentityServer4.Contrib.AspNetCore.Testing.Builder;
using IdentityServer4.Contrib.AspNetCore.Testing.Configuration;
using IdentityServer4.Contrib.AspNetCore.Testing.Services;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Book.API.IntegrationTests
{
    public class IntegrationTestBase
    {
        protected readonly IdentityServerWebHostProxy _identityServerProxy;
        private readonly ClientConfiguration _clientConf;

        public const string GRANT_TYPE_ADMIN = "custom_admin";

        public IntegrationTestBase()
        {
            _clientConf = new ClientConfiguration("client_id", "client_key");
            var random = new Random();

            var webHostBuilder = new IdentityServerTestWebHostBuilder()
                .AddIdentityResources(new IdentityResource[]
                 {
                     new IdentityResource()
                     {
                         Name = JwtClaimTypes.Role,
                         UserClaims = new List<string> { JwtClaimTypes.Role }
                     }
                 })
                .AddClients(new Client[]
                {
                    new Client
                    {
                        ClientId = _clientConf.Id,
                        AccessTokenType = AccessTokenType.Jwt,
                        AllowedGrantTypes = new[] { GRANT_TYPE_ADMIN },
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
                        ApiSecrets = { new Secret("book_secret_key".Sha256()) },
                        UserClaims = new List<string> { JwtClaimTypes.Role }
                    }
                })
                .AddApiScopes(new ApiScope[]
                {
                    new ApiScope
                    {
                        Name = "BookApi",
                        UserClaims = new List<string> { JwtClaimTypes.Role }
                    }
                })
                // used to add custom Role to accessToken
                .UseServices((context, collection) =>
                {
                    collection.AddScoped<IExtensionGrantValidator, FakeGrantValidator>();
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

        protected async Task<string> GetToken(string grantType)
        {
            var tokenResponse = await _identityServerProxy
                .GetTokenAsync(
                _clientConf,
                grantType,
                new Dictionary<string, string>());

            return tokenResponse.AccessToken;
        }
    }
}
