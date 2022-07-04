using System.Threading;
using System.Threading.Tasks;

namespace Lessons.ClientCardApi.Abstraction.Data.Repository
{
    public interface IDeleteRepository<TRequest> where TRequest : class
    {
        Task<Task> DeleteAsync(TRequest delete, CancellationToken cancellationToken);
    }
}