using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Boundaries;
using Microsoft.AspNetCore.Identity;
using Frontier.Controllers;
using Frontier.Stores;
using Frontier.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Serilog;

namespace Frontier
{
    public class Ignition
    {
        public static void Main(string[] args)
        {
			Log.Logger = new LoggerConfiguration()
				.WriteTo.Console()
				.CreateLogger();

			try
			{
				CreateHostBuilder(args)
					.UseSerilog()
					.Build()
					.Run();
			}
			catch (Exception ex)
			{
				Log.Fatal(ex, "Hollow failed unexpectedly.");
			}
			finally
			{
				Log.CloseAndFlush();
			}
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Ignition>();
                });
		
		public IConfiguration Configuration { get; }

		public Ignition(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "Web", Version = "v1" });
			});

			/////
			// Services 
			/////////////

			Services.CorePush pushNotifications = new();
			Services.CorePush.Initialise("", "", "", "", CorePush.Apple.ApnServerType.Development,
				"", "");

			services.AddTransient<ISMSService, TwilioService>();
			services.AddTransient<IEmailService, SendGridService>();
			// TwilioService.Initialise(Configuration["Twilio:AUTH_ID"], Configuration["Twilio:TOKEN"], Configuration["Twilio:NUMBER"]);

			//////
			// Connections
			////////////////

			CoreTerminal terminal = new(Repository.QueryStore.AccountDatabaseAccess,
				Repository.QueryStore.EventDatabaseAccess, Repository.QueryStore.EtchingDatabaseAccess,
				Repository.QueryStore.ProfileDatabaseAccess, Repository.QueryStore.ReportDatabaseAccess,
				Repository.QueryStore.NotificationDatabaseAccess, pushNotifications);

			foreach (var (GateType, Instance) in terminal.Gates)
			{
				services.AddSingleton(GateType, Instance);
			}

			
			/////////
			// Authentication Schema 
			//////////////////////////
			
			services.AddAuthentication(options =>
			{
				options.DefaultScheme = IdentityConstants.ApplicationScheme;
				options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
				options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
			})
				.AddIdentityCookies();

			services.AddIdentityCore<UserShard>()
				.AddUserStore<UserAccountStore>()
				.AddSignInManager()
				.AddDefaultTokenProviders();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Web v1"));
			}

			//app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthentication();
			app.UseCookiePolicy();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
