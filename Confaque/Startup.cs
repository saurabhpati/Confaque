using Confaque.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Confaque
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // configures mvc with a require https filter.
            services.AddMvc().AddMvcOptions(option => option.Filters.Add(new RequireHttpsAttribute()));
            services.AddSingleton<IConferenceService, ConferenceService>();
            services.AddSingleton<IProposalService, ProposalService>();
            services.AddSingleton<IAttendeeService, AttendeeService>();
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
               .UseMvc(route => route.MapRoute(name: "default", template: "{Controller=Conference}/{Action=Index}/{id?}"));
        }
    }
}
