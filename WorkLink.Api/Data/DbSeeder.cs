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

    private static readonly (string Name, string Slug)[] CategoryData =
    [
        ("Web Development", "web-development"),
        ("Mobile Development", "mobile-development"),
        ("Design & Creative", "design-creative"),
        ("Writing & Content", "writing-content"),
        ("Data Science & Analytics", "data-science-analytics"),
        ("Marketing & SEO", "marketing-seo"),
        ("IT & Networking", "it-networking"),
        ("Video & Animation", "video-animation"),
        ("Business & Consulting", "business-consulting"),
        ("Customer Support", "customer-support")
    ];

    public static async Task SeedAsync(AppDbContext db)
    {
        if (!await db.Skills.AnyAsync())
        {
            foreach (var name in SkillNames)
                db.Skills.Add(new Skill { Name = name });
        }

        if (!await db.Categories.AnyAsync())
        {
            foreach (var (name, slug) in CategoryData)
                db.Categories.Add(new Category { Name = name, Slug = slug });
        }

        await db.SaveChangesAsync();
    }
}
