using ChatService.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace ChatService.Storage;

public interface IImageStore
{
    Task<FileContentResult> DownloadImage(string id);
    Task<UploadImageResponse> UploadImage(IFormFile file);
    Task DeleteImage(string id);
}