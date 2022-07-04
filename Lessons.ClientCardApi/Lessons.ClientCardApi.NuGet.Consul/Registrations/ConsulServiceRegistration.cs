using Lessons.ClientCardApi.NuGet.Consul.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Lessons.ClientCardApi.NuGet.Consul.Registrations
{
    public static class ConsulServiceRegistration
    {
        public static IServiceCollection RegisterConsulService(this IServiceCollection services)
        {
            return services.AddSingleton<IHostedService, ConsulRegistryService>();
        }
    }
}