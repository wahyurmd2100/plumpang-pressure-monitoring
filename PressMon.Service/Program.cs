using PressMon.Service;
using System;

public class Program
{
    public static void Main(string[] args)
    {
        IHost host = CreateHostBuilder(args).Build();
        host.Run();
    }
  
    public static IHostBuilder CreateHostBuilder(string[] args) =>
                Host.CreateDefaultBuilder(args)
                   
                    .ConfigureServices((hostContext, services) => {
                        IConfiguration configuration = hostContext.Configuration;
                        string serverAddress = configuration.GetConnectionString("ServerAddress");
                        SensorConfig tankConfig = configuration.GetSection("SensorConfig").Get<SensorConfig>();
                        services.AddSingleton(tankConfig);
                        services.AddSingleton(serverAddress);
                        services.AddHostedService<Worker>();
                    })
        .ConfigureLogging((context, logging) => { logging.AddConfiguration(context.Configuration.GetSection("Logging")); });
}