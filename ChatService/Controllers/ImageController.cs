using System.Drawing;
using ChatService.Dtos;
using ChatService.Storage;
using Microsoft.AspNetCore.Mvc;

namespace ChatService.Controllers;

[ApiController]
[Route("[controller]")]

public class ImageController : ControllerBase
{
    private ProfileImageStore _profileImageStore;
    
    [HttpPost]
    public async Task<ActionResult<UploadImageResponse>> PostImage([FromForm] UploadImageRequest request)
    {
        var file = request.File;
        
        if (file.Length == 0)
        {
            return BadRequest("Please select a file to upload.");
        }
        
        var fileStream = file.OpenReadStream();
        
        return _profileImageStore.UploadImage(request.File);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<FileContentResult>> DownloadImage(string id)
    {
        return _profileImageStore.GetImage(id);
    }
}