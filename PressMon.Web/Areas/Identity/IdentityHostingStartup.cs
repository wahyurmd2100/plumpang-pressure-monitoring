using System;
using PressMon.Web.Areas.Identity.Data;
using PressMon.Web.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(PressMon.Web.Areas.Identity.IdentityHostingStartup))]
namespace PressMon.Web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            //builder.ConfigureServices((context, services) => {
            //    services.AddDbContext<CTMContext>(options =>
            //        options.UseSqlServer(
            //            context.Configuration.GetConnectionString("CTMWebContextConnection")));

            //    services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //        .AddEntityFrameworkStores<CTMContext>();
            //});
        }
    }
}