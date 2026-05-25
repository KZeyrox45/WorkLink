using Microsoft.EntityFrameworkCore;
using WorkLink.Api.Data;
using WorkLink.Api.DTOs;

namespace WorkLink.Api.Services;

public class SkillService
{
    private readonly AppDbContext _db;

    public SkillService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<SkillDto>> GetAllAsync()
    {
        return await _db.Skills
            .OrderBy(s => s.Name)
            .Select(s => new SkillDto { Id = s.Id, Name = s.Name })
            .ToListAsync();
    }
}
