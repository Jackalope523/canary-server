using System;
using Core.Boundaries;
using Microsoft.AspNetCore.Identity;
using Frontier.Stores;
using Frontier.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using Repository;
using Core;
using Frontier.Controllers;
using Microsoft.Extensions.Logging;

namespace Frontier
{
    public class Ignition
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.AzureApp()
                .MinimumLevel.Debug()
                .CreateLogger();

            Log.Information("Hollow starting up...");

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
                Log.Debug("Hollow shutting down.");
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

            var loggerFactory = new LoggerFactory()
                .AddSerilog(Log.Logger);


            /////
            // Services 
            /////////////

            var frontierLogger = loggerFactory.CreateLogger("Frontier");
            var coreLogger = loggerFactory.CreateLogger("Core");
            var repositoryLogger = loggerFactory.CreateLogger("Repository");

            Services.CorePush pushNotifications = new();
            Services.CorePush.Initialise("", "", "", "", CorePush.Apple.ApnServerType.Development,
                "", "");

            services.AddTransient<ISMSService, TwilioService>();
            services.AddTransient<IEmailService, SendGridService>();
            // TwilioService.Initialise(Configuration["Twilio:AUTH_ID"], Configuration["Twilio:TOKEN"], Configuration["Twilio:NUMBER"]);


            //////
            // Connections
            ////////////////

            Harbor harbor;

            if (IsDebug)
            {
                harbor = new(Harbor.Flag.Development, repositoryLogger);
            }
            else
            {
                harbor = new(Harbor.Flag.Production, repositoryLogger);
            }

            CoreTerminal terminal = CoreTerminal.CreateTerminal(
                coreLogger,
                harbor.AccountDatabaseAccess,
                harbor.AdminDatabaseAccess,
                new DebugBannerBypass(),
                harbor.EventDatabaseAccess,
                harbor.EtchingDatabaseAccess,
                harbor.ReportDatabaseAccess,
                harbor.KeyDatabaseAccess,
                harbor.MediaDatabaseAccess,
                harbor.NotificationDatabaseAccess,
                harbor.ProfileDatabaseAccess,
                pushNotifications);

            GuardBox box = new(frontierLogger,
                terminal.AccountOperations,
                terminal.BannerOperations,
                terminal.ProfileOperations,
                terminal.EventOperations,
                terminal.EtchingOperations,
                terminal.KeyOperations,
                terminal.DisciplineOperations,
                terminal.MediaOperations,
                terminal.NotificationOperations);

            services.AddSingleton(box);

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

        public static bool IsDebug
        {
            get
            {
                bool isDebug = false;
#if DEBUG
				isDebug = true;
#endif
                return isDebug;
            }
        }
    }
}