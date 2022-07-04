using System.Threading;
using System.Threading.Tasks;

namespace Lessons.ClientCardApi.Abstraction.Data.Repository
{
    public interface IUpdateRepository<TRequest> where TRequest : class
    {
        Task<Task> UpdateAsync(TRequest update, CancellationToken cancellationToken);
    }
}