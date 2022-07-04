namespace Lessons.ClientCardApi.Abstraction.Consul
{
    public abstract class ServiceBaseConfig
    {
        public string Url { get; set; }
        public string ServiceName { get; set; }
        public string ServiceId { get; set; }
    }
}