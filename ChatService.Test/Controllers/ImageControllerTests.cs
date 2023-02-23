using System.Net;
using System.Text;
using ChatService.Storage;
using ChatService.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;

namespace ChatService.Test.Controllers;

public class ImageControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly Mock<IImageStore> _imageStoreMock = new();
    private readonly HttpClient _httpClient;

    public ImageControllerTests(WebApplicationFactory<Program> factory)
    {
        _httpClient = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services => { services.AddSingleton(_imageStoreMock.Object); });
        }).CreateClient();
    }

    [Theory]
    [InlineData("existingImage")]
    public async Task DownloadImage(string existingImage)
    {
        // change to DOWNLOAD
        _imageStoreMock.Setup(m => m.DownloadImage(existingImage))
            .ReturnsAsync(new UploadImageResponse(existingImage));

        var response = await _httpClient.GetAsync($"/Image/{existingImage}");
        //Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadAsStringAsync();
        // depends what the api returns
        //Assert.Equal(profile.ProfilePictureId, JsonConvert.DeserializeObject<ProfileDto>(json));
    }
    
    [Fact]
    public async Task PostImage()
    {
        var request = new UploadImageRequest(null);
        var response = await _httpClient.PostAsync("/Image",
            new StringContent(JsonConvert.SerializeObject(request), Encoding.Default, "application/json"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("http://localhost/Images/foobar", response.Headers.GetValues("Location").First());

        _imageStoreMock.Verify(mock => mock.PostImage(request.File), Times.Once);
    }
    
}