using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PressMon.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using TMS.Web.Hubs;
using PressMon.Web.Areas.Identity.Data;

namespace PressMon.Web
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
            services.AddDbContext<DataContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("ContextConnection")));
            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
                options.SignIn.RequireConfirmedAccount = false;
            }).AddEntityFrameworkStores<DataContext>()
            .AddDefaultUI()
            .AddRoleManager<RoleManager<AppRole>>()
            .AddDefaultTokenProviders();
            // Add authorization services Admin
            services.AddControllersWithViews();
            services.AddAuthorization();
            services.AddSignalR();
   
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
            }
            app.UseRouting();
            app.UseAuthorization();
            app.UseAuthentication();

            

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<HomeHub>("/homehub");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
