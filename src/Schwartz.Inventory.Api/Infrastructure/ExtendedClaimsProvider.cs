using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Schwartz.Inventory.Api.Infrastructure
{
	public static class ExtendedClaimsProvider
	{
		public static IEnumerable<Claim> GetClaims(ApplicationUser user)
		{
			var claims = new List<Claim> {CreateClaim("InventoryAdmin", "1")};
			return claims;
		}

		public static Claim CreateClaim(string type, string value)
		{
			return new Claim(type, value, ClaimValueTypes.String);
		}
	}
}