using ChatService.Dtos;

namespace ChatService.Services;

public interface IMessageService
{
    Task<SendMessageResponse> SendMessage(string conversationId, SendMessageRequest messageRequest, long unixTime);

    Task<MessagesList> GetMessageList(string conversationId,
        string? continuationToken,
        string? limit,
        string? lastSeenMessageTime);
}