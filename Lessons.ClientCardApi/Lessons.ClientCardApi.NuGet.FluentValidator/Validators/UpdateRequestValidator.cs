using FluentValidation;
using Lessons.ClientCardApi.Abstraction.Requests;

namespace Lessons.ClientCardApi.NuGet.FluentValidator.Validators
{
    public sealed class UpdateRequestValidator : AbstractValidator<UpdateRequest>
    {
        public UpdateRequestValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .Matches("^[a-zA-Z0-9 ]*$")
                .NotNull()
                .NotEqual("=$%@></")
                .Length(2, 15);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .Matches("^[a-zA-Z0-9 ]*$")
                .NotNull()
                .NotEqual("=$%@></")
                .Length(2, 15);
            
            RuleFor(x => x.Patronymic)
                .NotEmpty()
                .Matches("^[a-zA-Z0-9 ]*$")
                .NotNull()
                .NotEqual("=$%@></")
                .Length(2, 15);

            RuleFor(x => x.BirthDate)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .NotNull()
                .GreaterThanOrEqualTo(11);

            RuleFor(x => x.PassportNumber)
                .NotEmpty()
                .NotNull()
                .GreaterThanOrEqualTo(10);

            RuleFor(x => x.CreditCardNumber)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.Email)
                .NotNull()
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.SearchId)
                .NotNull()
                .NotEmpty();
        }
    }
}