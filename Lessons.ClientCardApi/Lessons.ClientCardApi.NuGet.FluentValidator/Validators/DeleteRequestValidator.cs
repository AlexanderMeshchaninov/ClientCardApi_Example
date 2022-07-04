using FluentValidation;
using Lessons.ClientCardApi.Abstraction.Requests;

namespace Lessons.ClientCardApi.NuGet.FluentValidator.Validators
{
    public sealed class DeleteRequestValidator : AbstractValidator<DeleteRequest>
    {
        public DeleteRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotNull()
                .NotEmpty();
        }
    }
}