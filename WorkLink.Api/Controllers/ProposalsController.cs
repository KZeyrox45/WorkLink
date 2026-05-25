using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkLink.Api.DTOs;
using WorkLink.Api.Services;

namespace WorkLink.Api.Controllers;

[ApiController]
[Route("api/jobs/{jobId}/proposals")]
public class ProposalsController : ControllerBase
{
    private readonly ProposalService _proposalService;

    public ProposalsController(ProposalService proposalService)
    {
        _proposalService = proposalService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> List(int jobId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        try
        {
            var proposals = await _proposalService.ListByJobAsync(jobId, userId);
            return Ok(proposals);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Freelancer")]
    public async Task<IActionResult> Submit(int jobId, [FromBody] CreateProposalRequest request)
    {
        var freelancerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        try
        {
            var proposal = await _proposalService.SubmitAsync(jobId, freelancerId, request);
            return CreatedAtAction(nameof(List), new { jobId }, proposal);
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

    [HttpPut("~/api/proposals/{id}/accept")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> Accept(int id)
    {
        var clientId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        try
        {
            var proposal = await _proposalService.AcceptAsync(id, clientId);
            return Ok(proposal);
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

    [HttpPut("~/api/proposals/{id}/reject")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> Reject(int id)
    {
        var clientId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        try
        {
            var proposal = await _proposalService.RejectAsync(id, clientId);
            return Ok(proposal);
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

    [HttpGet("~/api/proposals/mine")]
    [Authorize(Roles = "Freelancer")]
    public async Task<IActionResult> GetMyProposals()
    {
        var freelancerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var proposals = await _proposalService.ListByFreelancerAsync(freelancerId);
        return Ok(proposals);
    }

    [HttpPut("~/api/proposals/{id}/withdraw")]
    [Authorize(Roles = "Freelancer")]
    public async Task<IActionResult> Withdraw(int id)
    {
        var freelancerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        try
        {
            var proposal = await _proposalService.WithdrawAsync(id, freelancerId);
            return Ok(proposal);
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
}
