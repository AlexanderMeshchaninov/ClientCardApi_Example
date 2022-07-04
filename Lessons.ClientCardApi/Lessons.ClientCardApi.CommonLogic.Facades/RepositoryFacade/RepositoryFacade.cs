using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics;
using AutoMapper;
using Lessons.ClientCardApi.Abstraction.Data.Models;
using Lessons.ClientCardApi.Abstraction.DTO;
using Lessons.ClientCardApi.Abstraction.Facades;
using Lessons.ClientCardApi.Abstraction.Jwt;
using Lessons.ClientCardApi.Abstraction.Requests;
using Lessons.ClientCardApi.Abstraction.Responses;
using Lessons.ClientCardApi.Nuget.Metrics.Metrics;
using Lessons.ClientCardApi.UnitOfWork.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nest;
using CardInfoModel = Lessons.ClientCardApi.Abstraction.DTO.CardInfoModel;
using DeleteRequest = Lessons.ClientCardApi.Abstraction.Requests.DeleteRequest;

namespace Lessons.ClientCardApi.CommonLogic.Facades.RepositoryFacade
{
    public interface IRepositoryFacade : 
        IRepositoryFacade<CreateRequest, Task>, 
        IRepositoryFacade<ReadRequest, ResponseFromRepoDto>,
        IRepositoryFacade<UpdateRequest, Task>,
        IRepositoryFacade<DeleteRequest, Task>,
        IRepositoryFacade<LoginRequest, LoginResponse>,
        IRepositoryFacade<RegisterRequest, Task>
    {
    }

    public sealed class RepositoryFacade : IRepositoryFacade
    {
        private IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly ILogger<RepositoryFacade> _logger;
        private readonly UserManager<UserAuth> _userManager;
        private readonly SignInManager<UserAuth> _signInManager;
        private readonly IJwtGenerator _jwtGenerator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMetrics _metrics;
        private readonly IElasticClient _elasticClient;

        public RepositoryFacade(
            IMapper mapper,
            ILogger<RepositoryFacade> logger,
            IConfiguration configuration,
            UserManager<UserAuth> userManager,
            SignInManager<UserAuth> signInManager,
            IJwtGenerator jwtGenerator,
            IUnitOfWork unitOfWork,
            IMetrics metrics,
            IElasticClient elasticClient)
        {
            _mapper = mapper;
            _logger = logger;
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtGenerator = jwtGenerator;
            _unitOfWork = unitOfWork;
            _metrics = metrics;
            _elasticClient = elasticClient;
        }

        public async Task<Task> EnterRepositoryFacadeAsync(CreateRequest createRequest, CancellationToken cancellationToken)
        {
            try
            {
                var create = _mapper.Map<CreditCardInfoModel>(createRequest);
                
                var createIndex = new ElasticClientsModel()
                {
                    Id = Guid.NewGuid(),
                    FirstName = create.FirstName,
                    LastName = create.LastName,
                    Patronymic = create.Patronymic,
                    BirthDate = create.BirthDate,
                    PassportNumber = create.PassportNumber,
                    PhoneNumber = create.PhoneNumber,
                    Email = create.Email,
                    CreditCardNumber = create.CreditCardNumber
                };
                
                switch (_configuration.GetValue<string>("RepositoryOptions:Repository"))
                {
                    case "Npgsql":
                        var responseFromNpgsql = await _unitOfWork
                            .GetNpgsqlRepository()
                            .CreateAsync(create, cancellationToken);
                        if (responseFromNpgsql.Exception is not null)
                        {
                            return Task.FromException(responseFromNpgsql.Exception);
                        }
                        
                        _metrics.Measure.Counter.Increment(MetricsRegistry.CreatedDbNpgsqlConnectionsCounter);
                        //--->Индексирование в elastic<---
                        await _elasticClient.IndexDocumentAsync(createIndex, cancellationToken);
                        await _unitOfWork.SaveChangesAsync(cancellationToken);

                        return Task.CompletedTask;
                    case "Ef":
                        var responseFromEf = _unitOfWork
                            .GetEfRepository()
                            .CreateAsync(create, cancellationToken);
                        if (responseFromEf.Exception is not null)
                        {
                            return Task.FromException(responseFromEf.Exception);
                        }
                        
                        _metrics.Measure.Counter.Increment(MetricsRegistry.CreatedDbEfConnectionsCounter);
                        
                        //--->Индексирование в elastic<---
                        await _elasticClient.IndexDocumentAsync(createIndex, cancellationToken);
                        await _unitOfWork.SaveChangesAsync(cancellationToken);

                        return Task.CompletedTask;
                }
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError("Create repository facade took a long time...");
                return Task.FromException(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Create repository {ex}");
                return Task.FromException(ex);
            }
            
            return Task.CompletedTask;
        }
        
        public async Task<ResponseFromRepoDto> EnterRepositoryFacadeAsync(ReadRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var response = new ResponseFromRepoDto() { Content = new List<CardInfoModel>() };
                
                switch (_configuration.GetValue<string>("RepositoryOptions:Repository"))
                {
                    case "Npgsql" :
                        var responseFromNpgsql = await _unitOfWork
                                .GetNpgsqlRepository()
                                .ReadAsync(request, cancellationToken);
                        if (responseFromNpgsql is null)
                        {
                            return null;
                        }

                        foreach (var cardInfo in responseFromNpgsql)
                        {
                            response.Content.Add(new CardInfoModel()
                            {
                                Id = cardInfo.Id,
                                FirstName = cardInfo.FirstName,
                                LastName = cardInfo.LastName,
                                Patronymic = cardInfo.Patronymic,
                                BirthDate = cardInfo.BirthDate,
                                Email = cardInfo.Email,
                                PassportNumber = cardInfo.PassportNumber,
                                PhoneNumber = cardInfo.PhoneNumber,
                                CreditCardNumber = cardInfo.CreditCardNumber
                            });
                        }
                        _metrics.Measure.Counter.Increment(MetricsRegistry.CreatedDbNpgsqlConnectionsCounter);
                        return response;
                    
                    case "Ef":
                        var responseFromEf = await _unitOfWork
                            .GetEfRepository()
                            .ReadAsync(request, cancellationToken);
                        if (responseFromEf is null)
                        {
                            return null;
                        }
    
                        foreach (var cardInfo in responseFromEf)
                        {
                            response.Content.Add(new CardInfoModel()
                            {
                                Id = cardInfo.Id,
                                FirstName = cardInfo.FirstName,
                                LastName = cardInfo.LastName,
                                Patronymic = cardInfo.Patronymic,
                                BirthDate = cardInfo.BirthDate,
                                Email = cardInfo.Email,
                                PassportNumber = cardInfo.PassportNumber,
                                PhoneNumber = cardInfo.PhoneNumber,
                                CreditCardNumber = cardInfo.CreditCardNumber
                            });
                        }
                        _metrics.Measure.Counter.Increment(MetricsRegistry.CreatedDbEfConnectionsCounter);
                        return response;
                }
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError($"Read ef repository facade took a long time...{ex}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Read repository return null {ex}");
                return null;
            }
            
            return null;
        }
        
        public async Task<Task> EnterRepositoryFacadeAsync(UpdateRequest request, CancellationToken cancellationToken)
        {
            try
            {
                switch (_configuration.GetValue<string>("RepositoryOptions:Repository"))
                {
                    case "Npgsql" :
                        var responseFromNpgsql = await _unitOfWork
                            .GetNpgsqlRepository()
                            .UpdateAsync(request, cancellationToken);
                        if (responseFromNpgsql.Exception is not null)
                        {
                            return Task.FromException(responseFromNpgsql.Exception);
                        }

                        await _unitOfWork.SaveChangesAsync(cancellationToken);
                        _metrics.Measure.Counter.Increment(MetricsRegistry.CreatedDbNpgsqlConnectionsCounter);
                        return Task.CompletedTask;
                    
                    case "Ef" :
                        var responseFromEf = await _unitOfWork
                            .GetEfRepository()
                            .UpdateAsync(request, cancellationToken);
                        if (responseFromEf.Exception is not null)
                        {
                            return Task.FromException(responseFromEf.Exception);
                        }

                        await _unitOfWork.SaveChangesAsync(cancellationToken);
                        _metrics.Measure.Counter.Increment(MetricsRegistry.CreatedDbEfConnectionsCounter);
                        return Task.CompletedTask;
                }
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError($"Update ef repository facade took a long time...{ex}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Update repository facade stop with exception {ex} - {DateTime.Now}");
                return Task.FromException(ex);
            }
            
            return Task.CompletedTask;
        }

        public async Task<Task> EnterRepositoryFacadeAsync(DeleteRequest request, CancellationToken cancellationToken)
        {
            try
            {
                switch (_configuration.GetValue<string>("RepositoryOptions:Repository"))
                {
                    case "Npgsql" :
                        var responseFromNpgsql = await _unitOfWork
                            .GetNpgsqlRepository()
                            .DeleteAsync(request, cancellationToken);
                        if (responseFromNpgsql.Exception is not null)
                        {
                            return Task.FromException(responseFromNpgsql.Exception);
                        }

                        await _unitOfWork.SaveChangesAsync(cancellationToken);
                        _metrics.Measure.Counter.Increment(MetricsRegistry.CreatedDbNpgsqlConnectionsCounter);
                        return Task.CompletedTask;
                    
                    case "Ef" :
                        var responseFromEf = await _unitOfWork
                            .GetEfRepository()
                            .DeleteAsync(request, cancellationToken);
                        if (responseFromEf.Exception is not null)
                        {
                            return Task.FromException(responseFromEf.Exception);
                        }

                        await _unitOfWork.SaveChangesAsync(cancellationToken);
                        _metrics.Measure.Counter.Increment(MetricsRegistry.CreatedDbEfConnectionsCounter);
                        return Task.CompletedTask;
                }
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError($"Delete ef repository facade took a long time...{ex}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Delete repository facade stop with exception {ex} - {DateTime.Now}");
                return Task.FromException(ex);
            }
            
            return Task.CompletedTask;
        }

        public async Task<LoginResponse> EnterRepositoryFacadeAsync(LoginRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user is null)
                {
                    var ex = new NullReferenceException();
                    return null;
                }
                
                var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
                if (result.Succeeded)
                {
                    return new LoginResponse()
                    {
                        Token = _jwtGenerator.CreateTokenAsync(user),    
                        UserName = user.UserName,
                    };
                }
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError($"Login repository facade took a long time...{ex}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Login repository facade stop with exception {ex} - {DateTime.Now}");
                return null;
            }
            
            return null;
        }

        public async Task<Task> EnterRepositoryFacadeAsync(RegisterRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user is null)
                {
                    var identityUser = new UserAuth()
                    {
                        Email = request.Email,
                        UserName = request.UserName,
                        PasswordHash = string.Empty
                    };
                    
                    var userPasswordHash = _userManager.PasswordHasher.HashPassword(identityUser,$"{request.Password}");
                    identityUser.PasswordHash = userPasswordHash;
                    
                    var result = await _userManager.CreateAsync(identityUser);
                    if (result.Succeeded)
                    {
                        return Task.CompletedTask;
                    }
                }
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError($"Register repository facade took a long time...{ex}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Register repository facade stop with exception {ex} - {DateTime.Now}");
                return null;
            }
            
            return null;
        }
    }
}