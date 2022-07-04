using FluentValidation;
using Lessons.ClientCardApi.NuGet.FluentValidator.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace Lessons.ClientCardApi.NuGet.FluentValidator.Registrations
{
    public static class RequestsValidatorsRegister
    {
        public static IServiceCollection RegisterRequestsValidators(this IServiceCollection services)
        {
            return services
                .AddValidatorsFromAssemblyContaining<CreateRequestValidator>(ServiceLifetime.Transient)
                .AddValidatorsFromAssemblyContaining<ReadRequestValidator>(ServiceLifetime.Transient)
                .AddValidatorsFromAssemblyContaining<UpdateRequestValidator>(ServiceLifetime.Transient)
                .AddValidatorsFromAssemblyContaining<DeleteRequestValidator>(ServiceLifetime.Transient);
        }
    }
}