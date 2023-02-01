using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Boundaries;
using Microsoft.AspNetCore.Identity;
using Web.Controllers;
using Web.Stores;
using Web.Services;

namespace Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Web", Version = "v1" });
            });

            IAccountDatabase.AccountDatabaseAccess = DataAccess.QueryStore.AccountDatabaseAccess;
            IEventDatabase.EventDatabaseAccess = DataAccess.QueryStore.EventDatabaseAccess;

            services.AddSingleton(IAccountOperations.AccountManager);
            services.AddSingleton(IEventOperations.EventManager);

            services.AddTransient<ISMSService, TwilioService>();
            services.AddTransient<IEmailService, SendGridService>();
            TwilioService.Initialise(Configuration["Twilio:AUTH_ID"], Configuration["Twilio:TOKEN"], Configuration["Twilio:NUMBER"]);

            services.AddAuthentication()
                .AddIdentityCookies();
            services.AddIdentityCore<ThinUser>()
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

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
