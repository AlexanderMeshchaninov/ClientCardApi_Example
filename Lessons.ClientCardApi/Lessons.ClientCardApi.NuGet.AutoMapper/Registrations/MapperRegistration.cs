using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Lessons.ClientCardApi.NuGet.AutoMapper.Registrations
{
    public static class MapperRegistration
    {
        public static IServiceCollection RegisterMapper(this IServiceCollection services)
        {
            var mapperConfiguration = new MapperConfiguration(mp =>
                mp.AddProfile(new MapperProfile.MapperProfile()));
            var mapper = mapperConfiguration.CreateMapper();
            
            return services.AddSingleton(mapper);
        }
    }
}