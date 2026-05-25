using System.ComponentModel.DataAnnotations;

namespace WorkLink.Api.DTOs;

public class UpdateProfileRequest
{
    [Required]
    public string DisplayName { get; set; } = string.Empty;

    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public List<int> SkillIds { get; set; } = [];
}

public class ProfileResponse
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public string Role { get; set; } = string.Empty;
    public List<SkillDto> Skills { get; set; } = [];
    public DateTime CreatedAt { get; set; }
}

public class SkillDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class PublicProfileResponse
{
    public string Id { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public string Role { get; set; } = string.Empty;
    public List<SkillDto> Skills { get; set; } = [];
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
