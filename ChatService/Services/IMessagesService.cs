using ChatService.Dtos;

namespace ChatService.Services;

public interface IMessagesService
{
    Task<SendMessageResponse> SendMessage(string conversationId, SendMessageRequest messageRequest);

    Task<ConversationDto> GetMessageList(string conversationId,
        string continuationToken,
        string limit,
        string lastSeenMessageTime);
}