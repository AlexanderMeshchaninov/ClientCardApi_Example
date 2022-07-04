using System;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Lessons.ClientCardApi.NuGet.Consul.Configurations;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Lessons.ClientCardApi.NuGet.Consul.Service
{
    public sealed class ConsulRegistryService : IHostedService
    {
        private readonly IConsulClient _consulClient;
        private readonly ClientCardApiConfiguration _clientCardApiOptions;
        private readonly PostgresConfiguration _postgresOptions;
        private readonly ElasticConfiguration _elasticOptions;
        private readonly KibanaConfiguration _kibanaOptions;
        private readonly GrafanaConfiguration _grafanaOptions;
        private readonly PrometheusConfiguration _prometheusOptions;
        private readonly ILogger<ConsulRegistryService> _logger;
        private CancellationTokenSource _cts;
        public ConsulRegistryService(
            IConsulClient consulClient,
            IOptions<ClientCardApiConfiguration> clientCardApiOptions,
            IOptions<PostgresConfiguration> postgresOptions,
            IOptions<ElasticConfiguration> elasticOptions,
            IOptions<KibanaConfiguration> kibanaOptions,
            ILogger<ConsulRegistryService> logger,
            IOptions<GrafanaConfiguration> grafanaOptions,
            IOptions<PrometheusConfiguration> prometheusOptions)
        {
            _consulClient = consulClient;
            _clientCardApiOptions = clientCardApiOptions.Value;
            _postgresOptions = postgresOptions.Value;
            _elasticOptions = elasticOptions.Value;
            _kibanaOptions = kibanaOptions.Value;
            _grafanaOptions = grafanaOptions.Value;
            _prometheusOptions = prometheusOptions.Value;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            var apiUri = new Uri(_clientCardApiOptions.Url);
            var postgresUri = new Uri(_postgresOptions.Url);
            var elasticUri = new Uri(_elasticOptions.Url);
            var kibanaUri = new Uri(_kibanaOptions.Url);
            var grafanaUri = new Uri(_grafanaOptions.Url);
            var prometheusUri = new Uri(_prometheusOptions.Url);

            var apiRegistration = new AgentServiceRegistration()
            {
                Address = apiUri.Host,
                Name = _clientCardApiOptions.ServiceName,
                Port = apiUri.Port,
                ID = _clientCardApiOptions.ServiceId,
                Tags = new []{_clientCardApiOptions.ServiceName},
                //Опционально, можно настроить...
                // {
                //     HTTP = $"{apiUri.Scheme}://{apiUri.Host}:{apiUri.Port}/api/health/status",
                //     Timeout = TimeSpan.FromSeconds(10),
                //     Interval = TimeSpan.FromSeconds(10)
                // }
            };
            var postgresRegistration = new AgentServiceRegistration()
            {
                Address = postgresUri.Host,
                Name = _postgresOptions.ServiceName,
                Port = postgresUri.Port,
                ID = _postgresOptions.ServiceId,
                Tags = new []{_postgresOptions.ServiceName},
            };
            var elasticRegistration = new AgentServiceRegistration()
            {
                Address = elasticUri.Host,
                Name = _elasticOptions.ServiceName,
                Port = elasticUri.Port,
                ID = _elasticOptions.ServiceId,
                Tags = new []{_elasticOptions.ServiceName},
            };
            var kibanaRegistration = new AgentServiceRegistration()
            {
                Address = kibanaUri.Host,
                Name = _kibanaOptions.ServiceName,
                Port = kibanaUri.Port,
                ID = _kibanaOptions.ServiceId,
                Tags = new []{_kibanaOptions.ServiceName},
            };
            var grafanaRegistration = new AgentServiceRegistration()
            {
                Address = grafanaUri.Host,
                Name = _grafanaOptions.ServiceName,
                Port = grafanaUri.Port,
                ID = _grafanaOptions.ServiceId,
                Tags = new []{_grafanaOptions.ServiceName},
            };
            var prometheusRegistration = new AgentServiceRegistration()
            {
                Address = prometheusUri.Host,
                Name = _prometheusOptions.ServiceName,
                Port = prometheusUri.Port,
                ID = _prometheusOptions.ServiceId,
                Tags = new []{_prometheusOptions.ServiceName},
            };

            //Дерегистрация
            await _consulClient.Agent.ServiceDeregister(_clientCardApiOptions.ServiceId, _cts.Token);
            await _consulClient.Agent.ServiceDeregister(_postgresOptions.ServiceId, _cts.Token);
            await _consulClient.Agent.ServiceDeregister(_elasticOptions.ServiceId, _cts.Token);
            await _consulClient.Agent.ServiceDeregister(_kibanaOptions.ServiceId, _cts.Token);
            await _consulClient.Agent.ServiceDeregister(_grafanaOptions.ServiceId, _cts.Token);
            await _consulClient.Agent.ServiceDeregister(_prometheusOptions.ServiceId, _cts.Token);
            
            //Регистрация
            await _consulClient.Agent.ServiceRegister(apiRegistration, _cts.Token);
            await _consulClient.Agent.ServiceRegister(postgresRegistration, _cts.Token);
            await _consulClient.Agent.ServiceRegister(elasticRegistration, _cts.Token);
            await _consulClient.Agent.ServiceRegister(kibanaRegistration, _cts.Token);
            await _consulClient.Agent.ServiceRegister(grafanaRegistration, _cts.Token);
            await _consulClient.Agent.ServiceRegister(prometheusRegistration, _cts.Token);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _cts.Cancel();

            try
            {
                //Дерегистрация
                await _consulClient.Agent.ServiceDeregister(_clientCardApiOptions.ServiceId, cancellationToken);
                await _consulClient.Agent.ServiceDeregister(_postgresOptions.ServiceId, cancellationToken);
                await _consulClient.Agent.ServiceDeregister(_elasticOptions.ServiceId, cancellationToken);
                await _consulClient.Agent.ServiceDeregister(_kibanaOptions.ServiceId, cancellationToken);
                await _consulClient.Agent.ServiceDeregister(_grafanaOptions.ServiceId, cancellationToken);
                await _consulClient.Agent.ServiceDeregister(_prometheusOptions.ServiceId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex} when trying to deregister service");
            }
        }
    }
}