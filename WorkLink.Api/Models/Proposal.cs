namespace WorkLink.Api.Models;

public enum ProposalStatus
{
    Pending,
    Accepted,
    Rejected,
    Withdrawn
}

public class Proposal
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public string FreelancerId { get; set; } = string.Empty;
    public string CoverLetter { get; set; } = string.Empty;
    public decimal BidAmount { get; set; }
    public int EstimatedDays { get; set; }
    public ProposalStatus Status { get; set; } = ProposalStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Job Job { get; set; } = null!;
    public ApplicationUser Freelancer { get; set; } = null!;
}
