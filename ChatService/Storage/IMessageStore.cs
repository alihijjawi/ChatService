using ChatService.Dtos;

namespace ChatService.Storage;

public interface IMessageStore
{
    Task<long> AddMessage(string conversationId, SendMessageRequest messageRequest);

    Task<ConversationDto> GetMessageList(string conversationId, string continuationToken, string limit,
        string lastSeenMessageTime);
}