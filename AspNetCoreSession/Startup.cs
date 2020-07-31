using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AspNetCoreSession
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
            services.AddDistributedSqlServerCache(options =>
                                                  {
                                                      options.ConnectionString = Configuration.GetConnectionString("DistCache_ConnectionString");
                                                      options.SchemaName       = "dbo";
                                                      options.TableName        = "CacheTable";

                                                      // options.DefaultSlidingExpiration  // 預設二十分鐘
                                                      // options.ExpiredItemsDeletionInterval // 預設三十分鐘後會刪除
                                                  });
            services.AddSession(options =>
                                {
                                    options.Cookie = new CookieBuilder
                                                     {
                                                         Name = "Test.Session"
                                                     };
                                    options.IdleTimeout = TimeSpan.FromMinutes(10);
                                });

            services.AddHttpContextAccessor();
            services.AddControllersWithViews();
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

            app.UseSession();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
                             {
                                 endpoints.MapControllerRoute(
                                                              name : "default",
                                                              pattern : "{controller=Home}/{action=Index}/{id?}");
                             });
        }
    }
}
