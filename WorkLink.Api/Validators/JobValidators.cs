using FluentValidation;
using WorkLink.Api.DTOs;

namespace WorkLink.Api.Validators;

public class CreateJobRequestValidator : AbstractValidator<CreateJobRequest>
{
    public CreateJobRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(10000);
        RuleFor(x => x.BudgetMin).InclusiveBetween(0, 1_000_000);
        RuleFor(x => x.BudgetMax).InclusiveBetween(0, 1_000_000);
        RuleFor(x => x).Must(x => x.BudgetMax >= x.BudgetMin)
            .When(x => x.BudgetMin.HasValue && x.BudgetMax.HasValue)
            .WithMessage("BudgetMax must be greater than or equal to BudgetMin.");
        RuleFor(x => x.DurationDays).InclusiveBetween(1, 365);
        RuleFor(x => x.CategoryId).GreaterThan(0);
    }
}

public class UpdateJobRequestValidator : AbstractValidator<UpdateJobRequest>
{
    public UpdateJobRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(10000);
        RuleFor(x => x.BudgetMin).InclusiveBetween(0, 1_000_000);
        RuleFor(x => x.BudgetMax).InclusiveBetween(0, 1_000_000);
        RuleFor(x => x).Must(x => x.BudgetMax >= x.BudgetMin)
            .When(x => x.BudgetMin.HasValue && x.BudgetMax.HasValue)
            .WithMessage("BudgetMax must be greater than or equal to BudgetMin.");
        RuleFor(x => x.DurationDays).InclusiveBetween(1, 365);
        RuleFor(x => x.CategoryId).GreaterThan(0);
    }
}
