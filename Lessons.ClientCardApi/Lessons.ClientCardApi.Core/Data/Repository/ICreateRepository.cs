using System.Threading;
using System.Threading.Tasks;

namespace Lessons.ClientCardApi.Abstraction.Data.Repository
{
    public interface ICreateRepository<TRequest>
    where TRequest : class
    {
        Task<Task> CreateAsync(TRequest create, CancellationToken cancellationToken);
    }
}