using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Lessons.ClientCardApi.Abstraction.Facades;
using Lessons.ClientCardApi.Abstraction.Requests;
using Microsoft.Extensions.Logging;

namespace Lessons.ClientCardApi.CommonLogic.Facades.ValidationFacade
{
    public interface IValidatorFacade :
        IValidatorFacade<CreateRequest, ValidationResult>,
        IValidatorFacade<ReadRequest, ValidationResult>,
        IValidatorFacade<UpdateRequest, ValidationResult>,
        IValidatorFacade<DeleteRequest, ValidationResult>,
        IValidatorFacade<LoginRequest, ValidationResult>,
        IValidatorFacade<RegisterRequest, ValidationResult>
    {
    }

    public sealed class ValidatorFacade : IValidatorFacade
    {
        private readonly ILogger<ValidatorFacade> _logger;
        private readonly IValidator<CreateRequest> _createValidator;
        private readonly IValidator<ReadRequest> _readValidator;
        private readonly IValidator<UpdateRequest> _updateValidator;
        private readonly IValidator<DeleteRequest> _deleteValidator;
        private readonly IValidator<LoginRequest> _loginValidator;
        private readonly IValidator<RegisterRequest> _registerValidator;
        private CancellationTokenSource _cts;
        public ValidatorFacade(
            ILogger<ValidatorFacade> logger,
            IValidator<CreateRequest> createValidator,
            IValidator<ReadRequest> readValidator,
            IValidator<UpdateRequest> updateValidator,
            IValidator<DeleteRequest> deleteValidator,
            IValidator<LoginRequest> loginValidator,
            IValidator<RegisterRequest> registerValidator)
        {
            _createValidator = createValidator;
            _readValidator = readValidator;
            _updateValidator = updateValidator;
            _deleteValidator = deleteValidator;
            _loginValidator = loginValidator;
            _registerValidator = registerValidator;
            _logger = logger;
        }
        
        public async Task<ValidationResult> EnterValidatorFacadeAsync(CreateRequest request)
        {
            _cts = new CancellationTokenSource();
            _cts.CancelAfter(3500);
            var cancellationToken = _cts.Token;

            try
            {
                _logger.LogInformation($"Create request validation facade start - {DateTime.Now}");
                var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);

                _logger.LogInformation($"Read request validation facade stop - {DateTime.Now}");
                return validationResult;
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError($"{ex}");
                return null;
            }
            finally
            {
                _cts.Dispose();
            }
        }

        public async Task<ValidationResult> EnterValidatorFacadeAsync(ReadRequest request)
        {
            _cts = new CancellationTokenSource();
            _cts.CancelAfter(3500);
            var cancellationToken = _cts.Token;
            
            try
            {
                _logger.LogInformation($"Read request validation facade start - {DateTime.Now}");
                var validationResult = await _readValidator.ValidateAsync(request, cancellationToken);
            
                _logger.LogInformation($"Read request validation facade stop - {DateTime.Now}");
                return validationResult;
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError($"{ex}");
                return null;
            }
            finally
            {
                _cts.Dispose();
            }
        }

        public async Task<ValidationResult> EnterValidatorFacadeAsync(UpdateRequest request)
        {
            _cts = new CancellationTokenSource();
            _cts.CancelAfter(3500);
            var cancellationToken = _cts.Token;
            
            try
            {
                _logger.LogInformation($"Update request validation facade start - {DateTime.Now}");
                var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
            
                _logger.LogInformation($"Update request validation facade stop - {DateTime.Now}");
                return validationResult;
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError($"{ex}");
                return null;
            }
            finally
            {
                _cts.Dispose();
            }
        }

        public async Task<ValidationResult> EnterValidatorFacadeAsync(DeleteRequest request)
        {
            _cts = new CancellationTokenSource();
            _cts.CancelAfter(3500);
            var cancellationToken = _cts.Token;
            
            try
            {
                _logger.LogInformation($"Delete request validation facade start - {DateTime.Now}");
                var validationResult = await _deleteValidator.ValidateAsync(request, cancellationToken);
            
                _logger.LogInformation($"Delete request validation facade stop - {DateTime.Now}");
                return validationResult;
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError($"{ex}");
                return null;
            }
            finally
            {
                _cts.Dispose();
            }
        }

        public async Task<ValidationResult> EnterValidatorFacadeAsync(LoginRequest request)
        {
            _cts = new CancellationTokenSource();
            _cts.CancelAfter(3500);
            var cancellationToken = _cts.Token;
            
            try
            {
                _logger.LogInformation($"Login request validation facade start - {DateTime.Now}");
                var validationResult = await _loginValidator.ValidateAsync(request, cancellationToken);
            
                _logger.LogInformation($"Login request validation facade stop - {DateTime.Now}");
                return validationResult;
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError($"{ex}");
                return null;
            }
            finally
            {
                _cts.Dispose();
            }
        }

        public async Task<ValidationResult> EnterValidatorFacadeAsync(RegisterRequest request)
        {
            _cts = new CancellationTokenSource();
            _cts.CancelAfter(3500);
            var cancellationToken = _cts.Token;
            
            try
            {
                _logger.LogInformation($"Login request validation facade start - {DateTime.Now}");
                var validationResult = await _registerValidator.ValidateAsync(request, cancellationToken);
            
                _logger.LogInformation($"Login request validation facade stop - {DateTime.Now}");
                return validationResult;
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError($"{ex}");
                return null;
            }
            finally
            {
                _cts.Dispose();
            }
        }
    }
}