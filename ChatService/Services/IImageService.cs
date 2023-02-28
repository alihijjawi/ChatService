using ChatService.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace ChatService.Services;

public interface IImageService
{
    Task<FileContentResult> DownloadImage(string id);
    Task<UploadImageResponse> UploadImage(IFormFile file);
    Task DeleteImage(string id);
}