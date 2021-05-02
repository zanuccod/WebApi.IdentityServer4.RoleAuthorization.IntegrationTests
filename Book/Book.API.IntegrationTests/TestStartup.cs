using System.Net.Http;
using Book.API.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Book.API.IntegrationTests
{
    public class TestStartup : Startup
    {
        private readonly HttpMessageHandler _httpMessageHandler;

        public TestStartup(IConfiguration configuration, HttpMessageHandler httpMessageHandler)
            : base(configuration)
        {
            _httpMessageHandler = httpMessageHandler;
        }

        protected override void AddAuthentication(IServiceCollection services)
        {
            // used to prevent "NotFound" errors because WebApi is on another project file
            services.AddControllers().AddApplicationPart(typeof(Startup).Assembly);

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "http://localhost";
                    options.RequireHttpsMetadata = false;

                    options.JwtBackChannelHandler = _httpMessageHandler;
                });
        }
    }
}
