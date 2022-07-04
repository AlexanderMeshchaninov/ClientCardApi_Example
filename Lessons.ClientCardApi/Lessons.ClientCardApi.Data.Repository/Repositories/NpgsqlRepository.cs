using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lessons.ClientCardApi.Abstraction.Data.Models;
using Lessons.ClientCardApi.Abstraction.Data.Repository;
using Lessons.ClientCardApi.Abstraction.Requests;
using Microsoft.Extensions.Logging;
using Npgsql;
using IDbConnection = Lessons.ClientCardApi.Abstraction.Data.ConnectionString.IDbConnection;

namespace Lessons.Lesson_1.Data.Repository.Repositories
{
    public interface INpgsqlRepository : 
        ICreateRepository<CreditCardInfoModel>,
        IReadRepository<ReadRequest, CreditCardInfoModel>,
        IUpdateRepository<UpdateRequest>,
        IDeleteRepository<DeleteRequest>
    {
    }

    public sealed class NpgsqlRepository : INpgsqlRepository
    {
        private readonly ILogger<NpgsqlRepository> _logger;
        private readonly IDbConnection _connectionString;
        
        public NpgsqlRepository(ILogger<NpgsqlRepository> logger, IDbConnection connectionString)
        {
            _logger = logger;
            _connectionString = connectionString;
        }

        public async Task<Task> CreateAsync(CreditCardInfoModel create, CancellationToken cancellationToken)
        {
            try
            {
                string createQuery = @"INSERT INTO public.""CreditCardInfo"" (
                                       ""FirstName"",
                                       ""LastName"",
                                       ""Patronymic"",
                                       ""BirthDate"",
                                       ""PassportNumber"",
                                       ""PhoneNumber"",
                                       ""Email"",
                                       ""CreditCardNumber"") 
                                        VALUES (
                                       @FirstName, 
                                       @LastName, 
                                       @Patronymic, 
                                       @BirthDate, 
                                       @PassportNumber, 
                                       @PhoneNumber, 
                                       @Email, 
                                       @CreditCardNumber)";

                await using var connection = new NpgsqlConnection(_connectionString.AddConnection());
                await connection.OpenAsync(cancellationToken);

                await using (var cmd = new NpgsqlCommand(createQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@FirstName", create.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", create.LastName);
                    cmd.Parameters.AddWithValue("@Patronymic", create.Patronymic);
                    cmd.Parameters.AddWithValue("@BirthDate", create.BirthDate);
                    cmd.Parameters.AddWithValue("@PassportNumber", create.PassportNumber);
                    cmd.Parameters.AddWithValue("@PhoneNumber", create.PhoneNumber);
                    cmd.Parameters.AddWithValue("@Email", create.Email);
                    cmd.Parameters.AddWithValue("@CreditCardNumber", create.CreditCardNumber);

                    await cmd.ExecuteNonQueryAsync(cancellationToken);
                }
                
                return Task.CompletedTask;
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError($"Card info creation npgsql took a long time...{ex}");
                return Task.FromException(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Card info creation npgsql stop with exception {ex} - {DateTime.Now}");
                return Task.FromException(ex);
            }
        }

        public async Task<IReadOnlyList<CreditCardInfoModel>> ReadAsync(ReadRequest read, CancellationToken cancellationToken)
        {
            try
            {
                await using var connection = new NpgsqlConnection(_connectionString.AddConnection());
                await connection.OpenAsync(cancellationToken);
                
                var responseList = new List<CreditCardInfoModel>();
             
                //Здесь есть проблема написания команд LIMIT и OFFSET получилось отсортировать только так (
                //В EF все работает (пейджинг)
                string readQuery = @$"SELECT * FROM public.""CreditCardInfo"" 
                                        WHERE ""Email"" = @Email";
                
                await using (var cmd = new NpgsqlCommand(readQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@Email", read.Email);

                    await using (var reader = await cmd.ExecuteReaderAsync(cancellationToken))
                    while (await reader.ReadAsync(cancellationToken))
                    {
                        var response = ReadCreditCardInfoModel(reader);
                        responseList.Add(response);
                    }
                    
                    return responseList;
                }
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError($"Card info reading npgsql took a long time...{ex}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Card info reading npgsql stop with exception {ex} - {DateTime.Now}");
                return null;
            }
        }

        public async Task<Task> UpdateAsync(UpdateRequest update, CancellationToken cancellationToken)
        {
            try
            {
                await using var connection = new NpgsqlConnection(_connectionString.AddConnection());
                await connection.OpenAsync(cancellationToken);

                var response = await SearchByIdAsync(connection, update.SearchId);

                if (response.FirstName is not null)
                {
                    var updateQuery = @"UPDATE public.""CreditCardInfo"" 
                                        SET ""FirstName"" = @FirstName,
                                            ""LastName"" = @LastName,
                                            ""Patronymic"" = @Patronymic,
                                            ""BirthDate"" = @BirthDate,
                                            ""PassportNumber"" = @PassportNumber,
                                            ""PhoneNumber"" = @PhoneNumber,
                                            ""Email"" = @Email,
                                            ""CreditCardNumber"" = @CreditCardNumber 
                                        WHERE ""Id"" = @SearchId";
                    
                    await using (var cmd = new NpgsqlCommand(updateQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@SearchId", update.SearchId);
                        cmd.Parameters.AddWithValue("@FirstName", update.FirstName);
                        cmd.Parameters.AddWithValue("@LastName", update.LastName);
                        cmd.Parameters.AddWithValue("@Patronymic", update.Patronymic);
                        cmd.Parameters.AddWithValue("@BirthDate", update.BirthDate);
                        cmd.Parameters.AddWithValue("@PassportNumber", update.PassportNumber);
                        cmd.Parameters.AddWithValue("@PhoneNumber", update.PhoneNumber);
                        cmd.Parameters.AddWithValue("@Email", update.Email);
                        cmd.Parameters.AddWithValue("@CreditCardNumber", update.CreditCardNumber);

                        await cmd.ExecuteNonQueryAsync(cancellationToken);
                    }
                    
                    return Task.CompletedTask;
                }
                
                return Task.FromException(new NullReferenceException());
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError($"Card info update npgsql took a long time...{ex}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Card info update npgsql stop with exception {ex} - {DateTime.Now}");
                return Task.FromException(ex);
            }
        }

        public async Task<Task> DeleteAsync(DeleteRequest delete, CancellationToken cancellationToken)
        {
            try
            {
                await using var connection = new NpgsqlConnection(_connectionString.AddConnection());
                await connection.OpenAsync(cancellationToken);
                
                string deleteQuery = @"DELETE FROM public.""CreditCardInfo"" WHERE ""Id"" = (@Id)";
                
                var response = await SearchByIdAsync(connection, delete.Id);

                if (response.FirstName is not null)
                {
                    await using (var cmd = new NpgsqlCommand(deleteQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@Id", delete.Id);
                    
                        await cmd.ExecuteNonQueryAsync(cancellationToken);
                        
                        return Task.CompletedTask;
                    }
                }
                
                return Task.FromException(new NullReferenceException());
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError($"Card info delete npgsql took a long time...{ex}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Card info delete npgsql stop with exception {ex} - {DateTime.Now}");
                return Task.FromException(ex);
            }
        }
        
        private static CreditCardInfoModel ReadCreditCardInfoModel(NpgsqlDataReader reader)
        {
            long? id = reader["Id"] as long?;
            string firstName = reader["FirstName"] as string;
            string lastName = reader["LastName"] as string;
            string patronymic = reader["Patronymic"] as string;
            DateTime? birthDate = reader["BirthDate"] as DateTime?;
            long? passportNumber = reader["PassportNumber"] as long?;
            long? phoneNumber = reader["PhoneNumber"] as long?;
            string email = reader["Email"] as string;
            long? creditCardNumber = reader["CreditCardNumber"] as long?;
            
            var response = new CreditCardInfoModel()
            {
                Id = id.Value,
                FirstName = firstName,
                LastName = lastName,
                Patronymic = patronymic,
                BirthDate = birthDate.Value,
                PassportNumber = passportNumber.Value,
                PhoneNumber = phoneNumber.Value,
                Email = email,
                CreditCardNumber = creditCardNumber.Value
            };

            return response;
        }
        private async Task<CreditCardInfoModel> SearchByIdAsync(NpgsqlConnection connection, long searchId)
        {
            var response = new CreditCardInfoModel();
            string readQuery = @$"SELECT * FROM public.""CreditCardInfo"" 
                                        WHERE ""Id"" = @Id";

            await using (var cmd = new NpgsqlCommand(readQuery, connection))
            {
                cmd.Parameters.AddWithValue("@Id", searchId);

                await using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        response = ReadCreditCardInfoModel(reader);
                    }
            }

            return response;
        }
    }
}