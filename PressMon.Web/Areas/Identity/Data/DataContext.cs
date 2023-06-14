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
using TMS.Web.Models;

namespace PressMon.Web.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {        
        }

        public DbSet<LiveData> LiveDatas { get; set; }
        public DbSet<Alarm> Alarms { get; set; }
        public DbSet<Historical> Historicals { get; set; }
        public DbSet<AlarmSettings> AlarmSettings { get; set; }

    }
}
