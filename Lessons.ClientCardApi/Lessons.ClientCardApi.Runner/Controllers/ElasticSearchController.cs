using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lessons.ClientCardApi.Abstraction.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nest;

namespace Lessons.ClientCardApi.Runner.Controllers
{
    [Route("api/elasticsearch")]
    [ApiController]
    public sealed class ElasticSearchController : ControllerBase
    {
        private readonly ILogger<ElasticSearchController> _logger;
        private readonly IElasticClient _elasticClient;
        private CancellationTokenSource _cts;
        public ElasticSearchController(
            ILogger<ElasticSearchController> logger,
            IElasticClient elasticClient)
        {
            _logger = logger;
            _elasticClient = elasticClient;
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("read")]
        public async Task<IActionResult> ReadByKeywordAsync(string keyword)
        {
            _cts = new CancellationTokenSource();
            _cts.CancelAfter(7000);
            var cancellationToken = _cts.Token;

            try
            {
                var result = await _elasticClient
                    .SearchAsync<ElasticClientsModel>(q =>
                        q.Query(c =>
                            c.QueryString(x =>
                                x.Query('*' + keyword + '*')
                            )
                        ).Size(1000), cancellationToken);

                return Ok(result.Documents.ToList());
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
    }
}