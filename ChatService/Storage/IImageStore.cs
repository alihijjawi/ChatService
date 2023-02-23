using ChatService.Dtos;

namespace ChatService.Storage;

public interface IImageStore
{
    Task<UploadImageResponse?> DownloadImage(string id);
    Task PostImage(IFormFile file);
}