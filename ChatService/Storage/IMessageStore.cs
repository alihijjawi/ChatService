using ChatService.Dtos;

namespace ChatService.Storage;

public interface IMessageStore
{
    Task AddMessage(string conversationId, SendMessageRequest messageRequest, long unixTime);

    Task<MessagesList> GetMessageList(string conversationId, string? continuationToken, string? limit,
        string? lastSeenMessageTime);

    Task DeleteMessage(string messageId, string conversationId);
}