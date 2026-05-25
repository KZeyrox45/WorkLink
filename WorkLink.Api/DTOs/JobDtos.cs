using System.ComponentModel.DataAnnotations;

namespace WorkLink.Api.DTOs;

public class CreateJobRequest
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Range(0, 1_000_000)]
    public decimal? BudgetMin { get; set; }

    [Range(0, 1_000_000)]
    public decimal? BudgetMax { get; set; }

    [Range(1, 365)]
    public int? DurationDays { get; set; }

    [Required]
    public int CategoryId { get; set; }

    public List<int> SkillIds { get; set; } = [];
}

public class UpdateJobRequest
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    public decimal? BudgetMin { get; set; }
    public decimal? BudgetMax { get; set; }
    public int? DurationDays { get; set; }
    public int CategoryId { get; set; }
    public List<int> SkillIds { get; set; } = [];
}

public class JobResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal? BudgetMin { get; set; }
    public decimal? BudgetMax { get; set; }
    public int? DurationDays { get; set; }
    public string Status { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public List<SkillDto> Skills { get; set; } = [];
    public int ProposalCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class JobListResponse
{
    public List<JobResponse> Items { get; set; } = [];
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
}
