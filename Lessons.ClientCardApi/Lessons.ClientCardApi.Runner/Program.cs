using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Lessons.ClientCardApi.Abstraction.Data.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Formatting.Compact;
using System.Reflection;
using App.Metrics.AspNetCore;
using App.Metrics.Formatters.Prometheus;
using Microsoft.Extensions.DependencyInjection;

namespace Lessons.ClientCardApi.Runner
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext().WriteTo.Console()
                .WriteTo.File(new RenderedCompactJsonFormatter(), 
                    $"{Directory.GetCurrentDirectory()}/LOGS/logFile.ndjson")
                .CreateLogger();
            try
            {
                Log.Information("Client cards API starting up...");
                
                var host = CreateHostBuilder(args).Build();

                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    try
                    {
                        var userManager = services.GetRequiredService<UserManager<UserAuth>>();
                        
                        // var users = LoadMyAssembly(userManager);
                        // foreach (var user in users)
                        // {
                        //     await userManager.CreateAsync(user, "12345");
                        // }
                    }
                    catch (Exception ex)
                    {
                        var logger = services.GetRequiredService<ILogger<Program>>();
                        logger.LogError(ex, "An error occurred while seeding the database.");
                    }
                }

                await host.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Client cards API start-up failed...");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static List<UserAuth> LoadMyAssembly(UserManager<UserAuth> userManager)
        {
            Assembly assembly = Assembly.LoadFrom("C:/Windows/Microsoft.NET/assembly/GAC_MSIL/" +
                                                  "AuthDataSeeding/v4.0_1.0.0.0__55ec8cb30b476ceb/AuthDataSeeding.dll");

            var type = assembly.GetType("AuthDataSeeding.AuthDataSeeding.DataSeed");
            if (type is not null)
            {
                var method = type.GetMethod("InitializeData");
                var instance = Activator.CreateInstance(type);
                var result = (List<UserAuth>)method?.Invoke(instance, new object[] {userManager});
                
                return result;
            }
            return null;
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .UseMetricsWebTracking()
                .UseMetrics(op =>
                {
                    op.EndpointOptions = endpointOptions =>
                    {
                        endpointOptions.MetricsTextEndpointOutputFormatter = new MetricsPrometheusTextOutputFormatter();
                        endpointOptions.MetricsEndpointOutputFormatter = new MetricsPrometheusProtobufOutputFormatter();
                        endpointOptions.EnvironmentInfoEndpointEnabled = false;
                    };
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}