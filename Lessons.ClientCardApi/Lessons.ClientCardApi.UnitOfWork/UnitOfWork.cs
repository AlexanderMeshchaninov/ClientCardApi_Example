using System.Threading;
using System.Threading.Tasks;
using Lessons.ClientCardApi.Abstraction.Data.ConnectionString;
using Lessons.ClientCardApi.Data.Context.Context;
using Lessons.ClientCardApi.UnitOfWork.Interfaces;
using Lessons.Lesson_1.Data.Repository.Repositories;
using Microsoft.Extensions.Logging;

namespace Lessons.ClientCardApi.UnitOfWork
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationContext _context;
        private readonly IDbConnection _dbConnection;
        private IEfRepository _efRepository;
        private INpgsqlRepository _npgsqlRepository;
        private readonly ILogger<EfRepository> _efLogger;
        private readonly ILogger<NpgsqlRepository> _npgsqlLogger;
        private bool disposed;

        public UnitOfWork(
            ApplicationContext context, 
            IDbConnection dbConnection, 
            ILogger<EfRepository> efLogger, 
            ILogger<NpgsqlRepository> npgsqlLogger)
        {
            _context = context;
            _dbConnection = dbConnection;
            _efLogger = efLogger;
            _npgsqlLogger = npgsqlLogger;
        }
        
        public void Dispose()
        {
            _context?.Dispose();
        }
        
        public void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                disposed = true;
            }
        }

        public IEfRepository GetEfRepository()
        {
            return _efRepository ?? (_efRepository = new EfRepository(_efLogger, _context));
        }

        public INpgsqlRepository GetNpgsqlRepository()
        {
            return _npgsqlRepository ?? (_npgsqlRepository = new NpgsqlRepository(_npgsqlLogger, _dbConnection));
        }

        public async Task<Task> SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
            return Task.CompletedTask;
        }
    }
}