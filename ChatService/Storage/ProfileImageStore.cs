using ChatService.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage.Blob;

namespace ChatService.Storage;

public class ProfileImageStore : IImageStore
{
    private readonly CloudBlobClient _cloudBlobClient;
    public ProfileImageStore(CloudBlobClient cloudBlobClient)
    {
        _cloudBlobClient = cloudBlobClient;
    }
    
    private CloudBlobContainer _cloudBlobContainer => _cloudBlobClient.GetContainerReference("images");

    //download the image using id
    public async Task<FileContentResult> DownloadImage(string id)
    {
        var blob = _cloudBlobContainer.GetBlockBlobReference(id);
        
        if (!await blob.ExistsAsync())
        {
            throw new ArgumentException($"Image with id:'{id}' does not exist");
        }
        
        var stream = new MemoryStream();
        await blob.DownloadToStreamAsync(stream);
        stream.Seek(0, SeekOrigin.Begin);
        
        return new FileContentResult(stream.ToArray(), "image/jpeg");
    }

    //upload image
    public async Task<UploadImageResponse> UploadImage(IFormFile file)
    {
        var fileStream = file.OpenReadStream();
        
        var blobName = Guid.NewGuid().ToString();
        var blob = _cloudBlobContainer.GetBlockBlobReference(blobName);
        
        await blob.UploadFromStreamAsync(fileStream);

        return ToResponse(blobName);
    }

    private static UploadImageResponse ToResponse(string blobName)
    {
        return new UploadImageResponse(blobName);
    }
    
    //delete image
    public async Task DeleteImage(string id)
    {
        var blob = _cloudBlobContainer.GetBlockBlobReference(id);
        
        if (!await blob.ExistsAsync())
        {
            throw new ArgumentException($"Image with id:'{id}' does not exist");
        }

        await blob.DeleteIfExistsAsync();
    }
}