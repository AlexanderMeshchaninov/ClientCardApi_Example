using Microsoft.AspNetCore.Identity;

namespace Lessons.ClientCardApi.Abstraction.Requests
{
    public sealed class RegisterRequest
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}