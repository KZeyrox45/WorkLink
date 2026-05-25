using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkLink.Api.Services;

namespace WorkLink.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ProfileService _profileService;

    public UsersController(ProfileService profileService)
    {
        _profileService = profileService;
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPublicProfile(string id)
    {
        try
        {
            var profile = await _profileService.GetPublicProfileAsync(id);
            return Ok(profile);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
