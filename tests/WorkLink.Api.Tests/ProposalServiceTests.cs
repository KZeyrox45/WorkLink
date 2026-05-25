using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using WorkLink.Api.Data;
using WorkLink.Api.DTOs;
using WorkLink.Api.Models;
using WorkLink.Api.Services;

namespace WorkLink.Api.Tests;

public class ProposalServiceTests
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
                new ApplicationUser { Id = "c1", Email = "c1@t.com", UserName = "c1@t.com", DisplayName = "Client1" },
                new ApplicationUser { Id = "c2", Email = "c2@t.com", UserName = "c2@t.com", DisplayName = "Client2" },
                new ApplicationUser { Id = "f1", Email = "f1@t.com", UserName = "f1@t.com", DisplayName = "Freelancer1" }
            );
            db.SaveChanges();
        }
    }

    [Test]
    public async Task SubmitAsync_FreelancerOwnsJob_ThrowsInvalidOperation()
    {
        var db = CreateDb(); SeedUsers(db);
        db.Jobs.Add(new Job { Title = "T", ClientId = "c1", Status = JobStatus.Open });
        await db.SaveChangesAsync();

        var svc = new ProposalService(db, MockUsers());
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            svc.SubmitAsync(1, "c1", new CreateProposalRequest { CoverLetter = "X", BidAmount = 100, EstimatedDays = 5 }));

        await Assert.That(ex?.Message).IsEqualTo("You cannot submit a proposal to your own job.");
    }

    [Test]
    public async Task SubmitAsync_JobNotOpen_ThrowsInvalidOperation()
    {
        var db = CreateDb(); SeedUsers(db);
        db.Jobs.Add(new Job { Title = "T", ClientId = "c1", Status = JobStatus.InProgress });
        await db.SaveChangesAsync();

        var svc = new ProposalService(db, MockUsers());
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            svc.SubmitAsync(1, "f1", new CreateProposalRequest { CoverLetter = "X", BidAmount = 100, EstimatedDays = 5 }));

        await Assert.That(ex?.Message).IsEqualTo("You can only submit proposals to open jobs.");
    }

    [Test]
    public async Task SubmitAsync_DuplicateProposal_ThrowsInvalidOperation()
    {
        var db = CreateDb(); SeedUsers(db);
        db.Jobs.Add(new Job { Title = "T", ClientId = "c1", Status = JobStatus.Open });
        await db.SaveChangesAsync();
        db.Proposals.Add(new Proposal { JobId = 1, FreelancerId = "f1", CoverLetter = "X", BidAmount = 100, EstimatedDays = 5 });
        await db.SaveChangesAsync();

        var svc = new ProposalService(db, MockUsers());
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            svc.SubmitAsync(1, "f1", new CreateProposalRequest { CoverLetter = "Y", BidAmount = 200, EstimatedDays = 3 }));

        await Assert.That(ex?.Message).IsEqualTo("You have already submitted a proposal for this job.");
    }

    [Test]
    public async Task SubmitAsync_ValidRequest_CreatesProposal()
    {
        var db = CreateDb(); SeedUsers(db);
        db.Jobs.Add(new Job { Title = "T", ClientId = "c1", Status = JobStatus.Open });
        await db.SaveChangesAsync();

        var svc = new ProposalService(db, MockUsers());
        var r = await svc.SubmitAsync(1, "f1", new CreateProposalRequest { CoverLetter = "I can!", BidAmount = 500, EstimatedDays = 10 });

        await Assert.That(r).IsNotNull();
        await Assert.That(r.JobId).IsEqualTo(1);
        await Assert.That(r.BidAmount).IsEqualTo(500);
        await Assert.That(r.Status).IsEqualTo("Pending");
    }

    [Test]
    public async Task AcceptAsync_NotPending_ThrowsInvalidOperation()
    {
        var db = CreateDb(); SeedUsers(db);
        db.Jobs.Add(new Job { Title = "T", ClientId = "c1", Status = JobStatus.Open });
        await db.SaveChangesAsync();
        var prop = new Proposal { JobId = 1, FreelancerId = "f1", Status = ProposalStatus.Accepted, CoverLetter = "X", BidAmount = 100, EstimatedDays = 5 };
        db.Proposals.Add(prop);
        await db.SaveChangesAsync();

        var svc = new ProposalService(db, MockUsers());
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => svc.AcceptAsync(prop.Id, "c1"));
        await Assert.That(ex?.Message).IsEqualTo("Only pending proposals can be accepted.");
    }

    [Test]
    public async Task AcceptAsync_ValidRequest_UpdatesStatusAndJob()
    {
        var db = CreateDb(); SeedUsers(db);
        db.Jobs.Add(new Job { Title = "T", ClientId = "c1", Status = JobStatus.Open });
        await db.SaveChangesAsync();
        var prop = new Proposal { JobId = 1, FreelancerId = "f1", Status = ProposalStatus.Pending, CoverLetter = "X", BidAmount = 100, EstimatedDays = 5 };
        db.Proposals.Add(prop);
        await db.SaveChangesAsync();

        var svc = new ProposalService(db, MockUsers());
        var r = await svc.AcceptAsync(prop.Id, "c1");
        await Assert.That(r.Status).IsEqualTo("Accepted");
        var updatedJob = await db.Jobs.FindAsync(1);
        await Assert.That(updatedJob!.Status).IsEqualTo(JobStatus.InProgress);
    }

    [Test]
    public async Task WithdrawAsync_NotPending_ThrowsInvalidOperation()
    {
        var db = CreateDb(); SeedUsers(db);
        db.Jobs.Add(new Job { Title = "T", ClientId = "c1", Status = JobStatus.Open });
        await db.SaveChangesAsync();
        var prop = new Proposal { JobId = 1, FreelancerId = "f1", Status = ProposalStatus.Accepted, CoverLetter = "X", BidAmount = 100, EstimatedDays = 5 };
        db.Proposals.Add(prop);
        await db.SaveChangesAsync();

        var svc = new ProposalService(db, MockUsers());
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => svc.WithdrawAsync(prop.Id, "f1"));
        await Assert.That(ex?.Message).IsEqualTo("Only pending proposals can be withdrawn.");
    }
}
