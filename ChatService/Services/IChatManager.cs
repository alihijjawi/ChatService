using ChatService.Dtos;

namespace ChatService.Services;

public interface IChatManager
{
    Task<StartConversationResponse> StartConversation(StartConversationRequest conversationRequest);

    Task<ConversationsList> GetConversationList(string username,
        string? continuationToken,
        string? limit,
        string? lastSeenMessageTime);
    
    Task<SendMessageResponse> SendMessage(string conversationId, SendMessageRequest messageRequest);

    Task<MessagesList> GetMessageList(string conversationId,
        string? continuationToken,
        string? limit,
        string? lastSeenMessageTime);
}