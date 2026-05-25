using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkLink.Api.DTOs;
using WorkLink.Api.Services;

namespace WorkLink.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly ReviewService _reviewService;

    public ReviewsController(ReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpPost("{jobId}")]
    [Authorize]
    public async Task<IActionResult> Create(int jobId, [FromBody] CreateReviewRequest request)
    {
        var reviewerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        try
        {
            var review = await _reviewService.CreateAsync(jobId, reviewerId, request);
            return CreatedAtAction(nameof(GetUserReviews), new { userId = reviewerId }, review);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("user/{userId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetUserReviews(string userId)
    {
        var reviews = await _reviewService.ListByUserAsync(userId);
        return Ok(reviews);
    }
}
