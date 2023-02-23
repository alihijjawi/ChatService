using ChatService.Dtos;
using ChatService.Storage;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace ChatService.IntegrationTest.Storage;

public class CosmosProfileStoreTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly IProfileStore _store;

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
        await _store.DeleteProfile(_profile.UserName);
    }

    public CosmosProfileStoreTests(WebApplicationFactory<Program> factory)
    {
        _store = factory.Services.GetRequiredService<IProfileStore>();
    }
    
    [Fact]
    public async Task AddNewProfile()
    {
        await _store.UpsertProfile(_profile);
        Assert.Equal(_profile, await _store.GetProfile(_profile.UserName));
    }
    
    [Fact]
    public async Task AddProfile_Conflict()
    {
        await _store.UpsertProfile(_profile);
        Assert.Equal(_profile, await _store.GetProfile(_profile.UserName));
    }
    
    [Theory]
    [InlineData(null, "Foo", "Bar", "image")]
    [InlineData("", "Foo", "Bar", "image")]
    [InlineData(" ", "Foo", "Bar", "image")]
    [InlineData("foobar", null, "Bar", "image")]
    [InlineData("foobar", "", "Bar", "image")]
    [InlineData("foobar", "   ", "Bar", "image")]
    [InlineData("foobar", "Foo", "", "image")]
    [InlineData("foobar", "Foo", null, "image")]
    [InlineData("foobar", "Foo", " ", "image")]
    [InlineData("foobar", "Foo", "Bar", null)]
    [InlineData("foobar", "Foo", "Bar", "  ")]
    [InlineData("foobar", "Foo", "Bar", "")]
    public async Task AddProfile_InvalidArgs(string username, string firstName, string lastName, string imageId)
    {
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _store.UpsertProfile(new ProfileDto(username, firstName, lastName, imageId));
        });
    }

    [Fact]
    public async Task GetNonExistingProfile()
    {
        Assert.Null(await _store.GetProfile(_profile.UserName));
    }
}