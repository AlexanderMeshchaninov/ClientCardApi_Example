using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lessons.ClientCardApi.Data.AuthContext.Registrations
{
    public static class PostgresAuthRegistration
    {
        public static IServiceCollection RegisterAuthPostgres(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddDbContext<AuthDbContext.AuthContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("AuthPostgresSQLConnection"));
            }, ServiceLifetime.Transient, ServiceLifetime.Transient);
        }
    }
}