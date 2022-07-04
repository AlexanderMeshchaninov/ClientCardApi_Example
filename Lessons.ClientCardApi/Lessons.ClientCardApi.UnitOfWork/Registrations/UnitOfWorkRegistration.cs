using Lessons.ClientCardApi.UnitOfWork.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Lessons.ClientCardApi.UnitOfWork.Registrations
{
    public static class UnitOfWorkRegistration
    {
        public static IServiceCollection RegisterUnitOfWork(this IServiceCollection services)
        {
            return services.AddTransient<IUnitOfWork, UnitOfWork>();
        }
    }
}