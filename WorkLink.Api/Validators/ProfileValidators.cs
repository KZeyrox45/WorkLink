using FluentValidation;
using WorkLink.Api.DTOs;

namespace WorkLink.Api.Validators;

public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
{
    public UpdateProfileRequestValidator()
    {
        RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Bio).MaximumLength(2000);
        RuleFor(x => x.AvatarUrl).MaximumLength(500);
    }
}
