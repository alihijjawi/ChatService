using System.Diagnostics;
using ChatService.Dtos;
using ChatService.Services;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;

namespace ChatService.Controllers;

[ApiController]
[Route("api/[controller]")]

public class ImagesController : ControllerBase
{
    private readonly IImageService _profileImageService;
    private readonly ILogger<ProfileController> _logger;
    private readonly TelemetryClient _telemetry;

    public ImagesController(IImageService profileImageService, ILogger<ProfileController> logger, TelemetryClient telemetry)
    {
        _profileImageService = profileImageService;
        _logger = logger;
        _telemetry = telemetry;
    }

    [HttpPost]
    public async Task<ActionResult<UploadImageResponse>> PostImage([FromForm] UploadImageRequest request)
    {
        using (_logger.BeginScope("{File}", request.File))
        {
            _logger.LogInformation("Uploading Image file {File}", request.File);
			
            var file = request.File;
            if (file.Length == 0)
            {
                _logger.LogInformation("Failed to upload a Profile picture {File}", request.File);
                return BadRequest("No profile picture was provided to upload.");
            }

            var timer = new Stopwatch();
            timer.Start();
            var response = await _profileImageService.UploadImage(file);
            timer.Stop();
            
            _telemetry.TrackEvent("Uploading a Profile Picture");
            _telemetry.TrackMetric("Uploading a Profile Picture time", timer.ElapsedMilliseconds);
            
            _logger.LogInformation("Profile Picture Uploaded: {ImageId}", response.ImageId);
            return CreatedAtAction(nameof(DownloadImage), new {id = response.ImageId}, response);
        }
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> DownloadImage(string id)
    {
        using (_logger.BeginScope("{ImageId}", id))
        {
            _logger.LogInformation("Downloading Image with Id: {ImageId}", id);
            try
            {
                var timer = new Stopwatch();
                timer.Start();
                var response = await _profileImageService.DownloadImage(id);
                timer.Stop();
                
                _telemetry.TrackEvent("Downloading a Profile Picture");
                _telemetry.TrackMetric("Downloading a Profile Picture time", timer.ElapsedMilliseconds);
                
                _logger.LogInformation("Profile Picture Downloaded: {ImageId}", id);
                return response;
            }
            catch (Exception e)
            {
                _logger.LogInformation("Failed to download image of Id: {ImageId}", id);
                return NotFound("Image doesn't exist.");
            }
        }
    }
}