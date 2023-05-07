using System.Diagnostics;
using ChatService.Dtos;
using ChatService.Services;
using ChatService.Storage;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Linq;

namespace ChatService.Controllers;

[ApiController]
[Route("api/[controller]")]

public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;
    private readonly ILogger<ProfileController> _logger;
    private readonly TelemetryClient _telemetry;

    public ProfileController(IProfileService profileService, ILogger<ProfileController> logger, TelemetryClient telemetry)
    {
        _profileService = profileService;
        _logger = logger;
        _telemetry = telemetry;
    }
    
    [HttpPost]
    public async Task<ActionResult<ProfileDto>> PostProfile(ProfileDto profile)
    {
        using (_logger.BeginScope("{ProfileUsername}", profile.UserName))
        {
            _logger.LogInformation("Creating Profile for user {ProfileUsername}", profile.UserName);
            var exists = await _profileService.GetProfile(profile.UserName);
            if (!(exists == null))
            {
                _logger.LogInformation("Failed to create a duplicate Profile for user {ProfileUsername}", profile.UserName);
                return Conflict($"A user with username {profile.UserName} already exists");
            }

            var timer = new Stopwatch();
            timer.Start();
            // await _profileService.EnqueueCreateProfile(profile);
            // for some reason it was not finding the service bus
            await _profileService.UpsertProfile(profile);
            timer.Stop();
            _telemetry.TrackEvent("Creating a Profile");
            _telemetry.TrackMetric("Creating a Profile time", timer.ElapsedMilliseconds);
            
            _logger.LogInformation("Profile created: {ProfileUsername}", profile.UserName);
            return CreatedAtAction(nameof(GetProfile), new {username = profile.UserName}, profile);
        }
    }

    [HttpGet("{userName}")]
    public async Task<ActionResult<ProfileDto>> GetProfile(string userName)
    {
        using (_logger.BeginScope("{Username}", userName))
        {
            _logger.LogInformation("Fetching Profile for user {Username}", userName);
            
            var timer = new Stopwatch();
            timer.Start();
            var profile = await _profileService.GetProfile(userName);
            timer.Stop();
            if (profile == null)
            {
                _logger.LogInformation("Failed to fetch a non-existent Profile for username {Username}", userName);
                return NotFound($"A user with username {userName} was not found");
            }
            
            _telemetry.TrackEvent("Fetching a Profile");
            _telemetry.TrackMetric("Fetching a Profile time", timer.ElapsedMilliseconds);
            
            _logger.LogInformation("Profile fetched: {Username}", profile.UserName);
            return Ok(profile);   
        }
    }
}