using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Counter;
using FluentValidation.Results;
using Lessons.ClientCardApi.Abstraction.DTO;
using Lessons.ClientCardApi.Abstraction.Requests;
using Lessons.ClientCardApi.CommonLogic.Facades.RepositoryFacade;
using Lessons.ClientCardApi.CommonLogic.Facades.ValidationFacade;
using Lessons.ClientCardApi.Runner.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using DeleteRequest = Lessons.ClientCardApi.Abstraction.Requests.DeleteRequest;

namespace WebApiTests
{
    public class CreditCardControllerTests
    {
        private readonly CreditCardController _creditCardController;
        private readonly Mock<ILogger<CreditCardController>> _loggerMock;
        private readonly Mock<ILogger<ElasticSearchController>> _loggerElasticMock;
        private readonly Mock<IRepositoryFacade> _repositoryFacadeMock;
        private readonly Mock<IValidatorFacade> _validatorFacadeMock;
        private readonly Mock<IMetrics> _metricsMock;

        public CreditCardControllerTests()
        {
            _loggerMock = new Mock<ILogger<CreditCardController>>();
            _loggerElasticMock = new Mock<ILogger<ElasticSearchController>>();
            _repositoryFacadeMock = new Mock<IRepositoryFacade>();
            _validatorFacadeMock = new Mock<IValidatorFacade>();
            _metricsMock = new Mock<IMetrics>();

            _creditCardController = new CreditCardController(
                _loggerMock.Object, 
                _repositoryFacadeMock.Object,
                _validatorFacadeMock.Object, 
                _metricsMock.Object);
        }

        [Fact]
        public async Task AddClientInfo_ReturnsOkResult()
        {
            //Arrange
            var createRequest = new CreateRequest()
            {
                FirstName = "Test01",
                LastName = "Testovsky01",
                Patronymic = "Testovich01",
                Email = "test01@mail.ru",
                BirthDate = Convert.ToDateTime("1988-04-04"),
                PassportNumber = 2655873666,
                PhoneNumber = 89459956565,
                CreditCardNumber = 8745231412346789,
            };

            _metricsMock.Setup(x => x.Measure.Counter.Increment(It.IsAny<CounterOptions>()));
            
            _validatorFacadeMock.Setup(x =>
                    x.EnterValidatorFacadeAsync(It.IsAny<CreateRequest>()))
                .ReturnsAsync(new ValidationResult());

            _repositoryFacadeMock.Setup(x => x.EnterRepositoryFacadeAsync(
                createRequest, 
                CancellationToken.None));
            
            // Act
            var okResult = await _creditCardController.AddClientInfoAsync(createRequest);
            
            // Assert
            Assert.IsType<OkObjectResult>(okResult as OkObjectResult);
        }
        
        [Fact]
        public async Task ReadClientInfoByParametersAsync_ReturnsNotFound()
        {
            //Arrange
            var readRequest = new ReadRequest() 
            {
                Email = "alex@mail.ru",
                Page = 1,
                Size = 2
            };
            
            //Mock setup
            _validatorFacadeMock.Setup(x =>
                    x.EnterValidatorFacadeAsync(It.IsAny<ReadRequest>()))
                .ReturnsAsync(new ValidationResult());
            
            _repositoryFacadeMock.Setup(x => x.EnterRepositoryFacadeAsync(
                readRequest, CancellationToken.None))
                .ReturnsAsync(new ResponseFromRepoDto()
                {
                    Content = new List<CardInfoModel>()
                });
            
            // Act
            var notFoundResult = await _creditCardController.ReadClientInfoByParametersAsync(
                readRequest.Email,
                readRequest.Page,
                readRequest.Size);
            
            var okResult = notFoundResult as NotFoundObjectResult;

            //Assert
            Assert.NotNull(okResult);
            Assert.Equal(404, okResult.StatusCode);
        }
        
        [Fact]
        public async Task UpdateClientInfoByIdAsync_ReturnsOkResult()
        {
            //Arrange
            var updateRequest = new UpdateRequest() 
            {
                SearchId = 1,
                FirstName = "Test01",
                LastName = "Testovsky01",
                Patronymic = "Testovich01",
                Email = "test01@mail.ru",
                BirthDate = Convert.ToDateTime("1988-04-04"),
                PassportNumber = 2655873666,
                PhoneNumber = 89459956565,
                CreditCardNumber = 8745231412346789,
            };
            
            //Mock setup
            _validatorFacadeMock.Setup(x =>
                    x.EnterValidatorFacadeAsync(It.IsAny<UpdateRequest>()))
                .ReturnsAsync(new ValidationResult());

            _repositoryFacadeMock.Setup(x => x.EnterRepositoryFacadeAsync(
                    updateRequest, CancellationToken.None))
                .ReturnsAsync(Task.CompletedTask);

            // Act
            var okResult = await _creditCardController.UpdateClientInfoByIdAsync(updateRequest);
            
            //Assert
            Assert.IsType<OkObjectResult>(okResult as OkObjectResult);
        }
        
        [Fact]
        public async Task DeleteClientInfoAsync_ReturnsOkResult()
        {
            //Arrange
            var deleteRequest = new DeleteRequest() 
            {
                Id = 1,
            };
            
            //Mock setup
            _validatorFacadeMock.Setup(x =>
                    x.EnterValidatorFacadeAsync(It.IsAny<DeleteRequest>()))
                .ReturnsAsync(new ValidationResult());

            _repositoryFacadeMock.Setup(x => x.EnterRepositoryFacadeAsync(
                    deleteRequest, CancellationToken.None))
                .ReturnsAsync(Task.CompletedTask);

            // Act
            var okResult = await _creditCardController.DeleteClientInfoAsync(deleteRequest);
            
            //Assert
            Assert.IsType<OkObjectResult>(okResult as OkObjectResult);
        }
    }
}