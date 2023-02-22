using ChatService.Dtos;

namespace ChatService.Storage;

public class ProfileImageStore : IImageStore
{
    public Task<UploadImageResponse?> GetImage(string id)
    {
        return new Task<UploadImageResponse>();
    }

    public Task UploadImage(IFormFile file)
    {
        return Task.CompletedTask;
    }
}