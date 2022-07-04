using Lessons.ClientCardApi.CommonLogic.Facades.RepositoryFacade;
using Microsoft.Extensions.DependencyInjection;

namespace Lessons.ClientCardApi.CommonLogic.Facades.Registrations
{
    public static class RepositoryFacadeRegistration
    {
        public static IServiceCollection RegisterRepositoryFacade(this IServiceCollection services)
        {
            return services.AddTransient<IRepositoryFacade, RepositoryFacade.RepositoryFacade>();
        }
    }
}