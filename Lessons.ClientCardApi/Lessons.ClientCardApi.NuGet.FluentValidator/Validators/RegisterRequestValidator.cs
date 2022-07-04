using FluentValidation;
using Lessons.ClientCardApi.Abstraction.Requests;

namespace Lessons.ClientCardApi.NuGet.FluentValidator.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty()
                .Matches("^[a-zA-Z0-9 ]*$")
                .NotNull()
                .NotEqual("=$%@></")
                .Length(2, 15);
            
            RuleFor(x => x.Email)
                .NotEmpty()
                .NotNull()
                .EmailAddress();
            
            RuleFor(x => x.Password)
                .NotEmpty()
                .NotNull();
                //Some login
        }
    }
}