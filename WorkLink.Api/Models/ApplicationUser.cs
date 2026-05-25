using Microsoft.AspNetCore.Identity;

namespace WorkLink.Api.Models;

public class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<UserSkill> UserSkills { get; set; } = [];
    public ICollection<Job> Jobs { get; set; } = [];
    public ICollection<Proposal> Proposals { get; set; } = [];
    public ICollection<Review> ReviewsGiven { get; set; } = [];
    public ICollection<Review> ReviewsReceived { get; set; } = [];
}
