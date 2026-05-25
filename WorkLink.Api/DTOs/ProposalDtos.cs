using System.ComponentModel.DataAnnotations;

namespace WorkLink.Api.DTOs;

public class CreateProposalRequest
{
    [Required]
    public string CoverLetter { get; set; } = string.Empty;

    [Range(0, 1_000_000)]
    public decimal BidAmount { get; set; }

    [Range(1, 365)]
    public int EstimatedDays { get; set; }
}

public class ProposalResponse
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string FreelancerId { get; set; } = string.Empty;
    public string FreelancerName { get; set; } = string.Empty;
    public string CoverLetter { get; set; } = string.Empty;
    public decimal BidAmount { get; set; }
    public int EstimatedDays { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
