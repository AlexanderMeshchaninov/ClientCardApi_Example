using Lessons.ClientCardApi.Abstraction.Jwt;
using Lessons.ClientCardApi.NuGet.JwtBearer.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lessons.ClientCardApi.NuGet.JwtBearer.Registrations
{
    public static class JwtGeneratorRegister
    {
        public static IServiceCollection RegisterJwtGenerator(this IServiceCollection services)
        {
            return services.AddTransient<IJwtGenerator, JwtGenerator>();
        }
    }
}