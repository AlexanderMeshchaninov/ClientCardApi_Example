namespace Lessons.ClientCardApi.Abstraction.Requests
{
    public sealed class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}