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
using Frontier.Controllers;
using Microsoft.Extensions.Logging;
using Core;
using Core.Daemons;

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

        public static string HollowSpecificOrigins = "_HollowSpecificOrigins";

        public IConfiguration Configuration { get; }

        public Ignition(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: HollowSpecificOrigins,
                    policy =>
                    {
                        policy.WithOrigins("https://almostcanary.com");
                    });
            });

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Web", Version = "v1" });
            });

            /////
            // Loggers
            ////////////

            var loggerFactory = new LoggerFactory()
                .AddSerilog(Log.Logger);

            var frontierLogger = loggerFactory.CreateLogger("Frontier");
            var coreLogger = loggerFactory.CreateLogger("Core");
            var repositoryLogger = loggerFactory.CreateLogger("Repository");

            /////
            // Database
            /////////////

            Harbor harbor;

            if (IsDebug)
            {
                harbor = new(Harbor.Flag.Development, repositoryLogger);
            }
            else
            {
                harbor = new(Harbor.Flag.Production, repositoryLogger);
            }

            var keyProvider = harbor.KeyDatabaseAccess;

            /////
            // Services 
            /////////////

            OneSignalService pushNotifications = new();
            OneSignalService.Initialise(frontierLogger,
                keyProvider.GetHollowOneSignalApiKeyAsync().Result,
                keyProvider.GetHollowOneSignalAppIdAsync().Result);
            
            TwilioService.Initialise(frontierLogger,
                keyProvider.GetHollowTwilioAccountKeyAsync().Result,
                keyProvider.GetHollowTwilioAuthTokenAsync().Result,
                keyProvider.GetHollowTwilioMessagingServiceAsync().Result);

            services.AddTransient<INotificationService, OneSignalService>(service => pushNotifications);
            services.AddTransient<ISMSService, TwilioService>();
            services.AddTransient<IEmailService, SendGridService>();

            //////
            // Connections
            ////////////////

            CoreTerminal terminal = CoreTerminal.CreateTerminal(
                coreLogger,
                harbor.AccountDatabaseAccess,
                harbor.AdminDatabaseAccess,
                harbor.BannerDatabaseAccess,
                harbor.GatheringDatabaseAccess,
                harbor.SnapshotDatabaseAccess,
                harbor.ReportDatabaseAccess,
                harbor.KeyDatabaseAccess,
                harbor.MediaDatabaseAccess,
                harbor.NotificationDatabaseAccess,
                harbor.NestDatabaseAccess,
                harbor.MiscellaneousDatabaseAccess,
                pushNotifications);

            GuardBox box = new(frontierLogger,
                terminal.AccountOperations,
                terminal.BannerOperations,
                terminal.NestOperations,
                terminal.GatheringOperations,
                terminal.SnapshotOperations,
                terminal.KeyOperations,
                terminal.DisciplineOperations,
                terminal.MediaOperations,
                terminal.NotificationOperations,
                terminal.MiscellaneousOperations);

            services.AddSingleton(box);

            /////
            // Daemons
            ////////////

            services.AddHostedService(services => terminal.CreateRepositoryCleanupService());
            services.AddHostedService(services => terminal.CreateTelegramCleanupService());

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

            services.AddIdentityCore<CoreUser>()
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

            app.UseCors(HollowSpecificOrigins);

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