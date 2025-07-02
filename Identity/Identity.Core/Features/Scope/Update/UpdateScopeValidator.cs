using FluentValidation;
using Identity.Core.Features.Scope.Create;

namespace Identity.Core.Features.Scope.Update;

public class UpdateScopeValidator : AbstractValidator<CreateScopeCommand>
{
    public UpdateScopeValidator()
    {
        RuleFor(x => x.Name).NotNull().NotEmpty().WithMessage("Scope name is required.");
    }
}