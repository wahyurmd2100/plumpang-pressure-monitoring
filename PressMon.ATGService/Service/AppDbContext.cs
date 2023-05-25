using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PressMon.ATGService.Models;

namespace PressMon.ATGService
{
    public class AppDbContext :DbContext
    {
        public DbSet<Product> Master_Products { get; set; }
        public DbSet<Tank> Tank { get; set; }
        public DbSet<TankLiveData> Tank_Live_Data { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { 
        }
    }
}