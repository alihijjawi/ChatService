using ChatService.Dtos;

namespace ChatService.Storage;

public interface IImageStore
{
    Task<UploadImageResponse?> GetImage(string id);
    Task UploadImage(IFormFile file);
}