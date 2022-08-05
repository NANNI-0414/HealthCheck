using BlazerDemo.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
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
using BlazerDemo.HealthChecks;

namespace BlazerDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();

            // add start
            // Configure Health Checks
            services.AddHealthChecks()
                                /*
                                .AddCheck("Foo Service", () => 
                                    {
                                        // Do your checks
                                        // ...
                                        return HealthCheckResult.Degraded("The check of the foo service did not work well.");
                                    }, new[] {"service"}
                                    )
                                    //HealthCheckResult.Healthy("The check of the foo service worked."));
                                    //HealthCheckResult.Unhealthy("The check of the foo service did not work."));
                                .AddCheck("Bar Service", () =>
                                    HealthCheckResult.Healthy("The check of the bar service worked."), new[] { "service" })
                                */
                .AddCheck<ResponseTimeHealthCheck>("Network speed test", null, new[] { "service" })
                //.AddCheck<ResponseTimeHealthCheck>("Network speed test", null, new[] { "service" })
                //.AddCheck<ResponseTimeHealthCheck>("Network speed test", null, new[] { "service" })
                //.AddCheck<ResponseTimeHealthCheck>("Network speed test", null, new[] { "service" });

                .AddCheck("Database", () =>
                    HealthCheckResult.Healthy("The check of the database worked."), new[] { "database", "sql" });
                // add end

            services.AddSingleton<WeatherForecastService>();
            services.AddSingleton<ResponseTimeHealthCheck>();   // Add
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                // add start
                endpoints.MapHealthChecks("/quickhealth", new HealthCheckOptions()
                {
                    Predicate = _ => false
                });

                endpoints.MapHealthChecks("/health/services", new HealthCheckOptions()
                {
                    Predicate = reg => reg.Tags.Contains("service"),
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });

                endpoints.MapHealthChecks("/health", new HealthCheckOptions() 
                {
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });  
                // add end : http 주소창에 /health를 붙이면 새 페이지가 뜸
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
