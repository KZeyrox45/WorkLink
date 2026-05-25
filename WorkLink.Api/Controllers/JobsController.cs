using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkLink.Api.DTOs;
using WorkLink.Api.Services;

namespace WorkLink.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly JobService _jobService;

    public JobsController(JobService jobService)
    {
        _jobService = jobService;
    }

    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] string? keyword,
        [FromQuery] int? categoryId,
        [FromQuery] decimal? budgetMin,
        [FromQuery] decimal? budgetMax,
        [FromQuery] string? skillIds,
        [FromQuery] string? status,
        [FromQuery] string? clientId,
        [FromQuery] string? sortBy = "newest",
        [FromQuery] string? sortOrder = "desc",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var parsedSkillIds = skillIds?
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse)
            .ToList();

        var result = await _jobService.ListAsync(
            keyword, categoryId, budgetMin, budgetMax,
            parsedSkillIds, status, clientId, sortBy, sortOrder,
            page, pageSize);

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var job = await _jobService.GetByIdAsync(id);
            return Ok(job);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> Create([FromBody] CreateJobRequest request)
    {
        var clientId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        try
        {
            var job = await _jobService.CreateAsync(clientId, request);
            return CreatedAtAction(nameof(GetById), new { id = job.Id }, job);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateJobRequest request)
    {
        var clientId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        try
        {
            var job = await _jobService.UpdateAsync(id, clientId, request);
            return Ok(job);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpPatch("{id}/complete")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> Complete(int id)
    {
        var clientId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        try
        {
            var job = await _jobService.CompleteAsync(id, clientId);
            return Ok(job);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> Delete(int id)
    {
        var clientId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        try
        {
            await _jobService.DeleteAsync(id, clientId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }
}
