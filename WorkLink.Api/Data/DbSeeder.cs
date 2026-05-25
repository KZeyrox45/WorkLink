using Microsoft.EntityFrameworkCore;
using WorkLink.Api.Models;

namespace WorkLink.Api.Data;

public static class DbSeeder
{
    private static readonly string[] SkillNames =
    [
        "JavaScript", "TypeScript", "Python", "C#", "Java", "Go", "Rust",
        "React", "Angular", "Vue.js", "Node.js", "ASP.NET Core", "Django",
        "PostgreSQL", "SQL Server", "MongoDB", "Docker", "Kubernetes",
        "AWS", "Azure", "GraphQL", "REST API", "Git", "CI/CD",
        "UI/UX Design", "Mobile Development", "Machine Learning", "DevOps"
    ];

    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Skills.AnyAsync())
            return;

        foreach (var name in SkillNames)
        {
            db.Skills.Add(new Skill { Name = name });
        }

        await db.SaveChangesAsync();
    }
}
