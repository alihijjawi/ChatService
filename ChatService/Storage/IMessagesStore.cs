using ChatService.Dtos;

namespace ChatService.Storage;

public interface IMessagesStore
{
    Task<SendMessageResponse> SendMessage(string conversationId, SendMessageRequest messageRequest);

    Task<ConversationDto> GetMessageList(string conversationId, string continuationToken, string limit,
        string lastSeenMessageTime);
}