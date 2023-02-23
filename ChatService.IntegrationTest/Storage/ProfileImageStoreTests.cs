using ChatService.Dtos;
using ChatService.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace ChatService.IntegrationTest.Storage;

public class ProfileImageStoreTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly IImageStore _store;
    private static readonly byte[] File = { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 };
    private static readonly string ImageId = Guid.NewGuid().ToString();
    
    private readonly ProfileDto _profile = new(
        UserName: Guid.NewGuid().ToString(),
        FirstName: "Foo",
        LastName: "Bar",
        ProfilePictureId: ImageId
    );
    
    private readonly UploadImageRequest _request = new(
        File: new FormFile(new MemoryStream(File), 0, 0, null, null)
    );
    
    private readonly UploadImageResponse _response = new(
        ImageId: ImageId
    );

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        // add delete image to imageStore
        await _store.DeleteImage(_profile.UserName);
    }

    public ProfileImageStoreTests(WebApplicationFactory<Program> factory)
    {
        _store = factory.Services.GetRequiredService<IImageStore>();
    }
    
    [Fact]
    public async Task AddNewImage()
    {
        await _store.PostImage(_request.File);
        Assert.Equal(_response, await _store.DownloadImage(_profile.ProfilePictureId));
    }
    
    [Fact]
    public async Task AddProfile_Conflict()
    {
        // depends on what the conflict does
        await _store.PostImage(_request.File);
        Assert.Equal(_response, await _store.DownloadImage(_profile.ProfilePictureId));
    }
    
    [Theory]
    [InlineData(null)]
    public async Task PostImage_InvalidArgs(IFormFile file)
    {
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _store.PostImage(file);
        });
    }

    [Fact]
    public async Task GetNonExistingProfile()
    {
        Assert.Null(await _store.DownloadImage(_profile.ProfilePictureId));
    }
    
}