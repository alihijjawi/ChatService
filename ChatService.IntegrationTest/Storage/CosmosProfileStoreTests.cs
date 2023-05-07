using ChatService.Dtos;
using ChatService.Services;
using ChatService.Storage;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace ChatService.IntegrationTest.Storage;

public class CosmosProfileStoreTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly IProfileService _profileService;

    private readonly ProfileDto _profile = new(
        UserName: Guid.NewGuid().ToString(),
        FirstName: "Foo",
        LastName: "Bar",
        ProfilePictureId: Guid.NewGuid().ToString()
    );

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _profileService.DeleteProfile(_profile.UserName);
    }

    public CosmosProfileStoreTests(WebApplicationFactory<Program> factory)
    {
        _profileService = factory.Services.GetRequiredService<IProfileService>();
    }
    
    [Fact]
    public async Task AddNewProfile()
    {
        await _profileService.UpsertProfile(_profile);
        Assert.Equal(_profile, await _profileService.GetProfile(_profile.UserName));
    }
    
    [Fact]
    public async Task AddProfile_Conflict()
    {
        await _profileService.UpsertProfile(_profile);
        Assert.Equal(_profile, await _profileService.GetProfile(_profile.UserName));
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
    public async Task? AddProfile_InvalidArgs(string username, string firstName, string lastName, string imageId)
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _profileService.UpsertProfile(new ProfileDto(username, firstName, lastName, imageId)));
    }

    [Fact]
    public async Task GetNonExistingProfile()
    {
        Assert.Null(await _profileService.GetProfile(_profile.UserName));
    }
}