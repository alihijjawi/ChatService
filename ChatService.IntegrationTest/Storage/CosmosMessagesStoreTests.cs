using ChatService.Dtos;
using ChatService.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace ChatService.IntegrationTest.Storage;

public class CosmosMessagesStoreTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly IMessageService _messageService;
    
    private readonly string MessageId = "message_test";
    private readonly string USername = "foobar";
    private readonly string ConversationId = "foobar_barfoo";
    private static readonly long UnixTime = DateTimeOffset.Now.ToUnixTimeSeconds();

    private static readonly MessageDto MessageDto = new(
        Text: "test message",
        SenderUsername: "foobar",
        UnixTime: UnixTime
    );
    
    private static readonly SendMessageRequest Request = new(
        Id: "message_test",
        SenderUsername: "foobar",
        Text: "test message"
        );

    private readonly MessagesList MessageList = new(
        Messages: new []{ MessageDto },
        NextUri: ""
    );


    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _messageService.DeleteMessage(MessageId, ConversationId);
    }

    public CosmosMessagesStoreTests(WebApplicationFactory<Program> factory)
    {
        _messageService = factory.Services.GetRequiredService<IMessageService>();
    }
    
    [Fact]
    public async Task SendMessage_Success()
    {
        await _messageService.SendMessage(ConversationId, Request, UnixTime);
        Assert.Equal(MessageList, await _messageService.GetMessageList(ConversationId, null, null, null));
    }
    
    [Fact]
    public async Task SendMessage_Conflict()
    {
        await Assert.ThrowsAsync<Microsoft.Azure.Cosmos.CosmosException>( () => _messageService.SendMessage(ConversationId, Request, UnixTime));
    }
}