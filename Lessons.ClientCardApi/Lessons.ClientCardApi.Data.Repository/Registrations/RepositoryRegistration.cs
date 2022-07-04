using Lessons.Lesson_1.Data.Repository.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Lessons.ClientCardApi.Data.Repository.Registrations
{
    public static class RepositoryRegistration
    {
        public static IServiceCollection RegisterRepository(this IServiceCollection services)
        {
            return services
                .AddTransient<IEfRepository, EfRepository>()
                .AddTransient<INpgsqlRepository, NpgsqlRepository>();
        }
    }
}