namespace Lessons.ClientCardApi.Abstraction.Responses
{
    public sealed class LoginResponse
    {
        public string UserName { get; set; }
        public string Token { get; set; }
    }
}