using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WorkLink.Api.Data;
using WorkLink.Api.DTOs;
using WorkLink.Api.Models;

namespace WorkLink.Api.Services;

public class ProposalService
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProposalService(AppDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<List<ProposalResponse>> ListByJobAsync(int jobId, string currentUserId)
    {
        var job = await _db.Jobs.FindAsync(jobId);
        if (job == null)
            throw new KeyNotFoundException("Job not found.");

        var query = _db.Proposals
            .Include(p => p.Freelancer)
            .Include(p => p.Job)
            .Where(p => p.JobId == jobId);

        if (job.ClientId != currentUserId)
            query = query.Where(p => p.FreelancerId == currentUserId);

        var proposals = await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
        return proposals.Select(MapToResponse).ToList();
    }

    public async Task<ProposalResponse> SubmitAsync(int jobId, string freelancerId, CreateProposalRequest request)
    {
        var job = await _db.Jobs.FindAsync(jobId);
        if (job == null)
            throw new KeyNotFoundException("Job not found.");
        if (job.ClientId == freelancerId)
            throw new InvalidOperationException("You cannot submit a proposal to your own job.");
        if (job.Status != JobStatus.Open)
            throw new InvalidOperationException("You can only submit proposals to open jobs.");

        var existing = await _db.Proposals.AnyAsync(p => p.JobId == jobId && p.FreelancerId == freelancerId);
        if (existing)
            throw new InvalidOperationException("You have already submitted a proposal for this job.");

        var freelancer = await _userManager.FindByIdAsync(freelancerId);
        if (freelancer == null)
            throw new KeyNotFoundException("Freelancer not found.");

        var proposal = new Proposal
        {
            JobId = jobId,
            FreelancerId = freelancerId,
            CoverLetter = request.CoverLetter,
            BidAmount = request.BidAmount,
            EstimatedDays = request.EstimatedDays,
            Status = ProposalStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _db.Proposals.Add(proposal);
        await _db.SaveChangesAsync();

        return (await GetByIdAsync(proposal.Id))!;
    }

    public async Task<ProposalResponse> AcceptAsync(int proposalId, string clientId)
    {
        var proposal = await _db.Proposals
            .Include(p => p.Job)
            .Include(p => p.Freelancer)
            .FirstOrDefaultAsync(p => p.Id == proposalId);

        if (proposal == null)
            throw new KeyNotFoundException("Proposal not found.");
        if (proposal.Job.ClientId != clientId)
            throw new UnauthorizedAccessException("You can only accept proposals on your own jobs.");
        if (proposal.Status != ProposalStatus.Pending)
            throw new InvalidOperationException("Only pending proposals can be accepted.");

        proposal.Status = ProposalStatus.Accepted;
        proposal.Job.Status = JobStatus.InProgress;

        var otherProposals = await _db.Proposals
            .Where(p => p.JobId == proposal.JobId && p.Id != proposalId && p.Status == ProposalStatus.Pending)
            .ToListAsync();

        foreach (var p in otherProposals)
            p.Status = ProposalStatus.Rejected;

        await _db.SaveChangesAsync();

        return MapToResponse(proposal);
    }

    public async Task<ProposalResponse> RejectAsync(int proposalId, string clientId)
    {
        var proposal = await _db.Proposals
            .Include(p => p.Job)
            .Include(p => p.Freelancer)
            .FirstOrDefaultAsync(p => p.Id == proposalId);

        if (proposal == null)
            throw new KeyNotFoundException("Proposal not found.");
        if (proposal.Job.ClientId != clientId)
            throw new UnauthorizedAccessException("You can only reject proposals on your own jobs.");
        if (proposal.Status != ProposalStatus.Pending)
            throw new InvalidOperationException("Only pending proposals can be rejected.");

        proposal.Status = ProposalStatus.Rejected;
        await _db.SaveChangesAsync();

        return MapToResponse(proposal);
    }

    public async Task<ProposalResponse> WithdrawAsync(int proposalId, string freelancerId)
    {
        var proposal = await _db.Proposals
            .Include(p => p.Job)
            .Include(p => p.Freelancer)
            .FirstOrDefaultAsync(p => p.Id == proposalId);

        if (proposal == null)
            throw new KeyNotFoundException("Proposal not found.");
        if (proposal.FreelancerId != freelancerId)
            throw new UnauthorizedAccessException("You can only withdraw your own proposals.");
        if (proposal.Status != ProposalStatus.Pending)
            throw new InvalidOperationException("Only pending proposals can be withdrawn.");

        proposal.Status = ProposalStatus.Withdrawn;
        await _db.SaveChangesAsync();

        return MapToResponse(proposal);
    }

    private async Task<ProposalResponse?> GetByIdAsync(int id)
    {
        var proposal = await _db.Proposals
            .Include(p => p.Job)
            .Include(p => p.Freelancer)
            .FirstOrDefaultAsync(p => p.Id == id);

        return proposal == null ? null : MapToResponse(proposal);
    }

    private ProposalResponse MapToResponse(Proposal proposal)
    {
        return new ProposalResponse
        {
            Id = proposal.Id,
            JobId = proposal.JobId,
            JobTitle = proposal.Job?.Title ?? "",
            FreelancerId = proposal.FreelancerId,
            FreelancerName = proposal.Freelancer?.DisplayName ?? "",
            CoverLetter = proposal.CoverLetter,
            BidAmount = proposal.BidAmount,
            EstimatedDays = proposal.EstimatedDays,
            Status = proposal.Status.ToString(),
            CreatedAt = proposal.CreatedAt
        };
    }
}
