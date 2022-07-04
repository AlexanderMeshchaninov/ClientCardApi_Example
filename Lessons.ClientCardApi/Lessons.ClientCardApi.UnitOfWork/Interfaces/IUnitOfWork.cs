using System;
using System.Threading;
using System.Threading.Tasks;
using Lessons.Lesson_1.Data.Repository.Repositories;

namespace Lessons.ClientCardApi.UnitOfWork.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IEfRepository GetEfRepository();
        INpgsqlRepository GetNpgsqlRepository();
        Task<Task> SaveChangesAsync(CancellationToken cancellationToken);
    }
}