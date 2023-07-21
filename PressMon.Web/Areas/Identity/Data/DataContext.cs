using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PressMon.Web.Models;
using PressMon.Web.Models;
using TMS.Web.Apis;
using PressMon.Web.Areas.Identity.Data;
using TMS.Web.Models;

namespace PressMon.Web.Data
{
    public class DataContext : IdentityDbContext<AppUser>
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {        
        }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<AppRole> AppRoles { get; set; }
        public DbSet<UserRole> userRoles { get; set; }
        public DbSet<LiveData> LiveDatas { get; set; }
        public DbSet<Alarm> Alarms { get; set; }
        public DbSet<Historical> Historicals { get; set; }
        public DbSet<AlarmSettings> AlarmSettings { get; set; }

    }
}
