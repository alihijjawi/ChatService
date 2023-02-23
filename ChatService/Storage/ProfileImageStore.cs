using ChatService.Dtos;

namespace ChatService.Storage;

public class ProfileImageStore : IImageStore
{
    public Task<UploadImageResponse?> DownloadImage(string id)
    {
        return null;
    }

    public Task PostImage(IFormFile file)
    {
        return Task.CompletedTask;
    }
}