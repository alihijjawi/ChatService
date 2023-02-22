using ChatService.Dtos;
using ChatService.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Linq;

namespace ChatService.Controllers;

[ApiController]
[Route("[controller]")]

public class ProfileController : ControllerBase
{
    private readonly IProfileStore _profileStore;

    public ProfileController(IProfileStore profileStore)
    {
        _profileStore = profileStore;
    }
    
    [HttpPost]
    public async Task<ActionResult<ProfileDto>> PostProfile(ProfileDto profile)
    {
        var exists = await _profileStore.GetProfile(profile.UserName);
        if (!exists.IsNull())
        {
            return Conflict($"A user with username {profile.UserName} already exists");
        }

        await _profileStore.UpsertProfile(profile);
        return CreatedAtAction(nameof(GetProfile), new {username = profile.UserName}, profile);
    }

    [HttpGet("{userName}")]
    public async Task<ActionResult<ProfileDto>> GetProfile(string userName)
    {
        var profile = await _profileStore.GetProfile(userName);
        if (profile.IsNull())
        {
            return NotFound($"A user with username {userName} was not found");
        }

        return Ok(profile);
    }
}