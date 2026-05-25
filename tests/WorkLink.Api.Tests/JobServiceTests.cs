using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using WorkLink.Api.Data;
using WorkLink.Api.Models;
using WorkLink.Api.Services;

namespace WorkLink.Api.Tests;

public class JobServiceTests
{
    private static AppDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private static JobService CreateSvc(AppDbContext db)
    {
        return new JobService(db, Substitute.For<UserManager<ApplicationUser>>(
            Substitute.For<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null));
    }

    private static void Seed(AppDbContext db)
    {
        db.Users.AddRange(
            new ApplicationUser { Id = "c1", Email = "c1@t.com", UserName = "c1@t.com", DisplayName = "C1" },
            new ApplicationUser { Id = "c2", Email = "c2@t.com", UserName = "c2@t.com", DisplayName = "C2" },
            new ApplicationUser { Id = "c3", Email = "c3@t.com", UserName = "c3@t.com", DisplayName = "C3" }
        );
        db.Categories.AddRange(
            new Category { Id = 10, Name = "Web", Slug = "web" },
            new Category { Id = 20, Name = "Mobile", Slug = "mobile" }
        );
        db.Skills.AddRange(
            new Skill { Id = 100, Name = "Angular" },
            new Skill { Id = 200, Name = "React" }
        );
        db.Jobs.AddRange(
            new Job { Id = 1, Title = "Angular App", Description = "Need Angular dev", CategoryId = 10, BudgetMin = 1000, BudgetMax = 5000, Status = JobStatus.Open, ClientId = "c1" },
            new Job { Id = 2, Title = "React Native App", Description = "Need RN dev", CategoryId = 20, BudgetMin = 2000, BudgetMax = 8000, Status = JobStatus.Open, ClientId = "c2" },
            new Job { Id = 3, Title = "Fix Bug", Description = "Urgent", CategoryId = 10, BudgetMin = 100, BudgetMax = 500, Status = JobStatus.InProgress, ClientId = "c3" },
            new Job { Id = 4, Title = "Migration", Description = "Migrate data", CategoryId = 10, BudgetMin = 5000, BudgetMax = 15000, Status = JobStatus.Completed, ClientId = "c1" }
        );
        db.JobSkills.AddRange(
            new JobSkill { JobId = 1, SkillId = 100 },
            new JobSkill { JobId = 2, SkillId = 200 },
            new JobSkill { JobId = 4, SkillId = 100 },
            new JobSkill { JobId = 4, SkillId = 200 }
        );
        db.SaveChanges();
    }

    [Test]
    public async Task ListAsync_KeywordFilter_ReturnsMatching()
    {
        var db = CreateDb(); Seed(db); var svc = CreateSvc(db);
        var r = await svc.ListAsync(keyword: "Angular", null, null, null, null, null, null, "newest", "desc", 1, 10);
        await Assert.That(r.Total).IsEqualTo(1);
        await Assert.That(r.Items[0].Title).IsEqualTo("Angular App");
    }

    [Test]
    public async Task ListAsync_CategoryFilter_ReturnsMatching()
    {
        var db = CreateDb(); Seed(db); var svc = CreateSvc(db);
        var r = await svc.ListAsync(null, categoryId: 20, null, null, null, null, null, "newest", "desc", 1, 10);
        await Assert.That(r.Total).IsEqualTo(1);
        await Assert.That(r.Items[0].Title).IsEqualTo("React Native App");
    }

    [Test]
    public async Task ListAsync_StatusFilter_ReturnsMatching()
    {
        var db = CreateDb(); Seed(db); var svc = CreateSvc(db);
        var r = await svc.ListAsync(null, null, null, null, null, status: "InProgress", null, "newest", "desc", 1, 10);
        await Assert.That(r.Total).IsEqualTo(1);
        await Assert.That(r.Items[0].Title).IsEqualTo("Fix Bug");
    }

    [Test]
    public async Task ListAsync_ClientFilter_ReturnsMatching()
    {
        var db = CreateDb(); Seed(db); var svc = CreateSvc(db);
        var r = await svc.ListAsync(null, null, null, null, null, null, clientId: "c1", "newest", "desc", 1, 10);
        await Assert.That(r.Total).IsEqualTo(2);
    }

    [Test]
    public async Task ListAsync_SkillFilter_ReturnsMatching()
    {
        var db = CreateDb(); Seed(db); var svc = CreateSvc(db);
        var r = await svc.ListAsync(null, null, null, null, skillIds: [200], null, null, "newest", "desc", 1, 10);
        await Assert.That(r.Total).IsEqualTo(2);
    }

    [Test]
    public async Task ListAsync_Pagination_ReturnsCorrectPage()
    {
        var db = CreateDb(); Seed(db); var svc = CreateSvc(db);
        for (int i = 10; i < 15; i++)
            db.Jobs.Add(new Job { Title = $"J{i}", Description = "D", CategoryId = 10, Status = JobStatus.Open, ClientId = "c1" });
        db.SaveChanges();

        var r = await svc.ListAsync(null, null, null, null, null, null, null, "newest", "desc", 2, 2);
        await Assert.That(r.Page).IsEqualTo(2);
        await Assert.That(r.Total).IsEqualTo(9);
        await Assert.That(r.Items.Count).IsEqualTo(2);
    }

    [Test]
    public async Task ListAsync_NoFilters_ReturnsAll()
    {
        var db = CreateDb(); Seed(db); var svc = CreateSvc(db);
        var r = await svc.ListAsync(null, null, null, null, null, null, null, "newest", "desc", 1, 10);
        await Assert.That(r.Total).IsEqualTo(4);
    }
}
