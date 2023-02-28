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
    public async Task DownloadImage(string id, Stream stream)
    {
        var blob = _cloudBlobContainer.GetBlockBlobReference(id);
        if (!await blob.ExistsAsync())
        {
            throw new ArgumentException($"Image with id:'{id}' does not exist");
        }
        
        await blob.DownloadToStreamAsync(stream);
        //stream.Seek(0, SeekOrigin.Begin);
    }

    //upload image
    public async Task UploadImage(string blobName, Stream fileStream)
    {
        var blob = _cloudBlobContainer.GetBlockBlobReference(blobName);
        await blob.UploadFromStreamAsync(fileStream);
    }

    //delete image
    public async Task DeleteImage(string id)
    {
        var blob = _cloudBlobContainer.GetBlockBlobReference(id);

        if (!await blob.DeleteIfExistsAsync())
        {
            throw new ArgumentException($"Image with id:'{id}' does not exist");
        }
    }
}