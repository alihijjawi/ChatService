using ChatService.Dtos;
using ChatService.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace ChatService.IntegrationTest.Storage;

public class CosmosConversationsStoreTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly IConversationsService _conversationsService;
    
    private static readonly string ConversationId = "foobar_barfoo";
    private static readonly long UnixTime = DateTimeOffset.Now.ToUnixTimeSeconds();

    private static readonly ProfileDto Sender = new(
        UserName: "foobar",
        FirstName: "Foo",
        LastName: "Bar",
        ProfilePictureId: string.Empty
    );
    
    private static readonly ProfileDto Receiver = new(
        UserName: "barfoo",
        FirstName: "Bar",
        LastName: "Foo",
        ProfilePictureId: string.Empty
    );
    
    private static readonly ConversationDto ConversationsDto = new(
        Id: ConversationId,
        LastModifiedUnixTime: UnixTime,
        Recipient: Receiver
    );
    
    private readonly ConversationsList _conversationsList = new(
        Conversations: new []{ ConversationsDto },
        NextUri: ""
    );


    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _conversationsService.DeleteConversation(ConversationId, Receiver.UserName);
    }

    public CosmosConversationsStoreTests(WebApplicationFactory<Program> factory)
    {
        _conversationsService = factory.Services.GetRequiredService<IConversationsService>();
    }
    
    [Fact]
    public async Task CreateConversation_Success()
    {
        await _conversationsService.CreateConversation(ConversationId, Receiver, UnixTime);
        Assert.Equal(_conversationsList, await _conversationsService.GetConversationList(Sender.UserName, null, null, null));
    }
    
    [Fact]
    public async Task CreateConversation_Conflict()
    {
        await _conversationsService.CreateConversation(ConversationId, Receiver, UnixTime);
        await Assert.ThrowsAsync<Microsoft.Azure.Cosmos.CosmosException>( () =>_conversationsService.CreateConversation(ConversationId, Receiver, UnixTime));
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
        //await Assert.ThrowsAsync<ArgumentException>(() => _conversationsService.UpsertProfile(new ProfileDto(username, firstName, lastName, imageId)));
    }

    [Fact]
    public async Task GetNonExistingProfile()
    {
        //Assert.Null(await _conversationsService.GetProfile(_profile.UserName));
    }
}