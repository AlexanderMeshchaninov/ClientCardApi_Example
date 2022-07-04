using Lessons.ClientCardApi.Data.AuthContext.AuthDbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AuthMigrations
{
    class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, service) =>
                {
                    service.AddDbContextFactory<AuthContext>(opt =>
                    {
                        opt.UseNpgsql(hostContext.Configuration.GetConnectionString("AuthPostgresSQLConnection"),
                                x => x.MigrationsAssembly(nameof(AuthMigrations)))
                            .UseLoggerFactory(LoggerFactory.Create(builder =>
                            {
                                builder.AddConsole(_ => { })
                                    .AddFilter((category, level) =>
                                        category == DbLoggerCategory.Database.Command.Name &&
                                        level == LogLevel.Information);
                            }));
                    });
                    service.AddHostedService<AuthWorker>();
                });
        }
    }
}