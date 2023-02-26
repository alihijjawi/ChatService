using System.Drawing;
using ChatService.Dtos;
using ChatService.Storage;
using Microsoft.AspNetCore.Mvc;

namespace ChatService.Controllers;

[ApiController]
[Route("[controller]")]

public class ImageController : ControllerBase
{
    private readonly IImageStore _profileImageStore;

    public ImageController(IImageStore profileImageStore)
    {
        _profileImageStore = profileImageStore;
    }

    [HttpPost]
    public async Task<ActionResult<UploadImageResponse>> PostImage([FromForm] UploadImageRequest request)
    {
        var file = request.File;
        
        if (file.Length == 0)
        {
            return BadRequest("No profile picture was provided to upload.");
        }
        
        var response = await _profileImageStore.UploadImage(file);
        
        return CreatedAtAction(nameof(DownloadImage), new {id = response.ImageId}, response);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> DownloadImage(string id)
    {
        return await _profileImageStore.DownloadImage(id);
    }
}