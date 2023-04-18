using ChatService.Dtos;
using ChatService.Storage;

namespace ChatService.Services;

public class MessageService: IMessageService
{
    private readonly IMessageStore _messageStore;

    public MessageService(IMessageStore messageStore)
    {
        _messageStore = messageStore;
    }
    public async Task<SendMessageResponse> SendMessage(string conversationId, SendMessageRequest messageRequest, long unixTime)
    {
        await _messageStore.AddMessage(conversationId, messageRequest, unixTime);

        return new SendMessageResponse(unixTime);
    }

    public async Task<MessagesList> GetMessageList(string conversationId, string? continuationToken, string? limit, string? lastSeenMessageTime)
    {
        return await _messageStore.GetMessageList(conversationId, continuationToken, limit, lastSeenMessageTime);
    }
}