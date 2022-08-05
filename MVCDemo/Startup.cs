using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;    // 추가
using Microsoft.AspNetCore.Diagnostics.HealthChecks;    // 추가
using HealthChecks.UI.Client;                           // 추가

namespace MVCDemo
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
            services.AddControllersWithViews();
            // add start
            services.AddHealthChecks()
               .AddCheck("Foo Service", () =>
               {
                    // Do your checks
                    // ...
                    return HealthCheckResult.Degraded("The check of the foo service did not work well.");
               }, new[] { "service" }
                   )
               //HealthCheckResult.Healthy("The check of the foo service worked."));
               //HealthCheckResult.Unhealthy("The check of the foo service did not work."));
               .AddCheck("Bar Service", () =>
                   HealthCheckResult.Healthy("The check of the bar service worked."), new[] { "service" })
               .AddCheck("Database", () =>
                   HealthCheckResult.Healthy("The check of the database worked."), new[] { "database", "sql" });
            // add end

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                // add start
                endpoints.MapHealthChecks("/health", new HealthCheckOptions()
                {
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                // add end
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
