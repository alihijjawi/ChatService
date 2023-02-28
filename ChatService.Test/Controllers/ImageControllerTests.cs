using System.Net;
using System.Text;
using ChatService.Storage;
using ChatService.Dtos;
using ChatService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Any;
using Moq;
using Newtonsoft.Json;

namespace ChatService.Test.Controllers;

public class ImageControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly Mock<IImageService> _imageStoreMock = new();
    private readonly HttpClient _httpClient;

    public ImageControllerTests(WebApplicationFactory<Program> factory)
    {
        _httpClient = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services => { services.AddSingleton(_imageStoreMock.Object); });
        }).CreateClient();
    }

    [Fact]
    public async Task DownloadImage_Successful()
    {
        string imageId = Guid.NewGuid().ToString();
        _imageStoreMock.Setup(m => m.DownloadImage(imageId))
            .ReturnsAsync(new FileContentResult(Array.Empty<byte>(), "image/jpeg"));

        var response = await _httpClient.GetAsync($"/image/{imageId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        _imageStoreMock.Verify(mock => mock.DownloadImage(imageId), Times.Once);
    }
    
    [Fact]
    public async Task DownloadImage_NotFound()
    {
        string imageId = Guid.NewGuid().ToString();
        _imageStoreMock.Setup(m => m.DownloadImage(imageId))
            .ThrowsAsync(new ArgumentException($"Image with id:'{imageId}' does not exist"));

        var response = await _httpClient.GetAsync($"/image/{imageId}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        
        _imageStoreMock.Verify(mock => mock.DownloadImage(imageId), Times.Once);
    }

    [Fact]
    public async Task PostImage_Successful()
    {
        _imageStoreMock.Setup(m => m.UploadImage(It.IsAny<IFormFile>()))
            .ReturnsAsync(new UploadImageResponse("foobar"));
        
        byte[] fileContents = { 0x12 };
        using var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(fileContents);
        content.Add(fileContent, "file", "filename.ext");
        var response = await _httpClient.PostAsync("/image", content);
        
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("http://localhost/Image/foobar", response.Headers.GetValues("Location").First());

        _imageStoreMock.Verify(mock => mock.UploadImage(It.IsAny<IFormFile>()), Times.Once);
    }
    
    [Fact]
    public async Task PostImage_EmptyFile()
    {
        using var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(Array.Empty<byte>());
        content.Add(fileContent, "file", "filename.ext");
        var response = await _httpClient.PostAsync("/image", content);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        _imageStoreMock.Verify(mock => mock.UploadImage(It.IsAny<IFormFile>()), Times.Never);
    }
    
}