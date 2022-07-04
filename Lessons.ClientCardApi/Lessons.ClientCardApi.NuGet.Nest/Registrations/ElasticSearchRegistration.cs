using System;
using Lessons.ClientCardApi.Abstraction.Data.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace Lessons.ClientCardApi.NuGet.Nest.Registrations
{
    public static class ElasticSearchRegistration
    {
        public static void RegisterElastic(this IServiceCollection services, IConfiguration configuration)
        {
            var url = configuration["ElasticConfiguration:Uri"];
            var defaultIndex = configuration["ElasticConfiguration:index"];

            var settings = new ConnectionSettings(new Uri(url))
                .PrettyJson()
                .DefaultIndex(defaultIndex);
            
            AddDefaultMappings(settings);

            var elasticClient = new ElasticClient(settings);
            
            services.AddSingleton<IElasticClient>(elasticClient);
            
            CreateIndex(elasticClient, defaultIndex);
        }

        private static void AddDefaultMappings(ConnectionSettings connectionSettings)
        {
            connectionSettings.DefaultMappingFor<ElasticClientsModel>(x =>
                        x.Ignore(x => x.Id)
                            .Ignore(x => x.CreditCardNumber)
                            .Ignore(x => x.BirthDate)
                            .Ignore(x => x.Patronymic)
                            .Ignore(x => x.FirstName));
        }

        private static void CreateIndex(IElasticClient elasticClient, string indexName)
        {
            elasticClient
                .Indices
                .Create(indexName, x =>
                    x.Map<ElasticClientsModel>(t =>
                        t.AutoMap()));
        }
    }
}