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

            services.AddAuthentication().AddGoogle(options => 
            {
                options.ClientId = "793755898736-dgat4m00gquv8af0pigoi244pbduh6o9.apps.googleusercontent.com";
                options.ClientSecret = "GT64NkddlIobE10r5q_G-VqR";
            }).AddMicrosoftAccount(options => 
            {
                options.ClientId = "8609190f-f8e1-4f7c-9221-e886225c5112";
                options.ClientSecret = "rstkGVPRZY103)ntyG60@!~";
            }).AddFacebook(options =>
            {
                options.AppId= "605402336518461";
                options.AppSecret = "3556b24e2713e5eb4e5aa4299a210583";
            }).AddTwitter(options => 
            {
                options.ConsumerKey = "Yee3aVzm3vgVHPprweD6QL5pm";
                options.ConsumerSecret = "LAZKYw2qGNfj401FscPMq7CdZIt5oK6o8vd7z6i3J5DqvXoMHn";
            });

            // configures mvc with a require https filter.
            services.AddMvc().AddMvcOptions(option => option.Filters.Add(new RequireHttpsAttribute()));
            services.AddSingleton<IConferenceService, ConferenceService>()
                    .AddSingleton<IProposalService, ProposalService>()
                    .AddSingleton<IAttendeeService, AttendeeService>()
                    .AddSingleton<IPurposeString, PurposeStringConstant>()
                    .AddSingleton<IEmailService, EmailService>()
                    .Configure<EmailSettingOptions>(this._configuration.GetSection("EmailSettings"))
                    .AddDbContext<ConfaqueDbContext>(options =>
                        options.UseSqlServer(this._configuration.GetConnectionString("ConfAqueConnection"),
                        sqlOptions => sqlOptions.MigrationsAssembly("Confaque")))
                    .AddIdentity<ConfaqueUser, IdentityRole>(option => option.Lockout.MaxFailedAccessAttempts = 5)
                    .AddEntityFrameworkStores<ConfaqueDbContext>()
                    .AddDefaultTokenProviders();

            services.AddScoped<UserManager<ConfaqueUser>>()
                    .AddScoped<SignInManager<ConfaqueUser>>()
                    .AddScoped<IdentityRole>()
                    .AddTransient<IUserClaimsPrincipalFactory<ConfaqueUser>, ConfaqueUserClaimsPrincipalFactory>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHsts(option => option.MaxAge(days: 10)) // Using http strict transport security protocol.
               //.UseCsp(option => option.DefaultSources(source => source.Self())) // Using content security protocol for anti xss.
               .UseXfo(option => option.Deny()) // Denying X-Frames to run on the site to prevent click-jacking.
               .UseStaticFiles()
               .UseStatusCodePages()
               .UseAuthentication()
               .UseMvc(route => route.MapRoute(name: "default", template: "{Controller=Conference}/{Action=Index}/{id?}"));
        }
    }
}
