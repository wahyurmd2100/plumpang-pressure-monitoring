using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSL.Web.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CSL.Web.Models;
using TMS.Web.Models;

namespace CSL.Web.Data
{
    public class TMSContext : IdentityDbContext<AppUser>
    {
        public TMSContext(DbContextOptions<TMSContext> options)
            : base(options)
        {        }

       

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Product> Master_Products { get; set; }
        public DbSet<Tank> Tank { get; set; }
        public DbSet<TankHistorical> Tank_Historical { get; set; }
        public DbSet<TankLiveData> Tank_Live_Data { get; set; }
        public DbSet<TankTicket> Tank_Ticket { get; set; }

    }
}
