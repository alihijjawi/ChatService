using ChatService.Dtos;
using ChatService.Services;
using ChatService.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Linq;

namespace ChatService.Controllers;

[ApiController]
[Route("[controller]")]

public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }
    
    [HttpPost]
    public async Task<ActionResult<ProfileDto>> PostProfile(ProfileDto profile)
    {
        var exists = await _profileService.GetProfile(profile.UserName);
        if (!(exists == null))
        {
            return Conflict($"A user with username {profile.UserName} already exists");
        }

        await _profileService.UpsertProfile(profile);
        return CreatedAtAction(nameof(GetProfile), new {username = profile.UserName}, profile);
    }

    [HttpGet("{userName}")]
    public async Task<ActionResult<ProfileDto>> GetProfile(string userName)
    {
        var profile = await _profileService.GetProfile(userName);
        if (profile == null)
        {
            return NotFound($"A user with username {userName} was not found");
        }

        return Ok(profile);
    }
}