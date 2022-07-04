
namespace Lessons.ClientCardApi.Abstraction.Requests
{
    public sealed class ReadRequest
    {
        public string Email { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }
    }
}