using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace Schwartz.Inventory.Api.Infrastructure
{
	public class ApplicationUserManager : UserManager<ApplicationUser>
	{
		public ApplicationUserManager(IUserStore<ApplicationUser> store)
			: base(store)
		{
		}

		public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options,
			IOwinContext context)
		{
			var userManager = new ApplicationUserManager(new CustomerUserStore());

			var dataProtectionProvider = options.DataProtectionProvider;
			if (dataProtectionProvider != null)
			{
				userManager.UserTokenProvider =
					new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"))
					{
						TokenLifespan = TimeSpan.FromHours(5)
					};
			}

			return userManager;
		}
	}

	public class CustomerUserStore : IUserStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>
	{
		public Task CreateAsync(ApplicationUser user)
		{
			throw new NotImplementedException();
		}

		public Task UpdateAsync(ApplicationUser user)
		{
			throw new NotImplementedException();
		}

		public Task DeleteAsync(ApplicationUser user)
		{
			throw new NotImplementedException();
		}

		public Task<ApplicationUser> FindByIdAsync(string userId)
		{
			var user = new ApplicationUser
			{
				FirstName = "Test",
				LastName = "Tester",
				UserName = "test@test.com"
			};

			return Task.FromResult(user);
		}

		public Task<ApplicationUser> FindByNameAsync(string userName)
		{
			var user = new ApplicationUser
			{
				FirstName = "Test",
				LastName = "Tester",
				UserName = "test@test.com"
			};

			return Task.FromResult(user);
		}

		public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash)
		{
			return Task.FromResult(true);
		}

		public Task<string> GetPasswordHashAsync(ApplicationUser user)
		{
			return Task.FromResult("test");
		}

		public Task<bool> HasPasswordAsync(ApplicationUser user)
		{
			return Task.FromResult(true);
		}

		public void Dispose()
		{

		}
	}
}