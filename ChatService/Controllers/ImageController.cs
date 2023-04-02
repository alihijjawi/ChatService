using ChatService.Dtos;
using ChatService.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatService.Controllers;

[ApiController]
[Route("api/[controller]")]

public class ImageController : ControllerBase
{
    private readonly IImageService _profileImageService;

    public ImageController(IImageService profileImageService)
    {
        _profileImageService = profileImageService;
    }

    [HttpPost]
    public async Task<ActionResult<UploadImageResponse>> PostImage([FromForm] UploadImageRequest request)
    {
        var file = request.File;
        if (file.Length == 0)
        {
            return BadRequest("No profile picture was provided to upload.");
        }
        
        var response = await _profileImageService.UploadImage(file);
        return CreatedAtAction(nameof(DownloadImage), new {id = response.ImageId}, response);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> DownloadImage(string id)
    {
        try
        {
            return await _profileImageService.DownloadImage(id);
        }
        catch (Exception e)
        {
            return NotFound("Image doesn't exist.");
        }
    }
}