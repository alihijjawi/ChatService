using System.Net;
using System.Text;
using ChatService.Storage;
using ChatService.Dtos;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;

namespace ChatService.Test.Controllers;

public class ProfileControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly Mock<IProfileStore> _profileStoreMock = new();
    private readonly HttpClient _httpClient;

    public ProfileControllerTests(WebApplicationFactory<Program> factory)
    {
        _httpClient = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services => { services.AddSingleton(_profileStoreMock.Object); });
        }).CreateClient();
    }

    [Fact]
    public async Task GetProfile()
    {
        var profile = new ProfileDto("foobar", "Foo", "Bar", "foobarImage");
        _profileStoreMock.Setup(m => m.GetProfile(profile.UserName))
            .ReturnsAsync(profile);

        var response = await _httpClient.GetAsync($"/Profile/{profile.UserName}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadAsStringAsync();
        Assert.Equal(profile, JsonConvert.DeserializeObject<ProfileDto>(json));
    }

    [Fact]
    public async Task GetProfile_NotFound()
    {
        _profileStoreMock.Setup(m => m.GetProfile("foobar"))
            .ReturnsAsync((ProfileDto?)null);

        var response = await _httpClient.GetAsync($"/Profile/foobar");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddProfile()
    {
        var profile = new ProfileDto("foobar", "Foo", "Bar", "foobarImage");
        var response = await _httpClient.PostAsync("/Profile",
            new StringContent(JsonConvert.SerializeObject(profile), Encoding.Default, "application/json"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("http://localhost/Profile/foobar", response.Headers.GetValues("Location").First());

        _profileStoreMock.Verify(mock => mock.UpsertProfile(profile), Times.Once);
    }

    [Fact]
    public async Task AddProfile_Conflict()
    {
        var profile = new ProfileDto("foobar", "Foo", "Bar", "foobarImage");
        _profileStoreMock.Setup(m => m.GetProfile(profile.UserName))
            .ReturnsAsync(profile);

        var response = await _httpClient.PostAsync("/Profile",
            new StringContent(JsonConvert.SerializeObject(profile), Encoding.Default, "application/json"));
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        _profileStoreMock.Verify(m => m.UpsertProfile(profile), Times.Never);
    }

    [Theory]
    [InlineData(null, "Foo", "Bar", "imageId")]
    [InlineData("", "Foo", "Bar", "imageId")]
    [InlineData(" ", "Foo", "Bar", "imageId")]
    [InlineData("foobar", null, "Bar", "imageId")]
    [InlineData("foobar", "", "Bar", "imageId")]
    [InlineData("foobar", "   ", "Bar", "imageId")]
    [InlineData("foobar", "Foo", "", "imageId")]
    [InlineData("foobar", "Foo", null, "imageId")]
    [InlineData("foobar", "Foo", " ", "imageId")]
    [InlineData("foobar", "Foo", "Bar", null)]
    [InlineData("foobar", "Foo", "Bar", "")]
    [InlineData("foobar", "Foo", "Bar", "  ")]
    public async Task AddProfile_InvalidArgs(string userName, string firstName, string lastName, string imageId)
    {
        var profile = new ProfileDto(userName, firstName, lastName, imageId);
        var response = await _httpClient.PostAsync("/Profile",
            new StringContent(JsonConvert.SerializeObject(profile), Encoding.Default, "application/json"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        _profileStoreMock.Verify(mock => mock.UpsertProfile(profile), Times.Never);
    }
}