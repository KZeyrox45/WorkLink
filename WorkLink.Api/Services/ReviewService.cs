using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WorkLink.Api.Data;
using WorkLink.Api.DTOs;
using WorkLink.Api.Models;

namespace WorkLink.Api.Services;

public class ReviewService
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public ReviewService(AppDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<ReviewResponse> CreateAsync(
        int jobId, string reviewerId, CreateReviewRequest request)
    {
        var job = await _db.Jobs.FindAsync(jobId);
        if (job == null)
            throw new KeyNotFoundException("Job not found.");
        if (job.Status != JobStatus.Completed)
            throw new InvalidOperationException("You can only review completed jobs.");

        var isClient = job.ClientId == reviewerId;
        var proposal = await _db.Proposals
            .FirstOrDefaultAsync(p => p.JobId == jobId && p.Status == ProposalStatus.Accepted);

        // Determine reviewee: client reviews freelancer, freelancer reviews client
        string revieweeId;
        if (isClient)
        {
            if (proposal == null)
                throw new InvalidOperationException("No accepted freelancer found for this job.");
            revieweeId = proposal.FreelancerId;
        }
        else
        {
            revieweeId = job.ClientId;
        }

        var hasExisting = await _db.Reviews
            .AnyAsync(r => r.JobId == jobId && r.ReviewerId == reviewerId);
        if (hasExisting)
            throw new InvalidOperationException("You have already reviewed this job.");

        var reviewer = await _userManager.FindByIdAsync(reviewerId);
        var reviewee = await _userManager.FindByIdAsync(revieweeId);
        if (reviewer == null || reviewee == null)
            throw new KeyNotFoundException("User not found.");

        var review = new Review
        {
            JobId = jobId,
            ReviewerId = reviewerId,
            RevieweeId = revieweeId,
            Rating = request.Rating,
            Comment = request.Comment,
            CreatedAt = DateTime.UtcNow
        };

        _db.Reviews.Add(review);
        await _db.SaveChangesAsync();

        return MapToResponse(review);
    }

    public async Task<List<ReviewResponse>> ListByUserAsync(string userId)
    {
        var reviews = await _db.Reviews
            .Include(r => r.Job)
            .Include(r => r.Reviewer)
            .Include(r => r.Reviewee)
            .Where(r => r.RevieweeId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return reviews.Select(MapToResponse).ToList();
    }

    public async Task<double> GetAverageRatingAsync(string userId)
    {
        if (!await _db.Reviews.AnyAsync(r => r.RevieweeId == userId))
            return 0;

        return await _db.Reviews
            .Where(r => r.RevieweeId == userId)
            .AverageAsync(r => (double)r.Rating);
    }

    private ReviewResponse MapToResponse(Review review)
    {
        return new ReviewResponse
        {
            Id = review.Id,
            JobId = review.JobId,
            JobTitle = review.Job?.Title ?? "",
            ReviewerId = review.ReviewerId,
            ReviewerName = review.Reviewer?.DisplayName ?? "",
            RevieweeId = review.RevieweeId,
            RevieweeName = review.Reviewee?.DisplayName ?? "",
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt
        };
    }
}
