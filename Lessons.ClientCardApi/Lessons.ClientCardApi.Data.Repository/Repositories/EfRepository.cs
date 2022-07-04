using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lessons.ClientCardApi.Abstraction.Data.Models;
using Lessons.ClientCardApi.Abstraction.Data.Repository;
using Lessons.ClientCardApi.Abstraction.Requests;
using Lessons.ClientCardApi.Data.Context.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lessons.Lesson_1.Data.Repository.Repositories
{
    public interface IEfRepository : 
        ICreateRepository<CreditCardInfoModel>,
        IReadRepository<ReadRequest, CreditCardInfoModel>,
        IUpdateRepository<UpdateRequest>,
        IDeleteRepository<DeleteRequest>
    {
    }

    public sealed class EfRepository : IEfRepository
    {
        private readonly ILogger<EfRepository> _logger;
        private readonly ApplicationContext _context;
        
        public EfRepository(ILogger<EfRepository> logger, ApplicationContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<Task> CreateAsync(CreditCardInfoModel createRequest, CancellationToken cancellationToken)
        {
            try
            {
                await _context.CreditCardInfo.AddAsync(createRequest, cancellationToken);
                return Task.CompletedTask;
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError($"Card info ef creation took a long time...{ex}");
                return Task.FromException(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Card info ef creation stop with exception {ex} - {DateTime.Now}");
                return Task.FromException(ex);
            }
        }

        public async Task<IReadOnlyList<CreditCardInfoModel>> ReadAsync(ReadRequest read, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _context.CreditCardInfo
                    .Where(x => x.Email.Equals(read.Email))
                    .Skip((read.Page - 1) * read.Size)
                    .Take(read.Size)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);
                return result;
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError($"Card info ef reading took a long time...{ex}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Card info ef reading stop with exception {ex} - {DateTime.Now}");
                return null;
            }
        }

        public async Task<Task> UpdateAsync(UpdateRequest update, CancellationToken cancellationToken)
        {
            var foundClient = await _context.CreditCardInfo
                .Where(x => x.Id.Equals(update.SearchId))
                .FirstOrDefaultAsync(cancellationToken);

            if (foundClient is not null)
            {
                try
                {
                    foundClient.Email = update.Email;
                    foundClient.FirstName = update.FirstName;
                    foundClient.LastName = update.LastName;
                    foundClient.Patronymic = update.Patronymic;
                    foundClient.BirthDate = update.BirthDate;
                    foundClient.PassportNumber = update.PassportNumber;
                    foundClient.CreditCardNumber = update.CreditCardNumber;
                    foundClient.PhoneNumber = update.PhoneNumber;
                    
                    await Task.Run(() => _context.CreditCardInfo.Update(foundClient), cancellationToken);
                    
                    return Task.CompletedTask;
                }
                catch (OperationCanceledException ex)
                {
                    _logger.LogError($"Card info ef reading took a long time...{ex}");
                    return null;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Card info ef update stop with exception {ex} - {DateTime.Now}");
                    return Task.FromException(ex);
                }
            }

            return Task.FromException(new NullReferenceException());
        }

        public async Task<Task> DeleteAsync(DeleteRequest delete, CancellationToken cancellationToken)
        {
            try
            {
                var foundClient = await _context.CreditCardInfo
                    .FirstOrDefaultAsync(x => x.Id == delete.Id, cancellationToken);

                if (foundClient != null)
                {
                    await Task.Run(() => _context.CreditCardInfo.Remove(foundClient), cancellationToken);
                    return Task.CompletedTask;
                }

                return Task.FromException(new NullReferenceException());
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError($"Card info ef delete took a long time...{ex}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Card info ef delete stop with exception {ex} - {DateTime.Now}");
                return Task.FromException(ex);
            }
        }
    }
}