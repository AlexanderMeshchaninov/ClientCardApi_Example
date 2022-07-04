using System.Threading;
using System.Threading.Tasks;

namespace Lessons.ClientCardApi.Abstraction.Facades
{
    public interface IRepositoryFacade<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
    
    {
        Task<TResponse> EnterRepositoryFacadeAsync(TRequest request, CancellationToken cancellationToken);
    }
}