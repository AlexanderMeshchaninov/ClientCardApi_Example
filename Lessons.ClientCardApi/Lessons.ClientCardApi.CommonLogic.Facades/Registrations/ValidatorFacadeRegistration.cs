using Lessons.ClientCardApi.CommonLogic.Facades.ValidationFacade;
using Microsoft.Extensions.DependencyInjection;

namespace Lessons.ClientCardApi.CommonLogic.Facades.Registrations
{
    public static class ValidatorFacadeRegistration
    {
        public static IServiceCollection RegisterValidationFacade(this IServiceCollection services)
        {
            return services.AddTransient<IValidatorFacade, ValidatorFacade>();
        }
    }
}