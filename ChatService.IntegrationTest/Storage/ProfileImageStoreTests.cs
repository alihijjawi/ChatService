using ChatService.Dtos;
using ChatService.Services;
using ChatService.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;

namespace ChatService.IntegrationTest.Storage;

public class ProfileImageStoreTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly IImageService _service;
    private readonly FormFile _image;
    private readonly byte[] _file = { 0x12 };
    
    public ProfileImageStoreTests(WebApplicationFactory<Program> factory)
    {   
        _service = factory.Services.GetRequiredService<IImageService>();
        _image = new FormFile(new MemoryStream(_file), 0, _file.Length, "file.txt", "file.txt");
    }

    [Fact]
    public async Task UploadAndDownloadImage_Successful()
    {
        var uploadResponse = await _service.UploadImage(_image);
        var downloadResponse = await _service.DownloadImage(uploadResponse.ImageId);
        Assert.Equal(_file, downloadResponse.FileContents);
        await _service.DeleteImage(uploadResponse.ImageId);
    }
    
    [Fact]
    public async Task DownloadImage_NotFound()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _service.DownloadImage("ANonExistingImageId"));
    }
    
    [Fact]
    public async Task DeleteImage_NotFound()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _service.DeleteImage("ANonExistingImageId"));
    }
}