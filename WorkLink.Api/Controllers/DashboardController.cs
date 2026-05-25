using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkLink.Api.Data;
using WorkLink.Api.Models;

namespace WorkLink.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _db;

    public DashboardController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var role = User.FindFirstValue(ClaimTypes.Role);

        if (role == "Client")
        {
            var openJobs = await _db.Jobs.CountAsync(j => j.ClientId == userId && j.Status == JobStatus.Open);
            var activeJobs = await _db.Jobs.CountAsync(j => j.ClientId == userId && j.Status == JobStatus.InProgress);
            var completedJobs = await _db.Jobs.CountAsync(j => j.ClientId == userId && j.Status == JobStatus.Completed);
            var totalProposals = await _db.Proposals
                .Where(p => p.Job.ClientId == userId)
                .CountAsync();

            var recentActivity = await _db.Proposals
                .Where(p => p.Job.ClientId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .Take(5)
                .Select(p => new
                {
                    type = "proposal",
                    message = "New proposal on \"" + p.Job.Title + "\"",
                    createdAt = p.CreatedAt
                })
                .ToListAsync();

            return Ok(new
            {
                openJobs, activeJobs, completedJobs, totalProposals, recentActivity
            });
        }
        else
        {
            var pendingProposals = await _db.Proposals
                .CountAsync(p => p.FreelancerId == userId && p.Status == ProposalStatus.Pending);
            var acceptedJobs = await _db.Proposals
                .CountAsync(p => p.FreelancerId == userId && p.Status == ProposalStatus.Accepted);
            var completedJobs = await _db.Proposals
                .CountAsync(p => p.FreelancerId == userId && p.Job.Status == JobStatus.Completed);
            var rejectedProposals = await _db.Proposals
                .CountAsync(p => p.FreelancerId == userId && p.Status == ProposalStatus.Rejected);

            var recentActivity = await _db.Proposals
                .Where(p => p.FreelancerId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .Take(5)
                .Select(p => new
                {
                    type = "proposal",
                    message = "Proposal for \"" + p.Job.Title + "\" is " + p.Status.ToString().ToLower(),
                    createdAt = p.CreatedAt
                })
                .ToListAsync();

            return Ok(new
            {
                pendingProposals, acceptedJobs, completedJobs, rejectedProposals, recentActivity
            });
        }
    }
}
