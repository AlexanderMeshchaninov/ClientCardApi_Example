using System;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics;
using Lessons.ClientCardApi.Abstraction.Requests;
using Lessons.ClientCardApi.CommonLogic.Facades.RepositoryFacade;
using Lessons.ClientCardApi.CommonLogic.Facades.ValidationFacade;
using Lessons.ClientCardApi.Nuget.Metrics.Metrics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DeleteRequest = Lessons.ClientCardApi.Abstraction.Requests.DeleteRequest;

namespace Lessons.ClientCardApi.Runner.Controllers
{
    [Route("api/card")]
    [ApiController]
    public sealed class CreditCardController : Controller
    {
        private readonly ILogger<CreditCardController> _logger;
        private readonly IRepositoryFacade _repositoryFacade;
        private readonly IValidatorFacade _validatorFacade;
        private readonly IMetrics _metrics;
        private CancellationTokenSource _cts;
        public CreditCardController(
            ILogger<CreditCardController> logger,
            IRepositoryFacade repositoryFacade,
            IValidatorFacade validatorFacade,
            IMetrics metrics)
        {
            _logger = logger;
            _repositoryFacade = repositoryFacade;
            _validatorFacade = validatorFacade;
            _metrics = metrics;
        }
        
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("add")]
        public async Task<IActionResult> AddClientInfoAsync([FromBody] CreateRequest createRequest)
        {
            _cts = new CancellationTokenSource();
            _cts.CancelAfter(7000);
            var cancellationToken = _cts.Token;

            var validationResult = await _validatorFacade.EnterValidatorFacadeAsync(createRequest);
            if (!validationResult.IsValid)
            {
                return BadRequest($"This request is not valid");
            }

            try
            {
                var responseFromFacade = await _repositoryFacade.EnterRepositoryFacadeAsync(createRequest, cancellationToken);
                if (responseFromFacade.Exception is not null)
                {
                    return BadRequest(
                        "Client card hasn't been added because of incorrect information or information has duplicates");
                }
                
                return Ok("Client card has been added successfully");
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError($"{ex}");
                return BadRequest("This request took a long time...");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Add client card info stop with exception {ex} : {DateTimeOffset.Now}");
                return BadRequest("Something goes wrong");
            }
            finally
            {
                //Собираем информацию о зарегистрированных пользователях
                _metrics.Measure.Counter.Increment(MetricsRegistry.CreatedClientsCounter);
                _cts.Dispose();
            }
        }
        
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("read")]
        public async Task<IActionResult> ReadClientInfoByParametersAsync(string email, int page, int size)
        {
            _cts = new CancellationTokenSource();
            _cts.CancelAfter(4000);
            var cancellationToken = _cts.Token;

            var readRequest = new ReadRequest()
            {
                Email = email,
                Page = page,
                Size = size
            };
            
            var validationResult = await _validatorFacade.EnterValidatorFacadeAsync(readRequest);
            if (!validationResult.IsValid)
            {
                return BadRequest($"This request is not valid");
            }
            
            try
            {
                var responseFromFacade = await _repositoryFacade.EnterRepositoryFacadeAsync(readRequest, cancellationToken);
                if (responseFromFacade is null)
                {
                    return NotFound("Client card info doesn't exists");
                }
                
                return Ok(responseFromFacade);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError($"{ex}");
                return BadRequest("This request took a long time...");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Read client card info stop with exception {ex} : {DateTimeOffset.Now}");
                return BadRequest("Something goes wrong");
            }
            finally
            {
                _cts.Dispose();
            }
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateClientInfoByIdAsync([FromBody] UpdateRequest updateRequest)
        {
            _cts = new CancellationTokenSource();
            _cts.CancelAfter(4000);
            var cancellationToken = _cts.Token;

            var validationResult = await _validatorFacade.EnterValidatorFacadeAsync(updateRequest);
            if (!validationResult.IsValid)
            {
                return BadRequest($"This request is not valid");
            }
            
            try
            {
                var responseFromFacade = await _repositoryFacade.EnterRepositoryFacadeAsync(updateRequest, cancellationToken);
                if (responseFromFacade.Exception is not null)
                {
                    return NotFound("Client card info doesn't exists or request has duplicates");
                }
                
                return Ok("Client card info has been updated successfully");
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError($"{ex}");
                return BadRequest("This request took a long time...");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Update client card info stop with exception {ex} : {DateTimeOffset.Now}");
                return BadRequest("Something goes wrong");
            }
            finally
            {
                _cts.Dispose();
            }
        }
        
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteClientInfoAsync([FromBody] DeleteRequest deleteRequest)
        {
            _cts = new CancellationTokenSource();
            _cts.CancelAfter(4000);
            var cancellationToken = _cts.Token;
            
            var validationResult = await _validatorFacade.EnterValidatorFacadeAsync(deleteRequest);
            if (!validationResult.IsValid)
            {
                return BadRequest($"This request is not valid");
            }
            
            try
            {
                var responseFromFacade = await _repositoryFacade.EnterRepositoryFacadeAsync(deleteRequest, cancellationToken);
                if (responseFromFacade.Exception is not null)
                {
                    return NotFound("Client card info doesn't exists");
                }
                
                return Ok("Client card info has been deleted successfully");
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError($"{ex}");
                return BadRequest("This request took a long time...");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Delete client card info stop with exception {ex} : {DateTimeOffset.Now}");
                return BadRequest("Something goes wrong");
            }
            finally
            {
                _cts.Dispose();
            }
        }
    }
}