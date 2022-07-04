using App.Metrics;
using App.Metrics.Counter;

namespace Lessons.ClientCardApi.Nuget.Metrics.Metrics
{
    public sealed class MetricsRegistry
    {
        public static CounterOptions CreatedClientsCounter => new CounterOptions()
        {
            Name = "Created clients",
            Context = "ClientsApi",
            MeasurementUnit = Unit.Calls
        };
        
        public static CounterOptions CreatedDbEfConnectionsCounter => new CounterOptions()
        {
            Name = "Created Database EF Connections",
            Context = "ClientsApi",
            MeasurementUnit = Unit.Calls
        };
        
        public static CounterOptions CreatedDbNpgsqlConnectionsCounter => new CounterOptions()
        {
            Name = "Created Database NPGSQL Connections",
            Context = "ClientsApi",
            MeasurementUnit = Unit.Calls
        };
    }
}