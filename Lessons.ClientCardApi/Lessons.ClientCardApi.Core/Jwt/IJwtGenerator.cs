using System.Threading.Tasks;
using Lessons.ClientCardApi.Abstraction.Data.Models;

namespace Lessons.ClientCardApi.Abstraction.Jwt
{
    public interface IJwtGenerator
    {
        string CreateTokenAsync(UserAuth user);
    }
}