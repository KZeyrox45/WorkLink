using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WorkLink.Api.Data;
using WorkLink.Api.DTOs;
using WorkLink.Api.Models;

namespace WorkLink.Api.Services;

public class JobService
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public JobService(AppDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<JobListResponse> ListAsync(
        string? keyword, int? categoryId, decimal? budgetMin, decimal? budgetMax,
        List<int>? skillIds, string? status, string? clientId,
        int page = 1, int pageSize = 10)
    {
        var query = _db.Jobs
            .Include(j => j.Category)
            .Include(j => j.Client)
            .Include(j => j.JobSkills).ThenInclude(js => js.Skill)
            .Include(j => j.Proposals)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
            query = query.Where(j =>
                j.Title.Contains(keyword) || j.Description.Contains(keyword));

        if (categoryId.HasValue)
            query = query.Where(j => j.CategoryId == categoryId.Value);

        if (budgetMin.HasValue)
            query = query.Where(j => j.BudgetMax >= budgetMin.Value);

        if (budgetMax.HasValue)
            query = query.Where(j => j.BudgetMin <= budgetMax.Value);

        if (skillIds?.Count > 0)
            query = query.Where(j => j.JobSkills.Any(js => skillIds.Contains(js.SkillId)));

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<JobStatus>(status, true, out var parsedStatus))
            query = query.Where(j => j.Status == parsedStatus);

        if (!string.IsNullOrWhiteSpace(clientId))
            query = query.Where(j => j.ClientId == clientId);

        var total = await query.CountAsync();
        query = query.OrderByDescending(j => j.CreatedAt)
                     .Skip((page - 1) * pageSize)
                     .Take(pageSize);

        var jobs = await query.ToListAsync();

        return new JobListResponse
        {
            Items = jobs.Select(MapToResponse).ToList(),
            Total = total,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(total / (double)pageSize)
        };
    }

    public async Task<JobResponse> GetByIdAsync(int id)
    {
        var job = await _db.Jobs
            .Include(j => j.Category)
            .Include(j => j.Client)
            .Include(j => j.JobSkills).ThenInclude(js => js.Skill)
            .Include(j => j.Proposals)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (job == null)
            throw new KeyNotFoundException("Job not found.");

        return MapToResponse(job);
    }

    public async Task<JobResponse> CreateAsync(string clientId, CreateJobRequest request)
    {
        var client = await _userManager.FindByIdAsync(clientId);
        if (client == null)
            throw new KeyNotFoundException("Client not found.");

        var job = new Job
        {
            Title = request.Title,
            Description = request.Description,
            BudgetMin = request.BudgetMin,
            BudgetMax = request.BudgetMax,
            DurationDays = request.DurationDays,
            CategoryId = request.CategoryId,
            ClientId = clientId,
            Status = JobStatus.Open,
            CreatedAt = DateTime.UtcNow
        };

        _db.Jobs.Add(job);
        await _db.SaveChangesAsync();

        if (request.SkillIds.Count != 0)
        {
            foreach (var skillId in request.SkillIds)
            {
                _db.JobSkills.Add(new JobSkill { JobId = job.Id, SkillId = skillId });
            }
            await _db.SaveChangesAsync();
        }

        return await GetByIdAsync(job.Id);
    }

    public async Task<JobResponse> UpdateAsync(int jobId, string clientId, UpdateJobRequest request)
    {
        var job = await _db.Jobs
            .Include(j => j.JobSkills)
            .FirstOrDefaultAsync(j => j.Id == jobId);

        if (job == null)
            throw new KeyNotFoundException("Job not found.");
        if (job.ClientId != clientId)
            throw new UnauthorizedAccessException("You can only edit your own jobs.");

        job.Title = request.Title;
        job.Description = request.Description;
        job.BudgetMin = request.BudgetMin;
        job.BudgetMax = request.BudgetMax;
        job.DurationDays = request.DurationDays;
        job.CategoryId = request.CategoryId;

        _db.JobSkills.RemoveRange(job.JobSkills);

        if (request.SkillIds.Count != 0)
        {
            foreach (var skillId in request.SkillIds)
            {
                _db.JobSkills.Add(new JobSkill { JobId = job.Id, SkillId = skillId });
            }
        }

        await _db.SaveChangesAsync();
        return await GetByIdAsync(job.Id);
    }

    public async Task DeleteAsync(int jobId, string clientId)
    {
        var job = await _db.Jobs.FindAsync(jobId);
        if (job == null)
            throw new KeyNotFoundException("Job not found.");
        if (job.ClientId != clientId)
            throw new UnauthorizedAccessException("You can only delete your own jobs.");

        _db.Jobs.Remove(job);
        await _db.SaveChangesAsync();
    }

    private JobResponse MapToResponse(Job job)
    {
        return new JobResponse
        {
            Id = job.Id,
            Title = job.Title,
            Description = job.Description,
            BudgetMin = job.BudgetMin,
            BudgetMax = job.BudgetMax,
            DurationDays = job.DurationDays,
            Status = job.Status.ToString(),
            CategoryId = job.CategoryId,
            CategoryName = job.Category?.Name ?? "",
            ClientId = job.ClientId,
            ClientName = job.Client?.DisplayName ?? "",
            Skills = job.JobSkills.Select(js => new SkillDto
            {
                Id = js.Skill.Id,
                Name = js.Skill.Name
            }).ToList(),
            ProposalCount = job.Proposals?.Count ?? 0,
            CreatedAt = job.CreatedAt
        };
    }
}
