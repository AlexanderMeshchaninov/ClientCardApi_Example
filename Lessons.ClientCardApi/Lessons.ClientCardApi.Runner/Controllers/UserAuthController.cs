using System;
using System.Threading;
using System.Threading.Tasks;
using Lessons.ClientCardApi.Abstraction.Requests;
using Lessons.ClientCardApi.CommonLogic.Facades.RepositoryFacade;
using Lessons.ClientCardApi.CommonLogic.Facades.ValidationFacade;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Lessons.ClientCardApi.Runner.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public sealed class UserAuthController : Controller
    {
        private readonly ILogger<UserAuthController> _logger;
        private readonly IRepositoryFacade _repositoryFacade;
        private readonly IValidatorFacade _validatorFacade;
        private CancellationTokenSource _cts;
        public UserAuthController(
            ILogger<UserAuthController> logger,
            IRepositoryFacade repositoryFacade,
            IValidatorFacade validatorFacade)
        {
            _logger = logger;
            _repositoryFacade = repositoryFacade;
            _validatorFacade = validatorFacade;
        }
        
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest loginRequest)
        {
            _cts = new CancellationTokenSource();
            _cts.CancelAfter(4000);
            var cancellationToken = _cts.Token;
            
            var validationResult = await _validatorFacade.EnterValidatorFacadeAsync(loginRequest);
            if (!validationResult.IsValid)
            {
                return BadRequest($"This request is not valid");
            }

            try
            {
                var responseFromFacade = await _repositoryFacade.EnterRepositoryFacadeAsync(loginRequest, cancellationToken);
                if (responseFromFacade is null)
                {
                    return BadRequest("Invalid email or password");
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
                _logger.LogError($"Login user stop with exception {ex} : {DateTimeOffset.Now}");
                return BadRequest("Something goes wrong");
            }
            finally
            {
                _cts.Dispose();
            }
        }
        
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest registerRequest)
        {
            _cts = new CancellationTokenSource();
            _cts.CancelAfter(4000);
            var cancellationToken = _cts.Token;

            var validationResult = await _validatorFacade.EnterValidatorFacadeAsync(registerRequest);
            if (!validationResult.IsValid)
            {
                return BadRequest($"This request is not valid");
            }
            
            try
            {
                var responseFromFacade = await _repositoryFacade.EnterRepositoryFacadeAsync(registerRequest, cancellationToken);
                if (responseFromFacade.Exception is not null)
                {
                    return BadRequest("Invalid email or password");
                }
                
                return Ok("User has been registered successfully");
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError($"{ex}");
                return BadRequest("This request took a long time...");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Login user stop with exception {ex} : {DateTimeOffset.Now}");
                return BadRequest("Something goes wrong");
            }
            finally
            {
                _cts.Dispose();
            }
        }
    }
}