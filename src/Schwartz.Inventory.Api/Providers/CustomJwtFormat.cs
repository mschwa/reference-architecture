using System;
using System.IdentityModel.Tokens;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Schwartz.Inventory.Api.Infrastructure;
using Thinktecture.IdentityModel.Tokens;

namespace Schwartz.Inventory.Api.Providers
{
	public class CustomJwtFormat : ISecureDataFormat<AuthenticationTicket>
	{
		public ApplicationSettings Settings { get; set; }

		public string Protect(AuthenticationTicket data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}

			var audienceId = Settings.AudienceId;
			var symmetricKeyAsBase64 = Settings.AudienceSecret;
			var keyByteArray = TextEncodings.Base64Url.Decode(symmetricKeyAsBase64);
			var signingKey = new HmacSigningCredentials(keyByteArray);
			var issued = data.Properties.IssuedUtc;
			var expires = data.Properties.ExpiresUtc;

			var token = new JwtSecurityToken(Settings.BaseUrl, audienceId, data.Identity.Claims, 
				issued.Value.UtcDateTime, expires.Value.UtcDateTime, signingKey);

			var handler = new JwtSecurityTokenHandler();
			var jwt = handler.WriteToken(token);

			return jwt;
		}

		public AuthenticationTicket Unprotect(string protectedText)
		{
			throw new NotImplementedException();
		}
	}
}