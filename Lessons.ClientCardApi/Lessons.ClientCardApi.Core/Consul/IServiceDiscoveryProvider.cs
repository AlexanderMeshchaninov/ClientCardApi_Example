using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lessons.ClientCardApi.Abstraction.Consul
{
    public interface IServiceDiscoveryProvider
    {
        Task<List<string>> GetServicesAsync();
    }
}