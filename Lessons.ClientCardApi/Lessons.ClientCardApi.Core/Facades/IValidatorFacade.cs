using System.Threading;
using System.Threading.Tasks;

namespace Lessons.ClientCardApi.Abstraction.Facades
{
    public interface IValidatorFacade<TRequest, TResponse> 
        where TRequest : class
        where TResponse : class
    {
        Task<TResponse> EnterValidatorFacadeAsync(TRequest request);
    }
}