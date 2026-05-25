namespace WorkLink.Api.Models;

public class UserSkill
{
    public string UserId { get; set; } = string.Empty;
    public int SkillId { get; set; }
    public ApplicationUser User { get; set; } = null!;
    public Skill Skill { get; set; } = null!;
}
