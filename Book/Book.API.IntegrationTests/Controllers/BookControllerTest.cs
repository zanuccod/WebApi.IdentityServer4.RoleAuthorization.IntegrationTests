using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;

namespace Book.API.IntegrationTests.Controllers
{
    public class BookControllerTest : IntegrationTestBase
    {
        private readonly HttpClient client;
        private const string _baseAddress = "http://localhost/api/book/";

        public BookControllerTest()
        {
            var server = new TestServer(new WebHostBuilder()
                    .UseStartup<TestStartup>()
                    .ConfigureServices(
                        services =>
                        {
                            services.AddSingleton(_identityServerProxy.IdentityServer.CreateHandler());
                        })
                    .UseDefaultServiceProvider(x => x.ValidateScopes = false)
                    .UseKestrel());

            client = server.CreateClient();
        }

        [Fact]
        public async Task Get_Authenticated_ShouldReturnAllBooks()
        {
            // Arrange
            client.SetBearerToken(await GetToken(GRANT_TYPE_ADMIN));

            // Act
            var response = await client
                .GetAsync(_baseAddress)
                .ConfigureAwait(false);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var returnedList = JsonConvert.DeserializeObject<IEnumerable<Models.Book>>(await response.Content.ReadAsStringAsync());
            Assert.Equal(2, returnedList.Count());
        }

        [Fact]
        public async Task Get_NotAuthenticated_ShouldReturnUnauthorized()
        {
            // Act
            var response = await client
                .GetAsync(_baseAddress)
                .ConfigureAwait(false);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }


        [Theory]
        [InlineData(GRANT_TYPE_ADMIN, HttpStatusCode.OK)]
        [InlineData("custom_admin_2", HttpStatusCode.Unauthorized)]
        public async Task FindAllAsync_UnauthorizedRole_ShouldReturn_Unauthorized(string role, HttpStatusCode expctedStatusCode)
        {
            // Arrange
            client.SetBearerToken(await GetToken(role));

            // Act
            var response = await client
                .GetAsync(_baseAddress)
                .ConfigureAwait(false);

            // Assert
            Assert.Equal(expctedStatusCode, response.StatusCode);
        }
    }
}
