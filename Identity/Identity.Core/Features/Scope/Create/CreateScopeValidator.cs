using FluentValidation;

namespace Identity.Core.Features.Scope.Create;

public class CreateScopeValidator : AbstractValidator<CreateScopeCommand>
{
    public CreateScopeValidator()
    {
        RuleFor(x => x.Name).NotNull().NotEmpty().WithMessage("Scope name is required.");
    }
}