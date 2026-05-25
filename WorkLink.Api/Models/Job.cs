namespace WorkLink.Api.Models;

public enum JobStatus
{
    Open,
    InProgress,
    Completed,
    Cancelled
}

public class Job
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal? BudgetMin { get; set; }
    public decimal? BudgetMax { get; set; }
    public int? DurationDays { get; set; }
    public int CategoryId { get; set; }
    public string ClientId { get; set; } = string.Empty;
    public JobStatus Status { get; set; } = JobStatus.Open;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Category Category { get; set; } = null!;
    public ApplicationUser Client { get; set; } = null!;
    public ICollection<JobSkill> JobSkills { get; set; } = [];
}
