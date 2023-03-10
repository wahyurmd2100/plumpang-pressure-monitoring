using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TMS.Models;

namespace TMS.Service
{
    public class AppDbContext :DbContext
    {
        public DbSet<Product> Master_Products { get; set; }
        public DbSet<Tank> Tank { get; set; }
        public DbSet<TankLiveData> Tank_Live_Data { get; set; }
        public DbSet<TankHistorical> Tank_Historical { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { 
        }
    }
}