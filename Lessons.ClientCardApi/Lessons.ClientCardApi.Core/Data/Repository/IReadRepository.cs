using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lessons.ClientCardApi.Abstraction.Data.Repository
{
    public interface IReadRepository<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
    {
        Task<IReadOnlyList<TResponse>> ReadAsync(TRequest read, CancellationToken cancellationToken);
    }
}