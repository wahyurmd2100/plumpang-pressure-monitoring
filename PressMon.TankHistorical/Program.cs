using Microsoft.EntityFrameworkCore;
using PressMon.Historical;
using PressMon.Service;
using PressMon.Settings;
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
                    .UseWindowsService(options => { options.ServiceName = "TMSHistorical"; })
                    .ConfigureServices((hostContext, services) => {
                        IConfiguration configuration = hostContext.Configuration;
                        //get connection SQL Server
                        AppSetting.ConnectionString = configuration.GetConnectionString("DefaultConnection");
                        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
                        optionsBuilder.UseSqlServer(AppSetting.ConnectionString);
                        services.AddScoped<AppDbContext>(db => new AppDbContext(optionsBuilder.Options));
                        //historical configuration
                        HistoricalConfiguration config = new HistoricalConfiguration();
                        config = configuration.GetSection("HistoricalConfiguration").Get<HistoricalConfiguration>();
                        services.AddSingleton(config);
                        services.AddHostedService<Worker>();
                    })
        .ConfigureLogging((context, logging) => { logging.AddConfiguration(context.Configuration.GetSection("Logging")); });
}
