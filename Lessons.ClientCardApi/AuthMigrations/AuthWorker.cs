using System.Threading;
using System.Threading.Tasks;
using Lessons.ClientCardApi.Data.AuthContext.AuthDbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace AuthMigrations
{
    public class AuthWorker : IHostedService
    {
        private readonly IDbContextFactory<AuthContext> _applicationContext;
        private CancellationTokenSource _cts;
        public AuthWorker(IDbContextFactory<AuthContext> applicationContext)
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