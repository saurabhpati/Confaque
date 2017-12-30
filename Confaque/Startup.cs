using System.IO;
using Confaque.Data;
using Confaque.Provider;
using Confaque.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Confaque
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            this._configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(setup => setup.AddPolicy("AllowConfaque", policy => policy.WithOrigins("https://localhost:44311")))
                    .AddDataProtection();
            
            // configures mvc with a require https filter.
            services.AddMvc().AddMvcOptions(option => option.Filters.Add(new RequireHttpsAttribute()));
            services.AddSingleton<IConferenceService, ConferenceService>()
                    .AddSingleton<IProposalService, ProposalService>()
                    .AddSingleton<IAttendeeService, AttendeeService>()
                    .AddSingleton<IPurposeString, PurposeStringConstant>()
                    .AddDbContext<ConfaqueDbContext>(options =>
                        options.UseSqlServer(this._configuration.GetConnectionString("ConfAqueConnection"),
                        sqlOptions => sqlOptions.MigrationsAssembly("Confaque")))
                        .AddIdentity<ConfaqueUser, IdentityRole>()
                    .AddEntityFrameworkStores<ConfaqueDbContext>();

            services.AddScoped<UserManager<ConfaqueUser>>()
                    .AddScoped<SignInManager<ConfaqueUser>>()
                    .AddScoped<IdentityRole>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHsts(option => option.MaxAge(days: 10)) // Using http strict transport security protocol.
               .UseCsp(option => option.DefaultSources(source => source.Self())) // Using content security protocol for anti xss.
               .UseXfo(option => option.Deny()) // Denying X-Frames to run on the site to prevent click-jacking.
               .UseStaticFiles()
               .UseStatusCodePages()
               .UseAuthentication()
               .UseMvc(route => route.MapRoute(name: "default", template: "{Controller=Conference}/{Action=Index}/{id?}"));
        }
    }
}
