using ChatService.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace ChatService.Storage;

public interface IImageStore
{
    Task DownloadImage(string id, Stream stream);
    Task UploadImage(string blobName, Stream fileStream);
    Task DeleteImage(string id);
}