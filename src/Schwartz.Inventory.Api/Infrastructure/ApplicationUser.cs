using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Schwartz.Inventory.Api.Infrastructure
{
	public class ApplicationUser : IUser
	{
		[Required]
		[MaxLength(100)]
		public string FirstName { get; set; }

		[Required]
		[MaxLength(100)]
		public string LastName { get; set; }

		public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authType)
		{
			return await manager.CreateIdentityAsync(this, authType);
		}

		public string Id => Guid.NewGuid().ToString();
		
		public string UserName { get; set; }
	}
}