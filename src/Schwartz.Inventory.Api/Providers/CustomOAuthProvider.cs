using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Schwartz.Inventory.Api.Infrastructure;

namespace Schwartz.Inventory.Api.Providers
{
	public class CustomOAuthProvider : OAuthAuthorizationServerProvider
	{
		public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
		{
			context.Validated();
			return Task.FromResult<object>("true");
		}

		public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
		{
			var allowedOrigin = "*";

			context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] {allowedOrigin});

			var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();

			var user = new ApplicationUser
			{
				FirstName = "Test",
				LastName = "Tester",
				UserName = "test@test.com"
			};

			var oAuthIdentity = await user.GenerateUserIdentityAsync(userManager, "JWT");
			oAuthIdentity.AddClaims(ExtendedClaimsProvider.GetClaims(user));
			var ticket = new AuthenticationTicket(oAuthIdentity, null);

			context.Validated(ticket);
		}
	}
}