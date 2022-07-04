using FluentValidation;
using Lessons.ClientCardApi.Abstraction.Requests;

namespace Lessons.ClientCardApi.NuGet.FluentValidator.Validators
{
    public sealed class ReadRequestValidator : AbstractValidator<ReadRequest>
    {
        public ReadRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotNull()
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Page)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.Size)
                .NotNull()
                .NotEmpty();
        }
    }
}