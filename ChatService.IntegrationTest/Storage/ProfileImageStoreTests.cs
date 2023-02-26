using ChatService.Dtos;
using ChatService.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;

namespace ChatService.IntegrationTest.Storage;

public class ProfileImageStoreTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly IImageStore _store;
    private readonly FormFile _image;
    private readonly byte[] _file = { 0x12 };
    
    public ProfileImageStoreTests(WebApplicationFactory<Program> factory)
    {   
        _store = factory.Services.GetRequiredService<IImageStore>();
        _image = new FormFile(new MemoryStream(_file), 0, _file.Length, "file.txt", "file.txt");
    }

    [Fact]
    public async Task UploadAndDownloadImage_Successful()
    {
        var uploadResponse = await _store.UploadImage(_image);
        var downloadResponse = await _store.DownloadImage(uploadResponse.ImageId);
        Assert.Equal(_file, downloadResponse.FileContents);
        await _store.DeleteImage(uploadResponse.ImageId);
    }
    
    [Fact]
    public async Task DownloadImage_NotFound()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _store.DownloadImage("ANonExistingImageId"));
    }
    
    [Fact]
    public async Task DeleteImage_NotFound()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _store.DeleteImage("ANonExistingImageId"));
    }
}