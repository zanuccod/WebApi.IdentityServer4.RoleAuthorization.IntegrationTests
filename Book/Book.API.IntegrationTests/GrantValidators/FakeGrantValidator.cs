using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Validation;

namespace Book.API.IntegrationTests.GrantValidators
{
    public class FakeGrantValidator : IExtensionGrantValidator
    {
        public FakeGrantValidator()
        {
        }

        public string GrantType => IntegrationTestBase.GRANT_TYPE_ADMIN;

        public Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var claims = new List<Claim>()
            {
                new Claim(JwtClaimTypes.Role, "Admin")
            };

            context.Result = new GrantValidationResult("Admin", GrantType, claims);
            return Task.CompletedTask;
        }
    }
}
