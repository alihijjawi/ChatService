using System.Runtime.InteropServices.ComTypes;
using ChatService.Dtos;
using ChatService.Storage;
using Microsoft.AspNetCore.Mvc;

namespace ChatService.Services;

public class ImageService : IImageService
{
    private readonly IImageStore _imageStore;

    public ImageService(IImageStore imageStore)
    {
        _imageStore = imageStore;
    }

    public async Task<FileContentResult> DownloadImage(string id)
    {
        var stream = new MemoryStream();
        await _imageStore.DownloadImage(id, stream);
        return await StreamToFileContentResult(stream);
    }

    public async Task<UploadImageResponse> UploadImage(IFormFile file)
    {
        var blobName = Guid.NewGuid().ToString();
        var fileStream = file.OpenReadStream();
        await _imageStore.UploadImage(blobName, fileStream);
        return ToResponse(blobName);
    }

    public async Task DeleteImage(string id)
    {
        await _imageStore.DeleteImage(id);
    }
    
    private static Task<FileContentResult> StreamToFileContentResult(MemoryStream stream)
    {
        return Task.FromResult(new FileContentResult(stream.ToArray(), "image/jpeg"));
    }
    
    private static UploadImageResponse ToResponse(string blobName)
    {
        return new UploadImageResponse(blobName);
    }
}