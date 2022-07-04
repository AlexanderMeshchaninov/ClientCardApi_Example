using Lessons.ClientCardApi.Abstraction.Data.ConnectionString;
using Lessons.ClientCardApi.Data.Context.ConnectionString;
using Lessons.ClientCardApi.Data.Context.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lessons.ClientCardApi.Data.Context.Registrations
{
    public static class PostgresRegistration
    {
        public static IServiceCollection RegisterPostgres(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddDbContext<ApplicationContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("PostgresSQLConnection"));
            }, ServiceLifetime.Transient, ServiceLifetime.Transient)
                .AddTransient<IDbConnection, NpgsqlConnection>();
        }
    }
}