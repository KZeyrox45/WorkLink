using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WorkLink.Api.Data;
using WorkLink.Api.DTOs;
using WorkLink.Api.Models;

namespace WorkLink.Api.Services;

public class ProfileService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppDbContext _db;

    public ProfileService(UserManager<ApplicationUser> userManager, AppDbContext db)
    {
        _userManager = userManager;
        _db = db;
    }

    public async Task<ProfileResponse> GetProfileAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            throw new KeyNotFoundException("User not found.");

        return await ToProfileResponse(user);
    }

    public async Task<ProfileResponse> UpdateProfileAsync(string userId, UpdateProfileRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            throw new KeyNotFoundException("User not found.");

        user.DisplayName = request.DisplayName;
        user.Bio = request.Bio;
        user.AvatarUrl = request.AvatarUrl;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new InvalidOperationException(
                string.Join("; ", result.Errors.Select(e => e.Description)));

        // Update skills
        var existingSkills = await _db.UserSkills
            .Where(us => us.UserId == userId)
            .ToListAsync();
        _db.UserSkills.RemoveRange(existingSkills);

        if (request.SkillIds.Count != 0)
        {
            var skills = await _db.Skills
                .Where(s => request.SkillIds.Contains(s.Id))
                .ToListAsync();

            foreach (var skill in skills)
            {
                _db.UserSkills.Add(new UserSkill { UserId = userId, SkillId = skill.Id });
            }
        }

        await _db.SaveChangesAsync();

        return await ToProfileResponse(user);
    }

    public async Task<PublicProfileResponse> GetPublicProfileAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            throw new KeyNotFoundException("User not found.");

        var roles = await _userManager.GetRolesAsync(user);

        return new PublicProfileResponse
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            Bio = user.Bio,
            AvatarUrl = user.AvatarUrl,
            Role = roles.FirstOrDefault() ?? "Freelancer",
            Skills = await GetUserSkillsAsync(userId),
            CreatedAt = user.CreatedAt
        };
    }

    private async Task<ProfileResponse> ToProfileResponse(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        return new ProfileResponse
        {
            Id = user.Id,
            Email = user.Email!,
            DisplayName = user.DisplayName,
            Bio = user.Bio,
            AvatarUrl = user.AvatarUrl,
            Role = roles.FirstOrDefault() ?? "Freelancer",
            Skills = await GetUserSkillsAsync(user.Id),
            CreatedAt = user.CreatedAt
        };
    }

    private async Task<List<SkillDto>> GetUserSkillsAsync(string userId)
    {
        return await _db.UserSkills
            .Where(us => us.UserId == userId)
            .Select(us => new SkillDto { Id = us.Skill.Id, Name = us.Skill.Name })
            .ToListAsync();
    }
}
