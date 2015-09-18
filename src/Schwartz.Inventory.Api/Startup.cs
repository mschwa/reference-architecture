using System;
using System.Configuration;
using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Owin;
using Schwartz.Inventory.Api.Infrastructure;
using Schwartz.Inventory.Api.Providers;

namespace Schwartz.Inventory.Api
{
	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			var config = new HttpConfiguration();

			var builder = new ContainerBuilder();
			builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
			builder.RegisterWebApiFilterProvider(config);
			builder.RegisterModule(new DependencyContainer());
			var container = builder.Build();

			config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

			config.MapHttpAttributeRoutes();

			ConfigureOAuthTokenGeneration(app, container);
			ConfigureOAuthTokenConsumption(app);
			
			// Evaluate for security
			app.UseCors(CorsOptions.AllowAll);
			app.UseWebApi(config);
		}

		private void ConfigureOAuthTokenGeneration(IAppBuilder app, IContainer container)
		{
			app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

			var OAuthServerOptions = new OAuthAuthorizationServerOptions
			{
				//For Dev enviroment only (on production should be AllowInsecureHttp = false)
				AllowInsecureHttp = true,
				TokenEndpointPath = new PathString("/oauth/token"),
				AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
				Provider = new CustomOAuthProvider(),
				AccessTokenFormat = container.Resolve<ISecureDataFormat<AuthenticationTicket>>()
			};

			// OAuth 2.0 Bearer Access Token Generation
			app.UseOAuthAuthorizationServer(OAuthServerOptions);
		}

		private void ConfigureOAuthTokenConsumption(IAppBuilder app)
		{
			var issuer = "http://localhost:59822";
			var audienceId = ConfigurationManager.AppSettings["as:AudienceId"];
			var audienceSecret = TextEncodings.Base64Url.Decode(ConfigurationManager.AppSettings["as:AudienceSecret"]);

			// Api controllers with an [Authorize] attribute will be validated with JWT
			app.UseJwtBearerAuthentication(
				new JwtBearerAuthenticationOptions
				{
					AuthenticationMode = AuthenticationMode.Active,
					AllowedAudiences = new[] {audienceId},
					IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
					{
						new SymmetricKeyIssuerSecurityTokenProvider(issuer, audienceSecret)
					}
				});
		}
	}
}