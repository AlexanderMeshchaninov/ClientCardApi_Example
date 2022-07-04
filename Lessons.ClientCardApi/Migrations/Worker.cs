using System.Threading;
using System.Threading.Tasks;
using Lessons.ClientCardApi.Data.Context.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Migrations
{
    public class Worker : IHostedService
    {
        private readonly IDbContextFactory<ApplicationContext> _applicationContext;
        private CancellationTokenSource _cts;
        public Worker(IDbContextFactory<ApplicationContext> applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            using (var dbContext = _applicationContext.CreateDbContext())
            {
                await dbContext.Database.MigrateAsync(_cts.Token);
            }

            await Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cts.Cancel();

            return Task.CompletedTask;
        }
    }
}