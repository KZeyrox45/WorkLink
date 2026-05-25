using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using WorkLink.Api.Data;
using WorkLink.Api.DTOs;
using WorkLink.Api.Models;
using WorkLink.Api.Services;

namespace WorkLink.Api.Tests;

public class ReviewServiceTests
{
    private static AppDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private static UserManager<ApplicationUser> MockUsers()
    {
        var mgr = Substitute.For<UserManager<ApplicationUser>>(
            Substitute.For<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);

        mgr.FindByIdAsync(Arg.Any<string>()).Returns(call =>
            new ApplicationUser
            {
                Id = call.Arg<string>(),
                Email = $"{call.Arg<string>()}@t.com",
                DisplayName = "Test",
                UserName = $"{call.Arg<string>()}@t.com"
            });

        return mgr;
    }

    private static void SeedUsers(AppDbContext db)
    {
        if (!db.Users.Any())
        {
            db.Users.AddRange(
                new ApplicationUser { Id = "c1", Email = "c1@t.com", UserName = "c1@t.com", DisplayName = "C1" },
                new ApplicationUser { Id = "f1", Email = "f1@t.com", UserName = "f1@t.com", DisplayName = "F1" },
                new ApplicationUser { Id = "r1", Email = "r1@t.com", UserName = "r1@t.com", DisplayName = "R1" },
                new ApplicationUser { Id = "r2", Email = "r2@t.com", UserName = "r2@t.com", DisplayName = "R2" },
                new ApplicationUser { Id = "u1", Email = "u1@t.com", UserName = "u1@t.com", DisplayName = "U1" }
            );
            db.SaveChanges();
        }
    }

    [Test]
    public async Task CreateReview_CompletedJobWithAcceptedProposal_CreatesReview()
    {
        var db = CreateDb(); SeedUsers(db);
        db.Jobs.Add(new Job { Id = 99, Title = "T", ClientId = "c1", Status = JobStatus.Completed });
        db.SaveChanges();
        db.Proposals.Add(new Proposal
        {
            JobId = 99, FreelancerId = "f1", Status = ProposalStatus.Accepted,
            CoverLetter = "X", BidAmount = 100, EstimatedDays = 5
        });
        db.SaveChanges();

        var svc = new ReviewService(db, MockUsers());
        var r = await svc.CreateAsync(99, "c1", new CreateReviewRequest { Rating = 5, Comment = "Great!" });
        await Assert.That(r).IsNotNull();
        await Assert.That(r.Rating).IsEqualTo(5);
        await Assert.That(r.Comment).IsEqualTo("Great!");
    }

    [Test]
    public async Task CreateReview_DuplicateReview_ThrowsInvalidOperation()
    {
        var db = CreateDb(); SeedUsers(db);
        db.Jobs.Add(new Job { Id = 99, Title = "T", ClientId = "c1", Status = JobStatus.Completed });
        db.SaveChanges();
        db.Proposals.Add(new Proposal
        {
            JobId = 99, FreelancerId = "f1", Status = ProposalStatus.Accepted,
            CoverLetter = "X", BidAmount = 100, EstimatedDays = 5
        });
        db.SaveChanges();
        db.Reviews.Add(new Review { JobId = 99, ReviewerId = "c1", RevieweeId = "f1", Rating = 5, Comment = "First" });
        db.SaveChanges();

        var svc = new ReviewService(db, MockUsers());
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            svc.CreateAsync(99, "c1", new CreateReviewRequest { Rating = 3, Comment = "Duplicate" }));

        await Assert.That(ex?.Message).IsEqualTo("You have already reviewed this job.");
    }

    [Test]
    public async Task GetAverageRating_ReturnsCorrectAverage()
    {
        var db = CreateDb(); SeedUsers(db);
        db.Categories.Add(new Category { Id = 10, Name = "Web", Slug = "web" });
        db.Jobs.AddRange(
            new Job { Id = 1, Title = "J1", Description = "D", CategoryId = 10, Status = JobStatus.Open, ClientId = "c1" },
            new Job { Id = 2, Title = "J2", Description = "D", CategoryId = 10, Status = JobStatus.Open, ClientId = "c1" }
        );
        db.SaveChanges();
        db.Reviews.AddRange(
            new Review { JobId = 1, ReviewerId = "r1", RevieweeId = "u1", Rating = 5, Comment = "A" },
            new Review { JobId = 2, ReviewerId = "r2", RevieweeId = "u1", Rating = 3, Comment = "B" }
        );
        db.SaveChanges();

        var svc = new ReviewService(db, MockUsers());
        var avg = await svc.GetAverageRatingAsync("u1");
        await Assert.That(avg).IsEqualTo(4.0);
    }

    [Test]
    public async Task ListByUser_ReturnsUserReviews()
    {
        var db = CreateDb(); SeedUsers(db);
        db.Categories.Add(new Category { Id = 10, Name = "Web", Slug = "web" });
        db.Jobs.AddRange(
            new Job { Id = 1, Title = "J1", Description = "D", CategoryId = 10, Status = JobStatus.Open, ClientId = "c1" },
            new Job { Id = 2, Title = "J2", Description = "D", CategoryId = 10, Status = JobStatus.Open, ClientId = "c1" }
        );
        db.SaveChanges();
        db.Reviews.AddRange(
            new Review { JobId = 1, ReviewerId = "r1", RevieweeId = "u1", Rating = 5, Comment = "A" },
            new Review { JobId = 2, ReviewerId = "r2", RevieweeId = "u1", Rating = 3, Comment = "B" }
        );
        db.SaveChanges();

        var svc = new ReviewService(db, MockUsers());
        var reviews = await svc.ListByUserAsync("u1");
        await Assert.That(reviews.Count).IsEqualTo(2);
    }
}
