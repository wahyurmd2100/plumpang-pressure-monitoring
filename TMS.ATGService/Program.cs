using TMS.ATGService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;

public class Program
{
    public static void Main(string[] args)
    {
        IHost host = CreateHostBuilder(args).Build();
        CreateDatabaseIfNotExist(host);
        host.Run();
    }
    private static void CreateDatabaseIfNotExist(IHost host)
    {
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<AppDbContext>();
                context.Database.EnsureCreated();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
    public static IHostBuilder CreateHostBuilder(string[] args) =>
                Host.CreateDefaultBuilder(args)
                    .UseWindowsService(options => { options.ServiceName = "ATGService"; })
                    .ConfigureServices((hostContext, services) => {
                        IConfiguration configuration = hostContext.Configuration;
                        //get connection SQL Server
                        AppSetting.ConnectionString = configuration.GetConnectionString("DefaultConnection");
                        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
                        optionsBuilder.UseSqlServer(AppSetting.ConnectionString); 
                        services.AddScoped<AppDbContext>(db => new AppDbContext(optionsBuilder.Options));
                        //get Tank Configuration
                        tankConfiguration tankConfig = configuration.GetSection("TankConfiguration").Get<tankConfiguration>();
                        services.AddSingleton(tankConfig);
                        
                        services.AddHostedService<Worker>();
                    })
        .ConfigureLogging((context, logging) => { logging.AddConfiguration(context.Configuration.GetSection("Logging")); });
}
