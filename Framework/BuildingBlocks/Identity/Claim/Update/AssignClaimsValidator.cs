using FluentValidation;

namespace Identity.Core.Features.Claim.Update;

public class AssignClaimsValidator : AbstractValidator<AssignClaimsCommand>
{
    public AssignClaimsValidator()
    {
        RuleFor(r => r.Owner)
            .NotEmpty();
        RuleFor(r => r.Claims.Count > 0)
            .NotNull();
    }
}