using FluentValidation;
using WorkLink.Api.DTOs;

namespace WorkLink.Api.Validators;

public class CreateProposalRequestValidator : AbstractValidator<CreateProposalRequest>
{
    public CreateProposalRequestValidator()
    {
        RuleFor(x => x.CoverLetter).NotEmpty().MaximumLength(5000);
        RuleFor(x => x.BidAmount).InclusiveBetween(1, 1_000_000);
        RuleFor(x => x.EstimatedDays).InclusiveBetween(1, 365);
    }
}
