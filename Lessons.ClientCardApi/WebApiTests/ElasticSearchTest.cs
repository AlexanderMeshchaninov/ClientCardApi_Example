using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lessons.ClientCardApi.Abstraction.Data.Models;
using Lessons.ClientCardApi.Runner.Controllers;
using Microsoft.Extensions.Logging;
using Moq;
using Nest;
using Xunit;

namespace WebApiTests
{
    public class ElasticSearchTest
    {
        [Fact]
        public Task ElasticSearch_ReturnsEqualTwoResult()
        {
            //Arrange
            var clients = new List<ElasticClientsModel>
            {
                new ElasticClientsModel() { FirstName = "Alex"},
                new ElasticClientsModel() { FirstName = "Vasiliy" },
            };
            
            //Mock setup
            var mockSearchResponse = new Mock<ISearchResponse<ElasticClientsModel>>();
            
            mockSearchResponse.Setup(x => x.Documents).Returns(clients);
            
            var mockElasticClient = new Mock<IElasticClient>();

            mockElasticClient
                .Setup(x => x.Search(It.IsAny<Func<SearchDescriptor<ElasticClientsModel>, ISearchRequest>>()))
                .Returns(mockSearchResponse.Object);
            
            var result = mockElasticClient.Object.Search<ElasticClientsModel>(s => s);
            
            // Assert
            Assert.Equal(2, result.Documents.Count);

            return Task.CompletedTask;
        }
    }
}