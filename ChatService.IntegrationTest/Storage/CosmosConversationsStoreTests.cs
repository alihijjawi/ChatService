using System.Net;
using ChatService.Dtos;
using ChatService.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace ChatService.IntegrationTest.Storage;

public class CosmosConversationsStoreTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly IConversationsService _conversationsService;
    
    private static readonly string _conversationId = Guid.NewGuid().ToString();
    private static readonly long _unixTime = long.MinValue;
    private static readonly ProfileDto _sender = new(
        UserName: "foobar",
        FirstName: "Foo",
        LastName: "Bar",
        ProfilePictureId: String.Empty
    );
    
    private static readonly ConversationDto _conversationsDto = new(
        Id: _conversationId,
        LastModifiedUnixTime: _unixTime,
        Recipient: _sender
    );
    
    private readonly ConversationsList _conversationsList = new(
        Conversations: new []{ _conversationsDto },
        NextUri: "idk what to put here"
    );


    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _conversationsService.DeleteConversation(_conversationId, _sender.UserName);
    }

    public CosmosConversationsStoreTests(WebApplicationFactory<Program> factory)
    {
        _conversationsService = factory.Services.GetRequiredService<IConversationsService>();
    }
    
    [Fact]
    public async Task CreateConversation_Success()
    {
        await _conversationsService.CreateConversation(_conversationId, _sender, _unixTime);
        Assert.Equal(_conversationsList, await _conversationsService.GetConversationList(_sender.UserName, null, null, null));
    }
    
    [Fact]
    public async Task CreateConversation_Conflict()
    {
        await _conversationsService.CreateConversation(_conversationId, _sender, _unixTime);
        Assert.Equal();
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
        await Assert.ThrowsAsync<ArgumentException>(() => _conversationsService.UpsertProfile(new ProfileDto(username, firstName, lastName, imageId)));
    }

    [Fact]
    public async Task GetNonExistingProfile()
    {
        Assert.Null(await _conversationsService.GetProfile(_profile.UserName));
    }
}